using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;
using System.Threading;

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

    // All of these need to be locked before access.
    // In general, NEVER LOCK MORE THAN ONE at the same time - 
    // That road leads to deadlock.
    protected readonly Util.UniqueQueue<DFCoord, Request> requestQueue; 
    protected readonly Queue<Result> resultQueue;
    protected readonly Stack<MapDataStore> recycledBlocks; // Object pool!

    protected BlockMesher() {
        requestQueue = new Util.UniqueQueue<DFCoord, Request>();
        recycledBlocks = new Stack<MapDataStore>();
        resultQueue = new Queue<Result>();
    }

    // Needs to be run frequently.
    // Handles either talking to threads (multi) or doing meshing (single)
    public abstract void Poll();

    // Enqueue a block for meshing; inclde 
    public void Enqueue(DFCoord targetLocation, bool tiles, bool liquids) {
        if (!GameMap.IsBlockCorner(targetLocation)) {
            throw new UnityException("Can't enqueue non-block-corners");
        }
        if (!tiles && !liquids) {
            throw new UnityException("Why mesh something without tiles or liquids?");
        }

        MapDataStore targetDataStore = AllocateBlockStore();
        // Using our object pool

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

    public Result? PopResult() {
        lock (resultQueue) {
            if (resultQueue.Count > 0) {
                return resultQueue.Dequeue();
            } else {
                return null;
            }
        }
    }

    // Object pooling!
    protected MapDataStore AllocateBlockStore() {
        lock (recycledBlocks) {
            if (recycledBlocks.Count == 0) {
                // Nothing to recycle
                return new MapDataStore(new DFCoord(0,0,0), MapDataStore.BLOCK_SIZE);
            } else {
                // Reuse another data store
                return recycledBlocks.Pop ();
            }
        }
    }

    // ACTUAL MESHING CODE!
    // --------------------

    protected Result CreateMeshes(Request request, TempBuffers temp) {
        Result result = new Result();
        result.location = request.data.SliceOrigin;
        if (request.tiles) {
            // Do tiles
        }
        if (request.liquids) {
            // Do liquids
        }
        return result;
    }

    static int coord2Index(int x, int y)
    {
        return (x * (GameMap.blockSize + 1)) + y;
    }
    MeshData GenerateLiquidSurface(MapDataStore data, int liquid_select, TempBuffers temp)
    {
        DFCoord block = data.SliceOrigin / 16;
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
                        int x = (block.x * GameMap.blockSize) + xx + xxx - 1;
                        int y = (block.y * GameMap.blockSize) + yy + yyy - 1;
                        if (x < 0 || y < 0 || x >= MapDataStore.MapSize.x || y >= MapDataStore.MapSize.y)
                        {
                            temp.heights[xxx, yyy] = -1;
                            continue;
                        }
                        var maybeTile = data[x, y, block.z];
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
                        temp.heights[xxx, yyy] = MapDataStore.Main.GetLiquidLevel(new DFCoord(x,y,block.z), liquid_select);
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

                finalVertices[coord2Index(xx, yy)] = GameMap.DFtoUnityCoord(((block.x * GameMap.blockSize) + xx), ((block.y * GameMap.blockSize) + yy), block.z);
                finalVertices[coord2Index(xx, yy)].x -= GameMap.tileWidth / 2.0f;
                finalVertices[coord2Index(xx, yy)].z += GameMap.tileWidth / 2.0f;
                finalVertices[coord2Index(xx, yy)].y += height;
                finalUVs[coord2Index(xx, yy)] = new Vector2(xx, yy);
            }
        for (int xx = 0; xx < GameMap.blockSize; xx++)
            for (int yy = 0; yy < GameMap.blockSize; yy++)
            {
                if (MapDataStore.Main.GetLiquidLevel(
                        new DFCoord((block.x * GameMap.blockSize) + xx, (block.y * GameMap.blockSize) + yy, block.z),
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
}

// Now we're cooking with charcoal.
sealed class MultiThreadedMesher : BlockMesher {
    public MultiThreadedMesher(int nThreads = 1) {
        throw new System.NotImplementedException("Can't do that yet, sorry");
    }

    public override void Poll() {
        throw new System.NotImplementedException("Can't do that yet, sorry");
    }
}