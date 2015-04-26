using DFHack;

public class ConnectionState {
    public bool is_connected = false;
    public RemoteFortressReader.MaterialList net_material_list;
    public RemoteFortressReader.TiletypeList net_tiletype_list;
    public RemoteFortressReader.BlockList net_block_list;
    public RemoteFortressReader.BlockRequest net_block_request;
    public RemoteFortressReader.UnitList net_unit_list;
    public RemoteFortressReader.ViewInfo net_view_info;
    public RemoteFortressReader.MapInfo net_map_info;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> MaterialListCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList> TiletypeListCall;
    public RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList> BlockListCall;
    public RemoteFunction<dfproto.EmptyMessage> HashCheckCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList> UnitListCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo> ViewInfoCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo> MapInfoCall;
    public RemoteFunction<dfproto.EmptyMessage> MapResetCall;
    color_ostream df_network_out;
    public RemoteClient network_client;

    public ConnectionState()
    {
        network_client = new DFHack.RemoteClient(df_network_out);
        is_connected = network_client.connect();
        if (!is_connected) return;
        net_block_request = new RemoteFortressReader.BlockRequest();
        MaterialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        MaterialListCall.bind(network_client, "GetMaterialList", "RemoteFortressReader");
        TiletypeListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList>();
        TiletypeListCall.bind(network_client, "GetTiletypeList", "RemoteFortressReader");
        BlockListCall = new RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList>();
        BlockListCall.bind(network_client, "GetBlockList", "RemoteFortressReader");
        HashCheckCall = new RemoteFunction<dfproto.EmptyMessage>();
        HashCheckCall.bind(network_client, "CheckHashes", "RemoteFortressReader");
        UnitListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList>();
        UnitListCall.bind(network_client, "GetUnitList", "RemoteFortressReader");
        ViewInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo>();
        ViewInfoCall.bind(network_client, "GetViewInfo", "RemoteFortressReader");
        MapInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MapInfo>();
        MapInfoCall.bind(network_client, "GetMapInfo", "RemoteFortressReader");
        MapResetCall = new RemoteFunction<dfproto.EmptyMessage>();
        MapResetCall.bind(network_client, "ResetMapHashes", "RemoteFortressReader");
    }

    public void Disconnect()
    {
        network_client.disconnect();
        network_client = null;
        df_network_out = null;
    }

}
