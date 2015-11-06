using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using DFHack;
using RemoteFortressReader;
using UnityEngine.UI;
using System.IO;
using UnitFlags;
using System.Text;

// The class responsible for talking to DF and meshing the data it gets.
// Relevant vocabulary: A "map tile" is an individual square on the map.
// A "map block" is a 16x16x1 area of map tiles stored by DF; think minecraft "chunks".

public class GameMap : MonoBehaviour
{
    // Things to be set from the Unity Editor.
    public Material basicTerrainMaterial;   // Can be any terrain you want.
    public Material stencilTerrainMaterial; // Foliage & other stenciled materials.
    public Material transparentTerrainMaterial; // Anything with partial transparency.
    public Material waterMaterial;
    public Material magmaMaterial;
    public Material invisibleMaterial;
    public Material invisibleStencilMaterial;
    Material BasicTopMaterial
    {
        get
        {
            if (firstPerson)
                return basicTerrainMaterial;
            else if (overheadShadows)
                return invisibleMaterial;
            else
                return null;
        }
    }
    Material StencilTopMaterial
    {
        get
        {
            if (firstPerson)
                return stencilTerrainMaterial;
            else if (overheadShadows)
                return invisibleStencilMaterial;
            else
                return null;
        }
    }
    Material TransparentTopMaterial
    {
        get
        {
            if (firstPerson)
                return transparentTerrainMaterial;
            else if (overheadShadows)
                return invisibleStencilMaterial;
            else
                return null;
        }
    }
    public Light magmaGlowPrefab;
    public Text genStatus;
    public Text cursorProperties;

    public bool overheadShadows = true;

    public bool firstPerson = false;

    public int cursX = -30000;
    public int cursY = -30000;
    public int cursZ = -30000;

    public WorldMapMaker worldMap;


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

    BlockMesher mesher;

    // The actual unity meshes used to draw things on screen.
    Mesh[,,] blocks;         // Terrain data.
    Mesh[,,] stencilBlocks;  // Foliage &ct.
    Mesh[,,] transparentBlocks;  // Glass &ct.
    Mesh[,,,] liquidBlocks; // Water & magma. Extra dimension is a liquid type.
    // Dirty flags for those meshes
    bool[,,] blockDirtyBits;
    bool[,,] liquidBlockDirtyBits;
    // Lights from magma.
    Light[,,] magmaGlow;

    DFCoord mapSize; //This is to keep track of changing size of the map.
    DFCoord mapPosition;

    // Stuff to let the material list & various meshes & whatnot be loaded from xml specs at runtime.
    Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition> materials;
    Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition> items;
    Dictionary<BuildingStruct, RemoteFortressReader.BuildingDefinition> buildings;

    // Coordinate system stuff.
    public const float tileHeight = 3.0f;
    public const float floorHeight = 0.5f;
    public const float tileWidth = 2.0f;
    public const int blockSize = 16;

