using DFHack;
using MaterialStore;
using RemoteFortressReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

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
        public CPUMesh tiles;
        public CPUMesh stencilTiles;
        public CPUMesh transparentTiles;
        public CPUMesh topTiles;
        public CPUMesh topStencilTiles;
        public CPUMesh topTransparentTiles;
        public CPUMesh water;
        public CPUMesh magma;
        public CPUMesh collisionMesh;
        public CPUMesh terrainMesh;
        public CPUMesh topTerrainMesh;
    }

    protected struct Request {
        public bool tiles;
        public bool liquids;
        public MapDataStore data;
    }

    [ThreadStatic]
    static MeshCombineUtility.MeshInstance[] meshBuffer;
    [ThreadStatic]
    static MeshCombineUtility.MeshInstance[] stencilMeshBuffer;
    [ThreadStatic]
    static MeshCombineUtility.MeshInstance[] transparentMeshBuffer;
    [ThreadStatic]
    static MeshCombineUtility.MeshInstance[] collisionMeshBuffer;
    [ThreadStatic]
    static MeshCombineUtility.MeshInstance[] terrainMeshBuffer;
    [ThreadStatic]
    static float[,] heights;

    static protected void InitBuffers()
    {
        meshBuffer =
            new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize * (int)MeshLayer.StaticCutout];
        stencilMeshBuffer =
            new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize * ((int)MeshLayer.StaticTransparent - (int)MeshLayer.StaticCutout)];
        transparentMeshBuffer =
            new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize * ((int)MeshLayer.Collision - (int)MeshLayer.StaticTransparent)];
        collisionMeshBuffer =
            new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize * ((int)MeshLayer.Count - (int)MeshLayer.Collision)];
        terrainMeshBuffer =
            new MeshCombineUtility.MeshInstance[GameMap.blockSize * GameMap.blockSize];
        heights = new float[2, 2];
    }

    // Stuff for runtime configuration.
    // These will be accessed from multiple threads, but DON'T need to be
    // locked, since they don't change after being loaded.
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

        // Load materials
        materials = new Dictionary<MatPairStruct, MaterialDefinition>();
        foreach (MaterialDefinition material in DFConnection.Instance.NetMaterialList.material_list)
        {
            materials[material.mat_pair] = material;
        }
        System.GC.Collect(); //force a garbage collect after initial load.
    }

    // Needs to be run frequently.
    // Handles either talking to threads (multi) or doing meshing (single)
    public abstract void Poll();

    // Does what you'd expect.
    public abstract void Terminate();

    /// <summary>
    /// Gets the status of the current generator
    /// </summary>
    /// <returns>status of the threaded mesher, if applicable.</returns>
    public abstract string Status();

    // Enqueue a block for meshing; inclde 
    public bool Enqueue(DFCoord targetLocation, bool tiles, bool liquids) {
        if (!GameMap.IsBlockCorner(targetLocation)) {
            throw new UnityException("Can't enqueue non-block-corners");
        }
        if (!tiles && !liquids) {
            throw new UnityException("Why mesh something without tiles or liquids?");
        }
        UnityEngine.Profiling.Profiler.BeginSample("BlockMesher.Enqueue");
        // Using our object pool
        MapDataStore targetDataStore = AllocateBlockStore();

        // Copy data
        if (!MapDataStore.Main.CopySliceTo(targetLocation, MapDataStore.BLOCK_SIZE, targetDataStore))
        {
            UnityEngine.Profiling.Profiler.EndSample();
            return true; //it's empty, but it isn't a failure condition.
        }

        // In case we displace another block update
        Request redundant;
        lock (requestQueue)
        {
            // Will be { tiles: false, liquids: false, data: null } if we don't have anything queued
            redundant = requestQueue[targetLocation];

            //if there's no existing queue member, don't add more than needed.
            if (redundant.data == null)
            {
                if (requestQueue.Count >= 16)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return false;
                }
            }

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
        UnityEngine.Profiling.Profiler.EndSample();
        return true;
    }

    public Result? Dequeue()
    {
        if (Monitor.TryEnter(resultQueue, 1000))
            try
            {
                if (resultQueue.Count > 0)
                {
                    return resultQueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                Monitor.Exit(resultQueue);
            }
        else
            return null;
    }

    public bool HasNewMeshes {
        get {
            if (Monitor.TryEnter(resultQueue, 1000))
                try
                {
                    return resultQueue.Count > 0;
                }
                finally
                {
                    Monitor.Exit(resultQueue);
                }
            else
                return false;
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

    protected Result CreateMeshes(Request request)
    {
        Result result = new Result();
        result.location = request.data.SliceOrigin;
        if (request.liquids)
        {
            GameMap.BeginSample("Liquid Generation");
            result.water = GenerateLiquidSurface(request.data, MapDataStore.WATER_INDEX);
            result.magma = GenerateLiquidSurface(request.data, MapDataStore.MAGMA_INDEX);
            GameMap.EndSample();
        }
        if (request.tiles)
        {
            GameMap.BeginSample("Tile Generation");
            GenerateTiles(request.data, out result.tiles, out result.stencilTiles, out result.transparentTiles, out result.topTiles, out result.topStencilTiles, out result.topTransparentTiles, out result.collisionMesh, out result.terrainMesh, out result.topTerrainMesh);
            GameMap.EndSample();
        }
        return result;
    }

    static int coord2Index(int x, int y)
    {
        return (x * (GameMap.blockSize + 1)) + y;
    }
    CPUMesh GenerateLiquidSurface(MapDataStore data, int liquid_select)
    {
        int block_x = data.SliceOrigin.x / GameMap.blockSize;
        int block_y = data.SliceOrigin.y / GameMap.blockSize;
        int block_z = data.SliceOrigin.z;

        Vector3[] finalVertices = new Vector3[(GameMap.blockSize + 1) * (GameMap.blockSize + 1)];
        Vector3[] finalNormals = new Vector3[(GameMap.blockSize + 1) * (GameMap.blockSize + 1)];
        Vector2[] finalUVs = new Vector2[(GameMap.blockSize + 1) * (GameMap.blockSize + 1)];
        List<int> finalFaces = new List<int>();

        // Is this necessary?
        heights[0,0] = 0;
        heights[0,1] = 0;
        heights[1,0] = 0;
        heights[1,1] = 0;

        for (int xx = 0; xx <= GameMap.blockSize; xx++)
            for (int yy = 0; yy <= GameMap.blockSize; yy++)
            {
                //first find the heights of all tiles sharing one corner.
                for (int xxx = 0; xxx < 2; xxx++)
                    for (int yyy = 0; yyy < 2; yyy++)
                    {
                        int x = (block_x * GameMap.blockSize) + xx + xxx - 1;
                        int y = (block_y * GameMap.blockSize) + yy + yyy - 1;
                        if (x < 0 || y < 0 || x >= MapDataStore.MapSize.x || y >= MapDataStore.MapSize.y)
                        {
                            heights[xxx, yyy] = -1;
                            continue;
                        }
                        var maybeTile = data[x, y, block_z];
                        if (maybeTile == null)
                        {
                            heights[xxx, yyy] = -1;
                            continue;
                        }
                        var tile = maybeTile;
                        if (tile.isWall)
                        {
                            heights[xxx, yyy] = -1;
                            continue;
                        }
                        heights[xxx, yyy] = data.GetLiquidLevel(new DFCoord(x,y,block_z), liquid_select);
                        heights[xxx, yyy] /= 7.0f;
                        if (tile.isFloor)
                        {
                            heights[xxx, yyy] *= (GameMap.tileHeight - GameMap.floorHeight);
                            heights[xxx, yyy] += GameMap.floorHeight;
                        }
                        else
                            heights[xxx, yyy] *= GameMap.tileHeight;

                    }

                //now find their average, discaring invalid ones.
                float height = 0;
                float total = 0;
                foreach (var item in heights)
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
                    (heights[0, 0] < 0 ? height : heights[0, 0]) +
                    (heights[0, 1] < 0 ? height : heights[0, 1])) / 2) - ((
                    (heights[1, 0] < 0 ? height : heights[1, 0]) +
                    (heights[1, 1] < 0 ? height : heights[1, 1])) / 2);
                float sy = ((
                    (heights[0, 0] < 0 ? height : heights[0, 0]) +
                    (heights[1, 0] < 0 ? height : heights[1, 0])) / 2) - ((
                    (heights[0, 1] < 0 ? height : heights[0, 1]) +
                    (heights[1, 1] < 0 ? height : heights[1, 1])) / 2);
                finalNormals[coord2Index(xx, yy)] = new Vector3(sx, GameMap.tileWidth * 2, -sy);
                finalNormals[coord2Index(xx, yy)].Normalize();

                finalVertices[coord2Index(xx, yy)] = GameMap.DFtoUnityCoord(xx, yy, -GameMap.MapZOffset);
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
            return new CPUMesh(vertices: finalVertices,
                                normals: finalNormals,
                                tangents: null,
                                uv: finalUVs,
                                uv2: null,
                                uv3: null,
                                colors: null,
                                triangles: finalFaces.ToArray());
        } else {
            return null;
        }
    }

    bool GenerateTiles(
        MapDataStore data,
        out CPUMesh tiles,
        out CPUMesh stencilTiles,
        out CPUMesh transparentTiles,
        out CPUMesh topTiles,
        out CPUMesh topStencilTiles,
        out CPUMesh topTransparentTiles,
        out CPUMesh collisionTiles,
        out CPUMesh terrainTiles,
        out CPUMesh topTerrainTiles)
    {
        int block_x = data.SliceOrigin.x / GameMap.blockSize;
        int block_y = data.SliceOrigin.y / GameMap.blockSize;
        int block_z = data.SliceOrigin.z;
        int bufferIndex = 0;
        int stencilBufferIndex = 0;
        int transparentBufferIndex = 0;
        int collisionIndex = 0;
        int terrainIndex = 0;
        GameMap.BeginSample("Fill Mesh Buffer");
        for (int i = 0; i < (int)MeshLayer.Count; i++)
        {
            for (int xx = (block_x * GameMap.blockSize); xx < (block_x + 1) * GameMap.blockSize; xx++)
                for (int yy = (block_y * GameMap.blockSize); yy < (block_y + 1) * GameMap.blockSize; yy++)
                {
                    if (!data.InSliceBounds(new DFCoord(xx, yy, block_z))) throw new UnityException("OOB");
                    if (data[xx, yy, block_z] == null) continue;

                    if (i < (int)MeshLayer.StaticCutout)
                    {
                        FillMeshBuffer(out meshBuffer[bufferIndex],
                            (MeshLayer)i,
                            data[xx, yy, block_z],
                            GameMap.DFtoUnityCoord(xx - (block_x * GameMap.blockSize),
                            yy - (block_y * GameMap.blockSize),
                            -GameMap.MapZOffset));
                        bufferIndex++;
                    }
                    else if (i < (int)MeshLayer.StaticTransparent)
                    {
                        FillMeshBuffer(out stencilMeshBuffer[stencilBufferIndex],
                            (MeshLayer)i,
                            data[xx, yy, block_z],
                            GameMap.DFtoUnityCoord(xx - (block_x * GameMap.blockSize),
                            yy - (block_y * GameMap.blockSize),
                            -GameMap.MapZOffset));
                        stencilBufferIndex++;
                    }
                    else if (i < (int)MeshLayer.Collision)
                    {
                        FillMeshBuffer(out transparentMeshBuffer[transparentBufferIndex],
                            (MeshLayer)i,
                            data[xx, yy, block_z],
                            GameMap.DFtoUnityCoord(xx - (block_x * GameMap.blockSize),
                            yy - (block_y * GameMap.blockSize),
                            -GameMap.MapZOffset));
                        transparentBufferIndex++;
                    }
                    else if (i < (int)MeshLayer.NaturalTerrain)
                    {
                        FillMeshBuffer(out collisionMeshBuffer[collisionIndex],
                            (MeshLayer)i,
                            data[xx, yy, block_z],
                            GameMap.DFtoUnityCoord(xx - (block_x * GameMap.blockSize),
                            yy - (block_y * GameMap.blockSize),
                            -GameMap.MapZOffset));
                        collisionIndex++;
                    }
                    else if (i == (int)MeshLayer.NaturalTerrain)
                    {
                        FillMeshBuffer(out terrainMeshBuffer[terrainIndex],
                            (MeshLayer)i,
                            data[xx, yy, block_z],
                            GameMap.DFtoUnityCoord(xx - (block_x * GameMap.blockSize),
                            yy - (block_y * GameMap.blockSize),
                            -GameMap.MapZOffset));
                        terrainIndex++;
                    }
                }
        }
        GameMap.EndSample();
        bool dontCare, success;
        GameMap.BeginSample("Generate Voxels");
        VoxelGenerator voxelGen = new VoxelGenerator();
        if (block_z == 0)
            voxelGen.bottomless = true;
         var naturalTerrain = voxelGen.Triangulate(data);
        GameMap.EndSample();
        GameMap.BeginSample("Combine Meshes");
        terrainTiles = MeshCombineUtility.ColorCombine(terrainMeshBuffer, terrainIndex, out dontCare, false, naturalTerrain);
        topTerrainTiles = MeshCombineUtility.ColorCombine(terrainMeshBuffer, terrainIndex, out dontCare, true);
        stencilTiles = MeshCombineUtility.ColorCombine(stencilMeshBuffer, stencilBufferIndex, out dontCare, false);
        topStencilTiles = MeshCombineUtility.ColorCombine(stencilMeshBuffer, stencilBufferIndex, out dontCare, true);
        transparentTiles = MeshCombineUtility.ColorCombine(transparentMeshBuffer, transparentBufferIndex, out dontCare, false);
        topTransparentTiles = MeshCombineUtility.ColorCombine(transparentMeshBuffer, transparentBufferIndex, out dontCare, true);
        topTiles = MeshCombineUtility.ColorCombine(meshBuffer, bufferIndex, out dontCare, true);
        tiles = MeshCombineUtility.ColorCombine(meshBuffer, bufferIndex, out success, false);
        collisionTiles = MeshCombineUtility.ColorCombine(collisionMeshBuffer, collisionIndex, out dontCare, false, naturalTerrain);
        GameMap.EndSample();

        return success;
    }

    /// <summary>
    /// This is the function that actually decides what mesh and texture each tile gets
    /// </summary>
    /// <param name="buffer">Buffer to put the chosen meshes into for combining</param>
    /// <param name="layer">layer currently being worked on</param>
    /// <param name="tile">The tile to get all the needed info from.</param>
    void FillMeshBuffer(out MeshCombineUtility.MeshInstance buffer, MeshLayer layer, MapDataStore.Tile tile, Vector3 pos)
    {
        buffer = new MeshCombineUtility.MeshInstance();
        Vector2 index1 = Vector2.zero;
        Vector2 index2 = Vector2.zero;
        MeshContent meshContent = null;
        buffer.color = Color.grey;
        if (layer == MeshLayer.Collision)
        {
            if (!ContentLoader.Instance.CollisionMeshConfiguration.GetValue(tile, layer, out meshContent))
            {
                buffer.meshData = null;
                return;
            }
            if (meshContent.MeshData.ContainsKey(MeshLayer.Collision))
                buffer.meshData = meshContent.MeshData[MeshLayer.Collision];
            else if (meshContent.MeshData.ContainsKey(MeshLayer.StaticMaterial))
                buffer.meshData = meshContent.MeshData[MeshLayer.StaticMaterial];
            else
            {
                buffer.meshData = null;
                return;
            }
            buffer.transform = Matrix4x4.TRS(pos, meshContent.GetRotation(tile), Vector3.one);
            buffer.hiddenFaces = CalculateHiddenFaces(tile, meshContent.Rotation);
            return;
        }
        if (ContentLoader.Instance.DesignationMeshConfiguration.GetValue(tile, layer, out meshContent))
        {
            if (!meshContent.MeshData.ContainsKey(layer))
            {
                buffer.meshData = null;
                return;
            }
            buffer.meshData = meshContent.MeshData[layer];
            buffer.transform = Matrix4x4.TRS(pos, meshContent.GetRotation(tile), Vector3.one);

            index1.x = ContentLoader.GetPatternIndex(tile.DesignationMat) / ContentLoader.Instance.PatternTextureDepth;
            buffer.color = ContentLoader.GetColor(tile.DesignationMat);
            if (meshContent.ShapeTexture != null)
                index1.y = meshContent.ShapeTexture.ArrayIndex;
            else
                index1.y = ContentLoader.Instance.DefaultShapeTexArrayIndex;
            if (meshContent.SpecialTexture != null)
                index2.x = meshContent.SpecialTexture.ArrayIndex;
            else
                index2.x = ContentLoader.Instance.DefaultSpecialTexArrayIndex;

            buffer.uv1Transform = Matrix4x4.identity;
            buffer.uv2Force = index1;
            buffer.uv3Force = index2;
            buffer.hiddenFaces = CalculateHiddenFaces(tile, meshContent.Rotation);
            return;
        }

        var matColor = ContentLoader.GetColor(tile.GetMaterial(layer));
        var matPatternIndex = ContentLoader.GetPatternIndex(tile.GetMaterial(layer));

        switch (layer)
        {
            case MeshLayer.GrowthMaterial:
            case MeshLayer.GrowthCutout:
            case MeshLayer.GrowthTransparent:
                matColor = ContentLoader.GetColor(new MatPairStruct(55, 0), tile.GetMaterial(layer));
                break;
            case MeshLayer.GrowthMaterial1:
            case MeshLayer.GrowthCutout1:
            case MeshLayer.GrowthTransparent1:
                matColor = ContentLoader.GetColor(new MatPairStruct(55, 1), tile.GetMaterial(layer));
                break;
            case MeshLayer.GrowthMaterial2:
            case MeshLayer.GrowthCutout2:
            case MeshLayer.GrowthTransparent2:
                matColor = ContentLoader.GetColor(new MatPairStruct(55, 2), tile.GetMaterial(layer));
                break;
            case MeshLayer.GrowthMaterial3:
            case MeshLayer.GrowthCutout3:
            case MeshLayer.GrowthTransparent3:
                matColor = ContentLoader.GetColor(new MatPairStruct(55, 3), tile.GetMaterial(layer));
                break;
            default:
                break;
        }

        switch (layer)
        {
            case MeshLayer.GrowthMaterial:
            case MeshLayer.GrowthMaterial1:
            case MeshLayer.GrowthMaterial2:
            case MeshLayer.GrowthMaterial3:
            case MeshLayer.GrowthCutout:
            case MeshLayer.GrowthCutout1:
            case MeshLayer.GrowthCutout2:
            case MeshLayer.GrowthCutout3:
            case MeshLayer.GrowthTransparent:
            case MeshLayer.GrowthTransparent1:
            case MeshLayer.GrowthTransparent2:
            case MeshLayer.GrowthTransparent3:
                {
                    switch (tile.tiletypeMaterial)
                    {
                        case TiletypeMaterial.PLANT:
                        case TiletypeMaterial.ROOT:
                        case TiletypeMaterial.TREE_MATERIAL:
                        case TiletypeMaterial.MUSHROOM:
                            if (!ContentLoader.Instance.GrowthMeshConfiguration.GetValue(tile, layer, out meshContent))
                            {
                                buffer.meshData = null;
                                return;
                            }
                            break;
                        default:
                            buffer.meshData = null;
                            return;
                    }
                }
                break;
            default:
                {
                    if (layer == MeshLayer.NaturalTerrain)
                    {
                        if (VoxelGenerator.IsNatural(tile) && !VoxelGenerator.HandleShape(tile) && !VoxelGenerator.UseBoth(tile))
                            layer = MeshLayer.StaticMaterial;
                        else
                        {
                            buffer.meshData = null;
                            return;
                        }
                    }
                    else if (VoxelGenerator.IsNatural(tile) && !VoxelGenerator.UseBoth(tile))
                    {
                        buffer.meshData = null;
                        return;
                    }
                    if (!ContentLoader.Instance.TileMeshConfiguration.GetValue(tile, layer, out meshContent))
                    {
                        buffer.meshData = null;
                        return;
                    }
                }
                break;
        }

        //Use the transparent shader instead of the opaque shader if the material is transparent.
        if(matColor.a < 0.5f)
        {
            switch (layer)
            {
                case MeshLayer.StaticMaterial:
                case MeshLayer.BaseMaterial:
                case MeshLayer.LayerMaterial:
                case MeshLayer.VeinMaterial:
                    buffer.meshData = null;
                    return;
                case MeshLayer.StaticTransparent:
                    layer = MeshLayer.StaticMaterial;
                    break;
                case MeshLayer.BaseTransparent:
                    layer = MeshLayer.BaseMaterial;
                    break;
                case MeshLayer.LayerTransparent:
                    layer = MeshLayer.LayerMaterial;
                    break;
                case MeshLayer.VeinTransparent:
                    layer = MeshLayer.VeinMaterial;
                    break;
                default:
                    break;
            }
        }

        if (!meshContent.MeshData.ContainsKey(layer))
        {

            buffer.meshData = null;
            return;
        }
        buffer.meshData = meshContent.MeshData[layer];
        buffer.transform = Matrix4x4.TRS(pos, meshContent.GetRotation(tile), Vector3.one);
        Matrix4x4 shapeTextTransform = ContentLoader.Instance.DefaultShapeTexTransform;
        Matrix4x4 specialTexTransform = Matrix4x4.identity;

        NormalContent tileTexContent;
        if (meshContent.ShapeTexture == null)
        {
            if (ContentLoader.Instance.ShapeTextureConfiguration.GetValue(tile, layer, out tileTexContent))
            {
                shapeTextTransform = tileTexContent.UVTransform;
                index1.y = tileTexContent.ArrayIndex;
            }
        }
        else
        {
            shapeTextTransform = meshContent.ShapeTexture.UVTransform;
            index1.y = meshContent.ShapeTexture.ArrayIndex;
        }

        index1.x = matPatternIndex / ContentLoader.Instance.PatternTextureDepth;



        if (meshContent.SpecialTexture != null)
        {
            specialTexTransform = meshContent.SpecialTexture.UVTransform;
            index2.x = meshContent.SpecialTexture.ArrayIndex;
        }
        else
        {
            specialTexTransform = ContentLoader.Instance.DefaultSpecialTexTransform;
            index2.x = ContentLoader.Instance.DefaultSpecialTexArrayIndex;
        }

        buffer.color = matColor;

        buffer.uv1Transform = Matrix4x4.identity;
        buffer.uv2Force = index1;
        buffer.uv3Force = index2;

        buffer.hiddenFaces = CalculateHiddenFaces(tile, meshContent.Rotation);
    }

    private MeshCombineUtility.HiddenFaces CalculateHiddenFaces(MapDataStore.Tile tile, RotationType rotation)
    {
        MeshCombineUtility.HiddenFaces hiddenFaces = MeshCombineUtility.HiddenFaces.None;
        if (rotation == RotationType.None)
        {
            if (tile.North != null && (tile.North.isWall || tile.North.Hidden))
                hiddenFaces |= MeshCombineUtility.HiddenFaces.North;
            if (tile.South != null && (tile.South.isWall || tile.South.Hidden))
                hiddenFaces |= MeshCombineUtility.HiddenFaces.South;
            if (tile.East != null && (tile.East.isWall || tile.East.Hidden))
                hiddenFaces |= MeshCombineUtility.HiddenFaces.East;
            if (tile.West != null && (tile.West.isWall || tile.West.Hidden))
                hiddenFaces |= MeshCombineUtility.HiddenFaces.West;
        }
        if (rotation != RotationType.TreeRound && rotation != RotationType.TreeRoundTall)//these two rotations don't give a flat rotation.
        {
            if (tile.Up != null && (tile.Up.isSolidBase || tile.Up.Hidden))
                hiddenFaces |= MeshCombineUtility.HiddenFaces.Up;
            if (tile.Down != null && (tile.Down.isWall || tile.Down.Hidden))
                hiddenFaces |= MeshCombineUtility.HiddenFaces.Down;
        }
        return hiddenFaces;
    }
}

