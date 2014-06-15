using DFHack;

public class ConnectionState {
    public bool is_connected = false;
    public isoworldremote.MapRequest net_request;
    public isoworldremote.MapReply net_reply;
    public isoworldremote.TileRequest net_tile_request;
    public isoworldremote.EmbarkTile net_embark_tile;
    public isoworldremote.RawNames net_material_names;
    public RemoteFortressReader.MaterialList net_material_list;
    public RemoteFunction<isoworldremote.MapRequest, isoworldremote.MapReply> EmbarkInfoCall;
    public RemoteFunction<isoworldremote.MapRequest, isoworldremote.RawNames> MaterialInfoCall;
    public RemoteFunction<isoworldremote.TileRequest, isoworldremote.EmbarkTile> EmbarkTileCall;
    public RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList> MaterialListCall;
    color_ostream df_network_out;
    RemoteClient network_client;

    public ConnectionState()
    {
        network_client = new DFHack.RemoteClient(df_network_out);
        is_connected = network_client.connect();
        if (!is_connected) return;
        net_request = new isoworldremote.MapRequest();
        net_reply = new isoworldremote.MapReply();
        net_tile_request = new isoworldremote.TileRequest();
        net_embark_tile = new isoworldremote.EmbarkTile();
        net_material_names = new isoworldremote.RawNames();
        EmbarkInfoCall = new RemoteFunction<isoworldremote.MapRequest, isoworldremote.MapReply>();
        EmbarkTileCall = new RemoteFunction<isoworldremote.TileRequest, isoworldremote.EmbarkTile>();
        MaterialInfoCall = new RemoteFunction<isoworldremote.MapRequest, isoworldremote.RawNames>();
        MaterialListCall = new RemoteFunction<dfproto.EmptyMessage, RemoteFortressReader.MaterialList>();
        EmbarkInfoCall.bind(network_client, "GetEmbarkInfo", "isoworldremote");
        EmbarkTileCall.bind(network_client, "GetEmbarkTile", "isoworldremote");
        MaterialInfoCall.bind(network_client, "GetRawNames", "isoworldremote");
        MaterialListCall.bind(network_client, "GetMaterialList", "RemoteFortressReader");
        EmbarkInfoCall.execute(net_request, out net_reply);
    }

    public void Disconnect()
    {
        network_client.disconnect();
        network_client = null;
        df_network_out = null;
    }

}
