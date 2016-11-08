using Collada141;
using DFHack;
using MapGen;
using RemoteFortressReader;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using UnitFlags;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// The class responsible for talking to DF and meshing the data it gets.
// Relevant vocabulary: A "map tile" is an individual square on the map.
// A "map block" is a 16x16x1 area of map tiles stored by DF; think minecraft "chunks".

public class GameMap : MonoBehaviour
{
    //Atlas materials
    public Material basicTerrainMaterial;   // Can be any terrain you want.
    public Material stencilTerrainMaterial; // Foliage & other stenciled materials.
    public Material transparentTerrainMaterial; // Anything with partial transparency.

    //Texture array materials
    public Material basicTerrainArrayMaterial;   // Can be any terrain you want.
    public Material stencilTerrainArrayMaterial; // Foliage & other stenciled materials.
    public Material transparentTerrainArrayMaterial; // Anything with partial transparency.

    bool arrayTextures = false;

    // Things to be set from the Unity Editor.
    public Material BasicTerrainMaterial
    {
        get
        {
            if (arrayTextures)
                return basicTerrainArrayMaterial;
            return basicTerrainMaterial;
        }
    }
    public Material StencilTerrainMaterial
    {
        get
        {
            if (arrayTextures)
                return stencilTerrainArrayMaterial;
            return stencilTerrainMaterial;
        }
    }
    public Material TransparentTerrainMaterial
    {
        get
        {
            if (arrayTextures)
                return transparentTerrainArrayMaterial;
            return transparentTerrainMaterial;
        }
    }
    public Material waterMaterial;
    public Material magmaMaterial;

    public Light magmaGlowPrefab;
    public Text cursorProperties;

    public bool overheadShadows = true;

    public bool firstPerson = false;

    public int cursX = -30000;
    public int cursY = -30000;
    public int cursZ = -30000;

    public WorldMapMaker worldMap;

    public RectTransform optionsPanel;

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

    // Stored view information
    ViewInfo view;

    BlockMesher mesher;

    // The actual unity meshes used to draw things on screen.
    Mesh[,,] blocks;         // Terrain data.
    Mesh[,,] stencilBlocks;  // Foliage &ct.
    Mesh[,,] transparentBlocks;  // Glass &ct.
    Mesh[,,] topBlocks;         // Terrain data.
    Mesh[,,] topStencilBlocks;  // Foliage &ct.
    Mesh[,,] topTransparentBlocks;  // Glass &ct.
    Mesh[,,,] liquidBlocks; // Water & magma. Extra dimension is a liquid type.
    MeshCollider[,,] collisionBlocks;
    public MeshCollider collisionTemplate;
    // Dirty flags for those meshes
    bool[,,] blockDirtyBits;
    bool[,,] blockContentBits;
    UpdateSchedule[,,] blockUpdateSchedules;
    bool[,,] liquidBlockDirtyBits;
    bool[] spatterBlockDirtyBits;
    Texture2D[] spatterLayers;
    // Lights from magma.
    Light[,,] magmaGlow;

    DFCoord mapSize; //This is to keep track of changing size of the map.
    DFCoord mapPosition;

    // Stuff to let the material list & various meshes & whatnot be loaded from xml specs at runtime.
    Dictionary<MatPairStruct, MaterialDefinition> materials;
    Dictionary<MatPairStruct, MaterialDefinition> items;
    Dictionary<BuildingStruct, BuildingDefinition> buildings;
    Dictionary<MatPairStruct, MaterialDefinition> creatures;

    //Items list. Pretty simple right now.
    Dictionary<int, Item> itemInstances = new Dictionary<int, Item>();
    Dictionary<DFCoord, Item> itemPositions = new Dictionary<DFCoord, Item>();

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
    public static Vector3 UnityToFloatingDFCoord(Vector3 input)
    {
        float x = input.x / tileWidth;
        float y = input.z / -tileWidth;
        float z = input.y / tileHeight;
        return new Vector3(x + 0.5f, y + 0.5f, z - MapZOffset);
    }
    public static bool IsBlockCorner(DFCoord input)
    {
        return input.x % blockSize == 0 &&
               input.y % blockSize == 0;
    }

    // Does about what you'd think it does.
    void Start()
    {
        enabled = false;

        DFConnection.RegisterConnectionCallback(OnConnectToDF);

        dfScreen.SetActive(GameSettings.Instance.game.showDFScreen);

        Texture2D clear = ContentLoader.CreateFlatTexture(Color.clear);


        spatterID = Shader.PropertyToID("_SpatterTex");
        BasicTerrainMaterial.SetTexture(spatterID, clear);
        StencilTerrainMaterial.SetTexture(spatterID, clear);
        TransparentTerrainMaterial.SetTexture(spatterID, clear);
        sharedMatBlock = new MaterialPropertyBlock();


    }

