using DF.Enums;
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
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using Newtonsoft.Json;

// The class responsible for talking to DF and meshing the data it gets.
// Relevant vocabulary: A "map tile" is an individual square on the map.
// A "map block" is a 16x16x1 area of map tiles stored by DF; think minecraft "chunks".

public class GameMap : MonoBehaviour
{
    public CanvasGroup helpOverlay;

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
    int posXTile = 0;
    int posYTile = 0;
    int posZ = 0;
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

    BlockMeshSet[,,] mapMeshes;

    // Dirty flags for those meshes
    bool[,,] blockDirtyBits;
    bool[,,] blockContentBits;
    bool[] layerDirtyBits;
    bool[] grassLayerDirtyBits;
    UpdateSchedule[,,] blockUpdateSchedules;
    bool[,,] liquidBlockDirtyBits;
    bool[] spatterBlockDirtyBits;
    Texture2D[] spatterLayers;
    Texture2D[] terrainSplatLayers;
    Texture2D[] terrainTintLayers;
    Texture2D[] grassSplatLayers;
    Texture2D[] grassTintLayers;
    // Lights from magma.
    Light[,,] magmaGlow;

    DFCoord mapSize; //This is to keep track of changing size of the map.
    DFCoord mapPosition;

    // Stuff to let the material list & various meshes & whatnot be loaded from xml specs at runtime.
    public static Dictionary<MatPairStruct, MaterialDefinition> materials;
    public static Dictionary<MatPairStruct, MaterialDefinition> items;
    public static Dictionary<BuildingStruct, BuildingDefinition> buildings;
    public static Dictionary<MatPairStruct, MaterialDefinition> creatures;

    // Coordinate system stuff.
    public const float tileHeight = 3.0f;
    public const float floorHeight = 0.5f;
    public const float tileWidth = 2.0f;
    public const int blockSize = 16;

    static object mapZOffsetLock = new object();
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

        Debug.Log("Started Armok Vision version " + BuildSettings.Instance.content_version + BuildManifest.Instance.buildNumber);

        Debug.Log("scmCommitId: " + BuildManifest.Instance.scmCommitId);
        Debug.Log("scmBranch: " + BuildManifest.Instance.scmBranch);
        Debug.Log("buildNumber: " + BuildManifest.Instance.buildNumber);
        Debug.Log("buildStartTime: " + BuildManifest.Instance.buildStartTime);
        Debug.Log("projectId: " + BuildManifest.Instance.projectId);
        Debug.Log("bundleId: " + BuildManifest.Instance.bundleId);
        Debug.Log("unityVersion: " + BuildManifest.Instance.unityVersion);
        Debug.Log("xcodeVersion: " + BuildManifest.Instance.xcodeVersion);
        Debug.Log("cloudBuildTargetName: " + BuildManifest.Instance.cloudBuildTargetName);

        DFConnection.RegisterConnectionCallback(OnConnectToDF);

        dfScreen.SetActive(GameSettings.Instance.game.showDFScreen);

