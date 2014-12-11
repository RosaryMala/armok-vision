using DFHack;

public class ConnectionState {
    public bool is_connected = false;
    public RemoteFortressReader.MaterialList net_material_list;
    public RemoteFortressReader.BlockList net_block_list;
    public RemoteFortressReader.BlockRequest net_block_request;
    public RemoteFortressReader.UnitList net_unit_list;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> MaterialListCall;
    public RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList> BlockListCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList> UnitListCall;
    color_ostream df_network_out;
    RemoteClient network_client;

    public ConnectionState()
    {
        network_client = new DFHack.RemoteClient(df_network_out);
        is_connected = network_client.connect();
        if (!is_connected) return;
        net_block_request = new RemoteFortressReader.BlockRequest();
        MaterialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        BlockListCall = new RemoteFunction<RemoteFortressReader.BlockRequest, RemoteFortressReader.BlockList>();
        UnitListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.UnitList>();
        MaterialListCall.bind(network_client, "GetMaterialList", "RemoteFortressReader");
        BlockListCall.bind(network_client, "GetBlockList", "RemoteFortressReader");
        UnitListCall.bind(network_client, "GetUnitList", "RemoteFortressReader");
    }

    public void Disconnect()
    {
        network_client.disconnect();
        network_client = null;
        df_network_out = null;
    }

}
