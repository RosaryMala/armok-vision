using DFHack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TokenLists;
using UnityEngine;
using Util;
using RemoteFortressReader;

// Class for async communication with DF.
// Will eventually run actual communication on a separate thread.
// Singleton-y - attached to a GameObject.

public sealed class DFConnection : MonoBehaviour
{

    // Singleton stuff

    private static DFConnection instance = null;
    private static List<Action> connectionCallbacks = new List<Action>();
    public static DFConnection Instance
    {
        get
        {
            return instance;
        }
    }

    static bool _actuallyConnected = false;
    // Can always be called
    public static bool Connected
    {
        get
        {
            return instance != null && instance.networkClient != null && _actuallyConnected;
        }
    }

    // Can be called before instance is initialized.
    public static void RegisterConnectionCallback(Action callback)
    {
        connectionCallbacks.Add(callback);
    }

    // Instance stuff

    // Assignable values
    public int BlocksToFetch = 4;
    public bool RunOnAlternateThread = false;
    public float refreshDelay = 100;

    // Thread management
    private ConnectionManager connectionManager;

    #region RPC Bindings

    //Static bindings
    private RemoteFunction<dfproto.EmptyMessage, dfproto.StringMessage> dfhackVersionCall;
    private RemoteFunction<dfproto.EmptyMessage, dfproto.StringMessage> dfVersionCall;
    private RemoteFunction<dfproto.EmptyMessage, dfproto.GetWorldInfoOut> dfWorldInfoCall;

    // Plugin bindings
    private RemoteFunction<dfproto.EmptyMessage, MaterialList> materialListCall;
    private RemoteFunction<dfproto.EmptyMessage, MaterialList> itemListCall;
    private RemoteFunction<dfproto.EmptyMessage, TiletypeList> tiletypeListCall;
    private RemoteFunction<BlockRequest, BlockList> blockListCall;
    private RemoteFunction<dfproto.EmptyMessage, UnitList> unitListCall;
    private RemoteFunction<dfproto.EmptyMessage, ViewInfo> viewInfoCall;
    private RemoteFunction<dfproto.EmptyMessage, MapInfo> mapInfoCall;
    private RemoteFunction<dfproto.EmptyMessage> mapResetCall;
    private RemoteFunction<dfproto.EmptyMessage, BuildingList> buildingListCall;
    private RemoteFunction<dfproto.EmptyMessage, WorldMap> worldMapCall;
    private RemoteFunction<dfproto.EmptyMessage, WorldMap> worldMapCenterCall;
    private RemoteFunction<dfproto.EmptyMessage, RegionMaps> regionMapCall;
    private RemoteFunction<dfproto.EmptyMessage, CreatureRawList> creatureRawListCall;
    private RemoteFunction<ListRequest, CreatureRawList> partialCreatureRawListCall;
    private RemoteFunction<dfproto.EmptyMessage, PlantRawList> plantRawListCall;
    private RemoteFunction<KeyboardEvent> keyboardEventCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ScreenCapture> copyScreenCall;
    private RemoteFunction<DigCommand> digCommandCall;
    private RemoteFunction<SingleBool> pauseCommandCall;
    private RemoteFunction<dfproto.EmptyMessage, SingleBool> pauseStatusCall;
    private RemoteFunction<dfproto.EmptyMessage, VersionInfo> versionInfoCall;
    private color_ostream dfNetworkOut = new color_ostream();
    private RemoteClient networkClient;

    /// <summary>
    /// Tries to bind an RPC function, leaving returning null if it fails.
    /// </summary>
    /// <typeparam name="Input">Protobuf class used as an input</typeparam>
    /// <param name="client">Connection to Dwarf Fortress</param>
    /// <param name="name">Name of the RPC function to bind to</param>
    /// <param name="proto">Name of the protobuf file to use</param>
    /// <returns>Bound remote function on success, otherwise null.</returns>
    RemoteFunction<Input> CreateAndBind<Input>(RemoteClient client, string name, string proto = "") where Input : class, ProtoBuf.IExtensible, new()
    {
        RemoteFunction<Input> output = new RemoteFunction<Input>();
        if (output.bind(client, name, proto))
            return output;
        else
            return null;
    }

    /// <summary>
    /// Tries to bind an RPC function, returning null if it fails.
    /// </summary>
    /// <typeparam name="Input">Protobuf class used as an input</typeparam>
    /// <typeparam name="Output">Protobuf class to use as an output</typeparam>
    /// <param name="client">Connection to Dwarf Fortress</param>
    /// <param name="name">Name of the RPC function to bind to</param>
    /// <param name="proto">Name of the protobuf file to use</param>
    /// <returns>Bound remote function on success, otherwise null.</returns>
    RemoteFunction<Input, Output> CreateAndBind<Input, Output>(RemoteClient client, string name, string proto = "")
        where Input : class, ProtoBuf.IExtensible, new()
        where Output : class, ProtoBuf.IExtensible, new()
    {
        RemoteFunction<Input, Output> output = new RemoteFunction<Input, Output>();
        if (output.bind(client, name, proto))
            return output;
        else
            return null;
    }

