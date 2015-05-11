using UnityEngine;
using DFHack;
using System.Collections.Generic;
using System.Linq;

// Class for async communication with DF.
// Will eventually run actual communication on a separate thread.
// Singleton-y - attached to a GameObject.

public class DFConnection : MonoBehaviour {

    // Singleton stuff

    private static DFConnection _instance = null;
    private static List<System.Action> connectionCallbacks = new List<System.Action>();
    public static DFConnection Instance {
        get {
            return _instance;
        }
    }

    // Can always be called
    public static bool Connected {
        get {
            return _instance != null && _instance.networkClient != null;
        }
    }

    // Can be called before instance is initialized.
    public static void RegisterConnectionCallback(System.Action callback) {
        connectionCallbacks.Add(callback);
    }

    // Instance stuff

    // Assignable values
    public int BlocksToFetch = 4;

    // Remote bindings
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> materialListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList> tiletypeListCall;
    private RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList> blockListCall;
    private RemoteFunction<dfproto.EmptyMessage> hashCheckCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList> unitListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo> viewInfoCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo> mapInfoCall;
    private RemoteFunction<dfproto.EmptyMessage> mapResetCall;
    private color_ostream dfNetworkOut;
    private RemoteClient networkClient;

    // Things we read from DF

    // Unchanging:
    private RemoteFortressReader.MaterialList _netMaterialList;
    private RemoteFortressReader.TiletypeList _netTiletypeList;
    private RemoteFortressReader.MapInfo _netMapInfo;

    // Changing (used like queues):
    private RemoteFortressReader.ViewInfo _netViewInfo;
    private RemoteFortressReader.UnitList _netUnitList;
    // Special sort of queue:
    // It pops elements in random order, but makes sure that
    // we don't bother storing two updates to the same block at once
    private Dictionary<DFCoord, RemoteFortressReader.MapBlock> pendingBlocks;
    // Cached block request
    private RemoteFortressReader.BlockRequest blockRequest;

    // Mutexes for changing / nullable objects
    private Object viewInfoLock = new Object();
    private Object unitListLock = new Object();

    // Unchanging properties
    public RemoteFortressReader.MapInfo NetMapInfo {
        get {
            return _netMapInfo;
        }
    }

    public RemoteFortressReader.MaterialList NetMaterialList {
        get {
            return _netMaterialList;
        }
    }

    public RemoteFortressReader.TiletypeList NetTiletypeList {
        get {
            return _netTiletypeList;
        }
    }
    // Coordinates of the region we're pulling data from:
    public DFCoord RequestRegionMin {
        get; private set;
    }
    public DFCoord RequestRegionMax {
        get; private set;
    }
    public void SetRequestRegion(DFCoord min, DFCoord max) {
        lock (blockRequest) {
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
    public RemoteFortressReader.ViewInfo PopViewInfoUpdate () {
        lock (viewInfoLock) {
            RemoteFortressReader.ViewInfo result = _netViewInfo;
            _netViewInfo = null;
            return result;
        }
    }

    // Pop a unit list update; return null if there isn't one.
    public RemoteFortressReader.UnitList PopUnitListUpdate () {
        lock (unitListLock) {
            RemoteFortressReader.UnitList result = _netUnitList;
            _netViewInfo = null;
            return result;
        }
    }

    // Pop a map block update; return null if there isn't one.
    public RemoteFortressReader.MapBlock PopMapBlockUpdate () {
        lock (pendingBlocks) {
            if (pendingBlocks.Count == 0) {
                return null;
            }
            var popped = pendingBlocks.First();
            pendingBlocks.Remove(popped.Key);
            return popped.Value;
        }
    }

    // Connect to DF, fetch initial data, start things running
    void ConnectAndInit () {
        blockRequest = new RemoteFortressReader.BlockRequest();
        blockRequest.blocks_needed = BlocksToFetch;
        pendingBlocks = new Dictionary<DFCoord, RemoteFortressReader.MapBlock>();
        networkClient = new DFHack.RemoteClient(dfNetworkOut);
        bool success = networkClient.connect();
        if (!success) {
            networkClient.disconnect();
            networkClient = null;
            throw new UnityException("DF Connection Failure");
        }
        BindMethods();
        FetchUnchangingInfo();
        PollDF();
        mapResetCall.execute();
        InitStatics();

        foreach (System.Action callback in connectionCallbacks) {
            callback.Invoke();
        }
        connectionCallbacks.Clear();
    }

    void Disconnect() {
        networkClient.disconnect();
        networkClient = null;
        _instance = null;
    }

    // Bind the RPC functions we'll be calling
    void BindMethods () {
        materialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        materialListCall.bind(networkClient, "GetMaterialList", "RemoteFortressReader");
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
    }

    // Get information that only needs to be read once
    void FetchUnchangingInfo () {
        materialListCall.execute(null, out _netMaterialList);
        tiletypeListCall.execute(null, out _netTiletypeList);
        mapInfoCall.execute(null, out _netMapInfo);
    }

    // Fetch information that changes & read some pending map blocks
    void PollDF () {
        networkClient.suspend_game();
        lock (viewInfoLock) {
            viewInfoCall.execute(null, out _netViewInfo);
        }
        lock (unitListLock) {
            unitListCall.execute(null, out _netUnitList);
        }
        RemoteFortressReader.BlockList resultList;
        blockListCall.execute(blockRequest, out resultList);
        lock (pendingBlocks) {
            //Debug.Log ("Read "+resultList.map_blocks.Count+" Map Blocks");
            foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks) {
                    pendingBlocks[new DFCoord(block.map_x, block.map_y, block.map_z)] =
                        block;
            }
        }
        networkClient.resume_game();
    }

    // Populate lists when we connect
    void InitStatics () {
        MaterialTokenList.matTokenList = _netMaterialList.material_list;
        TiletypeTokenList.tiletypeTokenList = _netTiletypeList.tiletype_list;
        MapDataStore.tiletypeTokenList = _netTiletypeList.tiletype_list;
        MapDataStore.InitMainMap(_netMapInfo.block_size_x * 16, _netMapInfo.block_size_y * 16, _netMapInfo.block_size_z);
        Debug.Log("Materials fetched: " + _netMaterialList.material_list.Count);
        Debug.Log("Tiletypes fetched: " + _netTiletypeList.tiletype_list.Count);
    }

	void Start () {
        if (_instance != null) {
            throw new UnityException("Can't have multiple dwarf fortress connections!");
        }
        _instance = this;

        ConnectAndInit();
    }
	
	void Update () {
	    if (!Connected) {
            return;
        }
        foreach (System.Action callback in connectionCallbacks) {
            callback.Invoke();
        }
        connectionCallbacks.Clear();

        PollDF();
	}

    void OnDestroy() {
        Disconnect();
    }
}
