using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;

public class GameMap : MonoBehaviour
{
    ConnectionState connectionState;
    public LocalEmbarkTile defaultEmbarkTile;
    public Dictionary<DFCoord2d, LocalEmbarkTile> worldMap;
    public int sizeX = 0;
    public int sizeY = 0;
    public int posX = 0;
    public int posY = 0;

    // Use this for initialization
    void Start()
    {
        Connect();
        GetMapInfo();
        Disconnect();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Connect()
    {
        if (connectionState != null)
            return;
        else
        {
            connectionState = new ConnectionState();
            if (!connectionState.is_connected)
                Disconnect();
        }
    }

    void Disconnect()
    {
        connectionState.Disconnect();
        connectionState = null;
    }

    void GetMapInfo()
    {
        if(connectionState.EmbarkInfoCall.execute(connectionState.net_request, out connectionState.net_reply) == command_result.CR_OK)
        {
            if (connectionState.net_reply.available)
            {
                posX = connectionState.net_reply.region_x;
                posY = connectionState.net_reply.region_y;
                sizeX = connectionState.net_reply.region_size_x;
                sizeY = connectionState.net_reply.region_size_y;
                Debug.Log("DF map is " + sizeX + "x" + sizeY + " tiles big, situated at " + posX + "," + posY);
                GetAllTiles();
            }
            else Debug.Log("DF map is not available");
        }
    }

    void GetAllTiles()
    {
        for(int yy = 0; yy < connectionState.net_reply.region_size_y; yy++)
            for(int xx = 0; xx < connectionState.net_reply.region_size_x; xx++)
            {
                GetEmbarkTile(new DFCoord2d(xx, yy));
            }
    }

    void GetEmbarkTile(DFCoord2d localCoord)
    {
        connectionState.net_tile_request.want_x = localCoord.x;
        connectionState.net_tile_request.want_y = localCoord.y;
        if (connectionState.net_tile_request != null)
            if (connectionState.EmbarkTileCall.execute(connectionState.net_tile_request, out connectionState.net_embark_tile) == DFHack.command_result.CR_OK)
            {
                if (!connectionState.net_embark_tile.is_valid)
                    return;
                if (!worldMap.ContainsKey(localCoord))
                {
                    LocalEmbarkTile tempTile = Instantiate(defaultEmbarkTile) as LocalEmbarkTile;
                    tempTile.makeTile(connectionState.net_embark_tile, connectionState.net_reply);
                }
            }
    }
}
