using AdventureControl;
using DFHack;
using dfproto;
using DwarfControl;
using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TokenLists;
using UnityEngine;
using Util;

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
    public float refreshDelay = 1;

    // Thread management
    private ConnectionManager connectionManager;

    #region RPC Bindings

    //Static bindings
    private RemoteFunction<EmptyMessage, StringMessage> dfhackVersionCall;
    private RemoteFunction<EmptyMessage, StringMessage> dfVersionCall;
    private RemoteFunction<EmptyMessage, GetWorldInfoOut> dfWorldInfoCall;

    // Plugin bindings
    private RemoteFunction<EmptyMessage, Language> languageCall;
    private RemoteFunction<EmptyMessage, MaterialList> materialListCall;
    private RemoteFunction<EmptyMessage, MaterialList> itemListCall;
    private RemoteFunction<EmptyMessage, TiletypeList> tiletypeListCall;
    private TimedRemoteFunction<BlockRequest, BlockList> blockListCall;
    private RemoteFunction<BlockRequest, UnitList> unitListCall;
    private RemoteFunction<EmptyMessage, UnitList> unitListCallLegacy;
    private RemoteFunction<EmptyMessage, ViewInfo> viewInfoCall;
    private RemoteFunction<EmptyMessage, MapInfo> mapInfoCall;
    private RemoteFunction<EmptyMessage> mapResetCall;
    private RemoteFunction<EmptyMessage, BuildingList> buildingListCall;
    private RemoteFunction<EmptyMessage, WorldMap> worldMapCall;
    private RemoteFunction<EmptyMessage, WorldMap> worldMapCenterCall;
    private RemoteFunction<EmptyMessage, RegionMaps> regionMapCall;
    private RemoteFunction<EmptyMessage, CreatureRawList> creatureRawListCall;
    private RemoteFunction<ListRequest, CreatureRawList> partialCreatureRawListCall;
    private RemoteFunction<EmptyMessage, PlantRawList> plantRawListCall;
    private RemoteFunction<KeyboardEvent> keyboardEventCall;
    private RemoteFunction<EmptyMessage, RemoteFortressReader.ScreenCapture> copyScreenCall;
    private RemoteFunction<DigCommand> digCommandCall;
    private RemoteFunction<SingleBool> pauseCommandCall;
    private RemoteFunction<EmptyMessage, SingleBool> pauseStatusCall;
    private RemoteFunction<EmptyMessage, VersionInfo> versionInfoCall;
    private RemoteFunction<EmptyMessage, Status> reportsCall;

    private RemoteFunction<MoveCommandParams> moveCommandCall;
    private RemoteFunction<MoveCommandParams> jumpCommandCall;
    private RemoteFunction<EmptyMessage, MenuContents> menuQueryCall;
    private RemoteFunction<IntMessage> movementSelectCommandCall;
    private RemoteFunction<MiscMoveParams> miscMoveCall;

    #region Dwarf Mode Control
    private RemoteFunction<EmptyMessage, SidebarState> getSideMenuCall;
    private RemoteFunction<SidebarCommand> setSideMenuCall;
    #endregion

    private readonly ColorOstream dfNetworkOut = new ColorOstream();
    private RemoteClient networkClient;

    TimedRemoteFunction<Input, Output> CreateAndBindTimed<Input, Output>(float interval, RemoteClient client, string name, string proto = "")
    where Input : class, ProtoBuf.IExtensible, new()
    where Output : class, ProtoBuf.IExtensible, new()
    {
        RemoteFunction<Input, Output> output = new RemoteFunction<Input, Output>();
        if (output.Bind(client, name, proto))
            return new TimedRemoteFunction<Input, Output>(interval, output);
        else
            return null;
    }

    /// <summary>
    /// Bind methods that aren't dependent on the RFR plugin.
    /// </summary>
    void BindStaticMethods()
    {
        dfhackVersionCall = new RemoteFunction<EmptyMessage, StringMessage>(networkClient, "GetVersion");
        dfVersionCall = new RemoteFunction<EmptyMessage, StringMessage>(networkClient, "GetDFVersion");
        dfWorldInfoCall = new RemoteFunction<EmptyMessage, GetWorldInfoOut>(networkClient, "GetWorldInfo");
    }

    /// <summary>
    /// Bind the RPC functions we'll be calling
    /// </summary>
    void BindMethods()
    {
        materialListCall = new RemoteFunction<EmptyMessage, MaterialList>(networkClient, "GetMaterialList", "RemoteFortressReader");
        itemListCall = new RemoteFunction<EmptyMessage, MaterialList>(networkClient, "GetItemList", "RemoteFortressReader");
        tiletypeListCall = new RemoteFunction<EmptyMessage, TiletypeList>(networkClient, "GetTiletypeList", "RemoteFortressReader");
        blockListCall = CreateAndBindTimed<BlockRequest, BlockList>(GameSettings.Instance.updateTimers.blockUpdate, networkClient, "GetBlockList", "RemoteFortressReader");
        unitListCall = new RemoteFunction<BlockRequest, UnitList>(networkClient, "GetUnitListInside", "RemoteFortressReader");
        unitListCallLegacy = new RemoteFunction<EmptyMessage, UnitList>(networkClient, "GetUnitList", "RemoteFortressReader");
        viewInfoCall = new RemoteFunction<EmptyMessage, ViewInfo>(networkClient, "GetViewInfo", "RemoteFortressReader");
        mapInfoCall = new RemoteFunction<EmptyMessage, MapInfo>(networkClient, "GetMapInfo", "RemoteFortressReader");
        mapResetCall = new RemoteFunction<EmptyMessage>(networkClient, "ResetMapHashes", "RemoteFortressReader");
        buildingListCall = new RemoteFunction<EmptyMessage, BuildingList>(networkClient, "GetBuildingDefList", "RemoteFortressReader");
        worldMapCall = new RemoteFunction<EmptyMessage, WorldMap>(networkClient, "GetWorldMapNew", "RemoteFortressReader");
        worldMapCenterCall = new RemoteFunction<EmptyMessage, WorldMap>(networkClient, "GetWorldMapCenter", "RemoteFortressReader");
        regionMapCall = new RemoteFunction<EmptyMessage, RegionMaps>(networkClient, "GetRegionMapsNew", "RemoteFortressReader");
        creatureRawListCall = new RemoteFunction<EmptyMessage, CreatureRawList>(networkClient, "GetCreatureRaws", "RemoteFortressReader");
        partialCreatureRawListCall = new RemoteFunction<ListRequest, CreatureRawList>(networkClient, "GetPartialCreatureRaws", "RemoteFortressReader");
        plantRawListCall = new RemoteFunction<EmptyMessage, PlantRawList>(networkClient, "GetPlantRaws", "RemoteFortressReader");
        keyboardEventCall = new RemoteFunction<KeyboardEvent>(networkClient, "PassKeyboardEvent", "RemoteFortressReader");
        copyScreenCall = new RemoteFunction<EmptyMessage, RemoteFortressReader.ScreenCapture>(networkClient, "CopyScreen", "RemoteFortressReader");
        digCommandCall = new RemoteFunction<DigCommand>(networkClient, "SendDigCommand", "RemoteFortressReader");
        pauseCommandCall = new RemoteFunction<SingleBool>(networkClient, "SetPauseState", "RemoteFortressReader");
        pauseStatusCall = new RemoteFunction<EmptyMessage, SingleBool>(networkClient, "GetPauseState", "RemoteFortressReader");
        versionInfoCall = new RemoteFunction<EmptyMessage, VersionInfo>(networkClient, "GetVersionInfo", "RemoteFortressReader");
        reportsCall = new RemoteFunction<EmptyMessage, Status>(networkClient, "GetReports", "RemoteFortressReader");
        moveCommandCall = new RemoteFunction<MoveCommandParams>(networkClient, "MoveCommand", "RemoteFortressReader");
        jumpCommandCall = new RemoteFunction<MoveCommandParams>(networkClient, "JumpCommand", "RemoteFortressReader");
        menuQueryCall = new RemoteFunction<EmptyMessage, MenuContents>(networkClient, "MenuQuery", "RemoteFortressReader");
        movementSelectCommandCall = new RemoteFunction<IntMessage>(networkClient, "MovementSelectCommand", "RemoteFortressReader");
        miscMoveCall = new RemoteFunction<MiscMoveParams>(networkClient, "MiscMoveCommand", "RemoteFortressReader");
        languageCall = new RemoteFunction<EmptyMessage, Language>(networkClient, "GetLanguage", "RemoteFortressReader");
        getSideMenuCall = new RemoteFunction<EmptyMessage, SidebarState>(networkClient, "GetSideMenu", "RemoteFortressReader");
        setSideMenuCall = new RemoteFunction<SidebarCommand>(networkClient, "SetSideMenu", "RemoteFortressReader");
    }

    #endregion

    // Things we read from DF
    GetWorldInfoOut netWorldInfo;

    // Unchanging
    private MaterialList netMaterialList;
    private MaterialList netItemList;
    private TiletypeList netTiletypeList;
    private BuildingList netBuildingList;
    private PlantRawList netPlantRawList;
    private Language netLanguageList;

    // Output queues
    private SingleBuffer<ViewInfo> netViewInfo;
    private SingleBuffer<UnitList> netUnitList;
    private SingleBuffer<WorldMap> netWorldMap;
    private SingleBuffer<RegionMaps> netRegionMaps;
    private RingBuffer<MapBlock> pendingBlocks
        = new RingBuffer<MapBlock>(4096);
    private SingleBuffer<RemoteFortressReader.ScreenCapture> netScreenCapture;
    private EventBuffer worldMapMoved;
    private SingleBuffer<Status> netStatus;
    private RingBuffer<List<Engraving>> engravings = new RingBuffer<List<Engraving>>(1024);

    // Input queues
    private RingBuffer<KeyboardEvent> keyPresses
        = new RingBuffer<KeyboardEvent>(128);

    #region Command DF

    private readonly RingBuffer<DigCommand> netDigCommands 
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

    private readonly RingBuffer<SidebarCommand> netSidebarSets
    = new RingBuffer<SidebarCommand>(8);

    public void EnqueueSidebarSet(SidebarCommand sidebar)
    {
        if (setSideMenuCall == null)
            return;
        if (!netSidebarSets.Full)
            netSidebarSets.Enqueue(sidebar);
    }

    private RingBuffer<SingleBool> pauseCommands
        = new RingBuffer<SingleBool>(8);

    public void SendPauseCommand(bool state)
    {
        SingleBool command = new SingleBool
        {
            Value = state
        };
        if (pauseCommands.Count < pauseCommands.Capacity)
            pauseCommands.Enqueue(command);
    }

    private RingBuffer<MoveCommandParams> moveCommands
        = new RingBuffer<MoveCommandParams>(8);

    public void SendMoveCommand(DFCoord direction)
    {
        MoveCommandParams command = new MoveCommandParams
        {
            direction = direction
        };
        if (moveCommands.Count < moveCommands.Capacity)
            moveCommands.Enqueue(command);
    }

    private RingBuffer<MoveCommandParams> jumpCommands
    = new RingBuffer<MoveCommandParams>(8);

    public void SendJumpCommand(DFCoord direction)
    {
        MoveCommandParams command = new MoveCommandParams
        {
            direction = direction
        };
        if (jumpCommands.Count < jumpCommands.Capacity)
            jumpCommands.Enqueue(command);
    }

    private RingBuffer<IntMessage> carefulMoveCommands
        = new RingBuffer<IntMessage>(8);

    public void SendCarefulMoveCommand(int option)
    {
        IntMessage message = new IntMessage
        {
            value = option
        };
        if (carefulMoveCommands.Count < carefulMoveCommands.Capacity)
            carefulMoveCommands.Enqueue(message);
    }

    private RingBuffer<MiscMoveParams> miscMoveCommands
        = new RingBuffer<MiscMoveParams>(8);
    public void SendMiscMoveCommand(MiscMoveType type)
    {
        MiscMoveParams command = new MiscMoveParams
        {
            type = type
        };
        if (miscMoveCommands.Count < miscMoveCommands.Capacity)
            miscMoveCommands.Enqueue(command);
    }


    public MenuContents AdventureMenuContents { get; private set; }

    public SidebarState SidebarState { get; private set; }

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
    private readonly UnityEngine.Object mapInfoLock = new UnityEngine.Object();

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

    public PlantRawList NetPlantRawList
    {
        get { return netPlantRawList; }
    }

    public Language NetLanguageList
    {
        get { return netLanguageList; }
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

    public Status PopStatusUpdate()
    {
        return netStatus.Pop();
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

    public List<Engraving> PopEngravingUpdate()
    {
        List<Engraving> result;
        engravings.TryDequeue(out result);
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

    public GetWorldInfoOut.Mode WorldMode { get; private set; }

    [NonSerialized]
    public List<Wave> waves;
    public bool UpdatedAnyBlocks;

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
        networkClient = new RemoteClient(dfNetworkOut);
        bool success = networkClient.Connect();
        if (!success)
        {
            networkClient.Disconnect();
            networkClient = null;
            ModalPanel.Instance.Choice(
                "Armok Vision could not find a running instance of Dwarf Fortress!\n\n" +
                "Please make sure you have Dwarf Fortress running, with the latest version of DFHack installed.", Connect, QuitGame, "Retry", "Quit");
            throw new UnityException("DF Connection Failure");
        }

        BindStaticMethods();

        if (dfWorldInfoCall != null)
        {
            dfWorldInfoCall.TryExecute(null, out netWorldInfo);
            if (netWorldInfo == null)
                Debug.Log("World not loaded.");
            else
            {
                Debug.Log("World Mode: " + netWorldInfo.mode);
                WorldMode = netWorldInfo.mode;
            }
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
            StringMessage dfVersion = new StringMessage();
            versionInfoCall.TryExecute(null, out versionInfo);
            dfVersionCall.TryExecute(null, out dfVersion);
            Debug.LogFormat("Connected to DF version {0}, running DFHack version {1}, and RemoteFortressReader version {2}", versionInfo.dwarf_fortress_version, versionInfo.dfhack_version, versionInfo.remote_fortress_reader_version);
            if (GoogleAnalyticsV4.instance != null)
                GoogleAnalyticsV4.instance.SendDeviceData(versionInfo.dwarf_fortress_version, versionInfo.remote_fortress_reader_version);
        }
        else if(dfVersionCall != null)
        {
            StringMessage dfVersion = new StringMessage();
            StringMessage dfHackVersion = new StringMessage();
            dfVersionCall.TryExecute(null, out dfVersion);
            dfhackVersionCall.TryExecute(null, out dfHackVersion);
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
        networkClient.SuspendGame();
        if (viewInfoCall != null)
        {
            ViewInfo viewInfo;
            viewInfoCall.TryExecute(null, out viewInfo);
            netViewInfo.Set(viewInfo);
        }
        if (copyScreenCall != null)
        {
            RemoteFortressReader.ScreenCapture screenCapture;
            copyScreenCall.TryExecute(null, out screenCapture);
            netScreenCapture.Set(screenCapture);
        }
        if (unitListCall != null)
        {
            UnitList unitList =  unitListCall.Execute();
            netUnitList.Set(unitList);
        }
        else if (unitListCallLegacy != null)
        {
            UnitList unitList = unitListCallLegacy.Execute();
            netUnitList.Set(unitList);
        }
        if (worldMapCall != null)
        {
            WorldMap worldMap;
            worldMapCall.TryExecute(null, out worldMap);
            netWorldMap.Set(worldMap);
        }
        if (regionMapCall != null)
        {
            RegionMaps regionMaps;
            regionMapCall.TryExecute(null, out regionMaps);
            netRegionMaps.Set(regionMaps);
        }
        networkClient.ResumeGame();

        if (mapResetCall != null)
            mapResetCall.TryExecute();
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

    string _dfhackPluginDir;

    string DFHackPluginDirectory
    {
        get
        {
            if(string.IsNullOrEmpty(_dfhackPluginDir))
            {
            DFStringStream tempStream = new DFStringStream();
            networkClient.RunCommand(tempStream, "lua", new List<string>(new string[] { "!dfhack.getHackPath()" }));

            _dfhackPluginDir = tempStream.Value.Trim() + "/plugins/";
            }
            return _dfhackPluginDir;
        }
    }

    string _AVPluginDir;
    string AVPluginDirectory
    {
        get
        {
            if(string.IsNullOrEmpty(_AVPluginDir))
            {
            if (dfhackVersionCall == null || dfVersionCall == null)
                return "";
            StringMessage dfHackVersion = new StringMessage();
            StringMessage dfVersion = new StringMessage();

            dfhackVersionCall.TryExecute(null, out dfHackVersion);
            dfVersionCall.TryExecute(null, out dfVersion);

            _AVPluginDir = "Plugins/" + dfVersion.value + "/" + dfHackVersion.value + "/";
#if UNITY_EDITOR
            _AVPluginDir = "ReleaseFiles/" + _AVPluginDir;
            Directory.CreateDirectory(_AVPluginDir);
#endif
            }
            return _AVPluginDir;
        }
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    readonly string pluginName = "RemoteFortressReader.plug.dll";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    readonly string pluginName = "RemoteFortressReader.plug.dylib";
#elif UNITY_STANDALONE_LINUX
    readonly string pluginName = "RemoteFortressReader.plug.so";
#else
    readonly string pluginName = "INVALID";
#endif

    private void CheckPlugin()
    {
        Version pluginVersion = new Version(0,0,0);
        Version avVersion = new Version(BuildSettings.Instance.content_version);

        DFStringStream tempStream = new DFStringStream();
        networkClient.RunCommand(tempStream, "RemoteFortressReader_version", new List<string>());
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

        networkClient.RunCommand("unload", new List<string>(new string[] { "RemoteFortressReader" }));
    File.Copy(AVPluginDirectory + pluginName, DFHackPluginDirectory + pluginName, true);
        networkClient.RunCommand("load", new List<string>(new string[] { "RemoteFortressReader" }));
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
        if (getSideMenuCall != null)
        {
            SidebarState = getSideMenuCall.Execute();
        }
        if (materialListCall != null)
            materialListCall.TryExecute(null, out netMaterialList);
        if (itemListCall != null)
            itemListCall.TryExecute(null, out netItemList);
        if (tiletypeListCall != null)
            tiletypeListCall.TryExecute(null, out netTiletypeList);
        if (buildingListCall != null)
            buildingListCall.TryExecute(null, out netBuildingList);
        if (partialCreatureRawListCall != null)
        {
            var creatureRaws = new List<CreatureRaw>();
            int returnedItems = int.MaxValue;
            CreatureRawList netCreatureRawList;
            int count = 0;
            for (int start = 0; returnedItems != 0; start += 50)
            {
                ListRequest request = new ListRequest
                {
                    list_start = start,
                    list_end = start + 50
                };
                partialCreatureRawListCall.TryExecute(request, out netCreatureRawList);
                returnedItems = netCreatureRawList.creature_raws.Count;
                creatureRaws.AddRange(netCreatureRawList.creature_raws);
                count++;
            }
            CreatureRaws.Instance.CreatureList = creatureRaws;
            Debug.LogFormat("Got {0} creatures raws in {1} batches", creatureRaws.Count, count);
        }
        else if (creatureRawListCall != null)
        {
            CreatureRawList netCreatureRawList;
            creatureRawListCall.TryExecute(null, out netCreatureRawList);
            CreatureRaws.Instance.CreatureList = netCreatureRawList.creature_raws;
        }
        if (plantRawListCall != null)
            plantRawListCall.TryExecute(null, out netPlantRawList);
        if (languageCall != null)
            netLanguageList = languageCall.Execute();
    }

    /// <summary>
    /// Populate lists when we connect.
    /// </summary>
    void InitStatics()
    {
        if (netMaterialList != null)
        {
            AddFakeMaterials(netMaterialList);
            MaterialRaws.Instance.MaterialList = netMaterialList.material_list;
            Debug.Log("Materials fetched: " + netMaterialList.material_list.Count);
        }
        if (netTiletypeList != null)
        {
            TiletypeTokenList.tiletypeTokenList = netTiletypeList.tiletype_list;
            Debug.Log("Tiletypes fetched: " + netTiletypeList.tiletype_list.Count);
        }
        if (netItemList != null)
        {
            ItemRaws.Instance.ItemList = netItemList.material_list;
            Debug.Log("Itemtypes fetched: " + netItemList.material_list.Count);
        }
        else
        {
            MaterialDefinition blankMaterial = new MaterialDefinition
            {
                id = "NONE",
                name = "NONE",
                mat_pair = new MatPair
                {
                    mat_type = -1,
                    mat_index = -1
                }
            };
            List<MaterialDefinition> blankItemList = new List<MaterialDefinition>
            {
                blankMaterial
            };
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

        if(CreatureRaws.Instance.CreatureList != null)
        {
            CreatureTokenList.CreatureRawList = CreatureRaws.Instance.CreatureList;
            Debug.Log("Creature Raws fetched: " + CreatureRaws.Instance.CreatureList.Count);
        }

        //Debug.Log("Buildingtypes fetched: " + netBuildingList.building_list.Count);
        //Debug.Log("Creature Raws fetched: " + netCreatureRawList.creature_raws.Count);
    }

    private void AddFakeMaterials(MaterialList netMaterialList)
    {
        for (int i = 0; i <= (int)DesignationType.UpStairs; i++)
        {
            MaterialDefinition item = new MaterialDefinition
            {
                id = "DESIGNATION:" + ((DesignationType)i).ToString(),
                name = ((DesignationType)i).ToString() + " Designation",
                mat_pair = new MatPairStruct((int)MatBasic.DESIGNATION, i)
            };
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
                SDL.Mod mod = SDL.Mod.KmodNone;
                if (e.shift)
                    mod |= SDL.Mod.KmodShift;
                if (e.control)
                    mod |= SDL.Mod.KmodCtrl;
                if (e.alt)
                    mod |= SDL.Mod.KmodAlt;
                if (e.command)
                    mod |= SDL.Mod.KmodMeta;
                if (e.capsLock)
                    mod |= SDL.Mod.KmodCaps;

                dfEvent.mod = (uint)mod;
                dfEvent.scancode = (uint)e.keyCode;
                dfEvent.sym = (uint)e.keyCode;
                dfEvent.unicode = e.character;

                if (e.keyCode == KeyCode.None && e.character != '\0')
                    StartCoroutine(DelayedKeyboardEvent(dfEvent)); // Unity doesn't give any keyboard events for character up, but DF expect it.
                else
                    keyPresses.Enqueue(dfEvent);
            }
        }
    }

    public IEnumerator DelayedKeyboardEvent(KeyboardEvent dfEvent)
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
        networkClient.SuspendGame();

        //everything that controls DF.
        #region DF Control

        if (mapResetRequested.Pop())
        {
            mapResetCall.TryExecute();
        }

        if (digCommandCall != null)
        {
            while (netDigCommands.Count > 0)
            {
                digCommandCall.TryExecute(netDigCommands.Dequeue());
            }
        }
        if(setSideMenuCall != null)
        {
            while(netSidebarSets.Count > 0)
            {
                setSideMenuCall.TryExecute(netSidebarSets.Dequeue());
            }
        }
        if(pauseCommandCall != null)
        {
            while(pauseCommands.Count > 0)
            {
                pauseCommandCall.TryExecute(pauseCommands.Dequeue());
            }
        }
        if (keyboardEventCall != null)
        {
            KeyboardEvent dfEvent;
            while (keyPresses.TryDequeue(out dfEvent))
            {
                keyboardEventCall.TryExecute(dfEvent);
            }
        }
        if (moveCommandCall != null)
        {
            while (moveCommands.Count > 0)
            {
                moveCommandCall.TryExecute(moveCommands.Dequeue());
            }
        }
        if (jumpCommandCall != null)
        {
            while (jumpCommands.Count > 0)
            {
                jumpCommandCall.TryExecute(jumpCommands.Dequeue());
            }
        }
        if(movementSelectCommandCall != null)
        {
            while (carefulMoveCommands.Count > 0)
                movementSelectCommandCall.TryExecute(carefulMoveCommands.Dequeue());
        }

        if(miscMoveCall != null)
        {
            while (miscMoveCommands.Count > 0)
                miscMoveCall.TryExecute(miscMoveCommands.Dequeue());
        }

        #endregion

        #region DF Read

        if (menuQueryCall != null)
        {
            AdventureMenuContents = menuQueryCall.Execute();
        }

        if(getSideMenuCall != null)
        {
            SidebarState = getSideMenuCall.Execute();
        }

        if (fetchMap)
        {
            if (mapInfoCall != null)
            {
                var mapInfo = mapInfoCall.Execute();
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
                            mapResetCall.TryExecute();
                        }
                    }
                }
                netMapInfo = mapInfo;
            }
            if (viewInfoCall != null)
            {
                ViewInfo viewInfo;
                viewInfoCall.TryExecute(null, out viewInfo);
                netViewInfo.Set(viewInfo);
            }
        }
        if (fetchScreen)
        {
            if (copyScreenCall != null)
            {
                RemoteFortressReader.ScreenCapture screenCapture;
                copyScreenCall.TryExecute(null, out screenCapture);
                netScreenCapture.Set(screenCapture);
            }
        }

        BlockCoord.Range? requestRangeUpdate = requestRegion.Pop();

        if (requestRangeUpdate != null)
        {
            blockRequest.min_x = requestRangeUpdate.Value.Min.x * GameMap.blockSize / 16;
            blockRequest.min_y = requestRangeUpdate.Value.Min.y * GameMap.blockSize / 16;
            blockRequest.min_z = requestRangeUpdate.Value.Min.z;
            blockRequest.max_x = requestRangeUpdate.Value.Max.x * GameMap.blockSize / 16;
            blockRequest.max_y = requestRangeUpdate.Value.Max.y * GameMap.blockSize / 16;
            blockRequest.max_z = requestRangeUpdate.Value.Max.z;
        }

        if (fetchUnits)
        {
            if (unitListCall != null)
            {
                UnitList unitList = unitListCall.Execute(blockRequest);
                netUnitList.Set(unitList);
            }
            else if (unitListCallLegacy != null)
            {
                UnitList unitList = unitListCallLegacy.Execute();
                netUnitList.Set(unitList);
            }
        }

        if (fetchWorldMap)
        {
            if (worldMapCenterCall != null)
            {
                WorldMap tempWorldMap;
                worldMapCenterCall.TryExecute(null, out tempWorldMap);
                if (tempWorldMap != null)
                    DFTime = new DFTime(tempWorldMap.cur_year, tempWorldMap.cur_year_tick);
                if (netWorldMapCenter == null || (tempWorldMap != null &&
                        (netWorldMapCenter.center_x != tempWorldMap.center_x
                        || netWorldMapCenter.center_y != tempWorldMap.center_y)))
                {
                    if (worldMapCall != null)
                    {
                        WorldMap worldMap;
                        worldMapCall.TryExecute(null, out worldMap);
                        netWorldMap.Set(worldMap);
                    }

                    netWorldMapCenter = tempWorldMap;

                    worldMapMoved.Set();
                    if (regionMapCall != null)
                    {
                        RegionMaps regionMaps;
                        regionMapCall.TryExecute(null, out regionMaps);
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
                    regionMapCall.TryExecute(null, out regionMaps);
                    netRegionMaps.Set(regionMaps);
                }
                nextRegionUpdate = DateTime.MaxValue;
            }
        }

        if(pauseCommandCall != null)
        {
            SingleBool status;
            pauseStatusCall.TryExecute(null, out status);
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
                if (blockListCall != null)
                {
                    //Don't pull more blocks than we have room for in the queue. It blocks things unneccesarily.
                    blockRequest.blocks_needed = Mathf.Min(blockRequest.blocks_needed, pendingBlocks.Capacity - pendingBlocks.Count - 1);
                    if(blockRequest.blocks_needed > 0)
                        resultList = blockListCall.Execute(blockRequest);
                }
            }
        }

        Status stat = null;
        if(reportsCall != null)
        {
            reportsCall.TryExecute(null, out stat);
            if (stat != null)
            {
                netStatus.Set(stat);
            }
        }
        #endregion

        //All communication with DF should be before this.
        networkClient.ResumeGame();

        if (resultList != null)
        {
            try
            {
                foreach (MapBlock mapBlock in resultList.map_blocks)
                {
                    pendingBlocks.Enqueue(mapBlock);
                }
                engravings.Enqueue(resultList.engravings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            waves = resultList.ocean_waves;
            UpdatedAnyBlocks = true;
        }
    }

    internal void ResumeGame()
    {
        networkClient.ResumeGame();
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
                if (Time.realtimeSinceStartup - prevTime < Instance.refreshDelay)
                    return;
                prevTime = Time.realtimeSinceStartup;
                if (ContentLoader.Instance != null)
                {
                    try
                    {
                        connection.PerformSingleUpdate();
                    }
                    catch (System.Exception e)
                    {
                        throw new DFRemoteException(e);
                    }
                }
            }

            public override void Terminate()
            {
                connection.networkClient.Disconnect();
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
                    if (ContentLoader.Instance != null)
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
                    }
                    Thread.Sleep((int)(Instance.refreshDelay * 1000));
                }
                // finished
                connection.networkClient.Disconnect();
                connection.networkClient = null;
            }
        }
    }
}