    /// <summary>
    /// Bind methods that aren't dependent on the RFR plugin.
    /// </summary>
    void BindStaticMethods()
    {
        dfhackVersionCall = CreateAndBind<dfproto.EmptyMessage, dfproto.StringMessage>(networkClient, "GetVersion");
        dfVersionCall = CreateAndBind<dfproto.EmptyMessage, dfproto.StringMessage>(networkClient, "GetDFVersion");
        dfWorldInfoCall = CreateAndBind<dfproto.EmptyMessage, dfproto.GetWorldInfoOut>(networkClient, "GetWorldInfo");
    }

    /// <summary>
    /// Bind the RPC functions we'll be calling
    /// </summary>
    void BindMethods()
    {
        materialListCall = CreateAndBind<dfproto.EmptyMessage, MaterialList>(networkClient, "GetMaterialList", "RemoteFortressReader");
        itemListCall = CreateAndBind<dfproto.EmptyMessage, MaterialList>(networkClient, "GetItemList", "RemoteFortressReader");
        tiletypeListCall = CreateAndBind<dfproto.EmptyMessage, TiletypeList>(networkClient, "GetTiletypeList", "RemoteFortressReader");
        blockListCall = CreateAndBind<BlockRequest, BlockList>(networkClient, "GetBlockList", "RemoteFortressReader");
        unitListCall = CreateAndBind<dfproto.EmptyMessage, UnitList>(networkClient, "GetUnitList", "RemoteFortressReader");
        viewInfoCall = CreateAndBind<dfproto.EmptyMessage, ViewInfo>(networkClient, "GetViewInfo", "RemoteFortressReader");
        mapInfoCall = CreateAndBind<dfproto.EmptyMessage, MapInfo>(networkClient, "GetMapInfo", "RemoteFortressReader");
        mapResetCall = CreateAndBind<dfproto.EmptyMessage>(networkClient, "ResetMapHashes", "RemoteFortressReader");
        buildingListCall = CreateAndBind<dfproto.EmptyMessage, BuildingList>(networkClient, "GetBuildingDefList", "RemoteFortressReader");
        worldMapCall = CreateAndBind<dfproto.EmptyMessage, WorldMap>(networkClient, "GetWorldMapNew", "RemoteFortressReader");
        worldMapCenterCall = CreateAndBind<dfproto.EmptyMessage, WorldMap>(networkClient, "GetWorldMapCenter", "RemoteFortressReader");
        regionMapCall = CreateAndBind<dfproto.EmptyMessage, RegionMaps>(networkClient, "GetRegionMapsNew", "RemoteFortressReader");
        creatureRawListCall = CreateAndBind<dfproto.EmptyMessage, CreatureRawList>(networkClient, "GetCreatureRaws", "RemoteFortressReader");
        partialCreatureRawListCall = CreateAndBind<ListRequest, CreatureRawList>(networkClient, "GetPartialCreatureRaws", "RemoteFortressReader");
        plantRawListCall = CreateAndBind<dfproto.EmptyMessage, PlantRawList>(networkClient, "GetPlantRaws", "RemoteFortressReader");
        keyboardEventCall = CreateAndBind<KeyboardEvent>(networkClient, "PassKeyboardEvent", "RemoteFortressReader");
        copyScreenCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.ScreenCapture>(networkClient, "CopyScreen", "RemoteFortressReader");
        digCommandCall = CreateAndBind<DigCommand>(networkClient, "SendDigCommand", "RemoteFortressReader");
        pauseCommandCall = CreateAndBind<SingleBool>(networkClient, "SetPauseState", "RemoteFortressReader");
        pauseStatusCall = CreateAndBind<dfproto.EmptyMessage, SingleBool>(networkClient, "GetPauseState", "RemoteFortressReader");
        versionInfoCall = CreateAndBind<dfproto.EmptyMessage, VersionInfo>(networkClient, "GetVersionInfo", "RemoteFortressReader");
    }

    #endregion

    // Things we read from DF
    dfproto.GetWorldInfoOut netWorldInfo;

    // Unchanging
    private MaterialList netMaterialList;
    private MaterialList netItemList;
    private TiletypeList netTiletypeList;
    private BuildingList netBuildingList;
    private List<CreatureRaw> creatureRaws;
    private PlantRawList netPlantRawList;

    // Output queues
    private SingleBuffer<ViewInfo> netViewInfo;
    private SingleBuffer<UnitList> netUnitList;
    private SingleBuffer<WorldMap> netWorldMap;
    private SingleBuffer<RegionMaps> netRegionMaps;
    private RingBuffer<MapBlock> pendingBlocks
        = new RingBuffer<MapBlock>(1024);
    private SingleBuffer<RemoteFortressReader.ScreenCapture> netScreenCapture;
    private EventBuffer worldMapMoved;

