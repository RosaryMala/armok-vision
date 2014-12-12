using DFHack;

public class ConnectionState {
    public bool is_connected = false;
    public RemoteFortressReader.MaterialList net_material_list;
    public RemoteFortressReader.TiletypeList net_tiletype_list;
    public RemoteFortressReader.BlockList net_block_list;
    public RemoteFortressReader.BlockRequest net_block_request;
    public RemoteFortressReader.UnitList net_unit_list;
    public RemoteFortressReader.ViewInfo net_view_info;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> MaterialListCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList> TiletypeListCall;
    public RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList> BlockListCall;
    public RemoteFunction<dfproto.EmptyMessage> HashCheckCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList> UnitListCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo> ViewInfoCall;
    color_ostream df_network_out;
    RemoteClient network_client;

    public ConnectionState()
    {
        network_client = new DFHack.RemoteClient(df_network_out);
        is_connected = network_client.connect();
        if (!is_connected) return;
        net_block_request = new RemoteFortressReader.BlockRequest();
        MaterialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        TiletypeListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.TiletypeList>();
        BlockListCall = new RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList>();
        HashCheckCall = new RemoteFunction<dfproto.EmptyMessage>();
        UnitListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList>();
        ViewInfoCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.ViewInfo>();
        MaterialListCall.bind(network_client, "GetMaterialList", "RemoteFortressReader");
        TiletypeListCall.bind(network_client, "GetTiletypeList", "RemoteFortressReader");
        BlockListCall.bind(network_client, "GetBlockList", "RemoteFortressReader");
        HashCheckCall.bind(network_client, "CheckHashes", "RemoteFortressReader");
        UnitListCall.bind(network_client, "GetUnitList", "RemoteFortressReader");
        ViewInfoCall.bind(network_client, "GetViewInfo", "RemoteFortressReader");
    }

    public void Disconnect()
    {
        network_client.disconnect();
        network_client = null;
        df_network_out = null;
    }

}
