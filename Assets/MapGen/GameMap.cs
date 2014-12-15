using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;

public class GameMap : MonoBehaviour
{
    public class MapTile
    {
        int tileType;
    }
    public MapTile[, ,] tiles;
    ConnectionState connectionState;
    public MapBlock defaultMapBlock;
    public GameObject defaultTile;
    public GameWindow viewCamera;
    public Material DefaultMaterial;
    public Mesh[] defaultMeshes = new Mesh[17];
    public List<MapBlock> blockCollection;
    public int rangeX = 0;
    public int rangeY = 0;
    public int rangeZup = 0;
    public int rangeZdown = 0;
    public int posX = 0;
    public int posY = 0;
    public int posZ = 0;

    // Use this for initialization
    void Start()
    {
        InitializeBlocks();
        Connect();
        //connectionState.HashCheckCall.execute();
        GetViewInfo();
        PositionCamera();
        GetMaterialList();
        GetTiletypeList();
        GetUnitList();
        InvokeRepeating("GetBlockList", 0, 1);
        //GetBlockList();
        //Disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        GetViewInfo();
        PositionCamera();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    void InitializeBlocks()
    {
        if (blockCollection == null)
            blockCollection = new List<MapBlock>();
        int wantedSize = rangeX * 2 * rangeY * 2 * (rangeZup + rangeZdown);
        if (blockCollection.Count < wantedSize)
            for (int i = blockCollection.Count; i < wantedSize; i++)
            {
                MapBlock newblock = Instantiate(defaultMapBlock) as MapBlock;
                newblock.transform.parent = this.transform;
                newblock.parent = this;
                blockCollection.Add(newblock);
            }
        else if (blockCollection.Count > wantedSize) //This shouldn't happen normally, but better to be prepared than not
            for (int i = blockCollection.Count - 1; i >= wantedSize; i--)
            {
                Destroy(blockCollection[i]);
                blockCollection.RemoveAt(i);
            }
    }

    void FreeAllBlocks()
    {
        foreach(MapBlock block in blockCollection)
        {
            block.gameObject.SetActive(false);
        }
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

    MapBlock getFreeBlock()
    {
        for (int i = 0; i < blockCollection.Count; i++)
        {
            if (blockCollection[i].gameObject.activeSelf == false)
                return blockCollection[i];
        }
        return null;
    }

    void GetTiletypeList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.TiletypeListCall.execute(null, out connectionState.net_tiletype_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_tiletype_list.tiletype_list.Count + " tiletypes gotten, took 1/" + (1.0 / stopwatch.Elapsed.TotalSeconds) + " seconds.\n");
    }

    void GetBlockList()
    {
        //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //stopwatch.Start();
        posX = (connectionState.net_view_info.view_pos_x + (connectionState.net_view_info.view_size_x/2)) / 16;
        posY = (connectionState.net_view_info.view_pos_y + (connectionState.net_view_info.view_size_y/2)) / 16;
        posZ = connectionState.net_view_info.view_pos_z+1;
        connectionState.net_block_request.min_x = posX - rangeX;
        connectionState.net_block_request.max_x = posX + rangeX;
        connectionState.net_block_request.min_y = posY - rangeY;
        connectionState.net_block_request.max_y = posY + rangeY;
        connectionState.net_block_request.min_z = posZ - rangeZdown;
        connectionState.net_block_request.max_z = posZ + rangeZup;
        connectionState.BlockListCall.execute(connectionState.net_block_request, out connectionState.net_block_list);
        //stopwatch.Stop();
        //Debug.Log(connectionState.net_block_list.map_blocks.Count + " blocks gotten, took 1/" + (1.0 / stopwatch.Elapsed.TotalSeconds) + " seconds.\n");
        //for (int i = 0; i < blockCollection.Count; i++)
        //{
        //    if (blockCollection[i].gameObject.activeSelf == true)
        //    {
        //        blockCollection[i].Reposition(connectionState.net_block_list);
        //    }
        //}
        //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        //watch.Start();
        FreeAllBlocks();
        for (int i = 0; i < connectionState.net_block_list.map_blocks.Count; i++)
        {
            MapBlock newBlock = getFreeBlock();
            if (newBlock == null)
                break;
            newBlock.gameObject.SetActive(true);
            newBlock.SetAllTiles(connectionState.net_block_list.map_blocks[i], connectionState.net_block_list, connectionState.net_tiletype_list);
            newBlock.Regenerate();
            newBlock.name = "MapBlock(" + newBlock.coordString + ")";
        }
        //watch.Stop();
        //Debug.Log("Generating " + connectionState.net_block_list.map_blocks.Count + " Meshes took " + watch.Elapsed.TotalSeconds + " seconds");
    }
    void GetUnitList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.UnitListCall.execute(null, out connectionState.net_unit_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_unit_list.creature_list.Count + " units gotten, took 1/" + (1.0 / stopwatch.Elapsed.TotalSeconds) + " seconds.\n");

    }
    void GetViewInfo()
    {
        connectionState.ViewInfoCall.execute(null, out connectionState.net_view_info);
        //Debug.Log(
        //    "x:" + connectionState.net_view_info.view_pos_x +
        //    ", y:" + connectionState.net_view_info.view_pos_y +
        //    ", z:" + connectionState.net_view_info.view_pos_z +
        //    ", w:" + connectionState.net_view_info.view_size_x +
        //    ", h:" + connectionState.net_view_info.view_size_y
        //    );
    }

    void PositionCamera()
    {
        viewCamera.transform.parent.transform.position = MapBlock.DFtoUnityCoord(
            (connectionState.net_view_info.view_pos_x + (connectionState.net_view_info.view_size_x/2)), 
            (connectionState.net_view_info.view_pos_y + (connectionState.net_view_info.view_size_y/2)), 
            connectionState.net_view_info.view_pos_z+1);
    }
}
