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
using UnityEngine.EventSystems;
using System.Collections;
using Building;

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
    public int PosXBlock
    {
        get
        {
            return posXTile / blockSize;
        }
    }
    public int PosYBlock
    {
        get
        {
            return posYTile / blockSize;
        }
    }
    [SerializeField]
    int posXTile = 0;
    [SerializeField]
    int posYTile = 0;
    [SerializeField]
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

    public BlockMeshSet[,,] mapMeshes;

    // Dirty flags for those meshes
    bool[,,] blockDirtyBits;
    bool[,,] blockContentBits;
    UpdateSchedule[,,] blockUpdateSchedules;
    bool[,,] liquidBlockDirtyBits;
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
    public static Vector3 DFtoUnityCoord(float x, float y, float z)
    {
        Vector3 outCoord = new Vector3(x * tileWidth, (z + MapZOffset) * tileHeight, y * (-tileWidth));
        return outCoord;
    }
    public static Vector3 DFtoUnityDirection(float x, float y, float z)
    {
        return new Vector3(x* tileWidth, z *tileHeight, y * (-tileWidth));
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

        Debug.Log("Started Armok Vision version " + BuildSettings.Instance.content_version);

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

#if DEVELOPMENT_BUILD
        Debug.Log("Dev build");
#endif

        dfScreen.SetActive(GameSettings.Instance.game.showDFScreen);
    }

    public static GameMap Instance { get; private set; }

    static Thread mainThread;
    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        Instance = this;
        cameraMovement = FindObjectOfType<CameraMovement>();
        mainThread = Thread.CurrentThread;
    }

    public static bool IsMainThread
    {
        get
        {
            return Thread.CurrentThread == mainThread;
        }
    }

    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void BeginSample(string name)
    {
        if (IsMainThread)
            UnityEngine.Profiling.Profiler.BeginSample(name);
    }
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void BeginSample(string name, Object targetObject)
    {
        if (IsMainThread)
            UnityEngine.Profiling.Profiler.BeginSample(name, targetObject);
    }

    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void EndSample()
    {
        if (IsMainThread)
            UnityEngine.Profiling.Profiler.EndSample();
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
                SaveMaterialList(materials, Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "MaterialList.csv"));
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
                SaveMaterialList(items, Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "ItemList.csv"));
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
        if (DFConnection.Instance.CreatureRaws != null)
        {
            if (creatures == null)
                creatures = new Dictionary<MatPairStruct, MaterialDefinition>();
            foreach (CreatureRaw creatureRaw in DFConnection.Instance.CreatureRaws)
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
            GeneratedCreatureTranslator.AddFakeCreaturesToList(creatures);
            if (GameSettings.Instance.debug.saveCreatureList)
                SaveMaterialList(creatures, Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "CreatureList.csv"));
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


        if (!GameSettings.Instance.game.showDFScreen && EventSystem.current.currentSelectedGameObject == null && DFConnection.Instance.WorldMode != dfproto.GetWorldInfoOut.Mode.MODE_ADVENTURE)
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
                string path = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "Screenshots");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string screenshotFilename;
                do
                {
                    screenshotCount++;
                    screenshotFilename = Path.Combine(path, "screenshot" + screenshotCount + ".png");

                } while (File.Exists(screenshotFilename));

                Debug.Log ("Saving screenshot to: " + screenshotFilename);

                if (Input.GetButton("Mod"))
                {
                    RenderTexture.active = Minimap.Instance.texture;
                    Texture2D virtualPhoto =
                        new Texture2D(Minimap.Instance.texture.width, Minimap.Instance.texture.height, TextureFormat.RGB24, false);
                    // false, meaning no need for mipmaps
                    virtualPhoto.ReadPixels(new Rect(0, 0, Minimap.Instance.texture.width, Minimap.Instance.texture.height), 0, 0);

                    RenderTexture.active = null; //can help avoid errors 

                    File.WriteAllBytes(screenshotFilename, virtualPhoto.EncodeToPNG());
                }
                else
                    UnityEngine.ScreenCapture.CaptureScreenshot(screenshotFilename);
            }
            if (Input.GetButtonDown("Refresh"))
            {
                Refresh();
            }
            if (Input.GetButtonDown("Cancel"))
            {
                optionsPanel.gameObject.SetActive(!optionsPanel.gameObject.activeSelf);
            }
            if(Input.GetButtonDown("ToggleMap"))
            {
                mapWindow.SetActive(!mapWindow.activeSelf);
                mapCamera.SetActive(!mapCamera.activeSelf);
            }
            if(Input.GetButtonDown("ToggleUI"))
            {
                mainUI.gameObject.SetActive(!mainUI.gameObject.activeSelf);
            }
        }

        if (ContentLoader.Instance == null)
            return;

        ShowCursorInfo();
        UpdateRequestRegion();
        UpdateBlocks();
        //DrawBlocks();
        UpdateBlockVisibility();
    }
    private void UpdateBlockVisibility()
    {
        Shader.SetGlobalVector("_ViewMin", DFtoUnityBottomCorner(new DFCoord(
            (PosXBlock - GameSettings.Instance.rendering.drawRangeSide + 1) * blockSize,
            (PosYBlock + GameSettings.Instance.rendering.drawRangeSide) * blockSize,
            PosZ - GameSettings.Instance.rendering.drawRangeDown
        )) + new Vector3(0, 0, GameMap.tileWidth));
        Shader.SetGlobalVector("_ViewMax", DFtoUnityBottomCorner(new DFCoord(
            (PosXBlock + GameSettings.Instance.rendering.drawRangeSide) * blockSize,
            (PosYBlock - GameSettings.Instance.rendering.drawRangeSide + 1) * blockSize,
            PosZ + (firstPerson ? GameSettings.Instance.rendering.drawRangeUp : 0)
        )) + new Vector3(0, 0, GameMap.tileWidth));

        for (int z = 0; z < mapMeshes.GetLength(2); z++)
        {
            UpdateBlockVisibility(z);
        }
    }

    void UpdateBlockVisibility(int z)
    {
        if (z < 0 || z >= mapMeshes.GetLength(2))
            return;
        for (int x = 0; x < mapMeshes.GetLength(0); x++)
            for (int y = 0; y < mapMeshes.GetLength(1); y++)
            {
                if (mapMeshes[x, y, z] == null)
                    continue;
                if(
                    x <= PosXBlock - GameSettings.Instance.rendering.drawRangeSide
                    || x >= PosXBlock + GameSettings.Instance.rendering.drawRangeSide
                    || y <= PosYBlock - GameSettings.Instance.rendering.drawRangeSide
                    || y >= PosYBlock + GameSettings.Instance.rendering.drawRangeSide
                )
                    mapMeshes[x, y, z].UpdateVisibility(BlockMeshSet.Visibility.None);
                else
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
                return BlockMeshSet.Visibility.Walls;
            else if (overheadShadows)
                return BlockMeshSet.Visibility.Shadows;
            else
                return BlockMeshSet.Visibility.None;
        }
        else if (z == PosZ - 1)
            return BlockMeshSet.Visibility.All;
        else if (z >= PosZ - GameSettings.Instance.rendering.drawRangeDown)
        {
            return BlockMeshSet.Visibility.Walls;
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
                        SplatManager.Instance.DirtyLayer(x, y, z);
                        SplatManager.Instance.DirtyGrass(x, y, z);
                        SplatManager.Instance.DirtySpatter(x, y, z);
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

        if(!cameraMovement.following)
            return;

        UnityEngine.Profiling.Profiler.BeginSample("UpdateView", this);
        //Debug.Log("Got view");
        view = newView;

        if (view.follow_unit_id != -1 && CreatureManager.Instance.Units != null)
        {
            int unitIndex = CreatureManager.Instance.Units.creature_list.FindIndex(x => x.id == view.follow_unit_id);
            if (unitIndex >= 0)
            {
                var unit = CreatureManager.Instance.Units.creature_list[unitIndex];
                posXTile = unit.pos_x;
                posYTile = unit.pos_y;
                posZ = unit.pos_z + 1;
                return;
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

        BuildingManager.Instance.BeginExistenceCheck();
        ItemManager.Instance.BeginExistenceCheck();
        UnityEngine.Profiling.Profiler.BeginSample("UpdateBlocks", this);
        MapBlock block;
        if(DFConnection.Instance.UpdatedAnyBlocks)
        {
            FlowManager.Instance.Clear();
            DFConnection.Instance.UpdatedAnyBlocks = false;
        }
        while((block = DFConnection.Instance.PopMapBlockUpdate()) != null)
        {
            bool setTiles;
            bool setLiquids;
            bool setSpatters;
            UnityEngine.Profiling.Profiler.BeginSample("StoreTiles", this);
            MapDataStore.StoreTiles(block, out setTiles, out setLiquids, out setSpatters);
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
            //foreach (var item in block.items)
            //{
            //    ///Send it to item manager later.
            //}
            UnityEngine.Profiling.Profiler.BeginSample("BuildingManager.LoadBlock", this);
            BuildingManager.Instance.LoadBlock(block);
            UnityEngine.Profiling.Profiler.EndSample();
            UnityEngine.Profiling.Profiler.BeginSample("ItemManager.LoadBlock", this);
            ItemManager.Instance.LoadBlock(block);
            UnityEngine.Profiling.Profiler.EndSample();
            FlowManager.Instance[new DFCoord(block.map_x, block.map_y, block.map_z)] = block.flows;
        }
        BuildingManager.Instance.EndExistenceCheck();
        ItemManager.Instance.EndExitenceCheck();
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
        SplatManager.Instance.Init(blockSizeX * 16 / blockSize, blockSizeY * 16 / blockSize, blockSizeZ);

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
        SplatManager.Instance.DirtyGrass(mapBlockX, mapBlockY, mapBlockZ);
        SplatManager.Instance.DirtyLayer(mapBlockX, mapBlockY, mapBlockZ);
        if (mapBlockZ < SplatManager.Instance.SizeZ - 1)
        {
            SplatManager.Instance.DirtyGrass(mapBlockX, mapBlockY, mapBlockZ + 1);
            SplatManager.Instance.DirtyLayer(mapBlockX, mapBlockY, mapBlockZ + 1);
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
        map_x /= blockSize;
        map_y /= blockSize;
        SplatManager.Instance.DirtySpatter(map_x, map_y, map_z);

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
        string fileName = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "TiletypeList.csv");
        try
        {
            File.Delete(fileName);
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (Tiletype item in DFConnection.Instance.NetTiletypeList.tiletype_list)
            {
                writer.WriteLine(
                    item.id + "," + 
                    "\"" + item.name + "\"," +
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

        string fileName = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "BuildingList.csv");

        try
        {
            File.Delete(fileName);
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (var item in DFConnection.Instance.NetBuildingList.building_list)
            {
                writer.WriteLine("\"" +
                    item.name + "\",\"" +
                    item.id + "\"," +
                    item.building_type.building_type + ":" +
                    item.building_type.building_subtype + ":" +
                    item.building_type.building_custom
                    );
            }
        }
    }
    class ListContainer
    {
        public List<MaterialDefinition> list = new List<MaterialDefinition>();
    }

    void SaveMaterialList(IEnumerable<KeyValuePair<MatPairStruct, MaterialDefinition>> list, string filename)
    {
        string jsonFile = Path.ChangeExtension(filename, "json");
        try
        {
            File.Delete(filename);
            File.Delete(jsonFile);
        }
        catch (IOException)
        {
            return;
        }
        ListContainer container = new ListContainer();
        using (StreamWriter writer = new StreamWriter(filename))
        {
            foreach (var item in list)
            {
                writer.WriteLine("\"" + 
                    item.Value.name + "\",\"" +
                    item.Value.id + "\"," +
                    item.Value.mat_pair.mat_type + "," +
                    item.Value.mat_pair.mat_index
                    );
                container.list.Add(item.Value);
            }
        }
        File.WriteAllText(jsonFile, Newtonsoft.Json.JsonConvert.SerializeObject(container, Newtonsoft.Json.Formatting.Indented));
    }

    void SavePlantList()
    {
        if (DFConnection.Instance.NetPlantRawList == null)
            return;
        string fileName = Path.Combine(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), Application.productName), "PlantList.csv");
        try
        {
            File.Delete(fileName);
        }
        catch (IOException)
        {
            return;
        }
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (var plant in TokenLists.PlantTokenList.GrowthIDs)
            {
                foreach (var growth in plant.Value)
                {
                    foreach (var print in growth.Value)
                    {
                        writer.Write("\"");
                        writer.Write(plant.Key);
                        writer.Write("\",\"");
                        writer.Write(growth.Key);
                        writer.Write("\",\"");
                        writer.Write(print.Key);
                        writer.Write("\",\"");
                        writer.Write(print.Value);
                        writer.Write("\"");
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


    // Have the mesher mesh all dirty tiles in the region
    void EnqueueMeshUpdates()
    {
        int queueCount = 0;
        int xmin = Mathf.Clamp(PosXBlock - GameSettings.Instance.rendering.drawRangeSide + 1, 0, mapMeshes.GetLength(0));
        int xmax = Mathf.Clamp(PosXBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(0));
        int ymin = Mathf.Clamp(PosYBlock - GameSettings.Instance.rendering.drawRangeSide + 1, 0, mapMeshes.GetLength(1));
        int ymax = Mathf.Clamp(PosYBlock + GameSettings.Instance.rendering.drawRangeSide, 0, mapMeshes.GetLength(1));
        int zmin = Mathf.Clamp(posZ - GameSettings.Instance.rendering.drawRangeDown, 0, mapMeshes.GetLength(2));
        int zmax = Mathf.Clamp(posZ + GameSettings.Instance.rendering.drawRangeUp, 0, mapMeshes.GetLength(2));
        if (PosXBlock >= 0 && PosXBlock < mapMeshes.GetLength(0) && PosYBlock >= 0 && PosYBlock < mapMeshes.GetLength(1))
            for (int zz = posZ - 1; zz >= zmin; zz--)
            {
                if (zz >= mapMeshes.GetLength(2))
                    continue;
                if (!blockDirtyBits[PosXBlock, PosYBlock, zz] && !liquidBlockDirtyBits[PosXBlock, PosYBlock, zz])
                {
                    continue;
                }
                if (!(zz == posZ - 1) && MapDataStore.IsFullyHidden(PosXBlock, PosYBlock, zz))
                    continue;
                //If we were not able to add it to the queue, don't try any more till next fame.
                if (!mesher.Enqueue(new DFCoord(PosXBlock * 16, PosYBlock * 16, zz), blockDirtyBits[PosXBlock, PosYBlock, zz], liquidBlockDirtyBits[PosXBlock, PosYBlock, zz]))
                    return;
                blockDirtyBits[PosXBlock, PosYBlock, zz] = false;
                liquidBlockDirtyBits[PosXBlock, PosYBlock, zz] = false;
                queueCount++;
                if (queueCount > GameSettings.Instance.meshing.queueLimit)
                    return;
            }
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
                    if (!(zz == posZ - 1) && MapDataStore.IsFullyHidden(PosXBlock, PosYBlock, zz))
                        continue;

                    //If we were not able to add it to the queue, don't try any more till next fame.
                    if (!mesher.Enqueue(new DFCoord(xx * 16, yy * 16, zz), blockDirtyBits[xx, yy, zz], liquidBlockDirtyBits[xx, yy, zz]))
                        return;
                    blockDirtyBits[xx, yy, zz] = false;
                    liquidBlockDirtyBits[xx, yy, zz] = false;
                    queueCount++;
                    if (queueCount > GameSettings.Instance.meshing.queueLimit)
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
                    if (MapDataStore.IsFullyHidden(PosXBlock, PosYBlock, zz))
                        continue;

                    //If we were not able to add it to the queue, don't try any more till next fame.
                    if (!mesher.Enqueue(new DFCoord(xx * 16, yy * 16, zz), blockDirtyBits[xx, yy, zz], liquidBlockDirtyBits[xx, yy, zz]))
                        return;
                    blockDirtyBits[xx, yy, zz] = false;
                    liquidBlockDirtyBits[xx, yy, zz] = false;
                    queueCount++;
                    if (queueCount > GameSettings.Instance.meshing.queueLimit)
                        return;
                }
        }
    }

    private BlockMeshSet blockPrefab;

    // Get new meshes from the mesher
    void FetchNewMeshes()
    {

        UnityEngine.Profiling.Profiler.BeginSample("FetchNewMeshes", this);
        while (mesher.HasNewMeshes)
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

                SplatManager.Instance.ApplyTerrain(mapMeshes[block_x, block_y, block_z], block_z);
                SplatManager.Instance.ApplyGrass(mapMeshes[block_x, block_y, block_z], block_z);
                SplatManager.Instance.ApplySpatter(mapMeshes[block_x, block_y, block_z], block_z);
            }

            var meshSet = mapMeshes[block_x, block_y, block_z];

            meshSet.LoadMeshes(newMeshes, string.Format("{0}_{1}_{2}", block_x, block_y, block_z));
            meshSet.UpdateVisibility(GetVisibility(block_z));

            if (newMeshes.collisionMesh != null)
            {

                Mesh collisionMesh = new Mesh();
                collisionMesh.name = string.Format("block_collision_{0}_{1}_{2}", block_x, block_y, block_z);
                newMeshes.collisionMesh.CopyToMesh(collisionMesh);
                meshSet.collisionBlocks.sharedMesh = null;
                meshSet.collisionBlocks.sharedMesh = collisionMesh;
            }
        }
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
        CreatureManager.Instance.Clear();
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

                statusText.Append(Building.BuildingManager.Instance.GetBuildingInfoText(new DFCoord(cursX, cursY, cursZ)));

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

            if (CreatureManager.Instance.Units != null)
            {
                UnitDefinition foundUnit = null;
                foreach (UnitDefinition unit in CreatureManager.Instance.Units.creature_list)
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
                    if (DFConnection.Instance.CreatureRaws != null)
                        creatureRaw = DFConnection.Instance.CreatureRaws[foundUnit.race.mat_type];

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

    private int screenshotCount;


    public void UpdateCenter(Vector3 pos)
    {
        if(cameraMovement.following)
            return;
        DFCoord dfPos = UnityToDFCoord(pos);
        posXTile = dfPos.x;
        posYTile = dfPos.y;
        if (posZ != dfPos.z + 1)
        {
            posZ = dfPos.z + 1;
        }
    }


    public ParticleSystem itemParticleSystem;
    public GameObject mapWindow;
    public GameObject mapCamera;
    public Canvas mainUI;
    //ParticleSystem.Particle[] itemParticles;
    //Dictionary<int, ParticleSystem> customItemParticleSystems = new Dictionary<int, ParticleSystem>();
    //Dictionary<int, ParticleSystem.Particle[]> customItemParticles = new Dictionary<int, ParticleSystem.Particle[]>();
    //Dictionary<int, int> customItemParticleCount = new Dictionary<int, int>();
    //Dictionary<int, bool> noCustomParticleColor = new Dictionary<int, bool>();
    //OpenSimplexNoise noise = new OpenSimplexNoise();
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