// For meshing on the Unity thread.
// Mostly for debugging purposes.
sealed class SingleThreadedMesher : BlockMesher {
    public SingleThreadedMesher()
    {
        InitBuffers();
    }

    public override void Poll()
    {
        if (ContentLoader.Instance == null)
            return;
        StatsReadout.QueueLength = requestQueue.Count;
        if (requestQueue.Count == 0) return;
        Request req = requestQueue.Dequeue();
        GameMap.BeginSample("Generate Meshes");
        resultQueue.Enqueue(CreateMeshes(req));
        recycledBlocks.Push(req.data);
        GameMap.EndSample();
    }

    public override string Status()
    {
        return "Single threaded.";
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
            threads[i] = new Thread(new ThreadStart(MeshForever));
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
        foreach (var item in threads)
        {
            if (item != null)
            {
                if (!item.Join(1000))
                {
                    Debug.Log("Thread failed to join in time, attempting to kill it.");
                    item.Abort(); //we asked nicely.
                }
            }
        }
    }

    // The actual computation
    public void MeshForever()
    {
        InitBuffers();
        long[] times = new long[10];
        int index = 0;
        System.Diagnostics.Stopwatch watch;
        // Loop forever
        while (!finished)
        {
            try
            {
                if (ContentLoader.Instance == null)
                {
                    //If there's nothing loaded yet, don't do anything.
                    Thread.Sleep(SLEEP_TIME);
                    continue;
                }
                Request? maybeWorkItem;
                // Check for an item
                if (Monitor.TryEnter(requestQueue, 1000))
                {
                    try
                    {
                        if (requestQueue.Count > 0)
                        {
                            maybeWorkItem = requestQueue.Dequeue();
                        }
                        else
                        {
                            maybeWorkItem = null;
                        }
                        StatsReadout.QueueLength = requestQueue.Count;
                    }
                    finally
                    {
                        Monitor.Exit(requestQueue);
                    }
                }
                else
                {
                    maybeWorkItem = null;
                    Debug.Log("Meshing thread timed out getting a lock on the task queue");
                }
                if (!maybeWorkItem.HasValue)
                {
                    // Wait a bit; TODO tune SLEEP_TIME
                    Thread.Sleep(SLEEP_TIME);
                    continue;
                }
                // We've got something to do!
                Request workItem = maybeWorkItem.Value;
                watch = System.Diagnostics.Stopwatch.StartNew();
                Result result = CreateMeshes(workItem);
                watch.Stop();
                times[index] = watch.ElapsedTicks;
                index++;
                if (index >= times.Length)
                    index = 0;
                long average = 0;
                foreach (long item in times)
                {
                    average += item;
                }
                average /= times.Length;
                StatsReadout.BlockProcessTime = new System.TimeSpan(average);
                lock (resultQueue)
                {
                    resultQueue.Enqueue(result);
                }
                lock (recycledBlocks)
                {
                    recycledBlocks.Push(workItem.data);
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public override string Status()
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < threads.Length; i++)
        {
            sb.Append("Thread ").Append(i).Append(": ").Append(threads[i] == null ? "NULL" : threads[i].ThreadState.ToString()).AppendLine();
        }
        return sb.ToString();
    }
}