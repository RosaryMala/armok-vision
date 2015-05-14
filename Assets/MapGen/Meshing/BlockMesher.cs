using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;
using System.Threading;
using RemoteFortressReader;

// Two implementations: single and multithreaded.
// All of the actual meshing code is exactly the same;
// the only difference is in how it's called.

abstract class BlockMesher {
    // If n = 0, single threaded; else multithreaded.
    public static BlockMesher GetMesher (int nThreads) {
        if (nThreads < 0) {
            throw new System.InvalidOperationException(nThreads + " threads? What??");
        } else if (nThreads == 0) {
            return new SingleThreadedMesher();
        } else {
            return new MultiThreadedMesher(nThreads);
        }
    }

    public struct Result {
        public DFCoord location;
        public MeshData tiles;
        public MeshData stencilTiles;
        public MeshData water;
        public MeshData magma;
    }

    protected struct Request {
        public bool tiles;
        public bool liquids;
        public MapDataStore data;
    }

    // There is REALLY NO REASON FOR THIS TO EXIST.
    // If unity was SENSIBLE we could use thread-local storage,
    // but unity is insensible, so we have to thread one of these through every method
    // that uses temporary buffers instead.
    protected struct TempBuffers {
        public void Init() {
            meshBuffer =
                new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize * (int)MeshLayer.StaticCutout];
            stencilMeshBuffer =
                new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize * ((int) MeshLayer.Count - (int)MeshLayer.StaticCutout)];
            heights = new float[2, 2];
        }

        public MeshCombineUtility.MeshInstance[] meshBuffer { get; private set; }
        public MeshCombineUtility.MeshInstance[] stencilMeshBuffer { get; private set; }
        public float[,] heights { get; private set; }
    }

    // Stuff for runtime configuration.
    // These will be accessed from multiple threads, but DON'T need to be
    // locked, since they don't change after being loaded.
    protected readonly ContentLoader contentLoader;
    protected readonly Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition> materials;

    // Some queues.
    // All of these need to be locked before access.
    // In general, NEVER LOCK MORE THAN ONE at the same time - 
    // That road leads to deadlock.
    protected readonly Util.UniqueQueue<DFCoord, Request> requestQueue; 
    protected readonly Queue<Result> resultQueue;
    protected readonly Stack<MapDataStore> recycledBlocks; // Object pool!

    protected BlockMesher() {
        // Make queues
        requestQueue = new Util.UniqueQueue<DFCoord, Request>();
        recycledBlocks = new Stack<MapDataStore>();
        resultQueue = new Queue<Result>();

        // Load meshes
        contentLoader = new ContentLoader();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        contentLoader.ParseContentIndexFile(Application.streamingAssetsPath + "/index.txt");
        watch.Stop();
        Debug.Log("Took a total of " + watch.ElapsedMilliseconds + "ms to load all XML files.");

        // Load materials
        materials = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
        foreach (RemoteFortressReader.MaterialDefinition material in DFConnection.Instance.NetMaterialList.material_list)
        {
            materials[material.mat_pair] = material;
        }
    }

    // Needs to be run frequently.
    // Handles either talking to threads (multi) or doing meshing (single)
    public abstract void Poll();

    // Does what you'd expect.
    public abstract void Terminate();

    // Enqueue a block for meshing; inclde 
    public void Enqueue(DFCoord targetLocation, bool tiles, bool liquids) {
        if (!GameMap.IsBlockCorner(targetLocation)) {
            throw new UnityException("Can't enqueue non-block-corners");
        }
        if (!tiles && !liquids) {
            throw new UnityException("Why mesh something without tiles or liquids?");
        }

        // Using our object pool
        MapDataStore targetDataStore = AllocateBlockStore();

        // Copy data
        MapDataStore.Main.CopySliceTo(targetLocation, MapDataStore.BLOCK_SIZE, targetDataStore);

        // In case we displace another block update
        Request redundant;
        lock (requestQueue) {
            // Will be { tiles: false, liquids: false, data: null } if we don't have anything queued
            redundant = requestQueue[targetLocation];

            Request meshRequest = new Request();
            // If either request wanted liquids, do liquids;
            // If either request wanted tiles, do tiles.
            meshRequest.liquids = liquids || redundant.liquids;
            meshRequest.tiles   = tiles   || redundant.tiles;
            meshRequest.data    = targetDataStore;
            requestQueue.EnqueueAndDisplace(targetLocation, meshRequest);
        }
        if (redundant.data != null) {
            lock (recycledBlocks) {
                // Object pooling!
                recycledBlocks.Push(redundant.data);
            }
        }
    }

    public Result? Dequeue() {
        lock (resultQueue) {
            if (resultQueue.Count > 0) {
                return resultQueue.Dequeue();
            } else {
                return null;
            }
        }
    }

    public bool HasNewMeshes {
        get {
            lock (resultQueue) {
                return resultQueue.Count > 0;
            }
        }
    }

    // Object pooling!
    protected MapDataStore AllocateBlockStore() {
        lock (recycledBlocks) {
            if (recycledBlocks.Count == 0) {
                // Nothing to recycle
                return new MapDataStore(new BlockCoord(0,0,0));
            } else {
                // Reuse another data store
                return recycledBlocks.Pop ();
            }
        }
    }

    // ACTUAL MESHING CODE!
    // -------------------------------------------------

    protected Result CreateMeshes(Request request, TempBuffers temp) {
        Result result = new Result();
        result.location = request.data.SliceOrigin;
        if (request.liquids) {
            result.water = GenerateLiquidSurface(request.data, MapDataStore.WATER_INDEX, temp);
            result.magma = GenerateLiquidSurface(request.data, MapDataStore.MAGMA_INDEX, temp);
        }
        if (request.tiles) {
            GenerateTiles (request.data, out result.tiles, out result.stencilTiles, temp);
        }
        return result;
    }

    static int coord2Index(int x, int y)
    {
        return (x * (GameMap.blockSize + 1)) + y;
    }
    MeshData GenerateLiquidSurface(MapDataStore data, int liquid_select, TempBuffers temp)
    {
        int block_x = data.SliceOrigin.x / GameMap.blockSize;
        int block_y = data.SliceOrigin.y / GameMap.blockSize;
        int block_z = data.SliceOrigin.z;

        Vector3[] finalVertices = new Vector3[(GameMap.blockSize + 1) * (GameMap.blockSize + 1)];
        Vector3[] finalNormals = new Vector3[(GameMap.blockSize + 1) * (GameMap.blockSize + 1)];
        Vector2[] finalUVs = new Vector2[(GameMap.blockSize + 1) * (GameMap.blockSize + 1)];
        List<int> finalFaces = new List<int>();

        // Is this necessary?
        temp.heights[0,0] = 0;
        temp.heights[0,1] = 0;
        temp.heights[1,0] = 0;
        temp.heights[1,1] = 0;

        for (int xx = 0; xx <= GameMap.blockSize; xx++)
            for (int yy = 0; yy <= GameMap.blockSize; yy++)
            {
                //first find the temp.heights of all tiles sharing one corner.
                for (int xxx = 0; xxx < 2; xxx++)
                    for (int yyy = 0; yyy < 2; yyy++)
                    {
                        int x = (block_x * GameMap.blockSize) + xx + xxx - 1;
                        int y = (block_y * GameMap.blockSize) + yy + yyy - 1;
                        if (x < 0 || y < 0 || x >= MapDataStore.MapSize.x || y >= MapDataStore.MapSize.y)
                        {
                            temp.heights[xxx, yyy] = -1;
                            continue;
                        }
                        var maybeTile = data[x, y, block_z];
                        if (maybeTile == null)
                        {
                            temp.heights[xxx, yyy] = -1;
                            continue;
                        }
                        var tile = maybeTile.Value;
                        if (tile.isWall)
                        {
                            temp.heights[xxx, yyy] = -1;
                            continue;
                        }
                        temp.heights[xxx, yyy] = data.GetLiquidLevel(new DFCoord(x,y,block_z), liquid_select);
                        temp.heights[xxx, yyy] /= 7.0f;
                        if (tile.isFloor)
                        {
                            temp.heights[xxx, yyy] *= (GameMap.tileHeight - GameMap.floorHeight);
                            temp.heights[xxx, yyy] += GameMap.floorHeight;
                        }
                        else
                            temp.heights[xxx, yyy] *= GameMap.tileHeight;

                    }

                //now find their average, discaring invalid ones.
                float height = 0;
                float total = 0;
                foreach (var item in temp.heights)
                {
                    if (item < 0)
                        continue;
                    height += item;
                    total++;
                }
                if (total >= 1)
                    height /= total;
                //find the slopes.
                float sx = ((
                    (temp.heights[0, 0] < 0 ? height : temp.heights[0, 0]) +
                    (temp.heights[0, 1] < 0 ? height : temp.heights[0, 1])) / 2) - ((
                    (temp.heights[1, 0] < 0 ? height : temp.heights[1, 0]) +
                    (temp.heights[1, 1] < 0 ? height : temp.heights[1, 1])) / 2);
                float sy = ((
                    (temp.heights[0, 0] < 0 ? height : temp.heights[0, 0]) +
                    (temp.heights[1, 0] < 0 ? height : temp.heights[1, 0])) / 2) - ((
                    (temp.heights[0, 1] < 0 ? height : temp.heights[0, 1]) +
                    (temp.heights[1, 1] < 0 ? height : temp.heights[1, 1])) / 2);
                finalNormals[coord2Index(xx, yy)] = new Vector3(sx, GameMap.tileWidth * 2, -sy);
                finalNormals[coord2Index(xx, yy)].Normalize();

                finalVertices[coord2Index(xx, yy)] = GameMap.DFtoUnityCoord(((block_x * GameMap.blockSize) + xx), ((block_y * GameMap.blockSize) + yy), block_z);
                finalVertices[coord2Index(xx, yy)].x -= GameMap.tileWidth / 2.0f;
                finalVertices[coord2Index(xx, yy)].z += GameMap.tileWidth / 2.0f;
                finalVertices[coord2Index(xx, yy)].y += height;
                finalUVs[coord2Index(xx, yy)] = new Vector2(xx, yy);
            }
        for (int xx = 0; xx < GameMap.blockSize; xx++)
            for (int yy = 0; yy < GameMap.blockSize; yy++)
            {
                if (data.GetLiquidLevel(
                        new DFCoord((block_x * GameMap.blockSize) + xx, (block_y * GameMap.blockSize) + yy, block_z),
                        liquid_select) == 0) {
                        continue;
                }
                finalFaces.Add(coord2Index(xx, yy));
                finalFaces.Add(coord2Index(xx + 1, yy));
                finalFaces.Add(coord2Index(xx + 1, yy + 1));

                finalFaces.Add(coord2Index(xx, yy));
                finalFaces.Add(coord2Index(xx + 1, yy + 1));
                finalFaces.Add(coord2Index(xx, yy + 1));
            }
        if (finalFaces.Count > 0) {
            return new MeshData(vertices: finalVertices,
                                normals: finalNormals,
                                tangents: null,
                                uv: finalUVs,
                                uv2: null,
                                colors: null,
                                triangles: finalFaces.ToArray());
        } else {
            return null;
        }
    }

    bool GenerateTiles(MapDataStore data, out MeshData tiles, out MeshData stencilTiles, TempBuffers temp)
    {
        int block_x = data.SliceOrigin.x / GameMap.blockSize;
        int block_y = data.SliceOrigin.y / GameMap.blockSize;
        int block_z = data.SliceOrigin.z;
        int bufferIndex = 0;
        int stencilBufferIndex = 0;
        for (int xx = (block_x * GameMap.blockSize); xx < (block_x + 1) * GameMap.blockSize; xx++)
            for (int yy = (block_y * GameMap.blockSize); yy < (block_y + 1) * GameMap.blockSize; yy++)
            {
                if (!data.InSliceBounds(new DFCoord(xx, yy, block_z))) throw new UnityException("OOB");
                if (data[xx, yy, block_z] == null) continue;

                for (int i = 0; i < (int)MeshLayer.Count; i++)
                {
                    MeshLayer layer = (MeshLayer)i;
                    switch (layer)
                    {
                        case MeshLayer.StaticMaterial:
                        case MeshLayer.BaseMaterial:
                        case MeshLayer.LayerMaterial:
                        case MeshLayer.VeinMaterial:
                        case MeshLayer.NoMaterial:
                            FillMeshBuffer(out temp.meshBuffer[bufferIndex], layer, data[xx, yy, block_z].Value);
                            bufferIndex++;
                            break;
                        case MeshLayer.StaticCutout:
                        case MeshLayer.BaseCutout:
                        case MeshLayer.LayerCutout:
                        case MeshLayer.VeinCutout:
                        case MeshLayer.Growth0Cutout:
                        case MeshLayer.Growth1Cutout:
                        case MeshLayer.Growth2Cutout:
                        case MeshLayer.Growth3Cutout:
                        case MeshLayer.NoMaterialCutout:
                            FillMeshBuffer(out temp.stencilMeshBuffer[stencilBufferIndex], layer, data[xx, yy, block_z].Value);
                            stencilBufferIndex++;
                            break;
                        default:
                            break;
                    }
                }
            }
        bool dontCare, success;
        stencilTiles = MeshCombineUtility.ColorCombine(temp.stencilMeshBuffer, out dontCare);
        tiles = MeshCombineUtility.ColorCombine(temp.meshBuffer, out success);
        return success;
    }

    void FillMeshBuffer(out MeshCombineUtility.MeshInstance buffer, MeshLayer layer, MapDataStore.Tile tile)
    {
        buffer = new MeshCombineUtility.MeshInstance();
        MeshContent content = null;
        if (!contentLoader.tileMeshConfiguration.GetValue(tile, layer, out content))
        {
            buffer.meshData = null;
            return;
        }
        buffer.meshData = content.meshData[(int)layer];
        buffer.transform = Matrix4x4.TRS(GameMap.DFtoUnityCoord(tile.position), Quaternion.identity, Vector3.one);
        int tileTexIndex = 0;
        IndexContent tileTexContent;
        if (contentLoader.tileTextureConfiguration.GetValue (tile, layer, out tileTexContent))
            tileTexIndex = tileTexContent.value;
        int matTexIndex = 0;
        IndexContent matTexContent;
        if (contentLoader.materialTextureConfiguration.GetValue (tile, layer, out matTexContent))
            matTexIndex = matTexContent.value;
        ColorContent newColorContent;
        Color newColor;
        if (contentLoader.colorConfiguration.GetValue (tile, layer, out newColorContent)) {
            newColor = newColorContent.value;
        } else {
            MatPairStruct mat;
            mat.mat_type = -1;
            mat.mat_index = -1;
            switch (layer) {
            case MeshLayer.StaticMaterial:
            case MeshLayer.StaticCutout:
                mat = tile.material;
                break;
            case MeshLayer.BaseMaterial:
            case MeshLayer.BaseCutout:
                mat = tile.base_material;
                break;
            case MeshLayer.LayerMaterial:
            case MeshLayer.LayerCutout:
                mat = tile.layer_material;
                break;
            case MeshLayer.VeinMaterial:
            case MeshLayer.VeinCutout:
                mat = tile.vein_material;
                break;
            case MeshLayer.NoMaterial:
            case MeshLayer.NoMaterialCutout:
                break;
            case MeshLayer.Growth0Cutout:
                break;
            case MeshLayer.Growth1Cutout:
                break;
            case MeshLayer.Growth2Cutout:
                break;
            case MeshLayer.Growth3Cutout:
                break;
            default:
                break;
            }
            MaterialDefinition mattie;
            if (materials.TryGetValue (mat, out mattie)) {

                ColorDefinition color = mattie.state_color;
                if (color == null)
                    newColor = Color.cyan;
                else
                    newColor = new Color (color.red / 255.0f, color.green / 255.0f, color.blue / 255.0f, 1);
            } else {
                newColor = Color.white;
            }
        }
        buffer.color = newColor;
        buffer.uv1Index = matTexIndex;
        buffer.uv2Index = tileTexIndex;
    }
}

