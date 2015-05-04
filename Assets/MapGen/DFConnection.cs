using UnityEngine;
using DFHack;
using System.Collections.Generic;

// Class for async communication with DF.
// Will eventually run actual communication on a separate thread.
// Singleton-y - attached to a GameObject.

public class DFConnection : MonoBehaviour {

    // Assignable values
    public int BlocksToFetch = 4;

    // Singleton stuff
    private static DFConnection _instance = null;
    private static List<System.Action> connectionCallbacks = new List<System.Action>();

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

    // Data from DF
    private RemoteFortressReader.MaterialList _netMaterialList;
    private RemoteFortressReader.TiletypeList _netTiletypeList;
    private RemoteFortressReader.MapInfo _netMapInfo;
    private RemoteFortressReader.ViewInfo _netViewInfo;
    private RemoteFortressReader.UnitList _netUnitList;

    // Special stuff for map blocks.
    // Coordinates of the region we're pulling data from:
    private DFCoord _requestRegionMin;
    private DFCoord _requestRegionMax;
    public DFCoord RequestRegionMin {
        get {
            return _requestRegionMin;
        }
        set {
            blockRequest.min_x = value.x;
            blockRequest.min_y = value.y;
            blockRequest.min_z = value.z;
            _requestRegionMin = value;
        }
    }
    public DFCoord RequestRegionMax {
        get {
            return _requestRegionMax;
        }
        set {

            blockRequest.max_x = value.x;
            blockRequest.max_y = value.y;
            blockRequest.max_z = value.z;
            _requestRegionMax = value;
        }
    }
    // Cached block request
    private RemoteFortressReader.BlockRequest blockRequest;
    // We use this as a (sort of) queue-
    // It pops elements in random order, but makes sure that
    // we don't bother storing two updates to the same block at once
    private Dictionary<DFCoord, RemoteFortressReader.MapBlock> pendingBlocks;

    public static DFConnection Instance {
        get {
            return _instance;
        }
    }

    public static bool Connected {
        get {
            return _instance != null && _instance.networkClient != null;
        }
    }
    public static void RegisterConnectionCallback(System.Action callback) {
        connectionCallbacks.Add(callback);
    }

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

    public RemoteFortressReader.ViewInfo NetViewInfo {
        get {
            return _netViewInfo;
        }
    }

    public RemoteFortressReader.MapBlock PopMapBlock () {
        if (pendingBlocks.Count == 0) {
            return null;
        }

        KeyValuePair<DFCoord, RemoteFortressReader.MapBlock> pair = pendingBlocks.GetEnumerator().Current;
        pendingBlocks.Remove(pair.Key);
        return pair.Value;
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
        PopulateTokenLists();

        foreach (System.Action callback in connectionCallbacks) {
            callback.Invoke();
        }
        connectionCallbacks.Clear();
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
        viewInfoCall.execute(null, out _netViewInfo);
        unitListCall.execute(null, out _netUnitList);
        RemoteFortressReader.BlockList resultList;
        blockListCall.execute(blockRequest, out resultList);
        foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks) {
                pendingBlocks[new DFCoord(block.map_x, block.map_y, block.map_z)] =
                    block;
        }
        networkClient.resume_game();
    }

    // 
    void PopulateTokenLists () {
        MaterialTokenList.matTokenList = _netMaterialList.material_list;
        TiletypeTokenList.tiletypeTokenList = _netTiletypeList.tiletype_list;
        MapTile.tiletypeTokenList = _netTiletypeList.tiletype_list;
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


}
