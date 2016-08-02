using DFHack;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private static List<System.Action> connectionCallbacks = new List<System.Action>();
    public static DFConnection Instance
    {
        get
        {
            return instance;
        }
    }

    // Can always be called
    public static bool Connected
    {
        get
        {
            return instance != null && instance.networkClient != null;
        }
    }

    // Can be called before instance is initialized.
    public static void RegisterConnectionCallback(System.Action callback)
    {
        connectionCallbacks.Add(callback);
    }

    // Instance stuff

    // Assignable values
    public int BlocksToFetch = 4;
    public bool RunOnAlternateThread = false;

    // Thread management
    private ConnectionManager connectionManager;

    // Remote bindings
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> materialListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> itemListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList> tiletypeListCall;
    private RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList> blockListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList> unitListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo> viewInfoCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo> mapInfoCall;
    private RemoteFunction<dfproto.EmptyMessage> mapResetCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.BuildingList> buildingListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.WorldMap> worldMapCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.WorldMap> worldMapCenterCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.RegionMaps> regionMapCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.CreatureRawList> creatureRawListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.PlantRawList> plantRawListCall;
    private RemoteFunction<RemoteFortressReader.KeyboardEvent> keyboardEventCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ScreenCapture> copyScreenCall;
    private color_ostream dfNetworkOut = new color_ostream();
    private RemoteClient networkClient;

    // Things we read from DF

    // Unchanging
    private RemoteFortressReader.MaterialList netMaterialList;
    private RemoteFortressReader.MaterialList netItemList;
    private RemoteFortressReader.TiletypeList netTiletypeList;
    private RemoteFortressReader.BuildingList netBuildingList;
    private RemoteFortressReader.CreatureRawList netCreatureRawList;
    private RemoteFortressReader.PlantRawList netPlantRawList;

    // Output queues
    private SingleBuffer<RemoteFortressReader.ViewInfo> netViewInfo;
    private SingleBuffer<RemoteFortressReader.UnitList> netUnitList;
    private SingleBuffer<RemoteFortressReader.WorldMap> netWorldMap;
    private SingleBuffer<RemoteFortressReader.RegionMaps> netRegionMaps;
    private RingBuffer<RemoteFortressReader.MapBlock> pendingBlocks
        = new RingBuffer<RemoteFortressReader.MapBlock>(1024);
    private SingleBuffer<RemoteFortressReader.ScreenCapture> netScreenCapture;
    private EventBuffer worldMapMoved;

    // Input queues
    private RingBuffer<RemoteFortressReader.KeyboardEvent> keyPresses
        = new RingBuffer<RemoteFortressReader.KeyboardEvent>(128);

    /// <summary>
    /// Queue to set the area we want to get updates for.
    /// </summary>
    private SingleBufferStruct<BlockCoord.Range> requestRegion;
    /// <summary>
    /// Queue to request map resets.
    /// </summary>
    private EventBuffer mapResetRequested;

    // Used to check whether the map has moved.
    private RemoteFortressReader.MapInfo netMapInfo;
    private RemoteFortressReader.WorldMap netWorldMapCenter;

    //Rarely changing info that can nevertheless change.
    private DFCoord embarkMapSize = new DFCoord(0, 0, 0);
    private DFCoord embarkMapPosition = new DFCoord(-1, -1, -1);

    // Mutexes for changing / nullable objects
    private UnityEngine.Object mapInfoLock = new UnityEngine.Object();

    // Cached block request
    private readonly RemoteFortressReader.BlockRequest blockRequest
        = new RemoteFortressReader.BlockRequest();


    // Unchanging properties

    public RemoteFortressReader.MaterialList NetMaterialList
    {
        get { return netMaterialList; }
    }

    public RemoteFortressReader.MaterialList NetItemList
    {
        get { return netItemList; }
    }

    public RemoteFortressReader.TiletypeList NetTiletypeList
    {
        get { return netTiletypeList; }
    }

    public RemoteFortressReader.BuildingList NetBuildingList
    {
        get { return netBuildingList; }
    }

    public RemoteFortressReader.CreatureRawList NetCreatureRawList
    {
        get { return netCreatureRawList; }
    }

    public RemoteFortressReader.PlantRawList NetPlantRawList
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
    public RemoteFortressReader.ViewInfo PopViewInfoUpdate()
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
    public RemoteFortressReader.UnitList PopUnitListUpdate()
    {
        return netUnitList.Pop();
    }

    /// <summary>
    /// Pop a world map update. The map doesn't change, but the clouds do.
    /// </summary>
    public RemoteFortressReader.WorldMap PopWorldMapUpdate()
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
    public RemoteFortressReader.RegionMaps PopRegionMapUpdate()
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
    public RemoteFortressReader.MapBlock PopMapBlockUpdate()
    {
        RemoteFortressReader.MapBlock result;
        pendingBlocks.TryDequeue(out result);
        return result;
    }

    /// <summary>
    /// Queue a map hash reset, forcing a re-send of all map blocks
    /// </summary>
    public void RequestMapReset()
    {
        mapResetRequested.Set();
    }

    /// <summary>
    /// Connect to DF, fetch initial data, start things running
    /// </summary>
    void ConnectAndInit()
    {
        blockRequest.blocks_needed = BlocksToFetch;
        networkClient = new DFHack.RemoteClient(dfNetworkOut);
        bool success = networkClient.connect();
        if (!success)
        {
            networkClient.disconnect();
            networkClient = null;
            throw new UnityException("DF Connection Failure");
        }
        BindMethods();
        FetchUnchangingInfo();

        // Get some initial stuff
        // Necessary for initialization, apparently.
        networkClient.suspend_game();
        if (viewInfoCall != null)
        {
            RemoteFortressReader.ViewInfo viewInfo;
            viewInfoCall.execute(null, out viewInfo);
            netViewInfo.Set(viewInfo);
        }
        if(copyScreenCall != null)
        {
            RemoteFortressReader.ScreenCapture screenCapture;
            copyScreenCall.execute(null, out screenCapture);
            netScreenCapture.Set(screenCapture);
        }
        if (unitListCall != null)
        {
            RemoteFortressReader.UnitList unitList;
            unitListCall.execute(null, out unitList);
            netUnitList.Set(unitList);
        }
        if (worldMapCall != null)
        {
            RemoteFortressReader.WorldMap worldMap;
            worldMapCall.execute(null, out worldMap);
            netWorldMap.Set(worldMap);
        }
        if (regionMapCall != null)
        {
            RemoteFortressReader.RegionMaps regionMaps;
            regionMapCall.execute(null, out regionMaps);
            netRegionMaps.Set(regionMaps);
        }
        networkClient.resume_game();

        if (mapResetCall != null)
            mapResetCall.execute();
        InitStatics();

        foreach (System.Action callback in connectionCallbacks)
        {
            callback.Invoke();
        }

        connectionCallbacks.Clear();

        if (RunOnAlternateThread)
        {
            connectionManager = new ConnectionManager.AltThread(this);
        }
        else {
            connectionManager = new ConnectionManager.UnityThread(this);
        }
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
    /// Bind the RPC functions we'll be calling
    /// </summary>
    void BindMethods()
    {
        materialListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>(networkClient, "GetMaterialList", "RemoteFortressReader");
        itemListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>(networkClient, "GetItemList", "RemoteFortressReader");
        tiletypeListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList>(networkClient, "GetTiletypeList", "RemoteFortressReader");
        blockListCall = CreateAndBind<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList>(networkClient, "GetBlockList", "RemoteFortressReader");
        unitListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.UnitList>(networkClient, "GetUnitList", "RemoteFortressReader");
        viewInfoCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo>(networkClient, "GetViewInfo", "RemoteFortressReader");
        mapInfoCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.MapInfo>(networkClient, "GetMapInfo", "RemoteFortressReader");
        mapResetCall = CreateAndBind<dfproto.EmptyMessage>(networkClient, "ResetMapHashes", "RemoteFortressReader");
        buildingListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.BuildingList>(networkClient, "GetBuildingDefList", "RemoteFortressReader");
        worldMapCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.WorldMap>(networkClient, "GetWorldMapNew", "RemoteFortressReader");
        worldMapCenterCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.WorldMap>(networkClient, "GetWorldMapCenter", "RemoteFortressReader");
        regionMapCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.RegionMaps>(networkClient, "GetRegionMapsNew", "RemoteFortressReader");
        creatureRawListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.CreatureRawList>(networkClient, "GetCreatureRaws", "RemoteFortressReader");
        plantRawListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.PlantRawList>(networkClient, "GetPlantRaws", "RemoteFortressReader");
        keyboardEventCall = CreateAndBind<RemoteFortressReader.KeyboardEvent>(networkClient, "PassKeyboardEvent", "RemoteFortressReader");
        copyScreenCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.ScreenCapture>(networkClient, "CopyScreen", "RemoteFortressReader");
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
        if (creatureRawListCall != null)
            creatureRawListCall.execute(null, out netCreatureRawList);
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
            RemoteFortressReader.MaterialDefinition blankMaterial = new RemoteFortressReader.MaterialDefinition();
            blankMaterial.id = "NONE";
            blankMaterial.name = "NONE";
            blankMaterial.mat_pair = new RemoteFortressReader.MatPair();
            blankMaterial.mat_pair.mat_type = -1;
            blankMaterial.mat_pair.mat_index = -1;
            List<RemoteFortressReader.MaterialDefinition> blankItemList = new List<RemoteFortressReader.MaterialDefinition>();
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

        ContentLoader.Instance = new ContentLoader();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        ContentLoader.Instance.ParseContentIndexFile(Application.streamingAssetsPath + "/index.txt");
        ContentLoader.Instance.FinalizeTextureAtlases();
        watch.Stop();
        Debug.Log("Took a total of " + watch.ElapsedMilliseconds + "ms to load all XML files.");

        //Debug.Log("Buildingtypes fetched: " + netBuildingList.building_list.Count);
        //Debug.Log("Creature Raws fetched: " + netCreatureRawList.creature_raws.Count);
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
            ConnectAndInit();
        }
        catch (UnityException e)
        {

            Debug.LogError(e.Message);
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
                RemoteFortressReader.KeyboardEvent dfEvent = new RemoteFortressReader.KeyboardEvent();
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

    public IEnumerator delayedKeyboardEvent(RemoteFortressReader.KeyboardEvent dfEvent)
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

    /// <summary>
    /// Performs a single update.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    void PerformSingleUpdate()
    {
        networkClient.suspend_game();

        if (mapResetRequested.Pop())
        {
            mapResetCall.execute();
        }

        if (mapInfoCall != null)
        {
            RemoteFortressReader.MapInfo mapInfo;
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
            else {
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
            RemoteFortressReader.ViewInfo viewInfo;
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
            RemoteFortressReader.UnitList unitList;
            unitListCall.execute(null, out unitList);
            netUnitList.Set(unitList);
        }

        if (worldMapCenterCall != null)
        {
            RemoteFortressReader.WorldMap tempWorldMap;
            worldMapCenterCall.execute(null, out tempWorldMap);
            if (tempWorldMap != null)
                DFTime = new DFTime(tempWorldMap.cur_year, tempWorldMap.cur_year_tick);
            if (netWorldMapCenter == null || (tempWorldMap != null &&
                    (netWorldMapCenter.center_x != tempWorldMap.center_x
                    || netWorldMapCenter.center_y != tempWorldMap.center_y)))
            {
                if (worldMapCall != null)
                {
                    RemoteFortressReader.WorldMap worldMap;
                    worldMapCall.execute(null, out worldMap);
                    netWorldMap.Set(worldMap);
                }

                netWorldMapCenter = tempWorldMap;

                worldMapMoved.Set();
                if (regionMapCall != null)
                {
                    RemoteFortressReader.RegionMaps regionMaps;
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
                RemoteFortressReader.RegionMaps regionMaps;
                regionMapCall.execute(null, out regionMaps);
                netRegionMaps.Set(regionMaps);
            }
            nextRegionUpdate = DateTime.MaxValue;
        }


        if (keyboardEventCall != null)
        {
            RemoteFortressReader.KeyboardEvent dfEvent;
            while(keyPresses.TryDequeue(out dfEvent))
            {
                keyboardEventCall.execute(dfEvent);
            }
        }

        // since enqueueing results can block, we do it after we've unsuspended df
        RemoteFortressReader.BlockList resultList = null;

        if (EmbarkMapSize.x > 0
            && EmbarkMapSize.y > 0
            && EmbarkMapSize.z > 0)
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
        networkClient.resume_game();

        if (resultList != null)
        {
            foreach (RemoteFortressReader.MapBlock mapBlock in resultList.map_blocks)
            {
                pendingBlocks.Enqueue(mapBlock);
            }
        }
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
            public UnityThread(DFConnection connection) : base(connection)
            { }

            public override void Poll()
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
            private static readonly System.TimeSpan SLEEP_TIME = System.TimeSpan.FromMilliseconds(16);

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
                connectionThread.Join();
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
                        return;
                    }
                    Thread.Sleep(SLEEP_TIME);
                }
                // finished
                connection.networkClient.disconnect();
                connection.networkClient = null;
            }
        }
    }
}
