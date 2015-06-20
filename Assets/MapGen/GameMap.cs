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

    public int cursX = -30000;
    public int cursY = -30000;
    public int cursZ = -30000;


    // Parameters managing the currently visible area of the map.
    // Tracking:
    int PosXBlock
    {
        get
        {
            return posXTile / 16;
        }
    }
    int PosYBlock
    {
        get
        {
            return posYTile / 16;
        }
    }
    public int posXTile = 0;
    public int posYTile = 0;
    public int posZ = 0;
    public int PosZ
    { // Public accessor; used from MapSelection
        get
        {
            return posZ;
        }
    }
    public int PosXTile
    {
        get
        {
            return posXTile;
        }
    }
    public int PosYTile
    {
        get
        {
            return posYTile;
        }
    }

    // Preferences:
    public int rangeX = 4;
    public int rangeY = 4;
    public int rangeZup = 0;
    public int rangeZdown = 5;
    public int blocksToProcess = 1;
    public int cameraViewDist = 25;
    public int meshingThreads = 0;

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
    Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition> items;

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
    public static Vector3 DFtoUnityTileCenter(DFCoord input)
    {
        Vector3 result = DFtoUnityCoord(input);
        result.y += tileHeight / 2;
        return result;
    }
    public static Vector3 DFtoUnityBottomCorner(DFCoord input)
    {
        Vector3 result = DFtoUnityCoord(input);
        result.x -= tileWidth / 2;
        result.z -= tileWidth / 2;
        return result;
    }
    public static DFCoord UnityToDFCoord(Vector3 input)
    {
        int x = Mathf.RoundToInt(input.x / tileWidth);
        int y = Mathf.RoundToInt(input.z / -tileWidth);
        int z = Mathf.FloorToInt(input.y / tileHeight);
        return new DFCoord(x, y, z);
    }
    public static bool IsBlockCorner(DFCoord input)
    {
        return input.x % blockSize == 0 &&
               input.y % blockSize == 0;
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

    void OnConnectToDF()
    {
        Debug.Log("Connected");
        enabled = true;
        mesher = BlockMesher.GetMesher(meshingThreads);
        // Initialize materials
        if (materials == null)
            materials = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
        materials.Clear();
        foreach (RemoteFortressReader.MaterialDefinition material in DFConnection.Instance.NetMaterialList.material_list)
        {
            materials[material.mat_pair] = material;
        }
        // Initialize items
        if (items == null)
            items = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
        items.Clear();
        foreach (RemoteFortressReader.MaterialDefinition material in DFConnection.Instance.NetItemList.material_list)
        {
            items[material.mat_pair] = material;
        }

        SaveTileTypeList();
        SaveMaterialList(DFConnection.Instance.NetMaterialList.material_list, "MaterialList.csv");
        SaveMaterialList(DFConnection.Instance.NetItemList.material_list, "ItemList.csv");

        UpdateView();

        blockListTimer.Start();
        cullTimer.Start();
        lazyLoadTimer.Start();

        InitializeBlocks();
    }

    // Run once per frame.
    void Update()
    {
        if (!enabled) return;
        //UpdateView();
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

    void OnDestroy()
    {
        if (mesher != null)
        {
            mesher.Terminate();
            mesher = null;
        }
    }

    void UpdateView()
    {
        RemoteFortressReader.ViewInfo newView = DFConnection.Instance.PopViewInfoUpdate();
        if (newView == null) return;
        //Debug.Log("Got view");
        if (view == null || view.view_pos_z != newView.view_pos_z)
        {
            posZDirty = true;
        }
        view = newView;

        posXTile = (view.view_pos_x + (view.view_size_x / 2));
        posYTile = (view.view_pos_y + (view.view_size_y / 2));
        posZ = view.view_pos_z + 1;
    }

    // Update the region we're requesting
    void UpdateRequestRegion()
    {
        DFConnection.Instance.SetRequestRegion(
            new BlockCoord(
                PosXBlock - rangeX,
                PosYBlock - rangeY,
                posZ - rangeZdown
            ),
            new BlockCoord(
                PosXBlock + rangeX,
                PosYBlock + rangeY,
                posZ + rangeZup
            ));
    }
    void UpdateBlocks()
    {
        loadWatch.Reset();
        loadWatch.Start();
        while (true)
        {
            RemoteFortressReader.MapBlock block = DFConnection.Instance.PopMapBlockUpdate();
            if (block == null) break;
            MapDataStore.Main.StoreTiles(block);
            if (block.tiles.Count > 0)
                SetDirtyBlock(block.map_x, block.map_y, block.map_z);
            if (block.water.Count > 0 || block.magma.Count > 0)
                SetDirtyLiquidBlock(block.map_x, block.map_y, block.map_z);
        }
        loadWatch.Stop();
        genWatch.Reset();
        genWatch.Start();
        EnqueueMeshUpdates();
        genWatch.Stop();

        mesher.Poll();

        FetchNewMeshes();
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
                    item.name + ";" +
                    item.shape + ":" +
                    item.special + ":" +
                    item.material + ":" +
                    item.variant + ":" +
                    item.direction
                    );
            }
        }
    }
    void SaveMaterialList(List<MaterialDefinition> list, string filename)
    {
        try
        {
            File.Delete(filename);
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var item in list)
            {
                writer.WriteLine(
                    item.name + ";" +
                    item.id + ";" +
                    item.mat_pair.mat_type + ";" +
                    item.mat_pair.mat_index
                    );
            }
        }
    }

    // Have the mesher mesh all dirty tiles in the region
    void EnqueueMeshUpdates()
    {
        for (int zz = DFConnection.Instance.RequestRegionMin.z; zz < DFConnection.Instance.RequestRegionMax.z; zz++)
            for (int yy = DFConnection.Instance.RequestRegionMin.y; yy <= DFConnection.Instance.RequestRegionMax.y; yy++)
                for (int xx = DFConnection.Instance.RequestRegionMin.x; xx <= DFConnection.Instance.RequestRegionMax.x; xx++)
                {
                    if (xx < 0 || yy < 0 || zz < 0 || xx >= blocks.GetLength(0) || yy >= blocks.GetLength(1) || zz >= blocks.GetLength(2))
                    {
                        continue;
                    }
                    if (!blockDirtyBits[xx, yy, zz] && !liquidBlockDirtyBits[xx, yy, zz])
                    {
                        continue;
                    }
                    mesher.Enqueue(new DFCoord(xx * 16, yy * 16, zz), blockDirtyBits[xx, yy, zz], liquidBlockDirtyBits[xx, yy, zz]);
                    blockDirtyBits[xx, yy, zz] = false;
                    liquidBlockDirtyBits[xx, yy, zz] = false;
                }
    }

    // Get new meshes from the mesher
    void FetchNewMeshes()
    {
        while (mesher.HasNewMeshes)
        {
            posZDirty = true; //the new blocks will also need to be hidden.
            var newMeshes = mesher.Dequeue().Value;
            int block_x = newMeshes.location.x / blockSize;
            int block_y = newMeshes.location.y / blockSize;
            int block_z = newMeshes.location.z;
            if (newMeshes.tiles != null)
            {
                if (blocks[block_x, block_y, block_z] == null)
                {
                    GameObject block = Instantiate(defaultMapBlock) as GameObject;
                    block.SetActive(true);
                    block.transform.parent = this.transform;
                    block.name = "terrain(" + block_x + ", " + block_y + ", " + block_z + ")";
                    blocks[block_x, block_y, block_z] = block.GetComponent<MeshFilter>();
                }
                MeshFilter tileFilter = blocks[block_x, block_y, block_z];
                tileFilter.mesh.Clear();
                newMeshes.tiles.CopyToMesh(tileFilter.mesh);
            }
            if (newMeshes.stencilTiles != null)
            {
                if (stencilBlocks[block_x, block_y, block_z] == null)
                {
                    GameObject stencilBlock = Instantiate(defaultStencilMapBlock) as GameObject;
                    stencilBlock.SetActive(true);
                    stencilBlock.transform.parent = this.transform;
                    stencilBlock.name = "foliage(" + block_x + ", " + block_y + ", " + block_z + ")";
                    stencilBlocks[block_x, block_y, block_z] = stencilBlock.GetComponent<MeshFilter>();
                }
                MeshFilter stencilFilter = stencilBlocks[block_x, block_y, block_z];
                stencilFilter.mesh.Clear();
                newMeshes.stencilTiles.CopyToMesh(stencilFilter.mesh);
            }
            if (newMeshes.water != null)
            {
                if (liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX] == null)
                {
                    GameObject block = Instantiate(defaultWaterBlock) as GameObject;
                    block.SetActive(true);
                    block.transform.parent = this.transform;
                    block.name = "water(" + block_x + ", " + block_y + ", " + block_z + ")";
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX] = block.GetComponent<MeshFilter>();
                }
                MeshFilter waterFilter = liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX];
                waterFilter.mesh.Clear();
                newMeshes.water.CopyToMesh(waterFilter.mesh);
            }
            if (newMeshes.magma != null)
            {
                if (liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX] == null)
                {
                    GameObject block = Instantiate(defaultMagmaBlock) as GameObject;
                    block.SetActive(true);
                    block.transform.parent = this.transform;
                    block.name = "magma(" + block_x + ", " + block_y + ", " + block_z + ")";
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX] = block.GetComponent<MeshFilter>();
                }
                MeshFilter magmaFilter = liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX];
                magmaFilter.mesh.Clear();
                newMeshes.magma.CopyToMesh(magmaFilter.mesh);
            }
        }
    }


    System.Diagnostics.Stopwatch loadWatch = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch genWatch = new System.Diagnostics.Stopwatch();


    void CullDistantBlocks()
    {
        //int centerX = (connectionState.net_view_info.view_pos_x + (connectionState.net_view_info.view_size_x / 2));
        //int centerY = (connectionState.net_view_info.view_pos_y + (connectionState.net_view_info.view_size_y / 2));
        for (int xx = 0; xx < blocks.GetLength(0); xx++)
            for (int yy = 0; yy < blocks.GetLength(1); yy++)
                for (int zz = 0; zz < blocks.GetLength(2); zz++)
                {
                    if (zz > PosZ + cameraViewDist)
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
                    if (zz < PosZ - cameraViewDist)
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
                        if (zz >= PosZ)
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
                        if (zz >= PosZ)
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
                            if (zz >= PosZ)
                                liquidBlocks[xx, yy, zz, qq].gameObject.GetComponent<Renderer>().material = invisibleMaterial;
                            else
                            {
                                if (qq == MapDataStore.MAGMA_INDEX)
                                    liquidBlocks[xx, yy, zz, qq].gameObject.GetComponent<Renderer>().material = magmaMaterial;
                                else
                                    liquidBlocks[xx, yy, zz, qq].gameObject.GetComponent<Renderer>().material = waterMaterial;
                            }
                        }
                    }
    }

    void ShowCursorInfo()
    {
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

            cursorProperties.text += "\n";

            var cons = tile.construction_item;
            cursorProperties.text += "Construction Item: ";
            cursorProperties.text += cons.mat_type + ",";
            cursorProperties.text += cons.mat_index + "\n";

            if (materials.ContainsKey(cons))
            {
                cursorProperties.text += "Construction Item Name: ";
                cursorProperties.text += items[cons].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Construction Item\n";
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

    public void UpdateCenter(Vector3 pos)
    {
        DFCoord dfPos = UnityToDFCoord(pos);
        posXTile = dfPos.x;
        posYTile = dfPos.y;
        if(posZ != dfPos.z+1)
        {
            posZ = dfPos.z+1;
            posZDirty = true;
        }
    }
}