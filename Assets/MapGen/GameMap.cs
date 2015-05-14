using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;
using RemoteFortressReader;
using UnityEngine.UI;
using System.IO;
using UnityExtension;

// The class responsible for talking to DF and meshing the data it gets.
// Relevant vocabulary: A "map tile" is an individual square on the map.
// A "map block" is a 16x16x1 area of map tiles stored by DF; think minecraft "chunks".

public class GameMap : MonoBehaviour
{
    // Things to be set from the Unity Editor.
    public Material basicTerrainMaterial;   // Can be any terrain you want.
    public Material stencilTerrainMaterial; // Foliage & other stenciled materials.
    public Material waterMaterial;
    public Material magmaMaterial;
    public Material invisibleMaterial;
    public Material invisibleStencilMaterial;
    public GameObject defaultMapBlock;
    public GameObject defaultStencilMapBlock;
    public GameObject defaultWaterBlock;
    public GameObject defaultMagmaBlock;
    public Light magmaGlowPrefab;
    public Text genStatus;
    public Text cursorProperties;

    // Parameters managing the currently visible area of the map.
    // Tracking:
    int posXBlock = 0;
    int posYBlock = 0;
    int posXTile = 0;
    int posYTile = 0;
    int posZ = 0;
    public int PosZ { // Public accessor; used from MapSelection
        get {
            return posZ;
        }
    }
    // Preferences:
    public int rangeX = 4;
    public int rangeY = 4;
    public int rangeZup = 0;
    public int rangeZdown = 5;
    public int blocksToProcess = 1;
    public int cameraViewDist = 25;
    public int nThreads = 0;

    // Stored view information
    RemoteFortressReader.ViewInfo view;
    bool posZDirty = true; // Set in GetViewInfo if z changes, and reset in HideMeshes after meshes hidden.

    BlockMesher mesher;

    // The actual unity meshes used to draw things on screen.
    MeshFilter[, ,] blocks;         // Terrain data.
    MeshFilter[, ,] stencilBlocks;  // Foliage &ct.
    MeshFilter[, , ,] liquidBlocks; // Water & magma. Extra dimension is a liquid type.
    // Dirty flags for those meshes
    bool[, ,] blockDirtyBits;
    bool[, ,] liquidBlockDirtyBits;
    // Lights from magma.
    Light[, ,] magmaGlow;

    // Stuff to let the material list & various meshes & whatnot be loaded from xml specs at runtime.
    Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition> materials;
    public ContentLoader contentLoader = new ContentLoader();

    // Coordinate system stuff.
    public const float tileHeight = 3.0f;
    public const float floorHeight = 0.5f;
    public const float tileWidth = 2.0f;
    public const int blockSize = 16;
    public static Vector3 DFtoUnityCoord(int x, int y, int z)
    {
        Vector3 outCoord = new Vector3(x * tileWidth, z * tileHeight, y * (-tileWidth));
        return outCoord;
    }
    public static Vector3 DFtoUnityCoord(DFCoord input)
    {
        Vector3 outCoord = new Vector3(input.x * tileWidth, input.z * tileHeight, input.y * (-tileWidth));
        return outCoord;
    }
    public static Vector3 DFtoUnityTileCenter(DFCoord input) {
        Vector3 result = DFtoUnityCoord(input);
        result.y += tileHeight/2;
        return result;
    }
    public static Vector3 DFtoUnityBottomCorner(DFCoord input) {
        Vector3 result = DFtoUnityCoord(input);
        result.x -= tileWidth / 2;
        result.z -= tileWidth / 2;
        return result;
    }
    public static DFCoord UnityToDFCoord(Vector3 input) {
        int x = Mathf.RoundToInt (input.x / tileWidth);
        int y = Mathf.RoundToInt (input.z / -tileWidth);
        int z = Mathf.FloorToInt (input.y / tileHeight);
        return new DFCoord(x, y, z);
    }
    public static bool IsBlockCorner(DFCoord input) {
        return input.x % blockSize == 0 &&
               input.y % blockSize == 0 &&
               input.z % blockSize == 0;
    }