// For meshing on one thread.
// Mostly for debugging purposes.
sealed class SingleThreadedMesher : BlockMesher {
    private readonly TempBuffers temp;
    public SingleThreadedMesher() {
        temp = new TempBuffers();
        temp.Init();
    }

    public override void Poll() {
        if (requestQueue.Count == 0) return;
        Request req = requestQueue.Dequeue();
        resultQueue.Enqueue(CreateMeshes(req, temp));
        recycledBlocks.Push(req.data);
    }

    public override void Terminate() {}
}

// Now we're cooking with charcoal.
sealed class MultiThreadedMesher : BlockMesher {
    private static readonly System.TimeSpan SLEEP_TIME = System.TimeSpan.FromMilliseconds(16);

    private volatile bool finished;
    private readonly Thread[] threads;

    public MultiThreadedMesher(int nThreads = 1) {
        finished = false;
        threads = new Thread[nThreads];

        for (int i = 0; i < nThreads; i++) {
            threads[i] = new Thread(new ThreadStart(this.MeshForever));
            threads[i].Start();
        }
    }

    public override void Poll() {
        if (finished) {
            Debug.Log ("Polling after completion");
        }
    }

    public override void Terminate() {
        finished = true;
    }

    // The actual computation
    public void MeshForever() {
        // Allocate our buffers
        TempBuffers temp = new TempBuffers();
        temp.Init();

        // Loop forever
        while (!finished) {
            Request? maybeWorkItem;
            // Check for an item
            lock (requestQueue) {
                if (requestQueue.Count > 0) {
                    maybeWorkItem = requestQueue.Dequeue();
                } else {
                    maybeWorkItem = null;
                }
            }
            if (!maybeWorkItem.HasValue) {
                // Wait a bit; TODO tune SLEEP_TIME
                Thread.Sleep(SLEEP_TIME);
                continue;
            }
            // We've got something to do!
            Request workItem = maybeWorkItem.Value;
            Result result = CreateMeshes(workItem, temp);
            lock (resultQueue) {
                resultQueue.Enqueue(result);
            }
            lock (recycledBlocks) {
                recycledBlocks.Push(workItem.data);
            }
        }
    }
}