    // Input queues
    private RingBuffer<KeyboardEvent> keyPresses
        = new RingBuffer<KeyboardEvent>(128);

    #region Command DF

    private RingBuffer<DigCommand> netDigCommands 
        = new RingBuffer<DigCommand>(8);

    /// <summary>
    /// Enqueue's a digging designation command to send to DF
    /// </summary>
    /// <param name="command"></param>
    public void EnqueueDigCommand(DigCommand command)
    {
        if (digCommandCall == null)
            return; //don't bother.
        if(netDigCommands.Count < netDigCommands.Capacity)
            netDigCommands.Enqueue(command);
    }

    private RingBuffer<SingleBool> pauseCommands
        = new RingBuffer<SingleBool>(8);

    public void SendPauseCommand(bool state)
    {
        SingleBool command = new SingleBool();

        command.Value = state;
        if (pauseCommands.Count < pauseCommands.Capacity)
            pauseCommands.Enqueue(command);
    }

    #endregion

    /// <summary>
    /// Queue to set the area we want to get updates for.
    /// </summary>
    private SingleBufferStruct<BlockCoord.Range> requestRegion;
    /// <summary>
    /// Queue to request map resets.
    /// </summary>
    private EventBuffer mapResetRequested;


    // Used to check whether the map has moved.
    private MapInfo netMapInfo;
    private WorldMap netWorldMapCenter;

    //Rarely changing info that can nevertheless change.
    private DFCoord embarkMapSize = new DFCoord(0, 0, 0);
    private DFCoord embarkMapPosition = new DFCoord(-1, -1, -1);

    // Mutexes for changing / nullable objects
    private UnityEngine.Object mapInfoLock = new UnityEngine.Object();

    // Cached block request
    private readonly BlockRequest blockRequest
        = new BlockRequest();


    // Unchanging properties

    public MaterialList NetMaterialList
    {
        get { return netMaterialList; }
    }

    public MaterialList NetItemList
    {
        get { return netItemList; }
    }

    public TiletypeList NetTiletypeList
    {
        get { return netTiletypeList; }
    }

    public BuildingList NetBuildingList
    {
        get { return netBuildingList; }
    }

    public List<CreatureRaw> CreatureRaws
    {
        get { return creatureRaws; }
    }

    public PlantRawList NetPlantRawList
    {
        get { return netPlantRawList; }
    }

    public void SetRequestRegion(BlockCoord min, BlockCoord max)
    {
        requestRegion.Set(new BlockCoord.Range(min, max));
    }

    /// <summary>
    /// Pop a view update; return null if there isn't one.
    /// </summary>
    public ViewInfo PopViewInfoUpdate()
    {
        return netViewInfo.Pop();
    }

    public RemoteFortressReader.ScreenCapture PopScreenUpdate()
    {
        return netScreenCapture.Pop();
    }

    /// <summary>
    // Pop a unit list update; return null if there isn't one.
    /// </summary>
    public UnitList PopUnitListUpdate()
    {
        return netUnitList.Pop();
    }

    /// <summary>
    /// Pop a world map update. The map doesn't change, but the clouds do.
    /// </summary>
    public WorldMap PopWorldMapUpdate()
    {
        return netWorldMap.Pop();
    }

    /// <summary>
    /// Pop a world map update. The map doesn't change, but the clouds do.
    /// </summary>
    public bool HasWorldMapPositionChanged()
    {
        return worldMapMoved.Pop();
    }

    /// <summary>
    /// Pop region map update. These change in adventure mode.
    /// </summary>
    public RegionMaps PopRegionMapUpdate()
    {
        return netRegionMaps.Pop();
    }

    /// <summary>
    /// Fetch the current embark map size
    /// </summary>
    /// <value>The size of the embark map.</value>
    public DFCoord EmbarkMapSize
    {
        get
        {
            lock (mapInfoLock)
            {
                return embarkMapSize;
            }
        }
    }

    /// <summary>
    /// Gets the embark map position.
    /// </summary>
    /// <value>The embark map position.</value>
    public DFCoord EmbarkMapPosition
    {
        get
        {
            lock (mapInfoLock)
            {
                return embarkMapPosition;
            }
        }
    }

    /// <summary>
    /// Pop a map block update; return null if there isn't one.
    /// </summary>
    public MapBlock PopMapBlockUpdate()
    {
        MapBlock result;
        pendingBlocks.TryDequeue(out result);
        return result;
    }

    bool _dfPauseState = false;
    public bool DfPauseState
    {
        get
        {
            return _dfPauseState;
        }
    }

    bool _needNewBlocks = false;
    public bool NeedNewBlocks
    {
        set
        {
            _needNewBlocks = value;
        }
    }