        spatterID = Shader.PropertyToID("_SpatterTex");
        terrainSplatID = Shader.PropertyToID("_Control");
        terrainTintID = Shader.PropertyToID("_Tint");
        grassSplatID = Shader.PropertyToID("_GrassControl");
        grassTintID = Shader.PropertyToID("_GrassTint");
        sharedMatBlock = new MaterialPropertyBlock();
    }

    public static GameMap Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance = this;
        cameraMovement = FindObjectOfType<CameraMovement>();
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


    GameObject selected;
    CameraMovement cameraMovement;

    // Run once per frame.
    void Update()
    {
        if (Input.GetButtonDown("ToggleDF") && EventSystem.current.currentSelectedGameObject == null)
        {
            GameSettings.Instance.game.showDFScreen = !GameSettings.Instance.game.showDFScreen;

            dfScreen.SetActive(GameSettings.Instance.game.showDFScreen);
        }

        if ((cameraMovement != null && cameraMovement.following) || GameSettings.Instance.game.showDFScreen)
            UpdateView();


        if (!GameSettings.Instance.game.showDFScreen && EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleHelp();
            }
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
                cameraMovement.following = true;
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

        if (ContentLoader.Instance == null)
            return;

        ShowCursorInfo();
        UpdateRequestRegion();
        UpdateCreatures();
        UpdateBlocks();
        UpdateSplatTextures();
        //DrawBlocks();
        UpdateBlockVisibility();
    }

    bool prevFirstPerson = false;
    bool prevShadows = false;
    int PrevZ = -1;
    private void UpdateBlockVisibility()
    {
        if (PosZ != PrevZ)
        {
            for (int z = Mathf.Min(PosZ, PrevZ) - 1; z <= Mathf.Max(PosZ, PrevZ) + 1; z++)
            {
                UpdateBlockVisibility(z);
                UpdateBlockVisibility(z + GameSettings.Instance.rendering.drawRangeUp);
                UpdateBlockVisibility(z - GameSettings.Instance.rendering.drawRangeDown);
            }
            PrevZ = PosZ;
        }
        if(firstPerson != prevFirstPerson || overheadShadows != prevShadows)
        {
            for (int z = PosZ; z <= PosZ + GameSettings.Instance.rendering.drawRangeUp; z++)
            {
                UpdateBlockVisibility(z);
            }
            prevFirstPerson = firstPerson;
            prevShadows = overheadShadows;
        }
    }

    void UpdateBlockVisibility(int z)
    {
        if (z < 0 || z >= mapMeshes.GetLength(2))
            return;
        for(int x = 0; x < mapMeshes.GetLength(0); x++)
            for(int y = 0; y < mapMeshes.GetLength(1); y++)
            {
                if (mapMeshes[x, y, z] == null)
                    continue;
                    mapMeshes[x, y, z].UpdateVisibility(GetVisibility(z));

            }
    }

    BlockMeshSet.Visibility GetVisibility(int z)
    {
        if (z > PosZ + GameSettings.Instance.rendering.drawRangeUp)
            return BlockMeshSet.Visibility.None;
        else if (z >= PosZ)
        {
            if (firstPerson)
                return BlockMeshSet.Visibility.All;
            else if(overheadShadows)
                return BlockMeshSet.Visibility.Shadows;
            else
                return BlockMeshSet.Visibility.None;
        }
        else if (z >= PosZ - GameSettings.Instance.rendering.drawRangeDown)
        {
            return BlockMeshSet.Visibility.All;
        }
        else
            return BlockMeshSet.Visibility.None;
    }

    public float helpFadeLength = 0.5f;
    Coroutine helpFade;
    bool helpEnabled = false;
    private void ToggleHelp()
    {
        StopCoroutine(helpFade);
        if (helpEnabled)
        {
            HideHelp();
        }
        else
        {
            ShowHelp();
        }
    }

    public void HideHelp()
    {
        helpFade = StartCoroutine(DisableHelp());
    }

    public void ShowHelp()
    {
        helpFade = StartCoroutine(EnableHelp());
    }

    IEnumerator DisableHelp()
    {
        helpEnabled = false;
        for (float f = helpOverlay.alpha; f >= 0; f -= (Time.deltaTime / helpFadeLength))
        {
            helpOverlay.alpha = f;
            yield return null;
        }
        helpOverlay.gameObject.SetActive(false);
        yield return null;
    }
    IEnumerator EnableHelp()
    {
        helpEnabled = true;
        helpOverlay.gameObject.SetActive(true);
        for (float f = helpOverlay.alpha; f <= 1; f += (Time.deltaTime / helpFadeLength))
        {
            helpOverlay.alpha = f;
            yield return null;
        }
        yield return null;
    }




    public void Refresh()
    {
        for (int z = 0; z < blockDirtyBits.GetLength(2); z++)
            for (int x = 0; x < blockDirtyBits.GetLength(0); x++)
                for (int y = 0; y < blockDirtyBits.GetLength(1); y++)
                {
                    if (blockContentBits[x, y, z])
                    {
                        layerDirtyBits[z] = true;
                        grassLayerDirtyBits[z] = true;
                        spatterBlockDirtyBits[z] = true;
                    }
                    blockDirtyBits[x, y, z] = blockContentBits[x, y, z];
                    liquidBlockDirtyBits[x, y, z] = blockContentBits[x, y, z];
                }
    }

    public Mesh testMesh;

    private void SaveMeshes()
    {
        //COLLADA exportScene = new COLLADA();

        //List<geometry> geometryList = new List<geometry>();

        ////if(testMesh!= null)
        ////{
        ////    geometry geo = COLLADA.MeshToGeometry(testMesh);
        ////    if (geo != null)
        ////        geometryList.Add(geo);
        ////}

        //Debug.Log("Starting mesh export");
        //foreach (Mesh mesh in blocks)
        //{
        //    if (mesh != null)
        //    {
        //        geometry geo = (COLLADA.MeshToGeometry(mesh));
        //        if (geo != null)
        //            geometryList.Add(geo);
        //    }
        //}
        //Debug.Log("Added opaque blocks");
        //foreach (Mesh mesh in stencilBlocks)
        //{
        //    if (mesh != null)
        //    {
        //        geometry geo = (COLLADA.MeshToGeometry(mesh));
        //        if (geo != null)
        //            geometryList.Add(geo);
        //    }
        //}
        //Debug.Log("Added stencil blocks");
        //foreach (Mesh mesh in transparentBlocks)
        //{
        //    if (mesh != null)
        //    {
        //        geometry geo = (COLLADA.MeshToGeometry(mesh));
        //        if (geo != null)
        //            geometryList.Add(geo);
        //    }
        //}
        //Debug.Log("Added transparent blocks");
        //foreach (Mesh mesh in liquidBlocks)
        //{
        //    if (mesh != null)
        //    {
        //        geometry geo = (COLLADA.MeshToGeometry(mesh));
        //        if (geo != null)
        //            geometryList.Add(geo);
        //    }
        //}
        //Debug.Log("Added liquid blocks");

        //library_geometries geometryLib = new library_geometries();
        //geometryLib.geometry = geometryList.ToArray();
        //Debug.Log("Added geometry to library");

        //library_visual_scenes visualSceneLib = new library_visual_scenes();
        //visual_scene visualScene = new visual_scene();

        //visualSceneLib.visual_scene = new visual_scene[1];
        //visualSceneLib.visual_scene[0] = visualScene;

        //visualScene.id = "Map";
        //visualScene.name = "Map";
        //visualScene.node = new node[geometryList.Count];
        //for (int i = 0; i < geometryList.Count; i++)
        //{
        //    node thisNode = new node();
        //    visualScene.node[i] = thisNode;
        //    geometry thisGeometry = geometryList[i];
        //    thisNode.id = thisGeometry.id.Remove(thisGeometry.id.Length - 4);
        //    thisNode.name = thisGeometry.name.Remove(thisGeometry.name.Length - 6);
        //    thisNode.sid = thisNode.id;

        //    thisNode.Items = new object[1];
        //    thisNode.Items[0] = COLLADA.ConvertMatrix(Matrix4x4.identity);
        //    thisNode.ItemsElementName = new ItemsChoiceType2[1];
        //    thisNode.ItemsElementName[0] = ItemsChoiceType2.matrix;

        //    thisNode.instance_geometry = new instance_geometry[1];
        //    thisNode.instance_geometry[0] = new instance_geometry();
        //    thisNode.instance_geometry[0].url = "#" + thisGeometry.id;
        //}
        //Debug.Log("Added geometry to scene");

        //COLLADAScene sceneInstance = new COLLADAScene();
        //sceneInstance.instance_visual_scene = new InstanceWithExtra();
        //sceneInstance.instance_visual_scene.url = "#" + visualScene.id;

        //exportScene.scene = sceneInstance;

        //exportScene.Items = new object[2];
        //exportScene.Items[0] = geometryLib;
        //exportScene.Items[1] = visualSceneLib;

        //asset assetHeader = new asset();
        //assetHeader.unit = new assetUnit();
        //assetHeader.unit.meter = 1;
        //assetHeader.unit.name = "meter";
        //assetHeader.up_axis = UpAxisType.Y_UP;

        //exportScene.asset = assetHeader;
        //Debug.Log("Setup Scene");

        //if (File.Exists("Map.dae"))
        //    File.Delete("Map.dae");
        //exportScene.Save("Map.dae");
        //Debug.Log("Saved Scene");

        //Texture2D mainTex = (Texture2D)BasicTerrainMaterial.GetTexture("_MainTex");

        //Color[] mainTexPixels = mainTex.GetPixels();
        //Color[] diffusePixels = new Color[mainTexPixels.Length];
        //Color[] roughnessPixels = new Color[mainTexPixels.Length];

        //for (int i = 0; i < mainTexPixels.Length; i++)
        //{
        //    diffusePixels[i] = new Color(mainTexPixels[i].r, mainTexPixels[i].g, mainTexPixels[i].b, 1.0f);
        //    roughnessPixels[i] = new Color(mainTexPixels[i].a, mainTexPixels[i].a, mainTexPixels[i].a, 1.0f);
        //}

        //Texture2D diffuseTex = new Texture2D(mainTex.width, mainTex.height);
        //Texture2D roughnessTex = new Texture2D(mainTex.width, mainTex.height);

        //diffuseTex.SetPixels(diffusePixels);
        //roughnessTex.SetPixels(roughnessPixels);

        //diffuseTex.Apply();
        //roughnessTex.Apply();

        //byte[] diffuseBytes = diffuseTex.EncodeToPNG();
        //byte[] roughnessBytes = roughnessTex.EncodeToPNG();

        //File.WriteAllBytes("pattern.png", diffuseBytes);
        //File.WriteAllBytes("specular.png", roughnessBytes);
        //Debug.Log("Saved Maintex");

        //Texture2D bumpMap = (Texture2D)BasicTerrainMaterial.GetTexture("_BumpMap");

        //Color[] bumpMapPixels = bumpMap.GetPixels();
        //Color[] normalMapPixels = new Color[bumpMapPixels.Length];
        //Color[] ambientMapPixels = new Color[bumpMapPixels.Length];
        //Color[] alphaMapPixels = new Color[bumpMapPixels.Length];

        //for (int i = 0; i < bumpMapPixels.Length; i++)
        //{
        //    normalMapPixels[i] = new Color(bumpMapPixels[i].a, bumpMapPixels[i].g, Mathf.Sqrt(1 - ((bumpMapPixels[i].a * 2 - 1) * (bumpMapPixels[i].a * 2 - 1)) + ((bumpMapPixels[i].g * 2 - 1) * (bumpMapPixels[i].g * 2 - 1))));
        //    ambientMapPixels[i] = new Color(bumpMapPixels[i].r, bumpMapPixels[i].r, bumpMapPixels[i].r, 1.0f);
        //    alphaMapPixels[i] = new Color(bumpMapPixels[i].b, bumpMapPixels[i].b, bumpMapPixels[i].b, 1.0f);
        //}

        //Texture2D normalTex = new Texture2D(bumpMap.width, bumpMap.height);
        //Texture2D ambientTex = new Texture2D(bumpMap.width, bumpMap.height);
        //Texture2D alphaTex = new Texture2D(bumpMap.width, bumpMap.height);

        //normalTex.SetPixels(normalMapPixels);
        //ambientTex.SetPixels(ambientMapPixels);
        //alphaTex.SetPixels(alphaMapPixels);

        //normalTex.Apply();
        //ambientTex.Apply();
        //alphaTex.Apply();

        //byte[] normalBytes = normalTex.EncodeToPNG();
        //byte[] ambientBytes = ambientTex.EncodeToPNG();
        //byte[] alphaBytes = alphaTex.EncodeToPNG();

        //File.WriteAllBytes("normal.png", normalBytes);
        //File.WriteAllBytes("occlusion.png", ambientBytes);
        //File.WriteAllBytes("alpha.png", alphaBytes);
        //Debug.Log("Saved DetailTex");

        //Debug.Log("Saved map!");
    }

    void OnDestroy()
    {
        if (mesher != null)
        {
            mesher.Terminate();
            mesher = null;
        }
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    void UpdateView()
    {
        RemoteFortressReader.ViewInfo newView = DFConnection.Instance.PopViewInfoUpdate();
        if (newView == null) return;

        UnityEngine.Profiling.Profiler.BeginSample("UpdateView", this);
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
        UnityEngine.Profiling.Profiler.EndSample();
    }
    // Update the region we're requesting
    void UpdateRequestRegion()
    {
        UnityEngine.Profiling.Profiler.BeginSample("UpdateRequestRegion", this);
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
        UnityEngine.Profiling.Profiler.EndSample();
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
        UnityEngine.Profiling.Profiler.BeginSample("UpdateBlocks", this);
        MapBlock block;
        while((block = DFConnection.Instance.PopMapBlockUpdate()) != null)
        {
            bool setTiles;
            bool setLiquids;
            bool setSpatters;
            UnityEngine.Profiling.Profiler.BeginSample("StoreTiles", this);
            MapDataStore.Main.StoreTiles(block, out setTiles, out setLiquids, out setSpatters);
            UnityEngine.Profiling.Profiler.EndSample();
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
                ///Send it to item manager later.
            }
            UnityEngine.Profiling.Profiler.BeginSample("BuildingManager.LoadBlock", this);
            Building.BuildingManager.Instance.LoadBlock(block);
            UnityEngine.Profiling.Profiler.EndSample();
        }
        DirtySeasonalBlocks();
        UnityEngine.Profiling.Profiler.BeginSample("EnqueueMeshUpdates", this);
        EnqueueMeshUpdates();
        UnityEngine.Profiling.Profiler.EndSample();

        mesher.Poll();

        FetchNewMeshes();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void SetMaterialBounds(Vector4 bounds)
    {
        MaterialManager.Instance.SetVector("_WorldBounds", bounds);
    }

    void InitializeBlocks()
    {
        int blockSizeX = DFConnection.Instance.EmbarkMapSize.x;
        int blockSizeY = DFConnection.Instance.EmbarkMapSize.y;
        int blockSizeZ = DFConnection.Instance.EmbarkMapSize.z;
        MapZOffset = DFConnection.Instance.EmbarkMapPosition.z;

        mapMeshes = new BlockMeshSet[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];

        blockDirtyBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        blockContentBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        blockUpdateSchedules = new UpdateSchedule[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        liquidBlockDirtyBits = new bool[blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ];
        magmaGlow = new Light[blockSizeX * 16, blockSizeY * 16, blockSizeZ];
        spatterBlockDirtyBits = new bool[blockSizeZ];
        layerDirtyBits = new bool[blockSizeZ];
        grassLayerDirtyBits = new bool[blockSizeZ];
        spatterLayers = new Texture2D[blockSizeZ];
        terrainSplatLayers = new Texture2D[blockSizeZ];
        terrainTintLayers = new Texture2D[blockSizeZ];
        grassSplatLayers = new Texture2D[blockSizeZ];
        grassTintLayers = new Texture2D[blockSizeZ];


        Vector3 min = DFtoUnityCoord(0, 0, 0) - new Vector3(tileWidth / 2, 0, -tileWidth / 2);
        Vector3 max = DFtoUnityCoord((blockSizeX * 16) - 1, (blockSizeY * 16) - 1, 0) + new Vector3(tileWidth / 2, 0, -tileWidth / 2);

        SetMaterialBounds(new Vector4(min.x, min.z, max.x, max.z));
    }

    void addSeasonalUpdates(MapBlock block, int mapBlockX, int mapBlockY, int mapBlockZ)
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
        grassLayerDirtyBits[mapBlockZ] = true;
        layerDirtyBits[mapBlockZ] = true;
        if (mapBlockZ < layerDirtyBits.Length - 1)
        {
            layerDirtyBits[mapBlockZ + 1] = true; //For ramp edges.
            grassLayerDirtyBits[mapBlockZ + 1] = true;
        }

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
        int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
        int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
        int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
        int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
        int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, mapMeshes.GetLength(2));
        int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, mapMeshes.GetLength(2));
        for (int zz = zmin; zz < zmax; zz++)
            for (int yy = ymin; yy < ymax; yy++)
                for (int xx = xmin; xx < xmax; xx++)
                {
                    if (blockUpdateSchedules[xx, yy, zz] != null && blockUpdateSchedules[xx, yy, zz].CheckUpdate(TimeHolder.DisplayedTime.CurrentYearTicks))
                        blockDirtyBits[xx, yy, zz] = true;
                }

    }

    void UpdateSplatTextures()
    {
        if (ContentLoader.Instance == null)
            return;
        UnityEngine.Profiling.Profiler.BeginSample("UpdateTerrainTextures", this);
        for (int z = layerDirtyBits.Length - 1; z >= 0; z--)
        {
            if (layerDirtyBits[z])
            {
                GenerateTerrainTexture(z);
                layerDirtyBits[z] = false;
                break; //don't do more than one set per frame.
            }
            if (grassLayerDirtyBits[z])
            {
                GenerateGrassTexture(z);
                grassLayerDirtyBits[z] = false;
                break;
            }
            if (spatterBlockDirtyBits[z])
            {
                GenerateSpatterTexture(z);
                spatterBlockDirtyBits[z] = false;
                break;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    // Have the mesher mesh all dirty tiles in the region
    void EnqueueMeshUpdates()
    {
        int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
        int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
        int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
        int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
        int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, mapMeshes.GetLength(2));
        int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, mapMeshes.GetLength(2));
        for (int zz = posZ - 1; zz >= zmin; zz--)
        {
            if (zz >= mapMeshes.GetLength(2))
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

    private BlockMeshSet blockPrefab;

    // Get new meshes from the mesher
    void FetchNewMeshes()
    {

        UnityEngine.Profiling.Profiler.BeginSample("FetchNewMeshes", this);
        if (mesher.HasNewMeshes)
        {
            if (blockPrefab == null)
                blockPrefab = Resources.Load<BlockMeshSet>("MapBlock");
            var newMeshes = mesher.Dequeue().Value;
            int block_x = newMeshes.location.x / blockSize;
            int block_y = newMeshes.location.y / blockSize;
            int block_z = newMeshes.location.z;
            if (mapMeshes[block_x, block_y, block_z] == null)
            {
                mapMeshes[block_x, block_y, block_z] = Instantiate(blockPrefab, DFtoUnityCoord(newMeshes.location), Quaternion.identity, transform);
                mapMeshes[block_x, block_y, block_z].name = string.Format("Block_{0}_{1}_{2}", block_x, block_y, block_z);
                mapMeshes[block_x, block_y, block_z].Init();
                mapMeshes[block_x, block_y, block_z].UpdateVisibility(GetVisibility(block_z));
                if (terrainSplatLayers[block_z] != null)
                    mapMeshes[block_x, block_y, block_z].SetTerrainMap(terrainSplatLayers[block_z], terrainTintLayers[block_z]);
            }

            var meshSet = mapMeshes[block_x, block_y, block_z];

            meshSet.LoadMeshes(newMeshes, string.Format("{0}_{1}_{2}", block_x, block_y, block_z));

            if (newMeshes.collisionMesh != null)
            {

                Mesh collisionMesh = new Mesh();
                collisionMesh.name = string.Format("block_collision_{0}_{1}_{2}", block_x, block_y, block_z);
                newMeshes.collisionMesh.CopyToMesh(collisionMesh);
                meshSet.collisionBlocks.sharedMesh = collisionMesh;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    Color[] grassColors;
    Color[] grassIndices;


    void GenerateGrassTexture(int z)
    {
        if (ContentLoader.Instance == null)
            return;
        UnityEngine.Profiling.Profiler.BeginSample("GenerateGrassTexture", this);

        if (grassColors == null || grassIndices == null || grassColors.Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
        {
            grassColors = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
            grassIndices = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
        }

        int grassTiles = 0;

        for (int x = 0; x < MapDataStore.MapSize.x; x++)
            for (int y = 0; y < MapDataStore.MapSize.y; y++)
            {
                int index = x + (y * MapDataStore.MapSize.x);
                var tile = MapDataStore.Main[x, y, z];
                if (tile == null)
                {
                    grassIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                    grassIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                    grassColors[index] = new Color(0, 0, 0, 0);
                    continue;
                }
                if (!(tile.tiletypeMaterial == TiletypeMaterial.GRASS_DARK
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_LIGHT
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DEAD
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DRY
                    ))
                    continue;

                grassTiles++;

                GrassContent grassTexture;
                if (ContentLoader.Instance.GrassTextureConfiguration.GetValue(tile, MeshLayer.StaticMaterial, out grassTexture))
                {
                    grassIndices[index].r = grassTexture.MaterialTexture.StorageIndex;
                    grassIndices[index].g = grassTexture.ShapeTexture.StorageIndex;
                }
                else
                {
                    grassIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                    grassIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                }

                ColorContent colorContent;
                if (ContentLoader.Instance.ColorConfiguration.GetValue(tile, MeshLayer.StaticMaterial, out colorContent))
                    grassColors[index] = colorContent.color;
                else
                    grassColors[index] = Color.gray;

            }
        if (grassTiles == 0)
            return;

        if (grassSplatLayers[z] == null)
        {
            grassSplatLayers[z] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGHalf, false, true);
            grassSplatLayers[z].filterMode = FilterMode.Point;
            grassSplatLayers[z].wrapMode = TextureWrapMode.Clamp;
        }
        if (grassSplatLayers[z].width != MapDataStore.MapSize.x || terrainSplatLayers[z].height != MapDataStore.MapSize.y)
            grassSplatLayers[z].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassSplatLayers[z].SetPixels(grassIndices);
        grassSplatLayers[z].Apply();

        if (grassTintLayers[z] == null)
        {
            grassTintLayers[z] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, true);
            grassTintLayers[z].filterMode = FilterMode.Point;
            grassTintLayers[z].wrapMode = TextureWrapMode.Clamp;
        }
        if (grassTintLayers[z].width != MapDataStore.MapSize.x || terrainTintLayers[z].height != MapDataStore.MapSize.y)
            grassTintLayers[z].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassTintLayers[z].SetPixels(grassColors);
        grassTintLayers[z].Apply();

        UnityEngine.Profiling.Profiler.EndSample();
    }

    Color[] terrainColors;
    Color[] terrainIndices;


    void GenerateTerrainTexture(int z)
    {
        if (ContentLoader.Instance == null)
            return;
        UnityEngine.Profiling.Profiler.BeginSample("GenerateTerrainTexture", this);

        if (terrainColors == null || terrainIndices == null || terrainColors.Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
        {
            terrainColors = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
            terrainIndices = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
        }
        UnityEngine.Profiling.Profiler.BeginSample("Update Terrain Color Array", this);
        for (int x = 0; x < MapDataStore.MapSize.x; x++)
            for (int y = 0; y < MapDataStore.MapSize.y; y++)
            {
                int index = x + (y * MapDataStore.MapSize.x);
                var tile = MapDataStore.Main[x, y, z];
                if (IsNullOrEmpty(tile))
                {
                    if (!IsNullOrEmpty(MapDataStore.Main[x - 1, y, z]))
                        tile = MapDataStore.Main[x - 1, y, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x, y - 1, z]))
                        tile = MapDataStore.Main[x, y - 1, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x + 1, y, z]))
                        tile = MapDataStore.Main[x + 1, y, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x, y + 1, z]))
                        tile = MapDataStore.Main[x, y + 1, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x - 1, y - 1, z]))
                        tile = MapDataStore.Main[x - 1, y - 1, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x - 1, y + 1, z]))
                        tile = MapDataStore.Main[x - 1, y + 1, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x + 1, y - 1, z]))
                        tile = MapDataStore.Main[x + 1, y - 1, z];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x + 1, y + 1, z]))
                        tile = MapDataStore.Main[x + 1, y + 1, z];
                    else
                    {
                        terrainIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                        terrainIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                        terrainColors[index] = Color.gray;
                        continue;
                    }
                }
                if (tile.shape == TiletypeShape.RAMP_TOP && tile.Down != null)
                    tile = tile.Down;

                var layer = MeshLayer.BaseMaterial;
                if (tile.tiletypeMaterial == TiletypeMaterial.GRASS_DARK
                || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DEAD
                || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DRY
                || tile.tiletypeMaterial == TiletypeMaterial.GRASS_LIGHT
                || tile.tiletypeMaterial == TiletypeMaterial.PLANT
                )
                    layer = MeshLayer.LayerMaterial;


                NormalContent normalContent;
                if (ContentLoader.Instance.TerrainShapeTextureConfiguration.GetValue(tile, layer, out normalContent))
                    terrainIndices[index].g = normalContent.StorageIndex;
                else
                    terrainIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;

                TextureContent materialContent;
                if (ContentLoader.Instance.MaterialTextureConfiguration.GetValue(tile, layer, out materialContent))
                    terrainIndices[index].r = materialContent.StorageIndex;
                else
                    terrainIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;

                ColorContent colorContent;
                if (ContentLoader.Instance.ColorConfiguration.GetValue(tile, layer, out colorContent))
                    terrainColors[index] = colorContent.color;
                else
                    terrainColors[index] = Color.gray;

            }
        UnityEngine.Profiling.Profiler.EndSample();
        UnityEngine.Profiling.Profiler.BeginSample("Apply terrain textures.", this);

        if (terrainSplatLayers[z] == null)
        {
            terrainSplatLayers[z] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGHalf, false, true);
            terrainSplatLayers[z].filterMode = FilterMode.Point;
            terrainSplatLayers[z].wrapMode = TextureWrapMode.Clamp;
        }
        if (terrainSplatLayers[z].width != MapDataStore.MapSize.x || terrainSplatLayers[z].height != MapDataStore.MapSize.y)
            terrainSplatLayers[z].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainSplatLayers[z].SetPixels(terrainIndices);
        terrainSplatLayers[z].Apply();

        if (terrainTintLayers[z] == null)
        {
            terrainTintLayers[z] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, true);
            terrainTintLayers[z].filterMode = FilterMode.Point;
            terrainTintLayers[z].wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < mapMeshes.GetLength(0); x++)
                for (int y = 0; y < mapMeshes.GetLength(1); y++)
                {
                    if (mapMeshes[x, y, z] != null)
                        mapMeshes[x, y, z].SetTerrainMap(terrainSplatLayers[z], terrainTintLayers[z]);
                }

        }
        if (terrainTintLayers[z].width != MapDataStore.MapSize.x || terrainTintLayers[z].height != MapDataStore.MapSize.y)
            terrainTintLayers[z].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainTintLayers[z].SetPixels(terrainColors);
        terrainTintLayers[z].Apply();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.EndSample();
    }

    private bool IsNullOrEmpty(MapDataStore.Tile tile)
    {
        if (tile == null)
            return true;
        if (tile.shape == TiletypeShape.EMPTY)
            return true;
        if (tile.shape == TiletypeShape.ENDLESS_PIT)
            return true;
        return false;
    }

    Color[] textureColors;

    void GenerateSpatterTexture(int z)
    {
        UnityEngine.Profiling.Profiler.BeginSample("GenerateSpatterTexture", this);
        if (textureColors == null || textureColors.Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
            textureColors = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];

        UnityEngine.Profiling.Profiler.BeginSample("UpdateSpatterArray", this);
        for (int x = 0; x < MapDataStore.MapSize.x; x++)
            for (int y = 0; y < MapDataStore.MapSize.y; y++)
            {
                Color totalColor = new Color(0, 0, 0, 0);
                int index = x + (y * MapDataStore.MapSize.x);

                var tile = MapDataStore.Main[x, y, z];
                if (tile == null)
                {
                    textureColors[index] = totalColor;
                    continue;
                }
                if (tile.spatters == null || tile.spatters.Count == 0)
                {
                    textureColors[index] = totalColor;
                    continue;
                }

                if (tile.Hidden)
                {
                    textureColors[index] = totalColor;
                    continue;
                }
                float totalAmount = 0;

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
                        color = cont.color;
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
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Apply Image", this);
        if (spatterLayers[z] == null)
        {
            spatterLayers[z] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.ARGB32, false);
            spatterLayers[z].wrapMode = TextureWrapMode.Clamp;
        }
        if (spatterLayers[z].width != MapDataStore.MapSize.x || spatterLayers[z].height != MapDataStore.MapSize.y)
            spatterLayers[z].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        spatterLayers[z].SetPixels(textureColors);
        spatterLayers[z].Apply();
        UnityEngine.Profiling.Profiler.EndSample();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void ClearMap()
    {
        foreach (var item in mapMeshes)
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
        UnityEngine.Profiling.Profiler.BeginSample("ShowCursorInfo", this);

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
                    statusText.AppendLine();

                    if (tile.buildingItems != null && tile.buildingItems.Count > 0)
                    {
                        statusText.Append("Building items:").AppendLine();
                        foreach (var item in tile.buildingItems)
                        {
                            if (items.ContainsKey(item.item.type))
                                statusText.Append(items[item.item.type].id);
                            else
                                statusText.Append(item.item.type);
                            statusText.Append(" [").Append(item.mode).Append("]");
                            statusText.Append((DFCoord)item.item.pos).AppendLine();
                        }
                        statusText.AppendLine();
                    }
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
            {
                UnitDefinition foundUnit = null;
                foreach (UnitDefinition unit in unitList.creature_list)
                {
                    UnitFlags1 flags1 = (UnitFlags1)unit.flags1;

                    if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead)
                         || ((flags1 & UnitFlags1.left) == UnitFlags1.left)
                         || ((flags1 & UnitFlags1.caged) == UnitFlags1.caged)
                         || ((flags1 & UnitFlags1.forest) == UnitFlags1.forest)
                         )
                        continue;
                    if (unit.pos_x == cursX && unit.pos_y == cursY && unit.pos_z == cursZ)
                    {
                        foundUnit = unit;
                        if ((flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
                            continue; // Keep looking until we find a standing unit.
                        else
                            break;
                    }
                }
                if (foundUnit != null)
                {
                    UnitFlags1 flags1 = (UnitFlags1)foundUnit.flags1;
                    UnitFlags2 flags2 = (UnitFlags2)foundUnit.flags2;

                    CreatureRaw creatureRaw = null;
                    if (DFConnection.Instance.NetCreatureRawList != null)
                        creatureRaw = DFConnection.Instance.NetCreatureRawList.creature_raws[foundUnit.race.mat_type];

                    if (creatureRaw != null)
                    {
                        statusText.Append("Unit:   \n");

                        statusText.Append("Race: ");
                        statusText.Append(creatureRaw.creature_id + ":");
                        statusText.Append(creatureRaw.caste[foundUnit.race.mat_index].caste_id);
                        statusText.AppendLine();

                        statusText.Append(flags1).AppendLine();
                        statusText.Append(flags2).AppendLine();
                        //statusText.Append(flags3).AppendLine();
                        statusText.Append("Length: ").Append(foundUnit.size_info.length_cur).Append("/").Append(Mathf.FloorToInt(Mathf.Pow(creatureRaw.adultsize * 10000, 1.0f / 3.0f))).AppendLine();
                        statusText.Append("Profession: ").Append((profession)foundUnit.profession_id).AppendLine();
                        foreach (var noble in foundUnit.noble_positions)
                        {
                            statusText.Append(noble).Append(", ");
                        }
                        statusText.AppendLine();
                    }
                }
            }


            //if (itemPositions.ContainsKey(new DFCoord(cursX, cursY, cursZ)))
            //{
            //    for (int itemIndex = 0; itemIndex < itemPositions[new DFCoord(cursX, cursY, cursZ)].Count && itemIndex < 5; itemIndex++)
            //    {
            //        var item = itemPositions[new DFCoord(cursX, cursY, cursZ)][itemIndex];
            //        statusText.Append("Item ").Append(item.id).Append(": ");
            //        if (materials.ContainsKey(item.material))
            //            statusText.Append(materials[item.material].id);
            //        else
            //            statusText.Append(((MatPairStruct)item.material).ToString());
            //        statusText.Append(" ");
            //        if (items.ContainsKey(item.type))
            //            statusText.Append(items[item.type].id);
            //        statusText.Append("(").Append(((MatPairStruct)item.type)).Append(")");
            //        statusText.AppendLine();
            //    }
            //    if (itemPositions[new DFCoord(cursX, cursY, cursZ)].Count > 4)
            //        statusText.Append(itemPositions[new DFCoord(cursX, cursY, cursZ)].Count - 4).Append(" items more.");
            //}
        }
        cursorProperties.text = statusText.ToString();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    Dictionary<int, Transform> creatureList;
    public Transform creatureTemplate;

    UnitList lastUnitList = null;
    UnitList unitList = null;
    private int screenshotCount;

    int unitSpriteID = int.MinValue;

    MaterialPropertyBlock creatureMaterialProperties = null;

    void UpdateCreatures()
    {
        if (!GameSettings.Instance.units.drawUnits)
            return;
        if (creatureTemplate == null)
            return;
        if (ContentLoader.Instance == null)
            return;

        if (creatureMaterialProperties == null)
            creatureMaterialProperties = new MaterialPropertyBlock();

        if (unitSpriteID == int.MinValue)
        {
            unitSpriteID = Shader.PropertyToID("_SpriteIndex");
        }
        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        var tempUnitList = DFConnection.Instance.PopUnitListUpdate();
        if (tempUnitList == null)
            return;
        else
            unitList = tempUnitList;
        UnityEngine.Profiling.Profiler.BeginSample("UpdateCreatures", this);
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

                    Color color = Color.white;
                    if (unit.profession_color != null)
                        color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1);

                    if (creatureRaw != null)
                    {
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().material.SetInt(unitSpriteID, creatureRaw.creature_tile);
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().material.color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1);
                    }

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
                    CameraFacing cameraFacing = creatureList[unit.id].GetComponentInChildren<CameraFacing>();
                    if ((flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
                    {
                        cameraFacing.transform.localPosition = Vector3.zero;
                        cameraFacing.enabled = false;
                        cameraFacing.transform.rotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        cameraFacing.transform.localPosition = new Vector3(0, 1.0f, 0);
                        cameraFacing.enabled = true;
                    }
                    Material mat;
                    int index;
                    bool colored;
                    if (ContentLoader.Instance.SpriteManager.getCreatureSprite(unit, out mat, out index, out colored))
                    {
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().material = mat;
                        creatureMaterialProperties.SetFloat(unitSpriteID, index);
                    }
                    else
                    {
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().material = creatureTemplate.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        if (creatureRaw != null)
                        {
                            if (unit.is_soldier && creatureRaw.creature_soldier_tile != 0)
                                creatureMaterialProperties.SetFloat(unitSpriteID, creatureRaw.creature_soldier_tile);
                            else
                                creatureMaterialProperties.SetFloat(unitSpriteID, creatureRaw.creature_tile);
                        }
                    }
                    if (colored && unit.profession_color != null)
                        creatureMaterialProperties.SetColor("_Color", new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1));
                    else
                        creatureMaterialProperties.SetColor("_Color", Color.white);

                    creatureList[unit.id].GetComponentInChildren<MeshRenderer>().SetPropertyBlock(creatureMaterialProperties);

                    if (creatureRaw != null)
                    {
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
        UnityEngine.Profiling.Profiler.EndSample();
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

    //private bool DrawSingleBlock(int xx, int yy, int zz, bool phantom, Vector3 pos, bool top)
    //{
    //    return DrawSingleBlock(xx, yy, zz, phantom, Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one), top);
    //}

    MaterialPropertyBlock sharedMatBlock;
    int spatterID;
    int terrainSplatID;
    int terrainTintID;
    int grassSplatID;
    int grassTintID;

    //private bool DrawSingleBlock(int xx, int yy, int zz, bool phantom, Matrix4x4 LocalTransform, bool top)
    //{
    //    if (mapMeshes[xx, yy, zz] == null)
    //        return false;
    //    MaterialPropertyBlock matBlock = null;
    //    MaterialManager.MaterialFlags flags = MaterialManager.MaterialFlags.None;
    //    if (spatterLayers[zz] != null)
    //    {
    //        matBlock = sharedMatBlock;
    //        matBlock.SetTexture(spatterID, spatterLayers[zz]);
    //        flags |= MaterialManager.MaterialFlags.Contaminants;
    //    }
    //    if (grassSplatLayers[zz] != null)
    //    {
    //        matBlock = sharedMatBlock;
    //        matBlock.SetTexture(grassSplatID, grassSplatLayers[zz]);
    //        matBlock.SetTexture(grassTintID, grassTintLayers[zz]);
    //        flags |= MaterialManager.MaterialFlags.Grass;
    //    }
    //    if (terrainSplatLayers[zz] != null)
    //    {
    //        if (matBlock == null)
    //            matBlock = sharedMatBlock;
    //        matBlock.SetTexture(terrainSplatID, terrainSplatLayers[zz]);
    //    }
    //    if (terrainTintLayers[zz] != null)
    //    {
    //        if (matBlock == null)
    //            matBlock = sharedMatBlock;
    //        matBlock.SetTexture(terrainTintID, terrainTintLayers[zz]);
    //    }
    //    //return mapMeshes[xx, yy, zz].Render(phantom, LocalTransform, top,
    //    //    MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Opaque, flags),
    //    //    MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Stencil, flags),
    //    //    MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Transparent, flags),
    //    //    MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.SplatMap, flags),
    //    //    waterMaterial, magmaMaterial, matBlock
    //    //    );
    //    return true;
    //}

    //private void DrawBlocks()
    //{
    //    if (mapMeshes == null)
    //        return;
    //    UnityEngine.Profiling.Profiler.BeginSample("DrawBlocks", this);
    //    int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
    //    int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
    //    int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
    //    int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
    //    int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, mapMeshes.GetLength(2));
    //    int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, mapMeshes.GetLength(2));

    //    int drawnBlocks = 0;

    //    for (int zz = posZ - 1; zz >= zmin; zz--)
    //    {
    //        if (zz >= mapMeshes.GetLength(2))
    //            continue;

    //        for (int xx = xmin; xx < xmax; xx++)
    //        {
    //            if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
    //                break;
    //            for (int yy = ymin; yy < ymax; yy++)
    //            {
    //                Vector3 pos = DFtoUnityCoord(xx * blockSize, yy * blockSize, zz);
    //                if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
    //                    break;
    //                if (DrawSingleBlock(xx, yy, zz, false, pos, zz == posZ - 1))
    //                    drawnBlocks++;
    //            }
    //        }
    //    }
    //    if (firstPerson || (overheadShadows && GameSettings.Instance.rendering.drawShadows))
    //        for (int zz = posZ; zz < zmax; zz++)
    //        {
    //            if (zz < 0)
    //                continue;

    //            for (int xx = xmin; xx < xmax; xx++)
    //            {
    //                if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
    //                    break;
    //                for (int yy = ymin; yy < ymax; yy++)
    //                {
    //                    if (drawnBlocks >= GameSettings.Instance.rendering.maxBlocksToDraw)
    //                        break;
    //                    Vector3 pos = DFtoUnityCoord(xx * blockSize, yy * blockSize, zz);

    //                    if (DrawSingleBlock(xx, yy, zz, (!firstPerson), pos, zz == posZ || zz == zmax - 1))
    //                        drawnBlocks++;
    //                }
    //            }
    //        }
    //    StatsReadout.BlocksDrawn = drawnBlocks;
    //    UnityEngine.Profiling.Profiler.EndSample();
    //}

    public ParticleSystem itemParticleSystem;
    ParticleSystem.Particle[] itemParticles;
    Dictionary<int, ParticleSystem> customItemParticleSystems = new Dictionary<int, ParticleSystem>();
    Dictionary<int, ParticleSystem.Particle[]> customItemParticles = new Dictionary<int, ParticleSystem.Particle[]>();
    Dictionary<int, int> customItemParticleCount = new Dictionary<int, int>();
    Dictionary<int, bool> noCustomParticleColor = new Dictionary<int, bool>();
    OpenSimplexNoise noise = new OpenSimplexNoise();
    //void DrawItems()
    //{
    //    return;
    //    if (ContentLoader.Instance == null)
    //        return;
    //    UnityEngine.Profiling.Profiler.BeginSample("DrawItems", this);
    //    if (itemParticles == null)
    //    {
    //        itemParticles = new ParticleSystem.Particle[itemParticleSystem.main.maxParticles];
    //    }
    //    MapDataStore.Tile tempTile = new MapDataStore.Tile(null, new DFCoord(0, 0, 0));
    //    int i = 0;
    //    foreach (var count in customItemParticleSystems)
    //    {
    //        customItemParticleCount[count.Key] = 0;
    //    }
    //    foreach (var item in itemPositions)
    //    {
    //        var pos = item.Key;
    //        if (!(pos.z < PosZ && pos.z >= (PosZ - GameSettings.Instance.rendering.drawRangeDown)))
    //            continue;

    //        for (int index = 0; index < item.Value.Count && index < 100; index++)
    //        {
    //            var currentItem = item.Value[index];
    //            tempTile.material = currentItem.material;
    //            tempTile.construction_item = currentItem.type;
    //            ColorContent colorContent;
    //            MeshContent meshContent;

    //            var part = new ParticleSystem.Particle();
    //            part.startSize = 1;
    //            part.position = DFtoUnityCoord(currentItem.pos) + new Vector3(0, floorHeight + 0.1f, 0) + Stacker.SpiralHemisphere(index);
    //            if (ContentLoader.Instance.ColorConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out colorContent))
    //                part.startColor = colorContent.color;
    //            else if (materials.ContainsKey(currentItem.material) && materials[currentItem.material].state_color != null)
    //            {
    //                var stateColor = materials[currentItem.material].state_color;
    //                part.startColor = new Color32((byte)stateColor.red, (byte)stateColor.green, (byte)stateColor.blue, 255);
    //            }
    //            else
    //                part.startColor = Color.gray;

    //            if (currentItem.dye != null)
    //            {
    //                part.startColor *= (Color)(new Color32((byte)currentItem.dye.red, (byte)currentItem.dye.green, (byte)currentItem.dye.blue, 255));
    //            }

    //            if (ContentLoader.Instance.ItemMeshConfiguration != null && ContentLoader.Instance.ItemMeshConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out meshContent))
    //            {
    //                ParticleSystem partSys;
    //                if (!customItemParticleSystems.ContainsKey(meshContent.UniqueIndex))
    //                {
    //                    partSys = Instantiate(itemParticleSystem);
    //                    partSys.transform.parent = transform;
    //                    var renderer = partSys.GetComponent<ParticleSystemRenderer>();
    //                    Mesh mesh = new Mesh();
    //                    if (meshContent.MeshData.ContainsKey(MeshLayer.StaticCutout))
    //                    {
    //                        meshContent.MeshData[MeshLayer.StaticCutout].CopyToMesh(mesh);
    //                        noCustomParticleColor[meshContent.UniqueIndex] = false;
    //                    }
    //                    else if (meshContent.MeshData.ContainsKey(MeshLayer.NoMaterialCutout))
    //                    {
    //                        meshContent.MeshData[MeshLayer.NoMaterialCutout].CopyToMesh(mesh);
    //                        noCustomParticleColor[meshContent.UniqueIndex] = true;
    //                    }
    //                    else
    //                    {
    //                        bool copied = false;
    //                        foreach (var backup in meshContent.MeshData)
    //                        {
    //                            backup.Value.CopyToMesh(mesh);
    //                            noCustomParticleColor[meshContent.UniqueIndex] = false;
    //                            copied = true;
    //                            break;
    //                        }
    //                        if (!copied)
    //                            continue;
    //                    }
    //                    renderer.mesh = mesh;
    //                    if (meshContent.MaterialTexture != null)
    //                        renderer.material.SetTexture("_MainTex", meshContent.MaterialTexture.Texture);
    //                    else
    //                    {
    //                        TextureContent texCon;
    //                        if (ContentLoader.Instance.MaterialTextureConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out texCon))
    //                            renderer.material.SetTexture("_MainTex", texCon.Texture);
    //                    }
    //                    if (meshContent.ShapeTexture != null)
    //                        renderer.material.SetTexture("_BumpMap", meshContent.ShapeTexture.Texture);
    //                    else
    //                    {
    //                        NormalContent normalCon;
    //                        if (ContentLoader.Instance.ShapeTextureConfiguration.GetValue(tempTile, MeshLayer.StaticMaterial, out normalCon))
    //                            renderer.material.SetTexture("_BumpMap", normalCon.Texture);
    //                    }
    //                    if (meshContent.SpecialTexture != null)
    //                        renderer.material.SetTexture("_SpecialTex", meshContent.SpecialTexture.Texture);
    //                    customItemParticleSystems[meshContent.UniqueIndex] = partSys;
    //                    customItemParticles[meshContent.UniqueIndex] = new ParticleSystem.Particle[partSys.main.maxParticles];
    //                    customItemParticleCount[meshContent.UniqueIndex] = 0;
    //                }
    //                if (meshContent.Rotation == RotationType.Random)
    //                    part.rotation = (float)noise.eval(pos.x, pos.y, pos.z) * 360;
    //                part.rotation += index * 254.558f;
    //                if (noCustomParticleColor[meshContent.UniqueIndex])
    //                    part.startColor = Color.gray;
    //                customItemParticles[meshContent.UniqueIndex][customItemParticleCount[meshContent.UniqueIndex]] = part;
    //                customItemParticleCount[meshContent.UniqueIndex]++;
    //            }
    //            else
    //            {
    //                itemParticles[i] = part;
    //                i++;
    //            }
    //        }
    //    }
    //    itemParticleSystem.SetParticles(itemParticles, i);
    //    foreach (var sys in customItemParticleSystems)
    //    {
    //        sys.Value.SetParticles(customItemParticles[sys.Key], customItemParticleCount[sys.Key]);
    //    }
    //    UnityEngine.Profiling.Profiler.EndSample();
    //}

}