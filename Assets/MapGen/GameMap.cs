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
        connectionState.net_request.save_folder = "ANY";
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
        GetMaterialList();
        PrintFullMaterialList();
    }

    void GetMaterialList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.MaterialListCall.execute(null, out connectionState.net_material_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_material_list.material_list.Count + " materials gotten, took " + stopwatch.Elapsed.TotalSeconds + " seconds.\n");
    }

    void PrintFullMaterialList()
    {
        int limit = connectionState.net_material_list.material_list.Count;
        if (limit >= 100)
            limit = 100;
        //Don't ever do this.
        for (int i = connectionState.net_material_list.material_list.Count - limit; i < connectionState.net_material_list.material_list.Count; i++)
        {
            //no really, don't.
            RemoteFortressReader.MaterialDefinition material = connectionState.net_material_list.material_list[i];
            Debug.Log("{" + material.mat_pair.mat_index + "," + material.mat_pair.mat_type + "}, " + material.id + ", " + material.name);
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
                if (worldMap == null)
                    worldMap = new Dictionary<DFCoord2d, LocalEmbarkTile>();
                if (!worldMap.ContainsKey(localCoord))
                {
                    LocalEmbarkTile tempTile = Instantiate(defaultEmbarkTile) as LocalEmbarkTile;
                    tempTile.makeTile(connectionState.net_embark_tile, connectionState.net_reply);
                }
            }
    }
}