    public static GameMap Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        arrayTextures = SystemInfo.supports2DArrayTextures;
        Instance = this;
    }

    void OnConnectToDF()
    {
        Debug.Log("Connected");
        enabled = true;
        mesher = BlockMesher.GetMesher(GameSettings.Instance.meshing.meshingThreads);
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
            if (GameSettings.Instance.debug.saveMaterialList)
                SaveMaterialList(materials, "MaterialList.csv");
        }
        // Initialize items, if available
        if (DFConnection.Instance.NetItemList != null)
        {
            if (items == null)
                items = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
            items.Clear();
            foreach (MaterialDefinition material in DFConnection.Instance.NetItemList.material_list)
            {
                items[material.mat_pair] = material;
            }
            if (GameSettings.Instance.debug.saveItemList)
                SaveMaterialList(items, "ItemList.csv");
        }
        if (DFConnection.Instance.NetBuildingList != null)
        {
            if (buildings == null)
                buildings = new Dictionary<BuildingStruct, BuildingDefinition>();
            buildings.Clear();
            foreach (BuildingDefinition building in DFConnection.Instance.NetBuildingList.building_list)
            {
                buildings[building.building_type] = building;
            }
            if (GameSettings.Instance.debug.saveBuildingList)
                SaveBuildingList();
        }
        if (DFConnection.Instance.NetCreatureRawList != null)
        {
            if (creatures == null)
                creatures = new Dictionary<MatPairStruct, MaterialDefinition>();
            foreach (CreatureRaw creatureRaw in DFConnection.Instance.NetCreatureRawList.creature_raws)
            {
                foreach (var caste in creatureRaw.caste)
                {
                    MatPairStruct creatureCaste = new MatPairStruct(creatureRaw.index, caste.index);
                    MaterialDefinition creatureDef = new MaterialDefinition();
                    creatureDef.mat_pair = creatureCaste;
                    creatureDef.id = creatureRaw.creature_id + ":" + caste.caste_id;
                    creatureDef.name = caste.caste_name[0];
                    creatureDef.state_color = creatureRaw.color;
                    creatures[creatureCaste] = creatureDef;
                }
            }
            if (GameSettings.Instance.debug.saveCreatureList)
                SaveMaterialList(creatures, "CreatureList.csv");
        }

        if (GameSettings.Instance.debug.saveTiletypeList)
            SaveTileTypeList();

        if (GameSettings.Instance.debug.savePlantList)
            SavePlantList();

        UpdateView();

        InitializeBlocks();
    }

    public GameObject dfScreen;

    // Run once per frame.
    void Update()
    {

        if (Input.GetButtonDown("ToggleDF"))
        {
            GameSettings.Instance.game.showDFScreen = !GameSettings.Instance.game.showDFScreen;

            dfScreen.SetActive(GameSettings.Instance.game.showDFScreen);
        }

        var camera = FindObjectOfType<CameraMovement>();

        if ((camera != null && camera.following) || GameSettings.Instance.game.showDFScreen)
            UpdateView();


        if (!GameSettings.Instance.game.showDFScreen)
        {
            if (Input.GetButtonDown("ScaleUnits"))
            {
                GameSettings.Instance.units.scaleUnits = !GameSettings.Instance.units.scaleUnits;
            }
            if (Input.GetButtonDown("OverheadShadows"))
            {
                overheadShadows = !overheadShadows;
            }
            if (Input.GetButtonDown("FollowDF"))
            {
                camera.following = true;
            }
            if (Input.GetButtonDown("SaveMeshes") && Input.GetButton("Mod"))
            {
                SaveMeshes();
            }
            // take screenshot on up->down transition of F9 key
            if (Input.GetButtonDown("TakeScreenshot"))
            {
                string path = Application.dataPath;
                if (Application.platform == RuntimePlatform.OSXPlayer)
                {
                    path += "/../../";
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer
                    || Application.platform == RuntimePlatform.LinuxPlayer)
                {
                    path += "/../";
                }

                string screenshotFilename;
                do
                {
                    screenshotCount++;
                    screenshotFilename = path + "screenshot" + screenshotCount + ".png";

                } while (File.Exists(screenshotFilename));

                if (Input.GetButton("Mod"))
                    Application.CaptureScreenshot(screenshotFilename, 8);
                else
                    Application.CaptureScreenshot(screenshotFilename);
            }
            if (Input.GetButtonDown("Refresh"))
            {
                Refresh();
            }
            if (Input.GetButtonDown("Cancel"))
            {
                optionsPanel.gameObject.SetActive(!optionsPanel.gameObject.activeSelf);
            }
        }

        ShowCursorInfo();
        UpdateRequestRegion();
        UpdateCreatures();
        UpdateBlocks();
        UpdateSpatters();
        DrawBlocks();
        DrawItems();
    }

    public void Refresh()
    {
        for (int z = 0; z < blockDirtyBits.GetLength(2); z++)
            for (int x = 0; x < blockDirtyBits.GetLength(0); x++)
                for (int y = 0; y < blockDirtyBits.GetLength(1); y++)
                {
                    blockDirtyBits[x, y, z] = blockContentBits[x, y, z];
                    liquidBlockDirtyBits[x, y, z] = blockContentBits[x, y, z];
                    spatterBlockDirtyBits[z] = blockContentBits[x, y, z] || spatterBlockDirtyBits[z];
                }
    }

    public Mesh testMesh;

    private void SaveMeshes()
    {
        COLLADA exportScene = new COLLADA();

        List<geometry> geometryList = new List<geometry>();

        //if(testMesh!= null)
        //{
        //    geometry geo = COLLADA.MeshToGeometry(testMesh);
        //    if (geo != null)
        //        geometryList.Add(geo);
        //}

        Debug.Log("Starting mesh export");
        foreach (Mesh mesh in blocks)
        {
            if (mesh != null)
            {
                geometry geo = (COLLADA.MeshToGeometry(mesh));
                if (geo != null)
                    geometryList.Add(geo);
            }
        }
        Debug.Log("Added opaque blocks");
        foreach (Mesh mesh in stencilBlocks)
        {
            if (mesh != null)
            {
                geometry geo = (COLLADA.MeshToGeometry(mesh));
                if (geo != null)
                    geometryList.Add(geo);
            }
        }
        Debug.Log("Added stencil blocks");
        foreach (Mesh mesh in transparentBlocks)
        {
            if (mesh != null)
            {
                geometry geo = (COLLADA.MeshToGeometry(mesh));
                if (geo != null)
                    geometryList.Add(geo);
            }
        }
        Debug.Log("Added transparent blocks");
        foreach (Mesh mesh in liquidBlocks)
        {
            if (mesh != null)
            {
                geometry geo = (COLLADA.MeshToGeometry(mesh));
                if (geo != null)
                    geometryList.Add(geo);
            }
        }
        Debug.Log("Added liquid blocks");

        library_geometries geometryLib = new library_geometries();
        geometryLib.geometry = geometryList.ToArray();
        Debug.Log("Added geometry to library");

        library_visual_scenes visualSceneLib = new library_visual_scenes();
        visual_scene visualScene = new visual_scene();

        visualSceneLib.visual_scene = new visual_scene[1];
        visualSceneLib.visual_scene[0] = visualScene;

        visualScene.id = "Map";
        visualScene.name = "Map";
        visualScene.node = new node[geometryList.Count];
        for (int i = 0; i < geometryList.Count; i++)
        {
            node thisNode = new node();
            visualScene.node[i] = thisNode;
            geometry thisGeometry = geometryList[i];
            thisNode.id = thisGeometry.id.Remove(thisGeometry.id.Length - 4);
            thisNode.name = thisGeometry.name.Remove(thisGeometry.name.Length - 6);
            thisNode.sid = thisNode.id;

            thisNode.Items = new object[1];
            thisNode.Items[0] = COLLADA.ConvertMatrix(Matrix4x4.identity);

            thisNode.instance_geometry = new instance_geometry[1];
            thisNode.instance_geometry[0] = new instance_geometry();
            thisNode.instance_geometry[0].url = "#" + thisGeometry.id;
            thisNode.ItemsElementName = new ItemsChoiceType2[1];
            thisNode.ItemsElementName[0] = ItemsChoiceType2.matrix;
        }
        Debug.Log("Added geometry to scene");

        COLLADAScene sceneInstance = new COLLADAScene();
        sceneInstance.instance_visual_scene = new InstanceWithExtra();
        sceneInstance.instance_visual_scene.url = "#" + visualScene.id;

        exportScene.scene = sceneInstance;

        exportScene.Items = new object[2];
        exportScene.Items[0] = geometryLib;
        exportScene.Items[1] = visualSceneLib;

        asset assetHeader = new asset();
        assetHeader.unit = new assetUnit();
        assetHeader.unit.meter = 1;
        assetHeader.unit.name = "meter";
        assetHeader.up_axis = UpAxisType.Y_UP;

        exportScene.asset = assetHeader;
        Debug.Log("Setup Scene");

        if (File.Exists("Map.dae"))
            File.Delete("Map.dae");
        exportScene.Save("Map.dae");
        Debug.Log("Saved Scene");

        Texture2D mainTex = (Texture2D)BasicTerrainMaterial.GetTexture("_MainTex");

        Color[] mainTexPixels = mainTex.GetPixels();
        Color[] diffusePixels = new Color[mainTexPixels.Length];
        Color[] roughnessPixels = new Color[mainTexPixels.Length];

        for (int i = 0; i < mainTexPixels.Length; i++)
        {
            diffusePixels[i] = new Color(mainTexPixels[i].r, mainTexPixels[i].g, mainTexPixels[i].b, 1.0f);
            roughnessPixels[i] = new Color(mainTexPixels[i].a, mainTexPixels[i].a, mainTexPixels[i].a, 1.0f);
        }

        Texture2D diffuseTex = new Texture2D(mainTex.width, mainTex.height);
        Texture2D roughnessTex = new Texture2D(mainTex.width, mainTex.height);

        diffuseTex.SetPixels(diffusePixels);
        roughnessTex.SetPixels(roughnessPixels);

        diffuseTex.Apply();
        roughnessTex.Apply();

        byte[] diffuseBytes = diffuseTex.EncodeToPNG();
        byte[] roughnessBytes = roughnessTex.EncodeToPNG();

        File.WriteAllBytes("pattern.png", diffuseBytes);
        File.WriteAllBytes("specular.png", roughnessBytes);
        Debug.Log("Saved Maintex");

        Texture2D bumpMap = (Texture2D)BasicTerrainMaterial.GetTexture("_BumpMap");

        Color[] bumpMapPixels = bumpMap.GetPixels();
        Color[] normalMapPixels = new Color[bumpMapPixels.Length];
        Color[] ambientMapPixels = new Color[bumpMapPixels.Length];
        Color[] alphaMapPixels = new Color[bumpMapPixels.Length];

        for (int i = 0; i < bumpMapPixels.Length; i++)
        {
            normalMapPixels[i] = new Color(bumpMapPixels[i].a, bumpMapPixels[i].g, Mathf.Sqrt(1 - ((bumpMapPixels[i].a * 2 - 1) * (bumpMapPixels[i].a * 2 - 1)) + ((bumpMapPixels[i].g * 2 - 1) * (bumpMapPixels[i].g * 2 - 1))));
            ambientMapPixels[i] = new Color(bumpMapPixels[i].r, bumpMapPixels[i].r, bumpMapPixels[i].r, 1.0f);
            alphaMapPixels[i] = new Color(bumpMapPixels[i].b, bumpMapPixels[i].b, bumpMapPixels[i].b, 1.0f);
        }

        Texture2D normalTex = new Texture2D(bumpMap.width, bumpMap.height);
        Texture2D ambientTex = new Texture2D(bumpMap.width, bumpMap.height);
        Texture2D alphaTex = new Texture2D(bumpMap.width, bumpMap.height);

        normalTex.SetPixels(normalMapPixels);
        ambientTex.SetPixels(ambientMapPixels);
        alphaTex.SetPixels(alphaMapPixels);

        normalTex.Apply();
        ambientTex.Apply();
        alphaTex.Apply();

        byte[] normalBytes = normalTex.EncodeToPNG();
        byte[] ambientBytes = ambientTex.EncodeToPNG();
        byte[] alphaBytes = alphaTex.EncodeToPNG();

        File.WriteAllBytes("normal.png", normalBytes);
        File.WriteAllBytes("occlusion.png", ambientBytes);
        File.WriteAllBytes("alpha.png", alphaBytes);
        Debug.Log("Saved DetailTex");

        Debug.Log("Saved map!");
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

        Profiler.BeginSample("UpdateView", this);
        //Debug.Log("Got view");
        view = newView;

        if (view.follow_unit_id != -1 && lastUnitList != null)
        {
            foreach (var unit in lastUnitList.creature_list)
            {
                if (unit.id == view.follow_unit_id)
                {
                    posXTile = unit.pos_x;
                    posYTile = unit.pos_y;
                    posZ = unit.pos_z + 1;
                    return;
                }
            }
        }

        posXTile = (view.view_pos_x + (view.view_size_x / 2));
        posYTile = (view.view_pos_y + (view.view_size_y / 2));
        posZ = view.view_pos_z + 1;
        Profiler.EndSample();
    }

    // Update the region we're requesting
    void UpdateRequestRegion()
    {
        Profiler.BeginSample("UpdateRequestRegion", this);
        DFConnection.Instance.SetRequestRegion(
            new BlockCoord(
                PosXBlock - GameSettings.Instance.rendering.drawRangeSide,
                PosYBlock - GameSettings.Instance.rendering.drawRangeSide,
                posZ - GameSettings.Instance.rendering.drawRangeDown
            ),
            new BlockCoord(
                PosXBlock + GameSettings.Instance.rendering.drawRangeSide,
                PosYBlock + GameSettings.Instance.rendering.drawRangeSide,
                posZ + GameSettings.Instance.rendering.drawRangeUp
            ));
        Profiler.EndSample();
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
        Profiler.BeginSample("UpdateBlocks", this);
        while (true)
        {
            Profiler.BeginSample("PopMapBlockUpdate", this);
            RemoteFortressReader.MapBlock block = DFConnection.Instance.PopMapBlockUpdate();
            Profiler.EndSample();
            if (block == null) break;
            bool setTiles;
            bool setLiquids;
            bool setSpatters;
            Profiler.BeginSample("StoreTiles", this);
            MapDataStore.Main.StoreTiles(block, out setTiles, out setLiquids, out setSpatters);
            Profiler.EndSample();
            if (setTiles)
            {
                addSeasonalUpdates(block, block.map_x, block.map_y, block.map_z);
                SetDirtyBlock(block.map_x, block.map_y, block.map_z);
                SetBlockContent(block.map_x, block.map_y, block.map_z);
            }
            if (setLiquids)
            {
                SetDirtyLiquidBlock(block.map_x, block.map_y, block.map_z);
                SetBlockContent(block.map_x, block.map_y, block.map_z);
            }
            if (setSpatters)
            {
                SetDirtySpatterBlock(block.map_x, block.map_y, block.map_z);
            }
            foreach (var item in block.items)
            {
                itemInstances[item.id] = item;
            }
        }
        itemPositions.Clear();
        foreach (var item in itemInstances)
        {
            itemPositions[item.Value.pos] = item.Value;
        }
        DirtySeasonalBlocks();
        Profiler.BeginSample("EnqueueMeshUpdates", this);
        EnqueueMeshUpdates();
        Profiler.EndSample();

        mesher.Poll();

        FetchNewMeshes();
        Profiler.EndSample();
    }

    void SetMaterialBounds(Vector4 bounds)
    {
        BasicTerrainMaterial.SetVector("_WorldBounds", bounds);
        StencilTerrainMaterial.SetVector("_WorldBounds", bounds);
        TransparentTerrainMaterial.SetVector("_WorldBounds", bounds);
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
        topBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        topStencilBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        topTransparentBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        liquidBlocks = new Mesh[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ, 2];
        blockDirtyBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        blockContentBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        blockUpdateSchedules = new UpdateSchedule[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        liquidBlockDirtyBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        magmaGlow = new Light[blockSizeX * 16, blockSizeY * 16, blockSizeZ];
        collisionBlocks = new MeshCollider[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        spatterBlockDirtyBits = new bool[blockSizeZ];
        spatterLayers = new Texture2D[blockSizeZ];

        Vector3 min = DFtoUnityCoord(0, 0, 0) - new Vector3(tileWidth / 2, 0, -tileWidth / 2);
        Vector3 max = DFtoUnityCoord((blockSizeX * 16) - 1, (blockSizeY * 16) - 1, 0) + new Vector3(tileWidth / 2, 0, -tileWidth / 2);

        SetMaterialBounds(new Vector4(min.x, min.z, max.x, max.z));
    }

    void addSeasonalUpdates(RemoteFortressReader.MapBlock block, int mapBlockX, int mapBlockY, int mapBlockZ)
    {
        if (DFConnection.Instance.NetPlantRawList == null)
            return;
        mapBlockX /= blockSize;
        mapBlockY /= blockSize;
        if (blockUpdateSchedules[mapBlockX, mapBlockY, mapBlockZ] != null)
            blockUpdateSchedules[mapBlockX, mapBlockY, mapBlockZ].Clear();
        foreach (var material in block.materials)
        {
            if (material.mat_type != 419
                    || material.mat_index < 0
                    || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= material.mat_index
                    || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths.Count == 0)
                continue;
            PlantRaw plantRaw = DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index];
            if (blockUpdateSchedules[mapBlockX, mapBlockY, mapBlockZ] == null)
                blockUpdateSchedules[mapBlockX, mapBlockY, mapBlockZ] = new UpdateSchedule();
            var schedule = blockUpdateSchedules[mapBlockX, mapBlockY, mapBlockZ];
            foreach (TreeGrowth growth in plantRaw.growths)
            {
                schedule.Add(growth.timing_start);
                schedule.Add(growth.timing_end);
                foreach (GrowthPrint print in growth.prints)
                {
                    schedule.Add(print.timing_start);
                    schedule.Add(print.timing_end);
                }
            }
        }
    }

    void SetDirtyBlock(int mapBlockX, int mapBlockY, int mapBlockZ)
    {
        mapBlockX /= blockSize;
        mapBlockY /= blockSize;
        blockDirtyBits[mapBlockX, mapBlockY, mapBlockZ] = true;
    }
    void SetBlockContent(int mapBlockX, int mapBlockY, int mapBlockZ)
    {
        mapBlockX /= blockSize;
        mapBlockY /= blockSize;
        blockContentBits[mapBlockX, mapBlockY, mapBlockZ] = true;
    }
    void SetDirtyLiquidBlock(int mapBlockX, int mapBlockY, int mapBlockZ)
    {
        mapBlockX /= blockSize;
        mapBlockY /= blockSize;
        liquidBlockDirtyBits[mapBlockX, mapBlockY, mapBlockZ] = true;
    }
    private void SetDirtySpatterBlock(int map_x, int map_y, int map_z)
    {
        spatterBlockDirtyBits[map_z] = true;
    }


    #region Debug Data Saving

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


    void SaveMaterialList(IEnumerable<KeyValuePair<MatPairStruct, MaterialDefinition>> list, string filename)
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
                    item.Value.name + ";" +
                    item.Value.id + ";" +
                    item.Value.mat_pair.mat_type + ";" +
                    item.Value.mat_pair.mat_index
                    );
            }
        }
    }

    void SavePlantList()
    {
        if (DFConnection.Instance.NetPlantRawList == null)
            return;
        try
        {
            File.Delete("PlantList.csv");
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter("PlantList.csv"))
        {
            foreach (var plant in TokenLists.PlantTokenList.GrowthIDs)
            {
                foreach (var growth in plant.Value)
                {
                    foreach (var print in growth.Value)
                    {
                        writer.Write(plant.Key);
                        writer.Write(":");
                        writer.Write(growth.Key);
                        writer.Write(":");
                        writer.Write(print.Key);
                        writer.Write(";");
                        writer.Write(print.Value);
                        writer.Write(";");
                        //var index = print.Value;
                        //if (index.building_type >= 0)
                        //{
                        //    PlantRaw plantRaw = DFConnection.Instance.NetPlantRawList.plant_raws[index.building_type];
                        //    if (index.building_type >= 0 && index.building_subtype >= 0 && index.building_custom >= 0)
                        //    {
                        //        var printRaw = plantRaw.growths[index.building_subtype].prints[index.building_custom];
                        //        writer.Write(CharacterConverter.Convert((byte)printRaw.tile));
                        //    }
                        //    else
                        //    {
                        //        writer.Write(CharacterConverter.Convert((byte)plantRaw.tile));
                        //    }
                        //}
                        writer.WriteLine();
                    }
                }
            }
        }
    }
    #endregion

    void DirtySeasonalBlocks()
    {
        int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(0));
        int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(0));
        int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(1));
        int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(1));
        int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, blocks.GetLength(2));
        int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, blocks.GetLength(2));
        for (int zz = zmin; zz < zmax; zz++)
            for (int yy = ymin; yy < ymax; yy++)
                for (int xx = xmin; xx < xmax; xx++)
                {
                    if (blockUpdateSchedules[xx, yy, zz] != null && blockUpdateSchedules[xx, yy, zz].CheckUpdate(TimeHolder.DisplayedTime.CurrentYearTicks))
                        blockDirtyBits[xx, yy, zz] = true;
                }

    }

    void UpdateSpatters()
    {
        if (ContentLoader.Instance == null)
            return;
        Profiler.BeginSample("UpdateSpatters", this);
        for (int z = spatterBlockDirtyBits.Length - 1; z >= 0; z--)
        {
            if (spatterBlockDirtyBits[z])
            {
                GenerateSpatterTexture(z);
                spatterBlockDirtyBits[z] = false;
            }
        }
        Profiler.EndSample();
    }

    // Have the mesher mesh all dirty tiles in the region
    void EnqueueMeshUpdates()
    {
        int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(0));
        int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(0));
        int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(1));
        int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(1));
        int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, blocks.GetLength(2));
        int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, blocks.GetLength(2));
        for (int zz = posZ - 1; zz >= zmin; zz--)
        {
            if (zz >= blocks.GetLength(2))
                continue;

            for (int yy = ymin; yy < ymax; yy++)
                for (int xx = xmin; xx < xmax; xx++)
                {
                    if (!blockDirtyBits[xx, yy, zz] && !liquidBlockDirtyBits[xx, yy, zz])
                    {
                        continue;
                    }

                    //If we were not able to add it to the queue, don't try any more till next fame.
                    if (!mesher.Enqueue(new DFCoord(xx * 16, yy * 16, zz), blockDirtyBits[xx, yy, zz], liquidBlockDirtyBits[xx, yy, zz]))
                        return;
                    blockDirtyBits[xx, yy, zz] = false;
                    liquidBlockDirtyBits[xx, yy, zz] = false;
                    return;
                }
        }
        for (int zz = posZ; zz < zmax; zz++)
        {
            if (zz < 0)
                continue;
            for (int yy = ymin; yy < ymax; yy++)
                for (int xx = xmin; xx < xmax; xx++)
                {
                    if (!blockDirtyBits[xx, yy, zz] && !liquidBlockDirtyBits[xx, yy, zz])
                    {
                        continue;
                    }

                    //If we were not able to add it to the queue, don't try any more till next fame.
                    if (!mesher.Enqueue(new DFCoord(xx * 16, yy * 16, zz), blockDirtyBits[xx, yy, zz], liquidBlockDirtyBits[xx, yy, zz]))
                        return;
                    blockDirtyBits[xx, yy, zz] = false;
                    liquidBlockDirtyBits[xx, yy, zz] = false;
                    return;
                }
        }
    }

    // Get new meshes from the mesher
    void FetchNewMeshes()
    {
        Profiler.BeginSample("FetchNewMeshes", this);
        if (mesher.HasNewMeshes)
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
                    blocks[block_x, block_y, block_z].name = string.Format("block_solid_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh tileMesh = blocks[block_x, block_y, block_z];
                tileMesh.Clear();
                newMeshes.tiles.CopyToMesh(tileMesh);
            }
            if (newMeshes.topTiles != null)
            {
                if (topBlocks[block_x, block_y, block_z] == null)
                {
                    topBlocks[block_x, block_y, block_z] = new Mesh();
                    topBlocks[block_x, block_y, block_z].name = string.Format("block_solid_top_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh tileMesh = topBlocks[block_x, block_y, block_z];
                tileMesh.Clear();
                newMeshes.topTiles.CopyToMesh(tileMesh);
            }
            if (newMeshes.stencilTiles != null)
            {
                if (stencilBlocks[block_x, block_y, block_z] == null)
                {
                    stencilBlocks[block_x, block_y, block_z] = new Mesh();
                    stencilBlocks[block_x, block_y, block_z].name = string.Format("block_stencil_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh stencilMesh = stencilBlocks[block_x, block_y, block_z];
                stencilMesh.Clear();
                newMeshes.stencilTiles.CopyToMesh(stencilMesh);
            }
            if (newMeshes.topStencilTiles != null)
            {
                if (topStencilBlocks[block_x, block_y, block_z] == null)
                {
                    topStencilBlocks[block_x, block_y, block_z] = new Mesh();
                    topStencilBlocks[block_x, block_y, block_z].name = string.Format("block_stencil_top_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh stencilMesh = topStencilBlocks[block_x, block_y, block_z];
                stencilMesh.Clear();
                newMeshes.topStencilTiles.CopyToMesh(stencilMesh);
            }
            if (newMeshes.transparentTiles != null)
            {
                if (transparentBlocks[block_x, block_y, block_z] == null)
                {
                    transparentBlocks[block_x, block_y, block_z] = new Mesh();
                    transparentBlocks[block_x, block_y, block_z].name = string.Format("block_transparent_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh transparentMesh = transparentBlocks[block_x, block_y, block_z];
                transparentMesh.Clear();
                newMeshes.transparentTiles.CopyToMesh(transparentMesh);
            }
            if (newMeshes.topTransparentTiles != null)
            {
                if (topTransparentBlocks[block_x, block_y, block_z] == null)
                {
                    topTransparentBlocks[block_x, block_y, block_z] = new Mesh();
                    topTransparentBlocks[block_x, block_y, block_z].name = string.Format("block_transparent_top_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh transparentMesh = topTransparentBlocks[block_x, block_y, block_z];
                transparentMesh.Clear();
                newMeshes.topTransparentTiles.CopyToMesh(transparentMesh);
            }
            if (newMeshes.water != null)
            {
                if (liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX] == null)
                {
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX] = new Mesh();
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.WATER_INDEX].name = string.Format("liquid_water_{0}_{1}_{2}", block_x, block_y, block_z);
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
                    liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX].name = string.Format("liquid_magma_{0}_{1}_{2}", block_x, block_y, block_z);
                }
                Mesh magmaMesh = liquidBlocks[block_x, block_y, block_z, MapDataStore.MAGMA_INDEX];
                magmaMesh.Clear();
                newMeshes.magma.CopyToMesh(magmaMesh);
            }
            if (newMeshes.collisionMesh != null)
            {
                if (collisionBlocks[block_x, block_y, block_z] == null)
                {
                    collisionBlocks[block_x, block_y, block_z] = Instantiate(collisionTemplate);
                    collisionBlocks[block_x, block_y, block_z].name = string.Format("collisionBlock_{0}_{1}_{2}", block_x, block_y, block_z);
                    collisionBlocks[block_x, block_y, block_z].transform.position = DFtoUnityCoord(block_x * blockSize, block_y * blockSize, block_z);
                    collisionBlocks[block_x, block_y, block_z].transform.parent = this.transform;
                }
                Mesh collisionMesh = new Mesh();
                collisionMesh.name = string.Format("block_collision_{0}_{1}_{2}", block_x, block_y, block_z);
                newMeshes.collisionMesh.CopyToMesh(collisionMesh);
                collisionBlocks[block_x, block_y, block_z].sharedMesh = collisionMesh;
            }
        }
        Profiler.EndSample();
    }

    void GenerateSpatterTexture(int z)
    {
        Profiler.BeginSample("GenerateSpatterTexture", this);
        Color[] textureColors = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];

        for (int x = 0; x < MapDataStore.MapSize.x; x++)
            for (int y = 0; y < MapDataStore.MapSize.y; y++)
            {
                var tile = MapDataStore.Main[x, y, z];
                if (tile == null)
                    continue;
                if (tile.spatters == null || tile.spatters.Count == 0)
                    continue;

                if (tile.Hidden)
                    continue;

                Color totalColor = new Color(0, 0, 0, 0);

                float totalAmount = 0;

                int index = x + (y * MapDataStore.MapSize.x);

                foreach (var spatter in tile.spatters)
                {
                    if (spatter.amount == 0)
                        continue;

                    MapDataStore.Tile fakeTile = new MapDataStore.Tile(null, new DFCoord(0, 0, 0));

                    fakeTile.material = spatter.material;

                    Color color = Color.white;

                    ColorContent cont;

                    if (spatter.material.mat_type == (int)MatBasic.ICE && spatter.state == MatterState.Powder)
                    {
                        color = Color.white;
                    }
                    else if (ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out cont))
                    {
                        color = cont.value;
                    }
                    else if (materials.ContainsKey(spatter.material))
                    {
                        var colorDef = materials[spatter.material].state_color;
                        color = new Color32((byte)colorDef.red, (byte)colorDef.green, (byte)colorDef.blue, 255);
                    }
                    float amount = spatter.amount;
                    if (spatter.item != null)
                        amount /= 3000;
                    else
                        amount /= 100;
                    //amount = Mathf.Clamp01(amount);

                    color *= amount;

                    color.a = amount;

                    totalColor += color;
                    totalAmount += amount;
                }
                if (totalAmount > 1)
                {
                    totalColor /= totalAmount;
                }
                textureColors[index] = totalColor;
            }

        if (spatterLayers[z] == null)
            spatterLayers[z] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y);
        if (spatterLayers[z].width != MapDataStore.MapSize.x || spatterLayers[z].height != MapDataStore.MapSize.y)
            spatterLayers[z].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        spatterLayers[z].SetPixels(textureColors);
        spatterLayers[z].Apply();
        Profiler.EndSample();
    }

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
        if (creatureList != null)
        {
            foreach (var item in creatureList)
            {
                Destroy(item.Value.gameObject);
            }
            creatureList.Clear();
        }
        if (MapDataStore.Main != null)
            MapDataStore.Main.Reset();
    }

    void ShowCursorInfo()
    {
        if (MapDataStore.Main == null)
            return; //No 
        Profiler.BeginSample("ShowCursorInfo", this);

        StringBuilder statusText = new StringBuilder();

        if (cursX >= 0 && cursY >= 0 && cursZ >= 0 && GameSettings.Instance.debug.drawDebugInfo)
        {
            statusText.Append("Cursor: ");
            statusText.Append(cursX).Append(",");
            statusText.Append(cursY).Append(",");
            statusText.Append(cursZ).AppendLine();
            var tile = MapDataStore.Main[cursX, cursY, cursZ];
            if (tile != null)
            {
                statusText.Append("Tiletype:\n");
                var tiletype = DFConnection.Instance.NetTiletypeList.tiletype_list
                    [tile.tileType];
                statusText.Append(tiletype.name).AppendLine();
                statusText.Append(
                    tiletype.shape).Append(":").Append(
                    tiletype.special).Append(":").Append(
                    tiletype.material).Append(":").Append(
                    tiletype.variant).Append(":").Append(
                    tiletype.direction).AppendLine();

                statusText.Append("Tree: ").Append(tile.positionOnTree).Append(" (").Append(tile.trunkPercent).Append("%)").AppendLine();

                statusText.Append("Desingation: ").Append(tile.digDesignation).AppendLine();

                if (tile.Hidden)
                    statusText.Append("Hidden").AppendLine();

                statusText.Append(tile.WallBuildingSides).AppendLine();

                var mat = tile.material;
                statusText.Append("Material: ");
                statusText.Append(mat);

                if (materials.ContainsKey(mat))
                {
                    statusText.Append(", ");
                    statusText.Append(materials[mat].id).AppendLine();
                }
                else
                    statusText.AppendLine();

                statusText.AppendLine();

                var basemat = tile.base_material;
                statusText.Append("Base Material: ");
                statusText.Append(basemat);

                if (materials.ContainsKey(basemat))
                {
                    statusText.Append(", ");
                    statusText.Append(materials[basemat].id).AppendLine();
                }
                else
                    statusText.Append("Unknown Base Material\n");

                statusText.AppendLine();

                var layermat = tile.layer_material;
                statusText.Append("Layer Material: ");
                statusText.Append(layermat);

                if (materials.ContainsKey(layermat))
                {
                    statusText.Append(", ");
                    statusText.Append(materials[layermat].id).AppendLine();
                }
                else
                    statusText.Append("Unknown Layer Material\n");

                statusText.AppendLine();

                var veinmat = tile.vein_material;
                statusText.Append("Vein Material: ");
                statusText.Append(veinmat);

                if (materials.ContainsKey(veinmat))
                {
                    statusText.Append(", ");
                    statusText.Append(materials[veinmat].id).AppendLine();
                }
                else
                    statusText.Append("Unknown Vein Material\n");

                statusText.AppendLine();

                var cons = tile.construction_item;
                statusText.Append("Construction Item: ");
                statusText.Append(cons);

                if (items.ContainsKey(cons))
                {
                    statusText.Append(", ");
                    statusText.Append(items[cons].id).AppendLine();
                }
                else
                    statusText.Append("Unknown Construction Item\n");

                statusText.AppendLine();

                if (tile.buildingType != default(BuildingStruct))
                {
                    if (buildings.ContainsKey(tile.buildingType))
                        statusText.Append("Building: ");
                    statusText.Append(buildings[tile.buildingType].id).AppendLine();

                    if (materials.ContainsKey(tile.buildingMaterial))
                    {
                        statusText.Append("Building Material: ");
                        statusText.Append(materials[tile.buildingMaterial].id).AppendLine();
                    }
                    else
                        statusText.Append("Unknown Building Material\n");

                    statusText.Append("Building Coord: ");
                    statusText.Append(tile.buildingLocalPos).AppendLine();
                    statusText.Append("Building Direction: ").Append(tile.buildingDirection).AppendLine();

                }

                if (tile.spatters != null)
                    foreach (var spatter in tile.spatters)
                    {
                        string matString = ((MatPairStruct)spatter.material).ToString();
                        if (materials.ContainsKey(spatter.material))
                            matString = materials[spatter.material].id;
                        if (spatter.item != null)
                        {
                            string item = ((MatPairStruct)spatter.item).ToString();
                            if (spatter.item.mat_type == 55)//Plant Growth
                            {
                                item = DFConnection.Instance.NetPlantRawList.plant_raws[spatter.material.mat_index].growths[spatter.item.mat_index].id;
                            }
                            else if (items.ContainsKey(spatter.item))
                                item = items[spatter.item].id;
                            else if (items.ContainsKey(new MatPairStruct(spatter.item.mat_type, -1)))
                                item = items[new MatPairStruct(spatter.item.mat_type, -1)].id;
                            statusText.AppendFormat("{0} {1}: {2}", matString, item, spatter.amount).AppendLine();
                        }
                        else
                            statusText.AppendFormat("{0} {1}: {2}", matString, spatter.state, spatter.amount).AppendLine();
                    }
            }

            if (unitList != null)
                foreach (UnitDefinition unit in unitList.creature_list)
                {
                    UnitFlags1 flags1 = (UnitFlags1)unit.flags1;
                    UnitFlags2 flags2 = (UnitFlags2)unit.flags2;
                    //UnitFlags3 flags3 = (UnitFlags3)unit.flags3;

                    if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead)
                         || ((flags1 & UnitFlags1.left) == UnitFlags1.left)
                         || ((flags1 & UnitFlags1.caged) == UnitFlags1.caged)
                         || ((flags1 & UnitFlags1.forest) == UnitFlags1.forest)
                         )
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
                        statusText.AppendLine();

                        statusText.Append(flags1).AppendLine();
                        statusText.Append(flags2).AppendLine();
                        //statusText.Append(flags3).AppendLine();
                        statusText.Append("Length: ").Append(unit.size_info.length_cur).Append("/").Append(Mathf.FloorToInt(Mathf.Pow(creatureRaw.adultsize * 10000, 1.0f / 3.0f))).AppendLine();

                    }
                    break;
                }

            if(itemPositions.ContainsKey(new DFCoord(cursX, cursY, cursZ)))
            {
                Item item = itemPositions[new DFCoord(cursX, cursY, cursZ)];
                statusText.Append("Item ").Append(item.id).Append(": ");
                if (materials.ContainsKey(item.material))
                    statusText.Append(materials[item.material].id);
                else
                    statusText.Append(((MatPairStruct)item.material).ToString());
                statusText.Append(" ");
                if (items.ContainsKey(item.type))
                    statusText.Append(items[item.type].id);
                statusText.Append("(").Append(((MatPairStruct)item.type)).Append(")");
                statusText.AppendLine();

                statusText.Append(itemInstances.Count).Append(" items total.");
            }
        }
        cursorProperties.text = statusText.ToString();
        Profiler.EndSample();
    }

    Dictionary<int, Transform> creatureList;
    public Transform creatureTemplate;

    UnitList lastUnitList = null;
    UnitList unitList = null;
    private int screenshotCount;

    void UpdateCreatures()
    {
        if (!GameSettings.Instance.units.drawUnits)
            return;
        if (creatureTemplate == null)
            return;
        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        unitList = DFConnection.Instance.PopUnitListUpdate();
        if (unitList == null) return;
        Profiler.BeginSample("UpdateCreatures", this);
        lastUnitList = unitList;
        foreach (var unit in unitList.creature_list)
        {
            if (creatureList == null)
                creatureList = new Dictionary<int, Transform>();
            UnitFlags1 flags1 = (UnitFlags1)unit.flags1;
            //UnitFlags2 flags2 = (UnitFlags2)unit.flags2;
            //UnitFlags3 flags3 = (UnitFlags3)unit.flags3;
            if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead)
                 || ((flags1 & UnitFlags1.left) == UnitFlags1.left)
                 || ((flags1 & UnitFlags1.caged) == UnitFlags1.caged)
                 || ((flags1 & UnitFlags1.forest) == UnitFlags1.forest)
                 )
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
                MapDataStore.Tile tile = null;
                if (MapDataStore.Main != null)
                    tile = MapDataStore.Main[unit.pos_x, unit.pos_y, unit.pos_z];
                creatureList[unit.id].gameObject.SetActive(
                    unit.pos_z < PosZ && unit.pos_z >= (PosZ - GameSettings.Instance.rendering.drawRangeDown)
                    && (tile != null ? !tile.Hidden : false)
                    );

                if (creatureList[unit.id].gameObject.activeSelf) //Only update stuff if it's actually visible.
                {
                    Vector3 position = DFtoUnityCoord(unit.pos_x, unit.pos_y, unit.pos_z);
                    creatureList[unit.id].transform.position = position + new Vector3(0, 0.51f, 0);
                    float scale;
                    if (GameSettings.Instance.units.scaleUnits)
                        scale = unit.size_info.length_cur / 391.0f;
                    else
                        scale = 1;
                    creatureList[unit.id].transform.localScale = new Vector3(scale, scale, scale);
                    creatureList[unit.id].GetComponentInChildren<Light>().range = scale * 10;
                    AtlasSprite sprite = creatureList[unit.id].GetComponentInChildren<AtlasSprite>();
                    if ((flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
                    {
                        sprite.transform.localPosition = Vector3.zero;
                        sprite.cameraFacing.enabled = false;
                        sprite.transform.rotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        sprite.transform.localPosition = new Vector3(0, 1.0f, 0);
                        sprite.cameraFacing.enabled = true;
                    }
                    if (unit.profession_color != null)
                        creatureList[unit.id].GetComponentInChildren<AtlasSprite>().SetColor(0, new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1));
                    if (creatureRaw != null)
                    {
                        if (unit.is_soldier && creatureRaw.creature_soldier_tile != 0)
                            creatureList[unit.id].GetComponentInChildren<AtlasSprite>().SetTile(0, creatureRaw.creature_soldier_tile);
                        else
                            creatureList[unit.id].GetComponentInChildren<AtlasSprite>().SetTile(0, creatureRaw.creature_tile);
                        Text unitText = creatureList[unit.id].GetComponentInChildren<Text>();
                        if (unitText != null)
                        {
                            if (unit.name == "")
                                unitText.text = textInfo.ToTitleCase(creatureRaw.caste[unit.race.mat_index].caste_name[0]);
                            else
                            {
                                unitText.text = unit.name;
                            }
                        }
                    }
                }

            }
        }
        Profiler.EndSample();
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

    private bool DrawSingleBlock(int xx, int yy, int zz, bool phantom, Vector3 pos, bool top)
    {
        return DrawSingleBlock(xx, yy, zz, phantom, Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one), top);
    }

    MaterialPropertyBlock sharedMatBlock;
    int spatterID;

    private bool DrawSingleBlock(int xx, int yy, int zz, bool phantom, Matrix4x4 LocalTransform, bool top)
    {
        if (blocks[xx, yy, zz] == null
            && topBlocks[xx, yy, zz] == null
            && stencilBlocks[xx, yy, zz] == null
            && topStencilBlocks[xx, yy, zz] == null
            && transparentBlocks[xx, yy, zz] == null
            && topTransparentBlocks[xx, yy, zz] == null
            && liquidBlocks[xx, yy, zz, MapDataStore.WATER_INDEX] == null
            && liquidBlocks[xx, yy, zz, MapDataStore.MAGMA_INDEX] == null)
            return false;

        Profiler.BeginSample("DrawSingleBlock", this);

        MaterialPropertyBlock matBlock = null;
        if (spatterLayers[zz] != null)
        {
            matBlock = sharedMatBlock;
            matBlock.SetTexture(spatterID, spatterLayers[zz]);
        }

        bool drewBlock = false;
        if (blocks[xx, yy, zz] != null && blocks[xx, yy, zz].vertexCount > 0)
        {
            Graphics.DrawMesh(blocks[xx, yy, zz], LocalTransform, BasicTerrainMaterial, 0, null, 0, matBlock, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }
        if (topBlocks[xx, yy, zz] != null && topBlocks[xx, yy, zz].vertexCount > 0 && top)
        {
            Graphics.DrawMesh(topBlocks[xx, yy, zz], LocalTransform, BasicTerrainMaterial, 0, null, 0, matBlock, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }

        if (stencilBlocks[xx, yy, zz] != null && stencilBlocks[xx, yy, zz].vertexCount > 0)
        {
            Graphics.DrawMesh(stencilBlocks[xx, yy, zz], LocalTransform, StencilTerrainMaterial, 0, null, 0, matBlock, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }
        if (topStencilBlocks[xx, yy, zz] != null && topStencilBlocks[xx, yy, zz].vertexCount > 0 && top)
        {
            Graphics.DrawMesh(topStencilBlocks[xx, yy, zz], LocalTransform, StencilTerrainMaterial, 0, null, 0, matBlock, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }

        if (transparentBlocks[xx, yy, zz] != null && transparentBlocks[xx, yy, zz].vertexCount > 0 && !phantom)
        {
            Graphics.DrawMesh(transparentBlocks[xx, yy, zz], LocalTransform, TransparentTerrainMaterial, 0, null, 0, matBlock);
            drewBlock = true;
        }
        if (topTransparentBlocks[xx, yy, zz] != null && topTransparentBlocks[xx, yy, zz].vertexCount > 0 && !phantom && top)
        {
            Graphics.DrawMesh(topTransparentBlocks[xx, yy, zz], LocalTransform, TransparentTerrainMaterial, 0, null, 0, matBlock);
            drewBlock = true;
        }

        if (liquidBlocks[xx, yy, zz, MapDataStore.WATER_INDEX] != null && liquidBlocks[xx, yy, zz, MapDataStore.WATER_INDEX].vertexCount > 0 && !phantom)
        {
            Graphics.DrawMesh(liquidBlocks[xx, yy, zz, MapDataStore.WATER_INDEX], LocalTransform, waterMaterial, 4);
            drewBlock = true;
        }

        if (liquidBlocks[xx, yy, zz, MapDataStore.MAGMA_INDEX] != null && liquidBlocks[xx, yy, zz, MapDataStore.MAGMA_INDEX].vertexCount > 0 && !phantom)
        {
            Graphics.DrawMesh(liquidBlocks[xx, yy, zz, MapDataStore.MAGMA_INDEX], LocalTransform, magmaMaterial, 4);
            drewBlock = true;
        }
        Profiler.EndSample();
        return drewBlock;
    }

    private void DrawBlocks()
    {
        if (blocks == null)
            return;
        Profiler.BeginSample("DrawBlocks", this);
        int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(0));
        int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(0));
        int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(1));
        int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, blocks.GetLength(1));
        int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, blocks.GetLength(2));
        int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, blocks.GetLength(2));

        int drawnBlocks = 0;

        for (int zz = posZ - 1; zz >= zmin; zz--)
        {
            if (zz >= blocks.GetLength(2))
                continue;

            for (int xx = xmin; xx < xmax; xx++)
            {
                if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
                    break;
                for (int yy = ymin; yy < ymax; yy++)
                {
                    Vector3 pos = DFtoUnityCoord(xx * blockSize, yy * blockSize, zz);
                    if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
                        break;
                    if (DrawSingleBlock(xx, yy, zz, false, pos, zz == posZ - 1))
                        drawnBlocks++;
                }
            }
        }
        if (firstPerson || (overheadShadows && GameSettings.Instance.rendering.drawShadows))
            for (int zz = posZ; zz < zmax; zz++)
            {
                if (zz < 0)
                    continue;

                for (int xx = xmin; xx < xmax; xx++)
                {
                    if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
                        break;
                    for (int yy = ymin; yy < ymax; yy++)
                    {
                        if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
                            break;
                        Vector3 pos = DFtoUnityCoord(xx * blockSize, yy * blockSize, zz);

                        if (DrawSingleBlock(xx, yy, zz, (!firstPerson), pos, zz == posZ || zz == zmax - 1))
                            drawnBlocks++;
                    }
                }
            }
        StatsReadout.BlocksDrawn = drawnBlocks;
        Profiler.EndSample();
    }

    public ParticleSystem itemParticleSystem;
    ParticleSystem.Particle[] itemParticles;
    Dictionary<int, ParticleSystem> customItemParticleSystems = new Dictionary<int, ParticleSystem>();
    Dictionary<int, ParticleSystem.Particle[]> customItemParticles = new Dictionary<int, ParticleSystem.Particle[]>();
    Dictionary<int, int> customItemParticleCount = new Dictionary<int, int>();
    Dictionary<int, bool> noCustomParticleColor = new Dictionary<int, bool>();
    OpenSimplexNoise noise = new OpenSimplexNoise();
    void DrawItems()
    {
        if (ContentLoader.Instance == null)
            return;
        if (itemParticles == null)
        {
            itemParticles = new ParticleSystem.Particle[itemParticleSystem.maxParticles];
        }
        MapDataStore.Tile tempTile = new MapDataStore.Tile(null, new DFCoord(0, 0, 0));
        int i = 0;
        foreach (var count in customItemParticleSystems)
        {
            customItemParticleCount[count.Key] = 0;
        }
        foreach (var item in itemPositions)
        {
            var pos = item.Key;
            if (!(pos.z < PosZ && pos.z >= (PosZ - GameSettings.Instance.rendering.drawRangeDown)))
                continue;

            tempTile.material = item.Value.material;
            tempTile.construction_item = item.Value.type;
            ColorContent colorContent;
            MeshContent meshContent;

            var part = new ParticleSystem.Particle();
            part.startSize = 1;
            part.position = DFtoUnityCoord(item.Value.pos) + new Vector3(0, floorHeight + 0.5f, 0);
            if (ContentLoader.Instance.ColorConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out colorContent))
                part.startColor = colorContent.value;
            else if (materials.ContainsKey(item.Value.material) && materials[item.Value.material].state_color != null)
            {
                var stateColor = materials[item.Value.material].state_color;
                part.startColor = new Color32((byte)stateColor.red, (byte)stateColor.green, (byte)stateColor.blue, 255);
            }
            else
                part.startColor = Color.gray;

            if(item.Value.dye != null)
            {
                part.startColor *= (Color)(new Color32((byte)item.Value.dye.red, (byte)item.Value.dye.green, (byte)item.Value.dye.blue, 255));
            }

            if (ContentLoader.Instance.ItemMeshConfiguration != null && ContentLoader.Instance.ItemMeshConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out meshContent))
            {
                ParticleSystem partSys;
                if (!customItemParticleSystems.ContainsKey(meshContent.UniqueIndex))
                {
                    partSys = Instantiate(itemParticleSystem);
                    partSys.transform.parent = transform;
                    var renderer = partSys.GetComponent<ParticleSystemRenderer>();
                    Mesh mesh = new Mesh();
                    if (meshContent.MeshData.ContainsKey(MeshLayer.StaticCutout))
                    {
                        meshContent.MeshData[MeshLayer.StaticCutout].CopyToMesh(mesh);
                        noCustomParticleColor[meshContent.UniqueIndex] = false;
                    }
                    else if (meshContent.MeshData.ContainsKey(MeshLayer.NoMaterialCutout))
                    {
                        meshContent.MeshData[MeshLayer.NoMaterialCutout].CopyToMesh(mesh);
                        noCustomParticleColor[meshContent.UniqueIndex] = true;
                    }
                    else
                    {
                        bool copied = false;
                        foreach (var backup in meshContent.MeshData)
                        {
                            backup.Value.CopyToMesh(mesh);
                            noCustomParticleColor[meshContent.UniqueIndex] = false;
                            copied = true;
                            break;
                        }
                        if (!copied)
                            continue;
                    }
                    renderer.mesh = mesh;
                    if (meshContent.MaterialTexture != null)
                        renderer.material.SetTexture("_MainTex", meshContent.MaterialTexture.Texture);
                    else
                    {
                        TextureContent texCon;
                        if(ContentLoader.Instance.MaterialTextureConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out texCon))
                            renderer.material.SetTexture("_MainTex", texCon.Texture);
                    }
                    if (meshContent.ShapeTexture != null)
                        renderer.material.SetTexture("_BumpMap", meshContent.ShapeTexture.Texture);
                    else
                    {
                        NormalContent normalCon;
                        if(ContentLoader.Instance.ShapeTextureConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out normalCon))
                            renderer.material.SetTexture("_BumpMap", normalCon.Texture);
                    }
                    if (meshContent.SpecialTexture != null)
                        renderer.material.SetTexture("_SpecialTex", meshContent.SpecialTexture.Texture);
                    customItemParticleSystems[meshContent.UniqueIndex] = partSys;
                    customItemParticles[meshContent.UniqueIndex] = new ParticleSystem.Particle[partSys.maxParticles];
                    customItemParticleCount[meshContent.UniqueIndex] = 0;
                }
                if (meshContent.Rotation == RotationType.Random)
                    part.rotation = (float)noise.eval(pos.x, pos.y, pos.z) * 360;
                part.position = DFtoUnityCoord(item.Value.pos) + new Vector3(0, floorHeight, 0);
                if (noCustomParticleColor[meshContent.UniqueIndex])
                    part.startColor = Color.gray;
                customItemParticles[meshContent.UniqueIndex][customItemParticleCount[meshContent.UniqueIndex]] = part;
                customItemParticleCount[meshContent.UniqueIndex]++;
            }
            else
            {
                part.position = DFtoUnityCoord(item.Value.pos) + new Vector3(0, floorHeight + 0.5f, 0);
                itemParticles[i] = part;
                i++;
            }
        }
        itemParticleSystem.SetParticles(itemParticles, i);
        foreach (var sys in customItemParticleSystems)
        {
            sys.Value.SetParticles(customItemParticles[sys.Key], customItemParticleCount[sys.Key]);
        }
    }

}