    /// <summary>
    /// Queue a map hash reset, forcing a re-send of all map blocks
    /// </summary>
    public void RequestMapReset()
    {
        mapResetRequested.Set();
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    /// <summary>
    /// Connect to DF, and register basic RPC functions.
    /// </summary>
    void Connect()
    {
        blockRequest.blocks_needed = BlocksToFetch;
        networkClient = new DFHack.RemoteClient(dfNetworkOut);
        bool success = networkClient.connect();
        if (!success)
        {
            networkClient.disconnect();
            networkClient = null;
            ModalPanel.Instance.Choice(
                "Armok Vision could not find a running instance of Dwarf Fortress!\n\n" +
                "Please make sure you have Dwarf Fortress running, with the latest version of DFHack installed.", Connect, QuitGame, "Retry", "Quit");
            throw new UnityException("DF Connection Failure");
        }

        BindStaticMethods();

        if (dfWorldInfoCall != null)
        {
            dfWorldInfoCall.execute(null, out netWorldInfo);
            if (netWorldInfo == null)
                Debug.Log("World not loaded.");
            else
                Debug.Log("World Mode: " + netWorldInfo.mode);
        }



        if (GameSettings.Instance.game.askToUpdatePlugin)
        {
            CheckPlugin();
        }
        else
        {
            Init();
        }

    }


    bool inited = false;
    /// <summary>
    /// Register AV-specific functions, and get things rolling.
    /// </summary>
    void Init()
    {
        if (inited)
            return;
        //if (netWorldInfo == null)
        //    return; //world isn't loaded
        inited = true;
        BindMethods();

        if (versionInfoCall != null && dfVersionCall != null)
        {
            VersionInfo versionInfo;
            dfproto.StringMessage dfVersion = new dfproto.StringMessage();
            versionInfoCall.execute(null, out versionInfo);
            dfVersionCall.execute(null, out dfVersion);
            Debug.LogFormat("Connected to DF version {0}, running DFHack version {1}, and RemoteFortressReader version {2}", versionInfo.dwarf_fortress_version, versionInfo.dfhack_version, versionInfo.remote_fortress_reader_version);
            if (GoogleAnalyticsV4.instance != null)
                GoogleAnalyticsV4.instance.SendDeviceData(versionInfo.dwarf_fortress_version, versionInfo.remote_fortress_reader_version);
        }
        else if(dfVersionCall != null)
        {
            dfproto.StringMessage dfVersion = new dfproto.StringMessage();
            dfproto.StringMessage dfHackVersion = new dfproto.StringMessage();
            dfVersionCall.execute(null, out dfVersion);
            dfhackVersionCall.execute(null, out dfHackVersion);
            Debug.LogFormat("Connected to DF version {0}, , running DFHack version {1}, and an old RemoteFortressReader plugin.", dfVersion.value, dfHackVersion.value);
            if (GoogleAnalyticsV4.instance != null)
                GoogleAnalyticsV4.instance.SendDeviceData(dfVersion.value, "old");

        }
        else
        {
            Debug.Log("Connected to an old version of RemoteFortressReader");
            if (GoogleAnalyticsV4.instance != null)
                GoogleAnalyticsV4.instance.SendDeviceData("old", "old");
        }

        FetchUnchangingInfo();

        // Get some initial stuff
        // Necessary for initialization, apparently.
        networkClient.suspend_game();
        if (viewInfoCall != null)
        {
            ViewInfo viewInfo;
            viewInfoCall.execute(null, out viewInfo);
            netViewInfo.Set(viewInfo);
        }
        if (copyScreenCall != null)
        {
            RemoteFortressReader.ScreenCapture screenCapture;
            copyScreenCall.execute(null, out screenCapture);
            netScreenCapture.Set(screenCapture);
        }
        if (unitListCall != null)
        {
            UnitList unitList;
            unitListCall.execute(null, out unitList);
            netUnitList.Set(unitList);
        }
        if (worldMapCall != null)
        {
            WorldMap worldMap;
            worldMapCall.execute(null, out worldMap);
            netWorldMap.Set(worldMap);
        }
        if (regionMapCall != null)
        {
            RegionMaps regionMaps;
            regionMapCall.execute(null, out regionMaps);
            netRegionMaps.Set(regionMaps);
        }
        networkClient.resume_game();

        if (mapResetCall != null)
            mapResetCall.execute();
        InitStatics();

        foreach (Action callback in connectionCallbacks)
        {
            callback.Invoke();
        }

        connectionCallbacks.Clear();

        if (RunOnAlternateThread)
        {
            connectionManager = new ConnectionManager.AltThread(this);
        }
        else
        {
            connectionManager = new ConnectionManager.UnityThread(this);
        }
        _actuallyConnected = true;
    }

    string DFHackPluginDirectory
    {
        get
        {
            DFStringStream tempStream = new DFStringStream();
            networkClient.run_command(tempStream, "lua", new List<string>(new string[] { "!dfhack.getHackPath()" }));

            return tempStream.Value.Trim() + "/plugins/";
        }
    }

    string AVPluginDirectory
    {
        get
        {
            if (dfhackVersionCall == null || dfVersionCall == null)
                return "";
            dfproto.StringMessage dfHackVersion = new dfproto.StringMessage();
            dfproto.StringMessage dfVersion = new dfproto.StringMessage();

            dfhackVersionCall.execute(null, out dfHackVersion);
            dfVersionCall.execute(null, out dfVersion);

            string pluginDirectory = "Plugins/" + dfVersion.value + "/" + dfHackVersion.value + "/";
#if UNITY_EDITOR
            pluginDirectory = "ReleaseFiles/" + pluginDirectory;
            Directory.CreateDirectory(pluginDirectory);
#endif
            return pluginDirectory;
        }
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    string pluginName = "plug.dll";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    string pluginName = "plug.dylib";
#elif UNITY_STANDALONE_LINUX
    string pluginName = "plug.so";
#else
    string pluginName = "INVALID";
#endif

    private void CheckPlugin()
    {
        Version pluginVersion = new Version(0,0,0);
        Version avVersion = new Version(BuildSettings.Instance.content_version);

        DFStringStream tempStream = new DFStringStream();
        networkClient.run_command(tempStream, "RemoteFortressReader_version", new List<string>());
        var results = tempStream.Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        string versionString = results[0].Trim();
        try
        {
            pluginVersion = new Version(versionString);
        }
        catch (Exception)
        {
        }

        if (avVersion > pluginVersion)
        {
            if (!File.Exists(AVPluginDirectory + pluginName))
            {
                Debug.Log ("Cannot find " + AVPluginDirectory + pluginName);
                ModalPanel.Instance.Choice(string.Format(
                    "You appear to be running on an out-dated version of the RemoteFortressReader plugin.\n\n" +
                    "You're running version {0} of the plugin, while Armok Vision expects a plugin versioned {1} or above.\n\n" +
                    "A compatible version of the plugin has not been found. Please let the developers know which version of DFHack and DF you are running, so they can make one.", pluginVersion, avVersion), Init, DisableUpdate, "Okay.", "Don't ask again");
                return; //We don't have a compatible plugin
            }
            else
            {
            Debug.Log ("Found " + AVPluginDirectory + pluginName);
                ModalPanel.Instance.Choice(string.Format(
                    "You appear to be running on an out-dated version of the RemoteFortressReader plugin.\n\n" +
                    "You're running version {0} of the plugin, while Armok Vision expects a plugin versioned {1} or above.\n\n" +
                    "A compatible version of the plugin has been found. Would you like to update it?", pluginVersion, avVersion), UpdatePlugin, Init, DisableUpdate, "Yes", "No.", "Don't ask again");
                return; //We don't have a compatible plugin

            }
        }
        else
            Init();
    }

    void DisableUpdate()
    {
        GameSettings.Instance.game.askToUpdatePlugin = false;
        Init();
    }

    void UpdatePlugin()
    {

        networkClient.run_command("unload", new List<string>(new string[] { "RemoteFortressReader" }));
    File.Copy(AVPluginDirectory + pluginName, DFHackPluginDirectory + pluginName, true);
        networkClient.run_command("load", new List<string>(new string[] { "RemoteFortressReader" }));
        Init();
    }

    void Disconnect()
    {
        instance = null;
        if (connectionManager != null)
        {
            connectionManager.Terminate();
            connectionManager = null;
        }
    }

    /// <summary>
    /// Get information that only needs to be read once.
    /// </summary>
    void FetchUnchangingInfo()
    {
        if (materialListCall != null)
            materialListCall.execute(null, out netMaterialList);
        if (itemListCall != null)
            itemListCall.execute(null, out netItemList);
        if (tiletypeListCall != null)
            tiletypeListCall.execute(null, out netTiletypeList);
        if (buildingListCall != null)
            buildingListCall.execute(null, out netBuildingList);
        if (partialCreatureRawListCall != null)
        {
            creatureRaws = new List<CreatureRaw>();
            int returnedItems = int.MaxValue;
            CreatureRawList netCreatureRawList;
            int count = 0;
            for (int start = 0; returnedItems != 0; start += 50)
            {
                ListRequest request = new ListRequest();
                request.list_start = start;
                request.list_end = start + 50;
                partialCreatureRawListCall.execute(request, out netCreatureRawList);
                returnedItems = netCreatureRawList.creature_raws.Count;
                creatureRaws.AddRange(netCreatureRawList.creature_raws);
                count++;
            }
            Debug.LogFormat("Got {0} creatures raws in {1} batches", creatureRaws.Count, count);
        }
        else if (creatureRawListCall != null)
        {
            CreatureRawList netCreatureRawList;
            creatureRawListCall.execute(null, out netCreatureRawList);
            creatureRaws = netCreatureRawList.creature_raws;
        }
        if (plantRawListCall != null)
            plantRawListCall.execute(null, out netPlantRawList);
    }

    /// <summary>
    /// Populate lists when we connect.
    /// </summary>
    void InitStatics()
    {
        if (netMaterialList != null)
        {
            AddFakeMaterials(netMaterialList);
            MaterialTokenList.MaterialTokens = netMaterialList.material_list;
            Debug.Log("Materials fetched: " + netMaterialList.material_list.Count);
        }
        if (netTiletypeList != null)
        {
            TiletypeTokenList.tiletypeTokenList = netTiletypeList.tiletype_list;
            Debug.Log("Tiletypes fetched: " + netTiletypeList.tiletype_list.Count);
        }
        if (netItemList != null)
        {
            ItemTokenList.ItemTokens = netItemList.material_list;
            Debug.Log("Itemtypes fetched: " + netItemList.material_list.Count);
        }
        else
        {
            MaterialDefinition blankMaterial = new MaterialDefinition();
            blankMaterial.id = "NONE";
            blankMaterial.name = "NONE";
            blankMaterial.mat_pair = new MatPair();
            blankMaterial.mat_pair.mat_type = -1;
            blankMaterial.mat_pair.mat_index = -1;
            List<MaterialDefinition> blankItemList = new List<MaterialDefinition>();
            blankItemList.Add(blankMaterial);
            ItemTokenList.ItemTokens = blankItemList;
            Debug.Log("Created dummy Itemtype list.");
        }
        if (netBuildingList != null)
        {
            BuildingTokenList.BuildingTokens = netBuildingList.building_list;
            Debug.Log("Buildingtypes fetched: " + netBuildingList.building_list.Count);
        }

        if (netTiletypeList != null)
        {
            MapDataStore.tiletypeTokenList = netTiletypeList.tiletype_list;
            Debug.Log("Tiletypes fetched: " + netTiletypeList.tiletype_list.Count);
        }

        if (netPlantRawList != null)
        {
            PlantTokenList.PlantRawList = netPlantRawList.plant_raws;
            Debug.Log("Plant Raws fetched: " + netPlantRawList.plant_raws.Count);
        }

        if(creatureRaws != null)
        {
            CreatureTokenList.CreatureRawList = creatureRaws;
            Debug.Log("Creature Raws fetched: " + creatureRaws.Count);
        }

        //Debug.Log("Buildingtypes fetched: " + netBuildingList.building_list.Count);
        //Debug.Log("Creature Raws fetched: " + netCreatureRawList.creature_raws.Count);
    }

    private void AddFakeMaterials(MaterialList netMaterialList)
    {
        for (int i = 0; i <= (int)DesignationType.UpStairs; i++)
        {
            MaterialDefinition item = new MaterialDefinition();
            item.id = "DESIGNATION:" + ((DesignationType)i).ToString();
            item.name = ((DesignationType)i).ToString() + " Designation";
            item.mat_pair = new MatPairStruct((int)MatBasic.DESIGNATION, i);
            netMaterialList.material_list.Add(item);
        }
    }

    void Start()
    {
        if (instance != null)
        {
            throw new UnityException("Can't have multiple dwarf fortress connections!");
        }
        instance = this;

        try
        {
            Connect();
        }
        catch (UnityException e)
        {
            Debug.LogException(e);
        }

    }

    void Update()
    {
        if (!Connected)
        {
            return;
        }
        foreach (System.Action callback in connectionCallbacks)
        {
            callback.Invoke();
        }
        connectionCallbacks.Clear();
        connectionManager.Poll();
    }

    // OnGUI is called for rendering and handling GUI events
    public void OnGUI()
    {
        if (GameSettings.Instance.game.showDFScreen)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                KeyboardEvent dfEvent = new KeyboardEvent();
                switch (e.type)
                {
                    case EventType.KeyDown:
                        dfEvent.type = 2;
                        dfEvent.state = 0;
                        break;
                    case EventType.KeyUp:
                        dfEvent.type = 3;
                        dfEvent.state = 0;
                        break;
                }
                SDL.Mod mod = SDL.Mod.KMOD_NONE;
                if (e.shift)
                    mod |= SDL.Mod.KMOD_SHIFT;
                if (e.control)
                    mod |= SDL.Mod.KMOD_CTRL;
                if (e.alt)
                    mod |= SDL.Mod.KMOD_ALT;
                if (e.command)
                    mod |= SDL.Mod.KMOD_META;
                if (e.capsLock)
                    mod |= SDL.Mod.KMOD_CAPS;

                dfEvent.mod = (uint)mod;
                dfEvent.scancode = (uint)e.keyCode;
                dfEvent.sym = (uint)e.keyCode;
                dfEvent.unicode = e.character;

                if (e.keyCode == KeyCode.None && e.character != '\0')
                    StartCoroutine(delayedKeyboardEvent(dfEvent)); // Unity doesn't give any keyboard events for character up, but DF expect it.
                else
                    keyPresses.Enqueue(dfEvent);
            }
        }
    }

