using UnityEngine;
using DFHack;
using System.Collections.Generic;

// Class for async communication with DF.
// Will eventually run actual communication on a separate thread.
// Singleton-y - attached to a GameObject.

public class DFConnection : MonoBehaviour {

    // Singleton stuff
    private static DFConnection _instance = null;
    private static List<System.Action> connectionCallbacks = new List<System.Action>();

    // Remote bindings
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> MaterialListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList> TiletypeListCall;
    private RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList> BlockListCall;
    private RemoteFunction<dfproto.EmptyMessage> HashCheckCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList> UnitListCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo> ViewInfoCall;
    private RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo> MapInfoCall;
    private RemoteFunction<dfproto.EmptyMessage> MapResetCall;
    private color_ostream dfNetworkOut;
    private RemoteClient networkClient;

    // Data from DF
    private RemoteFortressReader.MaterialList _netMaterialList;
    private RemoteFortressReader.TiletypeList _netTiletypeList;
    private RemoteFortressReader.MapInfo _netMapInfo;
    private RemoteFortressReader.ViewInfo _netViewInfo;
    private RemoteFortressReader.UnitList _netUnitList;

    // Special stuff for map blocks
    private Queue<RemoteFortressReader.BlockRequest> pendingBlockRequests;
    // We use this as a (sort of) queue-
    // It pulls elements in random order, but makes sure that
    // we don't bother storing two updates to the same block at once
    private Dictionary<DFCoord, RemoteFortressReader.MapBlock> pendingBlocks;

    private const int MAX_BLOCK_REQUESTS_TO_RUN = 2;

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

    public void RequestMapBlock (RemoteFortressReader.BlockRequest req) {
        pendingBlockRequests.Enqueue(req);
    }

    public RemoteFortressReader.MapBlock PopMapBlock () {
        if (pendingBlocks.Count == 0) {
            return null;
        }

        KeyValuePair<DFCoord, RemoteFortressReader.MapBlock> pair = pendingBlocks.GetEnumerator().Current;
        pendingBlocks.Remove(pair.Key);
        return pair.Value;
    }


    void Connect () {
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
        MapResetCall.execute();

        foreach (System.Action callback in connectionCallbacks) {
            callback.Invoke();
        }
        connectionCallbacks.Clear();
    }

    void BindMethods () {
        MaterialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        MaterialListCall.bind(networkClient, "GetMaterialList", "RemoteFortressReader");
        TiletypeListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList>();
        TiletypeListCall.bind(networkClient, "GetTiletypeList", "RemoteFortressReader");
        BlockListCall = new RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList>();
        BlockListCall.bind(networkClient, "GetBlockList", "RemoteFortressReader");
        HashCheckCall = new RemoteFunction<dfproto.EmptyMessage>();
        HashCheckCall.bind(networkClient, "CheckHashes", "RemoteFortressReader");
        UnitListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList>();
        UnitListCall.bind(networkClient, "GetUnitList", "RemoteFortressReader");
        ViewInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo>();
        ViewInfoCall.bind(networkClient, "GetViewInfo", "RemoteFortressReader");
        MapInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo>();
        MapInfoCall.bind(networkClient, "GetMapInfo", "RemoteFortressReader");
        MapResetCall = new RemoteFunction<dfproto.EmptyMessage>();
        MapResetCall.bind(networkClient, "ResetMapHashes", "RemoteFortressReader");
    }

	void Start () {
        if (_instance != null) {
            throw new UnityException("Can't have multiple dwarf fortress connections!");
        }
        _instance = this;
        pendingBlockRequests = new Queue<RemoteFortressReader.BlockRequest>();
        pendingBlocks = new Dictionary<DFCoord, RemoteFortressReader.MapBlock>();
        Connect();
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

    void FetchUnchangingInfo () {
        MaterialListCall.execute(null, out _netMaterialList);
        TiletypeListCall.execute(null, out _netTiletypeList);
        MapInfoCall.execute(null, out _netMapInfo);
    }

    void PollDF () {
        networkClient.suspend_game();
        ViewInfoCall.execute(null, out _netViewInfo);
        UnitListCall.execute(null, out _netUnitList);
        for (int i = 0; i < MAX_BLOCK_REQUESTS_TO_RUN; i++) {
            if (pendingBlockRequests.Count == 0) break;
            RemoteFortressReader.BlockRequest req = pendingBlockRequests.Dequeue();
            RemoteFortressReader.BlockList resultList;
            BlockListCall.execute(req, out resultList);
            foreach (RemoteFortressReader.MapBlock block in resultList.map_blocks) {
                pendingBlocks[new DFCoord(block.map_x, block.map_y, block.map_z)] =
                    block;
            }
        }
        networkClient.resume_game();
    }
}