    // Used while meshing blocks
    MeshCombineUtility.MeshInstance[] meshBuffer;
    MeshCombineUtility.MeshInstance[] stencilMeshBuffer;

    // Used for random diagnostics
    System.Diagnostics.Stopwatch blockListTimer = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch cullTimer = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch lazyLoadTimer = new System.Diagnostics.Stopwatch();

    // Does about what you'd think it does.
    void Start()
    {
        enabled = false;

        DFConnection.RegisterConnectionCallback(this.OnConnectToDF);
    }

    void OnConnectToDF() {
        Debug.Log("Connected");
        enabled = true;
        mesher = BlockMesher.GetMesher(nThreads);
        // Initialize materials
        if (materials == null)
            materials = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
        materials.Clear();
        foreach (RemoteFortressReader.MaterialDefinition material in DFConnection.Instance.NetMaterialList.material_list)
        {
            materials[material.mat_pair] = material;
        }

        UpdateView();

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        contentLoader.ParseContentIndexFile(Application.streamingAssetsPath + "/index.txt");
        watch.Stop();
        Debug.Log("Took a total of " + watch.ElapsedMilliseconds + "ms to load all XML files.");
        blockListTimer.Start();
        cullTimer.Start();
        lazyLoadTimer.Start();

        InitializeBlocks();
    }

    // Run once per frame.
    void Update()
    {
        if (!enabled) return;
        UpdateView ();
        ShowCursorInfo();
        UpdateRequestRegion();
        blockListTimer.Reset();
        blockListTimer.Start();
        //UpdateCreatures();
        UpdateBlocks();
        if (cullTimer.ElapsedMilliseconds > 100)
        {
            CullDistantBlocks();
            cullTimer.Reset();
            cullTimer.Start();
        }
        HideMeshes();
    }

    void UpdateView () {
        RemoteFortressReader.ViewInfo newView = DFConnection.Instance.PopViewInfoUpdate();
        if (newView == null) return;
        //Debug.Log("Got view");
        if (view == null || view.view_pos_z != newView.view_pos_z) {
            posZDirty = true;
        }
        view = newView;
       
        posXTile = (view.view_pos_x + (view.view_size_x / 2));
        posYTile = (view.view_pos_y + (view.view_size_y / 2));
        posXBlock = posXTile / 16;
        posYBlock = posYTile / 16;
        posZ = view.view_pos_z + 1;
    }

