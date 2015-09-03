using UnityEngine;
using DFHack;
using System.Collections.Generic;
using System.Linq;

// Class for async communication with DF.
// Will eventually run actual communication on a separate thread.
// Singleton-y - attached to a GameObject.
using System.Threading;
using System.Net.Sockets;

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
    private RemoteFunction<dfproto.EmptyMessage> hashCheckCall;
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
    private RemoteFortressReader.MapInfo _netMapInfo;
    private RemoteFortressReader.BuildingList _netBuildingList;
    private RemoteFortressReader.CreatureRawList _netCreatureRawList;

    // Changing (used like queues):
    private RemoteFortressReader.ViewInfo _netViewInfo;
    private RemoteFortressReader.UnitList _netUnitList;
    private RemoteFortressReader.WorldMap _netWorldMap;
    private RemoteFortressReader.RegionMaps _netRegionMaps;
    // Special sort of queue:
    // It pops elements in random order, but makes sure that
    // we don't bother storing two updates to the same block at once
    private Util.UniqueQueue<DFCoord, RemoteFortressReader.MapBlock> pendingBlocks;
    // Cached block request
    private RemoteFortressReader.BlockRequest blockRequest;

    // Mutexes for changing / nullable objects
    private Object viewInfoLock = new Object();
    private Object unitListLock = new Object();
    private Object worldMapLock = new Object();
    private Object regionMapLock = new Object();

    // Unchanging properties
    public RemoteFortressReader.MapInfo NetMapInfo
    {
        get
        {
            return _netMapInfo;
        }
    }

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
            _netViewInfo = null;
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

    // Pop a map block update; return null if there isn't one.
    public RemoteFortressReader.MapBlock PopMapBlockUpdate()
    {
        lock (pendingBlocks)
        {
            return pendingBlocks.Dequeue();
        }
    }

    // Connect to DF, fetch initial data, start things running
    void ConnectAndInit()
    {
        blockRequest = new RemoteFortressReader.BlockRequest();
        blockRequest.blocks_needed = BlocksToFetch;
        pendingBlocks = new Util.UniqueQueue<DFCoord, RemoteFortressReader.MapBlock>();
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
        viewInfoCall.execute(null, out _netViewInfo);
        unitListCall.execute(null, out _netUnitList);
        worldMapCall.execute(null, out _netWorldMap);
        regionMapCall.execute(null, out _netRegionMaps);
        networkClient.resume_game();

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

    // Bind the RPC functions we'll be calling
    void BindMethods()
    {
        materialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        materialListCall.bind(networkClient, "GetMaterialList", "RemoteFortressReader");
        itemListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        itemListCall.bind(networkClient, "GetItemList", "RemoteFortressReader");
        tiletypeListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList>();
        tiletypeListCall.bind(networkClient, "GetTiletypeList", "RemoteFortressReader");
        blockListCall = new RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList>();
        blockListCall.bind(networkClient, "GetBlockList", "RemoteFortressReader");
        hashCheckCall = new RemoteFunction<dfproto.EmptyMessage>();
        hashCheckCall.bind(networkClient, "CheckHashes", "RemoteFortressReader");
        unitListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList>();
        unitListCall.bind(networkClient, "GetUnitList", "RemoteFortressReader");
        viewInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo>();
        viewInfoCall.bind(networkClient, "GetViewInfo", "RemoteFortressReader");
        mapInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo>();
        mapInfoCall.bind(networkClient, "GetMapInfo", "RemoteFortressReader");
        mapResetCall = new RemoteFunction<dfproto.EmptyMessage>();
        mapResetCall.bind(networkClient, "ResetMapHashes", "RemoteFortressReader");
        buildingListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.BuildingList>();
        buildingListCall.bind(networkClient, "GetBuildingDefList", "RemoteFortressReader");
        worldMapCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.WorldMap>();
        worldMapCall.bind(networkClient, "GetWorldMap", "RemoteFortressReader");
        regionMapCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.RegionMaps>();
        regionMapCall.bind(networkClient, "GetRegionMaps", "RemoteFortressReader");
        creatureRawListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.CreatureRawList>();
        creatureRawListCall.bind(networkClient, "GetCreatureRaws", "RemoteFortressReader");
    }

    // Get information that only needs to be read once
    void FetchUnchangingInfo()
    {
        materialListCall.execute(null, out _netMaterialList);
        itemListCall.execute(null, out _netItemList);
        tiletypeListCall.execute(null, out _netTiletypeList);
        mapInfoCall.execute(null, out _netMapInfo);
        buildingListCall.execute(null, out _netBuildingList);
        creatureRawListCall.execute(null, out _netCreatureRawList);
    }

    // Populate lists when we connect
    void InitStatics()
    {
        MaterialTokenList.matTokenList = _netMaterialList.material_list;
        TiletypeTokenList.tiletypeTokenList = _netTiletypeList.tiletype_list;
        ItemTokenList.itemTokenList = _netItemList.material_list;
        MapDataStore.tiletypeTokenList = _netTiletypeList.tiletype_list;
        MapDataStore.InitMainMap(_netMapInfo.block_size_x * 16, _netMapInfo.block_size_y * 16, _netMapInfo.block_size_z);
        Debug.Log("Materials fetched: " + _netMaterialList.material_list.Count);
        Debug.Log("Tiletypes fetched: " + _netTiletypeList.tiletype_list.Count);
        Debug.Log("Itemtypes fetched: " + _netItemList.material_list.Count);
        Debug.Log("Buildingtypes fetched: " + _netBuildingList.building_list.Count);
        Debug.Log("Fetched " + _netRegionMaps.world_maps.Count + " surrounding regions");
        Debug.Log("Creature Raws fetched: " + _netCreatureRawList.creature_raws.Count);
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
                connection.viewInfoCall.execute(null, out connection._netViewInfo);
                connection.unitListCall.execute(null, out connection._netUnitList);
                connection.regionMapCall.execute(null, out connection._netRegionMaps);
                connection.worldMapCall.execute(null, out connection._netWorldMap);
                RemoteFortressReader.BlockList resultList;
                connection.blockListCall.execute(connection.blockRequest, out resultList);
                foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks)
                {
                    connection.pendingBlocks.EnqueueAndDisplace(new DFCoord(block.map_x, block.map_y, block.map_z), block);
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
                    lock (connection.viewInfoLock)
                    {
                        connection.viewInfoCall.execute(null, out connection._netViewInfo);
                    }
                    lock (connection.unitListLock)
                    {
                        connection.unitListCall.execute(null, out connection._netUnitList);
                    }
                    lock (connection.regionMapLock)
                    {
                        connection.regionMapCall.execute(null, out connection._netRegionMaps);
                    }
                    if (Monitor.TryEnter(connection.worldMapLock))
                        try
                        {
                            connection.worldMapCall.execute(null, out connection._netWorldMap);
                        }
                        finally
                        {
                            Monitor.Exit(connection.worldMapLock);
                        }
                    RemoteFortressReader.BlockList resultList;
                    lock (connection.blockRequest)
                    {
                        connection.blockListCall.execute(connection.blockRequest, out resultList);
                    }
                    lock (connection.pendingBlocks)
                    {
                        foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks)
                        {
                            connection.pendingBlocks.EnqueueAndDisplace(new DFCoord(block.map_x, block.map_y, block.map_z), block);
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