    static Object mapZOffsetLock = new Object();
    static int _mapZoffset = 0;
    public static int MapZOffset
    {
        get
        {
            lock (mapZOffsetLock)
            {
                return _mapZoffset;
            }
        }
        set
        {
            lock (mapZOffsetLock)
            {
                _mapZoffset = value;
            }
        }
    }
    public static Vector3 DFtoUnityCoord(int x, int y, int z)
    {
        Vector3 outCoord = new Vector3(x * tileWidth, (z + MapZOffset) * tileHeight, y * (-tileWidth));
        return outCoord;
    }
    public static Vector3 DFtoUnityCoord(DFCoord input)
    {
        Vector3 outCoord = new Vector3(input.x * tileWidth, (input.z + MapZOffset) * tileHeight, input.y * (-tileWidth));
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
        return new DFCoord(x, y, z - MapZOffset);
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
        // Initialize materials, if available
        if (DFConnection.Instance.NetMaterialList != null)
        {
            if (materials == null)
                materials = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
            materials.Clear();
            foreach (RemoteFortressReader.MaterialDefinition material in DFConnection.Instance.NetMaterialList.material_list)
            {
                materials[material.mat_pair] = material;
            }
            SaveMaterialList(DFConnection.Instance.NetMaterialList.material_list, "MaterialList.csv");
        }
        // Initialize items, if available
        if (DFConnection.Instance.NetItemList != null)
        {
            if (items == null)
                items = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
            items.Clear();
            foreach (RemoteFortressReader.MaterialDefinition material in DFConnection.Instance.NetItemList.material_list)
            {
                items[material.mat_pair] = material;
            }
            SaveMaterialList(DFConnection.Instance.NetItemList.material_list, "ItemList.csv");
        }
        if (DFConnection.Instance.NetBuildingList != null)
        {
            if (buildings == null)
                buildings = new Dictionary<BuildingStruct, BuildingDefinition>();
            buildings.Clear();
            foreach (RemoteFortressReader.BuildingDefinition building in DFConnection.Instance.NetBuildingList.building_list)
            {
                buildings[building.building_type] = building;
            }
            SaveBuildingList();
        }

        SaveTileTypeList();


        UpdateView();

        blockListTimer.Start();
        cullTimer.Start();
        lazyLoadTimer.Start();

        InitializeBlocks();
    }