    void InitializeBlocks()
    {
        int blockSizeX = DFConnection.Instance.NetMapInfo.block_size_x;
        int blockSizeY = DFConnection.Instance.NetMapInfo.block_size_y;
        int blockSizeZ = DFConnection.Instance.NetMapInfo.block_size_z;

        Debug.Log("Map Size: " + blockSizeX + ", " + blockSizeY + ", " + blockSizeZ);
        blocks = new MeshFilter[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        stencilBlocks = new MeshFilter[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        liquidBlocks = new MeshFilter[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ, 2];
        blockDirtyBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        liquidBlockDirtyBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        magmaGlow = new Light[blockSizeX * 16, blockSizeY * 16, blockSizeZ];
    }

    void SetDirtyBlock(int mapBlockX, int mapBlockY, int mapBlockZ)
    {
        mapBlockX = mapBlockX / blockSize;
        mapBlockY = mapBlockY / blockSize;
        blockDirtyBits[mapBlockX, mapBlockY, mapBlockZ] = true;
    }
    void SetDirtyLiquidBlock(int mapBlockX, int mapBlockY, int mapBlockZ)
    {
        mapBlockX = mapBlockX / blockSize;
        mapBlockY = mapBlockY / blockSize;
        liquidBlockDirtyBits[mapBlockX, mapBlockY, mapBlockZ] = true;
    }

    void PrintFullMaterialList()
    {
        int totalCount = DFConnection.Instance.NetMaterialList.material_list.Count;
        int limit = totalCount;
        if (limit >= 100)
            limit = 100;
        //Don't ever do this.
        for (int i = totalCount - limit; i < totalCount; i++)
        {
            //no really, don't.
            RemoteFortressReader.MaterialDefinition material = DFConnection.Instance.NetMaterialList.material_list[i];
            Debug.Log("{" + material.mat_pair.mat_index + "," + material.mat_pair.mat_type + "}, " + material.id + ", " + material.name);
        }
    }

    void SaveTileTypeList()
    {
        try
        {
            File.Delete("TiletypeList.csv");
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter("TiletypeList.csv"))
        {
            foreach (Tiletype item in DFConnection.Instance.NetTiletypeList.tiletype_list)
            {
                writer.WriteLine(
                    item.name + "," +
                    item.shape + ":" +
                    item.special + ":" +
                    item.material + ":" +
                    item.variant + ":" +
                    item.direction
                    );
            }
        }
    }

    bool GenerateLiquids(int block_x, int block_y, int block_z)
    {
        if (!liquidBlockDirtyBits[block_x, block_y, block_z])
            return true;
        liquidBlockDirtyBits[block_x, block_y, block_z] = false;
        GenerateLiquidSurface(block_x, block_y, block_z, MapDataStore.WATER_INDEX);
        GenerateLiquidSurface(block_x, block_y, block_z, MapDataStore.MAGMA_INDEX);
        return true;
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
        buffer.transform = Matrix4x4.TRS(DFtoUnityCoord(tile.position), Quaternion.identity, Vector3.one);
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

    bool GenerateTiles(int block_x, int block_y, int block_z)
    {
        if (!blockDirtyBits[block_x, block_y, block_z])
            return true;
        blockDirtyBits[block_x, block_y, block_z] = false;
        int bufferIndex = 0;
        int stencilBufferIndex = 0;
        for (int xx = (block_x * blockSize); xx < (block_x + 1) * blockSize; xx++)
            for (int yy = (block_y * blockSize); yy < (block_y + 1) * blockSize; yy++)
            {
                if (meshBuffer == null)
                    meshBuffer = new MeshCombineUtility.MeshInstance[blockSize * blockSize * (int)MeshLayer.StaticCutout];
                if (stencilMeshBuffer == null)
                    stencilMeshBuffer = new MeshCombineUtility.MeshInstance[blockSize * blockSize * ((int)MeshLayer.Count - (int)MeshLayer.StaticCutout)];

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
                            FillMeshBuffer(out meshBuffer[bufferIndex], layer, MapDataStore.Main[xx, yy, block_z].Value);
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
                            FillMeshBuffer(out stencilMeshBuffer[stencilBufferIndex], layer, MapDataStore.Main[xx, yy, block_z].Value);
                            stencilBufferIndex++;
                            break;
                        default:
                            break;
                    }
                }
            }
        if (blocks[block_x, block_y, block_z] == null)
        {
            GameObject block = Instantiate(defaultMapBlock) as GameObject;
            block.SetActive(true);
            block.transform.parent = this.transform;
            block.name = "terrain(" + block_x + ", " + block_y + ", " + block_z + ")";
            blocks[block_x, block_y, block_z] = block.GetComponent<MeshFilter>();
        }
        MeshFilter mf = blocks[block_x, block_y, block_z];
        if (mf == null)
            Debug.LogError("MF is null");
        if (mf.mesh == null)
            mf.mesh = new Mesh();
        mf.mesh.Clear();
        if (stencilBlocks[block_x, block_y, block_z] == null)
        {
            GameObject stencilBlock = Instantiate(defaultStencilMapBlock) as GameObject;
            stencilBlock.SetActive(true);
            stencilBlock.transform.parent = this.transform;
            stencilBlock.name = "foliage(" + block_x + ", " + block_y + ", " + block_z + ")";
            stencilBlocks[block_x, block_y, block_z] = stencilBlock.GetComponent<MeshFilter>();
        }
        MeshFilter mfs = stencilBlocks[block_x, block_y, block_z];
        if (mfs == null)
            Debug.LogError("MFS is null");
        if (mfs.mesh == null)
            mfs.mesh = new Mesh();
        mfs.mesh.Clear();
        MeshCombineUtility.ColorCombine(mfs.mesh, stencilMeshBuffer);
        return MeshCombineUtility.ColorCombine(mf.mesh, meshBuffer);
        //Debug.Log("Generated a mesh with " + (mf.mesh.triangles.Length / 3) + " tris");
    }
    static int coord2Index(int x, int y)
    {
        return (x * (blockSize + 1)) + y;
    }
    void GenerateLiquidSurface(int block_x, int block_y, int block_z, int liquid_select)
    {
        Vector3[] finalVertices = new Vector3[(blockSize + 1) * (blockSize + 1)];
        Vector3[] finalNormals = new Vector3[(blockSize + 1) * (blockSize + 1)];
        Vector2[] finalUVs = new Vector2[(blockSize + 1) * (blockSize + 1)];
        List<int> finalFaces = new List<int>();
        float[,] heights = new float[2, 2];
        for (int xx = 0; xx <= blockSize; xx++)
            for (int yy = 0; yy <= blockSize; yy++)
            {
                //first find the heights of all tiles sharing one corner.
                for (int xxx = 0; xxx < 2; xxx++)
                    for (int yyy = 0; yyy < 2; yyy++)
                    {
                        int x = (block_x * blockSize) + xx + xxx - 1;
                        int y = (block_y * blockSize) + yy + yyy - 1;
                        if (x < 0 || y < 0 || x >= MapDataStore.MapSize.x || y >= MapDataStore.MapSize.y)
                        {
                            heights[xxx, yyy] = -1;
                            continue;
                        }
                        var maybeTile = MapDataStore.Main[x, y, block_z];
                        if (maybeTile == null)
                        {
                            heights[xxx, yyy] = -1;
                            continue;
                        }
                        var tile = maybeTile.Value;
                        if (tile.isWall)
                        {
                            heights[xxx, yyy] = -1;
                            continue;
                        }
                        heights[xxx, yyy] = MapDataStore.Main.GetLiquidLevel(new DFCoord(x,y,block_z), liquid_select);
                        heights[xxx, yyy] /= 7.0f;
                        if (tile.isFloor)
                        {
                            heights[xxx, yyy] *= (tileHeight - floorHeight);
                            heights[xxx, yyy] += floorHeight;
                        }
                        else
                            heights[xxx, yyy] *= tileHeight;

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
                finalNormals[coord2Index(xx, yy)] = new Vector3(sx, tileWidth * 2, -sy);
                finalNormals[coord2Index(xx, yy)].Normalize();

                finalVertices[coord2Index(xx, yy)] = DFtoUnityCoord(((block_x * blockSize) + xx), ((block_y * blockSize) + yy), block_z);
                finalVertices[coord2Index(xx, yy)].x -= tileWidth / 2.0f;
                finalVertices[coord2Index(xx, yy)].z += tileWidth / 2.0f;
                finalVertices[coord2Index(xx, yy)].y += height;
                finalUVs[coord2Index(xx, yy)] = new Vector2(xx, yy);
            }
        for (int xx = 0; xx < blockSize; xx++)
            for (int yy = 0; yy < blockSize; yy++)
            {
                if (MapDataStore.Main.GetLiquidLevel(
                        new DFCoord((block_x * blockSize) + xx, (block_y * blockSize) + yy, block_z),
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
        if (finalFaces.Count > 0)
        {
            if (liquidBlocks[block_x, block_y, block_z, liquid_select] == null)
            {
                GameObject block;
                if (liquid_select == MapDataStore.MAGMA_INDEX)
                    block = Instantiate(defaultMagmaBlock) as GameObject;
                else
                    block = Instantiate(defaultWaterBlock) as GameObject;
                block.SetActive(true);
                block.transform.parent = this.transform;
                block.name = (liquid_select == MapDataStore.WATER_INDEX ? "water(" : "magma(") + block_x + ", " + block_y + ", " + block_z + ")";
                liquidBlocks[block_x, block_y, block_z, liquid_select] = block.GetComponent<MeshFilter>();
            }
        }
        MeshFilter mf = liquidBlocks[block_x, block_y, block_z, liquid_select];
        if (mf == null)
        {
            return;
        }
        if (mf.mesh == null)
            mf.mesh = new Mesh();
        mf.mesh.Clear();
        if (finalFaces.Count == 0)
            return;
        mf.mesh.vertices = finalVertices;
        mf.mesh.uv = finalUVs;
        mf.mesh.triangles = finalFaces.ToArray();
        mf.mesh.normals = finalNormals;
        mf.mesh.RecalculateBounds();
        mf.mesh.RecalculateTangents();
    }


    void UpdateMeshes()
    {
        int count = 0;
        int failed = 0;
        for (int zz = DFConnection.Instance.RequestRegionMin.z; zz < DFConnection.Instance.RequestRegionMax.z; zz++)
            for (int yy = (DFConnection.Instance.RequestRegionMin.y * 16 / blockSize); yy <= (DFConnection.Instance.RequestRegionMax.y * 16 / blockSize); yy++)
                for (int xx = (DFConnection.Instance.RequestRegionMin.x * 16 / blockSize); xx <= (DFConnection.Instance.RequestRegionMax.x * 16 / blockSize); xx++)
                {
                    if (xx < 0 || yy < 0 || zz < 0 || xx >= blocks.GetLength(0) || yy >= blocks.GetLength(1) || zz >= blocks.GetLength(2))
                    {
                        //Debug.Log(xx + ", " + yy + ", " + zz + " is outside of " + blocks.GetLength(0) + ", " + blocks.GetLength(1) + ", " + blocks.GetLength(2));
                        continue;
                    }
                    //Debug.Log("Generating tiles at " + xx + ", " + yy + ", " + zz);
                    GenerateLiquids(xx, yy, zz);
                    if (GenerateTiles(xx, yy, zz))
                        count++;
                    else
                        failed++;
                }
        //Debug.Log("Generating " + count + " meshes took " + watch.ElapsedMilliseconds + " ms");
    }

    System.Diagnostics.Stopwatch netWatch = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch loadWatch = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch genWatch = new System.Diagnostics.Stopwatch();

    // Update the region we're requesting
    void UpdateRequestRegion()
    {
        DFConnection.Instance.SetRequestRegion(
            new DFCoord(
                posXBlock - rangeX,
                posYBlock - rangeY,
                posZ - rangeZdown
            ),
            new DFCoord(
                posXBlock + rangeX,
                posYBlock + rangeY,
                posZ + rangeZup
            ));
    }
    void UpdateBlocks()
    {
        //stopwatch.Stop();
        //watch.Start();
        loadWatch.Reset();
        loadWatch.Start();
        for (int i = 0; i < blocksToProcess; i++)
        {
            RemoteFortressReader.MapBlock block = DFConnection.Instance.PopMapBlockUpdate();
            if (block == null) break;
            MapDataStore.Main.StoreTiles(block);
            if (block.tiles.Count > 0) 
                SetDirtyBlock(block.map_x, block.map_y, block.map_z);
            if (block.water.Count > 0 || block.magma.Count > 0)
                SetDirtyLiquidBlock(block.map_x, block.map_y, block.map_z);
            //Debug.Log ("Used map block");
        }
        loadWatch.Stop();
        genWatch.Reset();
        genWatch.Start();
        UpdateMeshes();
        genWatch.Stop();
        //watch.Stop();
        //Debug.Log("Generating " + connectionState.net_block_list.map_blocks.Count + " Meshes took " + watch.Elapsed.TotalSeconds + " seconds");
    }

    void CullDistantBlocks()
    {
        int centerZ = view.view_pos_z;
        //int centerX = (connectionState.net_view_info.view_pos_x + (connectionState.net_view_info.view_size_x / 2));
        //int centerY = (connectionState.net_view_info.view_pos_y + (connectionState.net_view_info.view_size_y / 2));
        for (int xx = 0; xx < blocks.GetLength(0); xx++)
            for (int yy = 0; yy < blocks.GetLength(1); yy++)
                for (int zz = 0; zz < blocks.GetLength(2); zz++)
                {
                    if (zz > centerZ + cameraViewDist)
                    {
                        if (blocks[xx, yy, zz] != null)
                        {
                            blocks[xx, yy, zz].mesh.Clear();
                            blockDirtyBits[xx, yy, zz] = true;

                        }
                        if (stencilBlocks[xx, yy, zz] != null)
                        {
                            stencilBlocks[xx, yy, zz].mesh.Clear();
                            blockDirtyBits[xx, yy, zz] = true;

                        }
                        for (int i = 0; i < 2; i++)
                            if (liquidBlocks[xx, yy, zz, i] != null)
                            {
                                liquidBlocks[xx, yy, zz, i].mesh.Clear();
                                liquidBlockDirtyBits[xx, yy, zz] = true;
                            }
                        continue;
                    }
                    if (zz < centerZ - cameraViewDist)
                    {
                        if (blocks[xx, yy, zz] != null)
                        {
                            blocks[xx, yy, zz].mesh.Clear();
                            blockDirtyBits[xx, yy, zz] = true;

                        }
                        if (stencilBlocks[xx, yy, zz] != null)
                        {
                            stencilBlocks[xx, yy, zz].mesh.Clear();
                            blockDirtyBits[xx, yy, zz] = true;

                        } 
                        for (int i = 0; i < 2; i++)
                            if (liquidBlocks[xx, yy, zz, i] != null)
                            {
                                liquidBlocks[xx, yy, zz, i].mesh.Clear();
                                liquidBlockDirtyBits[xx, yy, zz] = true;
                            }
                        continue;
                    }
                }
    }

    void ClearMap()
    {
        foreach (MeshFilter MF in blocks)
        {
            if (MF != null)
                MF.mesh.Clear();
        }
        foreach (var item in stencilBlocks)
        {
            if (item != null)
                item.mesh.Clear();
        }
        foreach (var item in liquidBlocks)
        {
            if (item != null)
                item.mesh.Clear();   
        }
        foreach (var item in magmaGlow)
        {
            Destroy(item);
        }
        MapDataStore.Main.Reset();
    }

    void HideMeshes()
    {
        if (!posZDirty) return;
        posZDirty = false;
        for (int zz = 0; zz < blocks.GetLength(2); zz++)
            for (int yy = 0; yy < blocks.GetLength(1); yy++)
                for (int xx = 0; xx < blocks.GetLength(0); xx++)
                {
                    if (blocks[xx, yy, zz] != null)
                    {
                        if (zz > view.view_pos_z)
                        {
                            blocks[xx, yy, zz].gameObject.GetComponent<Renderer>().material = invisibleMaterial;
                            //blocks[xx, yy, zz].gameObject.SetActive(false);
                        }
                        else
                        {
                            blocks[xx, yy, zz].gameObject.GetComponent<Renderer>().material = basicTerrainMaterial;
                            //blocks[xx, yy, zz].gameObject.SetActive(true);
                        }
                    }
                }
        for (int zz = 0; zz < stencilBlocks.GetLength(2); zz++)
            for (int yy = 0; yy < stencilBlocks.GetLength(1); yy++)
                for (int xx = 0; xx < stencilBlocks.GetLength(0); xx++)
                {
                    if (stencilBlocks[xx, yy, zz] != null)
                    {
                        if (zz > view.view_pos_z)
                        {
                            stencilBlocks[xx, yy, zz].gameObject.GetComponent<Renderer>().material = invisibleStencilMaterial;
                            //stencilBlocks[xx, yy, zz].gameObject.SetActive(false);
                        }
                        else
                        {
                            stencilBlocks[xx, yy, zz].gameObject.GetComponent<Renderer>().material = stencilTerrainMaterial;
                            //stencilBlocks[xx, yy, zz].gameObject.SetActive(true);
                        }
                    }
                }
        for (int qq = 0; qq < liquidBlocks.GetLength(3); qq++)
            for (int zz = 0; zz < liquidBlocks.GetLength(2); zz++)
                for (int yy = 0; yy < liquidBlocks.GetLength(1); yy++)
                    for (int xx = 0; xx < liquidBlocks.GetLength(0); xx++)
                    {
                        if (liquidBlocks[xx, yy, zz, qq] != null)
                        {
                            if (zz > view.view_pos_z)
                                liquidBlocks[xx, yy, zz, qq].gameObject.GetComponent<Renderer>().material = invisibleMaterial;
                            else
                            {
                                if(qq == MapDataStore.MAGMA_INDEX)
                                    liquidBlocks[xx, yy, zz, qq].gameObject.GetComponent<Renderer>().material = magmaMaterial;
                                else
                                    liquidBlocks[xx, yy, zz, qq].gameObject.GetComponent<Renderer>().material = waterMaterial;
                            }
                        }
                    }
    }

    void ShowCursorInfo()
    {
        int cursX = view.cursor_pos_x;
        int cursY = view.cursor_pos_y;
        int cursZ = view.cursor_pos_z;
        cursorProperties.text = "";
        cursorProperties.text += "Cursor: ";
        cursorProperties.text += cursX + ",";
        cursorProperties.text += cursY + ",";
        cursorProperties.text += cursZ + "\n";
        var maybeTile = MapDataStore.Main[cursX, cursY, cursZ];
        if (maybeTile != null)
        {
            var tile = maybeTile.Value;
            cursorProperties.text += "Tiletype:\n";
            var tiletype = DFConnection.Instance.NetTiletypeList.tiletype_list
                [tile.tileType];
            cursorProperties.text += tiletype.name + "\n";
            cursorProperties.text +=
                tiletype.shape + ":" +
                tiletype.special + ":" +
                tiletype.material + ":" +
                tiletype.variant + ":" +
                tiletype.direction + "\n";
            var mat = tile.material;
            cursorProperties.text += "Material: ";
            cursorProperties.text += mat.mat_type + ",";
            cursorProperties.text += mat.mat_index + "\n";

            if (materials.ContainsKey(mat))
            {
                cursorProperties.text += "Material Name: ";
                cursorProperties.text += materials[mat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Material\n";

            cursorProperties.text += "\n";

            var basemat = tile.base_material;
            cursorProperties.text += "Base Material: ";
            cursorProperties.text += basemat.mat_type + ",";
            cursorProperties.text += basemat.mat_index + "\n";

            if (materials.ContainsKey(basemat))
            {
                cursorProperties.text += "Base Material Name: ";
                cursorProperties.text += materials[basemat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Base Material\n";

            cursorProperties.text += "\n";

            var layermat = tile.layer_material;
            cursorProperties.text += "Layer Material: ";
            cursorProperties.text += layermat.mat_type + ",";
            cursorProperties.text += layermat.mat_index + "\n";

            if (materials.ContainsKey(layermat))
            {
                cursorProperties.text += "Layer Material Name: ";
                cursorProperties.text += materials[layermat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Layer Material\n";

            cursorProperties.text += "\n";

            var veinmat = tile.vein_material;
            cursorProperties.text += "Vein Material: ";
            cursorProperties.text += veinmat.mat_type + ",";
            cursorProperties.text += veinmat.mat_index + "\n";

            if (materials.ContainsKey(veinmat))
            {
                cursorProperties.text += "Vein Material Name: ";
                cursorProperties.text += materials[veinmat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Vein Material\n";

        }
    }

    Dictionary<int, GameObject> creatureList;
    public GameObject creatureTemplate;

    void UpdateCreatures()
    {
        RemoteFortressReader.UnitList unitList = DFConnection.Instance.PopUnitListUpdate();
        if (unitList == null) return;
        foreach (var unit in unitList.creature_list)
        {
            if (creatureList == null)
                creatureList = new Dictionary<int, GameObject>();
            if (!creatureList.ContainsKey(unit.id))
            {
                creatureList[unit.id] = Instantiate(creatureTemplate) as GameObject;
                creatureList[unit.id].GetComponent<LayeredSprite>().Do_Sprite = true;
                creatureList[unit.id].transform.parent = gameObject.transform;
            }
            creatureList[unit.id].transform.position = DFtoUnityCoord(unit.pos_x, unit.pos_y, unit.pos_z) + new Vector3(0, 2, 0);
        }
    }

}
