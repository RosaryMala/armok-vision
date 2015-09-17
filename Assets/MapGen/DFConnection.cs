using UnityEngine;
using DFHack;
using System.Collections.Generic;

// Class for async communication with DF.
// Will eventually run actual communication on a separate thread.
// Singleton-y - attached to a GameObject.
using System.Threading;

public class DFConnection : MonoBehaviour
{

    // Singleton stuff

    private static DFConnection _instance = null;
    private static List<System.Action> connectionCallbacks = new List<System.Action>();
    public static DFConnection Instance
    {
        get
        {
            return _instance;
        }
    }

    // Can always be called
    public static bool Connected
    {
        get
        {
            return _instance != null && _instance.networkClient != null;
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
    private ConnectionMode connectionMode;

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
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.RegionMaps> regionMapCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.CreatureRawList> creatureRawListCall;
    private color_ostream dfNetworkOut = new color_ostream();
    private RemoteClient networkClient;

    // Things we read from DF

    // Unchanging:
    private RemoteFortressReader.MaterialList _netMaterialList;
    private RemoteFortressReader.MaterialList _netItemList;
    private RemoteFortressReader.TiletypeList _netTiletypeList;
    private RemoteFortressReader.BuildingList _netBuildingList;
    private RemoteFortressReader.CreatureRawList _netCreatureRawList;

    // Changing (used like queues):
    private RemoteFortressReader.ViewInfo _netViewInfo;
    private RemoteFortressReader.UnitList _netUnitList;
    private RemoteFortressReader.WorldMap _netWorldMap;
    private RemoteFortressReader.RegionMaps _netRegionMaps;
    private RemoteFortressReader.MapInfo _netMapInfo;
    // Special sort of queue:
    // It pops elements in random order, but makes sure that
    // we don't bother storing two updates to the same block at once
    private Util.UniqueQueue<DFCoord, RemoteFortressReader.MapBlock> pendingLandscapeBlocks;
    private Util.UniqueQueue<DFCoord, RemoteFortressReader.MapBlock> pendingLiquidBlocks;
    // Cached block request
    private RemoteFortressReader.BlockRequest blockRequest;

    //Rarely changing info that can nevertheless change.
    private DFCoord _embarkMapSize = new DFCoord(0, 0, 0);
    private DFCoord _embarkMapPosition = new DFCoord(-1, -1, -1);
    private bool _mapResetRequested = false;

    // Mutexes for changing / nullable objects
    private Object viewInfoLock = new Object();
    private Object unitListLock = new Object();
    private Object worldMapLock = new Object();
    private Object regionMapLock = new Object();
    private Object mapInfoLock = new Object();
    private Object mapResetLock = new Object();

    // Unchanging properties

    public RemoteFortressReader.MaterialList NetMaterialList
    {
        get
        {
            return _netMaterialList;
        }
    }
    public RemoteFortressReader.MaterialList NetItemList
    {
        get
        {
            return _netItemList;
        }
    }

    public RemoteFortressReader.TiletypeList NetTiletypeList
    {
        get
        {
            return _netTiletypeList;
        }
    }

    public RemoteFortressReader.BuildingList NetBuildingList
    {
        get
        {
            return _netBuildingList;
        }
    }

    public RemoteFortressReader.CreatureRawList NetCreatureRawList
    {
        get
        {
            return _netCreatureRawList;
        }
    }

    // Coordinates of the region we're pulling data from.
    // In block space - multiply x and y by 16 to get tile coordinates.
    public BlockCoord RequestRegionMin
    {
        get;
        private set;
    }
    public BlockCoord RequestRegionMax
    {
        get;
        private set;
    }
    public void SetRequestRegion(BlockCoord min, BlockCoord max)
    {
        lock (blockRequest)
        {
            RequestRegionMin = min;
            RequestRegionMax = max;
            blockRequest.min_x = min.x;
            blockRequest.min_y = min.y;
            blockRequest.min_z = min.z;
            blockRequest.max_x = max.x;
            blockRequest.max_y = max.y;
            blockRequest.max_z = max.z;
        }
    }

    // Pop a view update; return null if there isn't one.
    public RemoteFortressReader.ViewInfo PopViewInfoUpdate()
    {
        lock (viewInfoLock)
        {
            RemoteFortressReader.ViewInfo result = _netViewInfo;
            _netViewInfo = null;
            return result;
        }
    }

    // Pop a unit list update; return null if there isn't one.
    public RemoteFortressReader.UnitList PopUnitListUpdate()
    {
        lock (unitListLock)
        {
            RemoteFortressReader.UnitList result = _netUnitList;
            _netUnitList = null;
            return result;
        }
    }

    // Pop a world map update. The map doesn't change, but the clouds do.
    public RemoteFortressReader.WorldMap PopWorldMapUpdate()
    {
        RemoteFortressReader.WorldMap result = null;
        if (Monitor.TryEnter(worldMapLock))
            try
            {
                result = _netWorldMap;
                _netWorldMap = null;
            }
            finally
            {
                Monitor.Exit(worldMapLock);
            }
        return result;
    }

    // Pop region map update. These change in adventure mode.
    public RemoteFortressReader.RegionMaps PopRegionMapUpdate()
    {
        lock (regionMapLock)
        {
            RemoteFortressReader.RegionMaps result = _netRegionMaps;
            _netRegionMaps = null;
            return result;
        }
    }

    //Fetch the current embark map size
    public DFCoord EmbarkMapSize
    {
        get
        {
            lock(mapInfoLock)
            {
                return _embarkMapSize;
            }
        }
    }

    public DFCoord EmbarkMapPosition
    {
        get
        {
            lock(mapInfoLock)
            {
                return _embarkMapPosition;
            }
        }
    }

    // Pop a map block update; return null if there isn't one.
    public RemoteFortressReader.MapBlock PopLandscapeMapBlockUpdate()
    {
        lock (pendingLandscapeBlocks)
        {
            return pendingLandscapeBlocks.Dequeue();
        }
    }
    public RemoteFortressReader.MapBlock PopLiquidMapBlockUpdate()
    {
        lock (pendingLiquidBlocks)
        {
            return pendingLiquidBlocks.Dequeue();
        }
    }

    //Queue a map hash reset, forcing a re-send of all map blocks
    public void RequestMapReset()
    {
        lock(mapResetLock)
        {
            _mapResetRequested = true;
        }
    }

    // Connect to DF, fetch initial data, start things running
    void ConnectAndInit()
    {
        blockRequest = new RemoteFortressReader.BlockRequest();
        blockRequest.blocks_needed = BlocksToFetch;
        pendingLandscapeBlocks = new Util.UniqueQueue<DFCoord, RemoteFortressReader.MapBlock>();
        pendingLiquidBlocks = new Util.UniqueQueue<DFCoord, RemoteFortressReader.MapBlock>();
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

        networkClient.suspend_game();
        if (viewInfoCall != null)
            viewInfoCall.execute(null, out _netViewInfo);
        if (unitListCall != null)
            unitListCall.execute(null, out _netUnitList);
        if (worldMapCall != null)
            worldMapCall.execute(null, out _netWorldMap);
        if (regionMapCall != null)
            regionMapCall.execute(null, out _netRegionMaps);
        networkClient.resume_game();

        if(mapResetCall!=null)
            mapResetCall.execute();
        InitStatics();

        foreach (System.Action callback in connectionCallbacks)
        {
            callback.Invoke();
        }
        connectionCallbacks.Clear();
        connectionMode = ConnectionMode.GetConnectionMode(this, RunOnAlternateThread);
    }

    void Disconnect()
    {
        _instance = null;
        if (connectionMode != null)
        {
            connectionMode.Terminate();
            connectionMode = null;
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
    /// Tries to bind an RPC function, leaving returning null if it fails.
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

    // Bind the RPC functions we'll be calling
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
        worldMapCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.WorldMap>(networkClient, "GetWorldMap", "RemoteFortressReader");
        regionMapCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.RegionMaps>(networkClient, "GetRegionMaps", "RemoteFortressReader");
        creatureRawListCall = CreateAndBind<dfproto.EmptyMessage, RemoteFortressReader.CreatureRawList>(networkClient, "GetCreatureRaws", "RemoteFortressReader");
    }

    // Get information that only needs to be read once
    void FetchUnchangingInfo()
    {
        if (materialListCall != null)
            materialListCall.execute(null, out _netMaterialList);
        if (itemListCall != null)
            itemListCall.execute(null, out _netItemList);
        if (tiletypeListCall != null)
            tiletypeListCall.execute(null, out _netTiletypeList);
        if (buildingListCall != null)
            buildingListCall.execute(null, out _netBuildingList);
        if (creatureRawListCall != null)
            creatureRawListCall.execute(null, out _netCreatureRawList);
    }

    // Populate lists when we connect
    void InitStatics()
    {
        if (_netMaterialList != null)
        {
            MaterialTokenList.matTokenList = _netMaterialList.material_list;
            Debug.Log("Materials fetched: " + _netMaterialList.material_list.Count);
        }
        if (_netTiletypeList != null)
        {
            TiletypeTokenList.tiletypeTokenList = _netTiletypeList.tiletype_list;
            Debug.Log("Tiletypes fetched: " + _netTiletypeList.tiletype_list.Count);
        }
        if (_netItemList != null)
        {
            ItemTokenList.itemTokenList = _netItemList.material_list;
            Debug.Log("Itemtypes fetched: " + _netItemList.material_list.Count);
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
            ItemTokenList.itemTokenList = blankItemList;
            Debug.Log("Created dummy Itemtype list.");
        }
        if (_netTiletypeList != null)
            MapDataStore.tiletypeTokenList = _netTiletypeList.tiletype_list;

        //Debug.Log("Buildingtypes fetched: " + _netBuildingList.building_list.Count);
        //Debug.Log("Creature Raws fetched: " + _netCreatureRawList.creature_raws.Count);
    }

    void Start()
    {
        if (_instance != null)
        {
            throw new UnityException("Can't have multiple dwarf fortress connections!");
        }
        _instance = this;

        ConnectAndInit();
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
        connectionMode.Poll();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    public sealed class DFRemoteException : System.Exception
    {
        public DFRemoteException(System.Exception inner) : base("Remote exception!", inner) { }
    }

    private abstract class ConnectionMode
    {
        public static ConnectionMode GetConnectionMode(DFConnection connection, bool runOnAltThread)
        {
            if (runOnAltThread)
            {
                return new AltThreadMode(connection);
            }
            else
            {
                return new UnityThreadMode(connection);
            }
        }

        protected readonly DFConnection connection;

        private ConnectionMode() { }
        protected ConnectionMode(DFConnection connection)
        {
            this.connection = connection;
        }

        public abstract void Poll();
        public abstract void Terminate();
    }

    // Single-threaded connection; for debugging.
    private sealed class UnityThreadMode : ConnectionMode
    {
        public UnityThreadMode(DFConnection connection) : base(connection) { }

        public override void Poll()
        {
            try
            {
                // No need for locks, single threaded.
                connection.networkClient.suspend_game();
                if (connection._mapResetRequested)
                {
                    connection.mapResetCall.execute();
                    connection._mapResetRequested = false;
                }
                if (connection.mapInfoCall != null)
                {
                    RemoteFortressReader.MapInfo mapInfo;
                    connection.mapInfoCall.execute(null, out mapInfo);
                    if(mapInfo == null)
                    {
                        if (connection._netMapInfo != null)
                        {
                            connection._embarkMapPosition = new DFCoord(-1, -1, -1);
                            connection._embarkMapSize = new DFCoord(0, 0, 0);
                            MapDataStore.InitMainMap(connection.EmbarkMapSize.x * 16, connection.EmbarkMapSize.y * 16, connection.EmbarkMapSize.z);
                        }
                    }
                    else
                    {
                        if((connection._netMapInfo == null)
                            || mapInfo.block_pos_x != connection._netMapInfo.block_pos_x
                            || mapInfo.block_pos_y != connection._netMapInfo.block_pos_y
                            || mapInfo.block_pos_z != connection._netMapInfo.block_pos_z
                            || mapInfo.block_size_x != connection._netMapInfo.block_size_x
                            || mapInfo.block_size_y != connection._netMapInfo.block_size_y
                            || mapInfo.block_size_z != connection._netMapInfo.block_size_z
                            )
                        {
                            connection._embarkMapPosition = new DFCoord(mapInfo.block_pos_x, mapInfo.block_pos_y, mapInfo.block_pos_z);
                            connection._embarkMapSize = new DFCoord(mapInfo.block_size_x, mapInfo.block_size_y, mapInfo.block_size_z);
                            MapDataStore.InitMainMap(connection.EmbarkMapSize.x * 16, connection.EmbarkMapSize.y * 16, connection.EmbarkMapSize.z);
                            connection.mapResetCall.execute();
                        }
                    }
                    connection._netMapInfo = mapInfo;
                }
                connection.viewInfoCall.execute(null, out connection._netViewInfo);
                connection.unitListCall.execute(null, out connection._netUnitList);
                connection.regionMapCall.execute(null, out connection._netRegionMaps);
                connection.worldMapCall.execute(null, out connection._netWorldMap);
                if (connection.EmbarkMapSize.x > 0
                    && connection.EmbarkMapSize.y > 0
                    && connection.EmbarkMapSize.z > 0)
                {
                    RemoteFortressReader.BlockList resultList;
                    connection.blockListCall.execute(connection.blockRequest, out resultList);
                    foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks)
                    {
                        if (block.tiles.Count > 0)
                            connection.pendingLandscapeBlocks.EnqueueAndDisplace(new DFCoord(block.map_x, block.map_y, block.map_z), block);
                        if (block.water.Count > 0 || block.magma.Count > 0)
                            connection.pendingLiquidBlocks.EnqueueAndDisplace(new DFCoord(block.map_x, block.map_y, block.map_z), block);
                    }
                }
                connection.networkClient.resume_game();
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

    // Connection running on alternate thread.
    private sealed class AltThreadMode : ConnectionMode
    {
        private static readonly System.TimeSpan SLEEP_TIME = System.TimeSpan.FromMilliseconds(16);

        // Use to terminate computation thread
        private volatile bool finished;
        // Said thread
        private readonly Thread connectionThread;
        // Catch errors on the throwing thread and catch them on the main thread
        private System.Object errorLock;
        private System.Exception crashError;

        public AltThreadMode(DFConnection connection)
            : base(connection)
        {
            finished = false;
            errorLock = new Object();
            crashError = null;
            connectionThread = new Thread(new ThreadStart(this.RunForever));
            connectionThread.Start();
        }

        public override void Poll()
        {
            lock (errorLock)
            {
                if (crashError != null)
                {
                    System.Exception error = crashError;
                    crashError = null;
                    throw new DFRemoteException(error);
                }
            }
        }

        public override void Terminate()
        {
            finished = true;
        }

        private void RunForever()
        {
            while (!finished)
            {
                try
                {
                    connection.networkClient.suspend_game();
                    lock(connection.mapResetLock)
                    {
                        if (connection._mapResetRequested)
                        {
                            connection.mapResetCall.execute();
                            connection._mapResetRequested = false;
                        }
                    }
                    if (connection.mapInfoCall != null)
                    {
                        RemoteFortressReader.MapInfo mapInfo;
                        connection.mapInfoCall.execute(null, out mapInfo);
                        if (mapInfo == null)
                        {
                            if (connection._netMapInfo != null)
                            {
                                lock(connection.mapInfoLock)
                                {
                                    connection._embarkMapPosition = new DFCoord(-1, -1, -1);
                                    connection._embarkMapSize = new DFCoord(0, 0, 0);
                                    MapDataStore.InitMainMap(connection.EmbarkMapSize.x * 16, connection.EmbarkMapSize.y * 16, connection.EmbarkMapSize.z);
                                }
                            }
                        }
                        else
                        {
                            if ((connection._netMapInfo == null)
                                || mapInfo.block_pos_x != connection._netMapInfo.block_pos_x
                                || mapInfo.block_pos_y != connection._netMapInfo.block_pos_y
                                || mapInfo.block_pos_z != connection._netMapInfo.block_pos_z
                                || mapInfo.block_size_x != connection._netMapInfo.block_size_x
                                || mapInfo.block_size_y != connection._netMapInfo.block_size_y
                                || mapInfo.block_size_z != connection._netMapInfo.block_size_z
                                )
                            {
                                lock (connection.mapInfoLock)
                                {
                                    connection._embarkMapPosition = new DFCoord(mapInfo.block_pos_x, mapInfo.block_pos_y, mapInfo.block_pos_z);
                                    connection._embarkMapSize = new DFCoord(mapInfo.block_size_x, mapInfo.block_size_y, mapInfo.block_size_z);
                                    MapDataStore.InitMainMap(connection.EmbarkMapSize.x * 16, connection.EmbarkMapSize.y * 16, connection.EmbarkMapSize.z);
                                    connection.mapResetCall.execute();
                                }
                            }
                        }
                        connection._netMapInfo = mapInfo;
                    }
                    lock (connection.viewInfoLock)
                    {
                        if (connection.viewInfoCall != null)
                            connection.viewInfoCall.execute(null, out connection._netViewInfo);
                    }
                    lock (connection.unitListLock)
                    {
                        if (connection.unitListCall != null)
                            connection.unitListCall.execute(null, out connection._netUnitList);
                    }
                    lock (connection.regionMapLock)
                    {
                        if (connection.regionMapCall != null)
                            connection.regionMapCall.execute(null, out connection._netRegionMaps);
                    }
                    lock (connection.worldMapLock)
                    {
                        if (connection.worldMapCall != null)
                            connection.worldMapCall.execute(null, out connection._netWorldMap);
                    }
                    if (connection.EmbarkMapSize.x > 0
                        && connection.EmbarkMapSize.y > 0
                        && connection.EmbarkMapSize.z > 0)
                    {
                        RemoteFortressReader.BlockList resultList = null;
                        lock (connection.blockRequest)
                        {
                            if (connection.blockListCall != null)
                                connection.blockListCall.execute(connection.blockRequest, out resultList);
                        }
                        if (resultList != null)
                        {
                            lock (connection.pendingLandscapeBlocks)
                            {
                                foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks)
                                {
                                    connection.pendingLandscapeBlocks.EnqueueAndDisplace(new DFCoord(block.map_x, block.map_y, block.map_z), block);
                                }
                            }
                            lock (connection.pendingLiquidBlocks)
                            {
                                foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks)
                                {
                                    connection.pendingLiquidBlocks.EnqueueAndDisplace(new DFCoord(block.map_x, block.map_y, block.map_z), block);
                                }
                            }
                        }
                    }
                    connection.networkClient.resume_game();
                    Thread.Sleep(SLEEP_TIME);
                }
                catch (System.Exception e)
                {
                    // For now, just pass on any exceptions and exit
                    lock (errorLock)
                    {
                        crashError = e;
                        return;
                    }
                }
            }
            // finished
            connection.networkClient.disconnect();
            connection.networkClient = null;
        }
    }
}