    public IEnumerator delayedKeyboardEvent(KeyboardEvent dfEvent)
    {
        keyPresses.Enqueue(dfEvent);
        yield return new WaitForSeconds(0.1f);
        dfEvent.type = 3;
        dfEvent.state = 0;
        keyPresses.Enqueue(dfEvent);
    }

    void OnDestroy()
    {
        Disconnect();
    }

    long _timeTicks;

    public DFTime DFTime
    {
        get
        {
            return new DFTime(_timeTicks);
        }
        set
        {
            Interlocked.Exchange(ref _timeTicks, value.Ticks);
        }
    }
    DateTime nextRegionUpdate = DateTime.MaxValue;

    public bool fetchMap;
    public bool fetchScreen;
    public bool fetchUnits;
    public bool fetchWorldMap;

    /// <summary>
    /// Performs a single update.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    void PerformSingleUpdate()
    {
        //pause df here, so it doesn't try to resume while we're working.
        networkClient.suspend_game();

        //everything that controls DF.
        #region DF Control

        if (mapResetRequested.Pop())
        {
            mapResetCall.execute();
        }

        if (digCommandCall != null)
        {
            while (netDigCommands.Count > 0)
            {
                digCommandCall.execute(netDigCommands.Dequeue());
            }
        }
        if(pauseCommandCall != null)
        {
            while(pauseCommands.Count > 0)
            {
                pauseCommandCall.execute(pauseCommands.Dequeue());
            }
        }
        if (keyboardEventCall != null)
        {
            KeyboardEvent dfEvent;
            while (keyPresses.TryDequeue(out dfEvent))
            {
                keyboardEventCall.execute(dfEvent);
            }
        }

        #endregion

        #region DF Read

        if (fetchMap)
        {
            if (mapInfoCall != null)
            {
                MapInfo mapInfo;
                mapInfoCall.execute(null, out mapInfo);
                if (mapInfo == null)
                {
                    if (netMapInfo != null)
                    {
                        lock (mapInfoLock)
                        {
                            embarkMapPosition = new DFCoord(-1, -1, -1);
                            embarkMapSize = new DFCoord(0, 0, 0);
                            MapDataStore.InitMainMap(embarkMapSize.x * 16, embarkMapSize.y * 16, embarkMapSize.z);
                        }
                    }
                }
                else
                {
                    if ((netMapInfo == null)
                        || mapInfo.block_pos_x != netMapInfo.block_pos_x
                        || mapInfo.block_pos_y != netMapInfo.block_pos_y
                        || mapInfo.block_pos_z != netMapInfo.block_pos_z
                        || mapInfo.block_size_x != netMapInfo.block_size_x
                        || mapInfo.block_size_y != netMapInfo.block_size_y
                        || mapInfo.block_size_z != netMapInfo.block_size_z)
                    {
                        lock (mapInfoLock)
                        {
                            embarkMapPosition = new DFCoord(mapInfo.block_pos_x, mapInfo.block_pos_y, mapInfo.block_pos_z);
                            embarkMapSize = new DFCoord(mapInfo.block_size_x, mapInfo.block_size_y, mapInfo.block_size_z);
                            MapDataStore.InitMainMap(EmbarkMapSize.x * 16, EmbarkMapSize.y * 16, EmbarkMapSize.z);
                            mapResetCall.execute();
                        }
                    }
                }
                netMapInfo = mapInfo;
            }
            if (viewInfoCall != null)
            {
                ViewInfo viewInfo;
                viewInfoCall.execute(null, out viewInfo);
                netViewInfo.Set(viewInfo);
            }
        }
        if (fetchScreen)
        {
            if (copyScreenCall != null)
            {
                RemoteFortressReader.ScreenCapture screenCapture;
                copyScreenCall.execute(null, out screenCapture);
                netScreenCapture.Set(screenCapture);
            }
        }

        if (fetchUnits)
        {
            if (unitListCall != null)
            {
                UnitList unitList;
                unitListCall.execute(null, out unitList);
                netUnitList.Set(unitList);
            }
        }

        if (fetchWorldMap)
        {
            if (worldMapCenterCall != null)
            {
                WorldMap tempWorldMap;
                worldMapCenterCall.execute(null, out tempWorldMap);
                if (tempWorldMap != null)
                    DFTime = new DFTime(tempWorldMap.cur_year, tempWorldMap.cur_year_tick);
                if (netWorldMapCenter == null || (tempWorldMap != null &&
                        (netWorldMapCenter.center_x != tempWorldMap.center_x
                        || netWorldMapCenter.center_y != tempWorldMap.center_y)))
                {
                    if (worldMapCall != null)
                    {
                        WorldMap worldMap;
                        worldMapCall.execute(null, out worldMap);
                        netWorldMap.Set(worldMap);
                    }

                    netWorldMapCenter = tempWorldMap;

                    worldMapMoved.Set();
                    if (regionMapCall != null)
                    {
                        RegionMaps regionMaps;
                        regionMapCall.execute(null, out regionMaps);
                        netRegionMaps.Set(regionMaps);
                    }
                    nextRegionUpdate = DateTime.Now.AddSeconds(0.2); //add another region map update after this one, to get delayed regions.s
                }
            }


            if (nextRegionUpdate < DateTime.Now)
            {
                if (regionMapCall != null)
                {
                    RegionMaps regionMaps;
                    regionMapCall.execute(null, out regionMaps);
                    netRegionMaps.Set(regionMaps);
                }
                nextRegionUpdate = DateTime.MaxValue;
            }
        }

        if(pauseCommandCall != null)
        {
            SingleBool status;
            pauseStatusCall.execute(null, out status);
            if (status != null)
                _dfPauseState = status.Value;
        }

        // since enqueueing results can block, we do it after we've unsuspended df
        BlockList resultList = null;
        if (fetchMap)
        {

            if (EmbarkMapSize.x > 0
                && EmbarkMapSize.y > 0
                && EmbarkMapSize.z > 0 && _needNewBlocks)
            {

                BlockCoord.Range? requestRangeUpdate = requestRegion.Pop();

                if (requestRangeUpdate != null)
                {
                    blockRequest.min_x = requestRangeUpdate.Value.Min.x;
                    blockRequest.min_y = requestRangeUpdate.Value.Min.y;
                    blockRequest.min_z = requestRangeUpdate.Value.Min.z;
                    blockRequest.max_x = requestRangeUpdate.Value.Max.x;
                    blockRequest.max_y = requestRangeUpdate.Value.Max.y;
                    blockRequest.max_z = requestRangeUpdate.Value.Max.z;
                }

                if (blockListCall != null)
                {
                    blockListCall.execute(blockRequest, out resultList);
                }
            }
        }
        #endregion

        //All communication with DF should be before this.
        networkClient.resume_game();

        if (resultList != null)
        {
            foreach (MapBlock mapBlock in resultList.map_blocks)
            {
                pendingBlocks.Enqueue(mapBlock);
            }
        }
    }

