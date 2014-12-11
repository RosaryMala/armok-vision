using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;

public class GameMap : MonoBehaviour
{
    ConnectionState connectionState;
    public MapBlock defaultMapBlock;
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
        GetMaterialList();
        GetUnitList();
        GetBlockList();
        Disconnect();
    }

    // Update is called once per frame
    void Update()
    {

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
                blockCollection.Add(newblock);
            }
        else if (blockCollection.Count > wantedSize) //This shouldn't happen normally, but better to be prepared than not
            for (int i = blockCollection.Count - 1; i >= wantedSize; i--)
            {
                Destroy(blockCollection[i]);
                blockCollection.RemoveAt(i);
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

    void GetBlockList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.net_block_request.min_x = posX - rangeX;
        connectionState.net_block_request.max_x = posX + rangeX;
        connectionState.net_block_request.min_y = posY - rangeY;
        connectionState.net_block_request.max_y = posY + rangeY;
        connectionState.net_block_request.min_z = posZ - rangeZdown;
        connectionState.net_block_request.max_z = posZ + rangeZup;
        connectionState.BlockListCall.execute(connectionState.net_block_request, out connectionState.net_block_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_block_list.map_blocks.Count + " blocks gotten, took 1/" + (1.0 / stopwatch.Elapsed.TotalSeconds) + " seconds.\n");
        for (int i = 0; i < blockCollection.Count; i++)
        {
            if (blockCollection[i].gameObject.activeSelf == true)
            {
                blockCollection[i].Reposition(connectionState.net_block_list);
            }
        }
        for (int i = 0; i < connectionState.net_block_list.map_blocks.Count; i++)
        {
            MapBlock newBlock = getFreeBlock();
            if (newBlock == null)
                break;
            newBlock.gameObject.SetActive(true);
            newBlock.SetAllTiles(connectionState.net_block_list.map_blocks[i], connectionState.net_block_list);
            newBlock.Regenerate();
            newBlock.name = "MapBlock(" + newBlock.coordString + ")";
        }

    }
    void GetUnitList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.UnitListCall.execute(null, out connectionState.net_unit_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_unit_list.creature_list.Count + " units gotten, took 1/" + (1.0 / stopwatch.Elapsed.TotalSeconds) + " seconds.\n");

    }
}