    // Run once per frame.
    void Update()
    {
        //UpdateView();
        ShowCursorInfo();
        UpdateRequestRegion();
        blockListTimer.Reset();
        blockListTimer.Start();
        UpdateCreatures();
        UpdateBlocks();

        DrawBlocks();
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
        if (DFConnection.Instance.EmbarkMapSize != mapSize)
        {
            ClearMap();
            InitializeBlocks();
            mapSize = DFConnection.Instance.EmbarkMapSize;
            DFConnection.Instance.RequestMapReset();
        }
        if (DFConnection.Instance.EmbarkMapPosition != mapPosition)
        {
            ClearMap();
            mapPosition = DFConnection.Instance.EmbarkMapPosition;
            MapZOffset = DFConnection.Instance.EmbarkMapPosition.z;
            DFConnection.Instance.RequestMapReset();
        }
        if (MapDataStore.MapSize.x < 48)
            return;
        loadWatch.Reset();
        loadWatch.Start();
        while (true)
        {
            RemoteFortressReader.MapBlock block = DFConnection.Instance.PopLandscapeMapBlockUpdate();
            if (block == null) break;
            MapDataStore.Main.StoreTiles(block);
            SetDirtyBlock(block.map_x, block.map_y, block.map_z);
        }
        while (true)
        {
            RemoteFortressReader.MapBlock block = DFConnection.Instance.PopLiquidMapBlockUpdate();
            if (block == null) break;
            MapDataStore.Main.StoreTiles(block);
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
        int blockSizeX = DFConnection.Instance.EmbarkMapSize.x;
        int blockSizeY = DFConnection.Instance.EmbarkMapSize.y;
        int blockSizeZ = DFConnection.Instance.EmbarkMapSize.z;
        MapZOffset = DFConnection.Instance.EmbarkMapPosition.z;

        blocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        stencilBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        transparentBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        liquidBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ, 2];
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
        if (DFConnection.Instance.NetTiletypeList == null)
            return;
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
    void SaveBuildingList()
    {
        if (DFConnection.Instance.NetBuildingList == null)
            return;
        try
        {
            File.Delete("BuildingList.csv");
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter("BuildingList.csv"))
        {
            foreach (var item in DFConnection.Instance.NetBuildingList.building_list)
            {
                writer.WriteLine(
                    item.name + ";" +
                    item.id + ";" +
                    item.building_type.building_type + ":" +
                    item.building_type.building_subtype + ":" +
                    item.building_type.building_custom
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
            var newMeshes = mesher.Dequeue().Value;
            int block_x = newMeshes.location.x / blockSize;
            int block_y = newMeshes.location.y / blockSize;
            int block_z = newMeshes.location.z;
            if (newMeshes.tiles != null)
            {
                if (blocks[block_x, block_y, block_z] == null)
                {
                    blocks[block_x, block_y, block_z] = new Mesh();
                }
                Mesh tileMesh = blocks[block_x, block_y, block_z];
                tileMesh.Clear();
                newMeshes.tiles.CopyToMesh(tileMesh);
            }
            if (newMeshes.stencilTiles != null)
            {
                if (stencilBlocks[block_x, block_y, block_z] == null)
                {

                    stencilBlocks[block_x, block_y, block_z] = new Mesh();
                }
                Mesh stencilMesh = stencilBlocks[block_x, block_y, block_z];
                stencilMesh.Clear();
                newMeshes.stencilTiles.CopyToMesh(stencilMesh);
            }
            if (newMeshes.transparentTiles != null)
            {
                if (transparentBlocks[block_x, block_y, block_z] == null)
                {

                    transparentBlocks[block_x, block_y, block_z] = new Mesh();
                }
                Mesh transparentMesh = transparentBlocks[block_x, block_y, block_z];
                transparentMesh.Clear();
                newMeshes.transparentTiles.CopyToMesh(transparentMesh);
            }
            if (newMeshes.water != null)
            {
                if (liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX] == null)
                {
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX] = new Mesh();
                }
                Mesh waterMesh = liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX];
                waterMesh.Clear();
                newMeshes.water.CopyToMesh(waterMesh);
            }
            if (newMeshes.magma != null)
            {
                if (liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX] == null)
                {
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX] = new Mesh();
                }
                Mesh magmaMesh = liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX];
                magmaMesh.Clear();
                newMeshes.magma.CopyToMesh(magmaMesh);
            }
        }
    }


    System.Diagnostics.Stopwatch loadWatch = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch genWatch = new System.Diagnostics.Stopwatch();



    void ClearMap()
    {
        foreach (var item in blocks)
        {
            if (item != null)
                item.Clear();
        }
        foreach (var item in stencilBlocks)
        {
            if (item != null)
                item.Clear();
        }
        foreach (var item in transparentBlocks)
        {
            if (item != null)
                item.Clear();
        }
        foreach (var item in liquidBlocks)
        {
            if (item != null)
                item.Clear();
        }
        foreach (var item in magmaGlow)
        {
            Destroy(item);
        }
        if (MapDataStore.Main != null)
            MapDataStore.Main.Reset();
    }

    void ShowCursorInfo()
    {
        if (MapDataStore.Main == null)
            return; //No 

        StringBuilder statusText = new StringBuilder();

        statusText.Append("Cursor: ");
        statusText.Append(cursX).Append(",");
        statusText.Append(cursY).Append(",");
        statusText.Append(cursZ).Append("\n");
        var tile = MapDataStore.Main[cursX, cursY, cursZ];
        if (tile != null)
        {
            statusText.Append("Tiletype:\n");
            var tiletype = DFConnection.Instance.NetTiletypeList.tiletype_list
                [tile.tileType];
            statusText.Append(tiletype.name).Append("\n");
            statusText.Append(
                tiletype.shape).Append(":").Append(
                tiletype.special).Append(":").Append(
                tiletype.material).Append(":").Append(
                tiletype.variant).Append(":").Append(
                tiletype.direction).Append("\n");

            statusText.Append(tile.WallBuildingSides).Append("\n");

            var mat = tile.material;
            statusText.Append("Material: ");
            statusText.Append(mat.mat_type).Append(",");
            statusText.Append(mat.mat_index).Append("\n");

            if (materials.ContainsKey(mat))
            {
                statusText.Append("Material Name: ");
                statusText.Append(materials[mat].id).Append("\n");
            }
            else
                statusText.Append("Unknown Material\n");

            statusText.Append("\n");

            var basemat = tile.base_material;
            statusText.Append("Base Material: ");
            statusText.Append(basemat.mat_type).Append(",");
            statusText.Append(basemat.mat_index).Append("\n");

            if (materials.ContainsKey(basemat))
            {
                statusText.Append("Base Material Name: ");
                statusText.Append(materials[basemat].id).Append("\n");
            }
            else
                statusText.Append("Unknown Base Material\n");

            statusText.Append("\n");

            var layermat = tile.layer_material;
            statusText.Append("Layer Material: ");
            statusText.Append(layermat.mat_type).Append(",");
            statusText.Append(layermat.mat_index).Append("\n");

            if (materials.ContainsKey(layermat))
            {
                statusText.Append("Layer Material Name: ");
                statusText.Append(materials[layermat].id).Append("\n");
            }
            else
                statusText.Append("Unknown Layer Material\n");

            statusText.Append("\n");

            var veinmat = tile.vein_material;
            statusText.Append("Vein Material: ");
            statusText.Append(veinmat.mat_type).Append(",");
            statusText.Append(veinmat.mat_index).Append("\n");

            if (materials.ContainsKey(veinmat))
            {
                statusText.Append("Vein Material Name: ");
                statusText.Append(materials[veinmat].id).Append("\n");
            }
            else
                statusText.Append("Unknown Vein Material\n");

            statusText.Append("\n");

            var cons = tile.construction_item;
            statusText.Append("Construction Item: ");
            statusText.Append(cons.mat_type).Append(",");
            statusText.Append(cons.mat_index).Append("\n");

            if (items.ContainsKey(cons))
            {
                statusText.Append("Construction Item Name: ");
                statusText.Append(items[cons].id).Append("\n");
            }
            else
                statusText.Append("Unknown Construction Item\n");

            statusText.Append("\n");

            if (tile.buildingType != default(BuildingStruct))
            {
                if (buildings.ContainsKey(tile.buildingType))
                    statusText.Append("Building: ");
                statusText.Append(buildings[tile.buildingType].id).Append("\n");

                if (materials.ContainsKey(tile.buildingMaterial))
                {
                    statusText.Append("Building Material: ");
                    statusText.Append(materials[tile.buildingMaterial].id).Append("\n");
                }
                else
                    statusText.Append("Unknown Building Material\n");

                statusText.Append("Building Coord: ");
                statusText.Append(tile.buildingLocalPos).Append("\n");
                statusText.Append("Building Direction: ").Append(tile.buildingDirection).Append("\n");

            }
        }

        if (unitList != null)
            foreach (UnitDefinition unit in unitList.creature_list)
            {
                UnitFlags1 flags1 = (UnitFlags1)unit.flags1;
                UnitFlags2 flags2 = (UnitFlags2)unit.flags2;
                UnitFlags3 flags3 = (UnitFlags3)unit.flags3;

                if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead) ||
                    ((flags1 & UnitFlags1.left) == UnitFlags1.left))
                    continue;
                if (unit.pos_x != cursX || unit.pos_y != cursY || unit.pos_z != cursZ)
                    continue;

                CreatureRaw creatureRaw = null;
                if (DFConnection.Instance.NetCreatureRawList != null)
                    creatureRaw = DFConnection.Instance.NetCreatureRawList.creature_raws[unit.race.mat_type];

                if (creatureRaw != null)
                {
                    statusText.Append("Unit:   \n");

                    statusText.Append("Race: ");
                    statusText.Append(creatureRaw.creature_id + ":");
                    statusText.Append(creatureRaw.caste[unit.race.mat_index].caste_id);
                    statusText.Append("\n");

                    statusText.Append(flags1).AppendLine();
                    statusText.Append(flags2).AppendLine();
                    statusText.Append(flags3).AppendLine();
                }
                break;
            }
        cursorProperties.text = statusText.ToString();
    }

    Dictionary<int, Transform> creatureList;
    public Transform creatureTemplate;

    RemoteFortressReader.UnitList unitList = null;

    void UpdateCreatures()
    {
        unitList = DFConnection.Instance.PopUnitListUpdate();
        if (unitList == null) return;
        foreach (var unit in unitList.creature_list)
        {
            if (creatureList == null)
                creatureList = new Dictionary<int, Transform>();
            UnitFlags1 flags1 = (UnitFlags1)unit.flags1;
            //UnitFlags2 flags2 = (UnitFlags2)unit.flags2;
            //UnitFlags3 flags3 = (UnitFlags3)unit.flags3;
            if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead) ||
                ((flags1 & UnitFlags1.left) == UnitFlags1.left))
            {
                if (creatureList.ContainsKey(unit.id))
                {
                    Destroy(creatureList[unit.id].gameObject);
                    creatureList.Remove(unit.id);
                }
            }
            else
            {
                CreatureRaw creatureRaw = null;
                if (DFConnection.Instance.NetCreatureRawList != null)
                    creatureRaw = DFConnection.Instance.NetCreatureRawList.creature_raws[unit.race.mat_type];

                if (!creatureList.ContainsKey(unit.id))
                {
                    creatureList[unit.id] = Instantiate(creatureTemplate);
                    creatureList[unit.id].transform.parent = gameObject.transform;
                    creatureList[unit.id].name = "Unit_" + unit.id;
                    creatureList[unit.id].GetComponentInChildren<AtlasSprite>().ClearMesh();

                    Color color = Color.white;
                    if (unit.profession_color != null)
                        color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1);

                    if (creatureRaw != null)
                        creatureList[unit.id].GetComponentInChildren<AtlasSprite>().AddTile(creatureRaw.creature_tile, color);

                }
                creatureList[unit.id].gameObject.SetActive(unit.pos_z < PosZ && unit.pos_z >= (PosZ - cameraViewDist));
                if (creatureList[unit.id].gameObject.activeSelf) //Only update stuff if it's actually visible.
                {
                    Vector3 position = DFtoUnityCoord(unit.pos_x, unit.pos_y, unit.pos_z);
                    if ((flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
                    {
                        creatureList[unit.id].transform.position = position + new Vector3(0, 0.51f, 0);
                        creatureList[unit.id].GetComponentInChildren<AtlasSprite>().cameraFacing.enabled = false;
                        creatureList[unit.id].transform.rotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        creatureList[unit.id].transform.position = position + new Vector3(0, 1.5f, 0);
                        creatureList[unit.id].GetComponentInChildren<AtlasSprite>().cameraFacing.enabled = true;
                    }
                    if (unit.profession_color != null)
                        creatureList[unit.id].GetComponentInChildren<AtlasSprite>().SetColor(0, new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1));
                    if (creatureRaw != null)
                    {
                        if (unit.is_soldier)
                            creatureList[unit.id].GetComponentInChildren<AtlasSprite>().SetTile(0, creatureRaw.creature_soldier_tile);
                        else
                            creatureList[unit.id].GetComponentInChildren<AtlasSprite>().SetTile(0, creatureRaw.creature_tile);
                    }
                }

            }
        }
    }

    public void UpdateCenter(Vector3 pos)
    {
        DFCoord dfPos = UnityToDFCoord(pos);
        posXTile = dfPos.x;
        posYTile = dfPos.y;
        if (posZ != dfPos.z + 1)
        {
            posZ = dfPos.z + 1;
        }
    }

    private void DrawBlocks()
    {
        if (blocks == null)
            return;
        int startX = PosXBlock - rangeX;
        int startY = PosYBlock - rangeY;
        int endX = PosXBlock + rangeX;
        int endY = PosYBlock + rangeY;
        if (startX < 0) startX = 0;
        if (startY < 0) startY = 0;
        if (endX > blocks.GetLength(0)) endX = blocks.GetLength(0);
        if (endY > blocks.GetLength(1)) endY = blocks.GetLength(1);

        for (int z = posZ - cameraViewDist; z < posZ; z++)
        {
            if (z < 0) z = 0;
            if (z >= blocks.GetLength(2))
                continue;
            for (int x = startX; x < endX; x++)
                for (int y = startY; y < endY; y++)
                {
                    if (blocks[x, y, z] != null && blocks[x, y, z].vertexCount > 0)
                        Graphics.DrawMesh(blocks[x, y, z], Vector3.zero, Quaternion.identity, basicTerrainMaterial, 0, null, 0, null, ShadowCastingMode.On, true, transform);

                    if (stencilBlocks[x, y, z] != null && stencilBlocks[x, y, z].vertexCount > 0)
                        Graphics.DrawMesh(stencilBlocks[x, y, z], Vector3.zero, Quaternion.identity, stencilTerrainMaterial, 1, null, 0, null, ShadowCastingMode.On, true, transform);

                    if (transparentBlocks[x, y, z] != null && transparentBlocks[x, y, z].vertexCount > 0)
                        Graphics.DrawMesh(transparentBlocks[x, y, z], Vector3.zero, Quaternion.identity, transparentTerrainMaterial, 1, null, 0, null, ShadowCastingMode.On, true, transform);

                    if (liquidBlocks[x, y, z, MapDataStore.WATER_INDEX] != null && liquidBlocks[x, y, z, MapDataStore.WATER_INDEX].vertexCount > 0)
                        Graphics.DrawMesh(liquidBlocks[x, y, z, MapDataStore.WATER_INDEX], Vector3.zero, Quaternion.identity, waterMaterial, 4, null, 0, null, ShadowCastingMode.On, true, transform);

                    if (liquidBlocks[x, y, z, MapDataStore.MAGMA_INDEX] != null && liquidBlocks[x, y, z, MapDataStore.MAGMA_INDEX].vertexCount > 0)
                        Graphics.DrawMesh(liquidBlocks[x, y, z, MapDataStore.MAGMA_INDEX], Vector3.zero, Quaternion.identity, magmaMaterial, 4, null, 0, null, ShadowCastingMode.On, true, transform);
                }
        }
        for (int z = posZ; z <= posZ + cameraViewDist; z++)
        {
            if (z < 0) z = 0;
            if (z >= blocks.GetLength(2))
                continue;
            for (int x = startX; x < endX; x++)
                for (int y = startY; y < endY; y++)
                {
                    if (blocks[x, y, z] != null && blocks[x, y, z].vertexCount > 0 && BasicTopMaterial != null)
                        Graphics.DrawMesh(blocks[x, y, z], Vector3.zero, Quaternion.identity, BasicTopMaterial, 0, null, 0, null, ShadowCastingMode.On, true, transform);

                    if (stencilBlocks[x, y, z] != null && stencilBlocks[x, y, z].vertexCount > 0 && StencilTopMaterial != null)
                        Graphics.DrawMesh(stencilBlocks[x, y, z], Vector3.zero, Quaternion.identity, StencilTopMaterial, 1, null, 0, null, ShadowCastingMode.On, true, transform);

                    //if (transparentBlocks[x, y, z] != null && transparentBlocks[x, y, z].vertexCount > 0 && StencilTopMaterial != null)
                    //    Graphics.DrawMesh(transparentBlocks[x, y, z], Vector3.zero, Quaternion.identity, TransparentTopMaterial, 1, null, 0, null, ShadowCastingMode.On, true, transform);

                    //if (liquidBlocks[x, y, z, MapDataStore.WATER_INDEX] != null && liquidBlocks[x, y, z, MapDataStore.WATER_INDEX].vertexCount > 0)
                    //    Graphics.DrawMesh(liquidBlocks[x, y, z, MapDataStore.WATER_INDEX], Matrix4x4.identity, waterMaterial, 4);

                    //if (liquidBlocks[x, y, z, MapDataStore.MAGMA_INDEX] != null && liquidBlocks[x, y, z, MapDataStore.MAGMA_INDEX].vertexCount > 0)
                    //    Graphics.DrawMesh(liquidBlocks[x, y, z, MapDataStore.MAGMA_INDEX], Matrix4x4.identity, magmaMaterial, 4);
                }
        }
    }

}