    internal void ResumeGame()
    {
        networkClient.resume_game();
    }

    /// <summary>
    /// An exception with communication.
    /// </summary>
    public sealed class DFRemoteException : System.Exception
    {
        public DFRemoteException(System.Exception inner) : base("Remote exception!", inner)
        { }
    }

    /// <summary>
    /// Manages polling df.
    /// </summary>
    private abstract class ConnectionManager
    {
        protected readonly DFConnection connection;

        /// <summary>
        /// Used by base classes.
        /// </summary>
        /// <param name="connection">Connection.</param>
        protected ConnectionManager(DFConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Check for updates and exceptions.
        /// </summary>
        /// <exception cref="DFRemoteException">If communication with the remote failed.</exception>
        public abstract void Poll();

        /// <summary>
        /// Terminate this instance.
        /// </summary>
        public abstract void Terminate();


        // IMPLEMENTATIONS


        /// <summary>
        /// Polls on the main unity thread.
        /// </summary>
        public sealed class UnityThread : ConnectionManager
        {
            private float prevTime;

            public UnityThread(DFConnection connection) : base(connection)
            { }

            public override void Poll()
            {
                if (Time.time - prevTime < Instance.refreshDelay)
                    return;
                prevTime = Time.time;
                try
                {
                    connection.PerformSingleUpdate();
                }
                catch (System.Exception e)
                {
                    throw new DFRemoteException(e);
                }
            }

            public override void Terminate()
            {
                connection.networkClient.disconnect();
                connection.networkClient = null;
            }
        }

        /// <summary>
        /// Polls on an alternate thread.
        /// </summary>
        public sealed class AltThread : ConnectionManager
        {
            /// Use to terminate computation thread
            private volatile bool finished;
            /// Said thread
            private readonly Thread connectionThread;
            /// Catch errors on the throwing thread and throw them on the main thread
            private SingleBuffer<System.Exception> crashError;

            public AltThread(DFConnection connection)
                : base(connection)
            {
                finished = false;
                connectionThread = new Thread(new ThreadStart(this.RunForever));
                connectionThread.Start();
            }

            public override void Poll()
            {
                System.Exception error = crashError.Pop();
                if (error != null)
                {
                    throw new DFRemoteException(error);
                }
            }

            public override void Terminate()
            {
                finished = true;
                if (!connectionThread.Join(1000))
                    connectionThread.Abort();//we asked nicely once.
            }

            private void RunForever()
            {
                while (!finished)
                {
                    try
                    {
                        connection.PerformSingleUpdate();
                    }
                    catch (System.Exception e)
                    {
                        crashError.Set(e);
                        connection.ResumeGame();
                            return;
                    }
                    Thread.Sleep((int)(Instance.refreshDelay * 1000));
                }
                // finished
                connection.networkClient.disconnect();
                connection.networkClient = null;
            }
        }
    }
}
