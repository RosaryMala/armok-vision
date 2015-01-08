using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DFHack;
using RemoteFortressReader;
using UnityEngine.UI;

public class GameMap : MonoBehaviour
{
    public ContentLoader contentLoader = new ContentLoader();

    public class MapTile
    {
        public int tileType;
        public MatPairStruct material;
        public MatPairStruct base_material;
        public MatPairStruct layer_material;
        public MatPairStruct vein_material;
    }
    MapTile[, ,] tiles;
    public GenericTile tileSelector;
    public ConnectionState connectionState;
    public GameObject defaultMapBlock;
    public GameWindow viewCamera;
    public Material DefaultMaterial;
    public MeshFilter[,,] blocks; // Dumb blocks for holding the terrain data.
    public int rangeX = 0;
    public int rangeY = 0;
    public int rangeZup = 0;
    public int rangeZdown = 0;
    public int posX = 0;
    public int posY = 0;
    public int posZ = 0;
    public int map_x;
    public int map_y;
    public Text genStatus;
    public Text cursorProperties;

    Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition> materials;

    public static float tileHeight = 3.0f;
    public static float tileWidth = 2.0f;
    public static int blockSize = 48;
    public static Vector3 DFtoUnityCoord(int x, int y, int z)
    {
        Vector3 outCoord = new Vector3(x * tileWidth, z * tileHeight, y * (-tileWidth));
        return outCoord;
    }

    MeshCombineUtility.MeshInstance[] meshBuffer;
    //CombineInstance[] meshBuffer;

    // Use this for initialization
    void Start()
    {
        Connect();
        InitializeBlocks();
        //connectionState.HashCheckCall.execute();
        GetViewInfo();
        PositionCamera();
        GetMaterialList();
        GetTiletypeList();
        GetUnitList();
        InvokeRepeating("GetBlockList", 0, 0.25f);
        //GetBlockList();
        //Disconnect();
        contentLoader.matTokenList = connectionState.net_material_list.material_list;
        contentLoader.ParseContentIndexFile(Application.streamingAssetsPath + "\\index.txt");
    }

    // Update is called once per frame
    void Update()
    {
        GetViewInfo();
        PositionCamera();
        HideMeshes();
        ShowCursorInfo();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    void InitializeBlocks()
    {
        GetMapInfo();
        Debug.Log("Map Size: " + connectionState.net_map_info.block_size_x + ", " + connectionState.net_map_info.block_size_y + ", " + connectionState.net_map_info.block_size_z);
        tiles = new MapTile[connectionState.net_map_info.block_size_x * 16, connectionState.net_map_info.block_size_y * 16, connectionState.net_map_info.block_size_z];
        blocks = new MeshFilter[connectionState.net_map_info.block_size_x * 16 / blockSize, connectionState.net_map_info.block_size_y * 16 / blockSize, connectionState.net_map_info.block_size_z];
    }

    //void InitializeBlocks()
    //{
    //    if (blockCollection == null)
    //        blockCollection = new List<MapBlock>();
    //    int wantedSize = rangeX * 2 * rangeY * 2 * (rangeZup + rangeZdown);
    //    if (blockCollection.Count < wantedSize)
    //        for (int i = blockCollection.Count; i < wantedSize; i++)
    //        {
    //            MapBlock newblock = Instantiate(defaultMapBlock) as MapBlock;
    //            newblock.transform.parent = this.transform;
    //            newblock.parent = this;
    //            blockCollection.Add(newblock);
    //        }
    //    else if (blockCollection.Count > wantedSize) //This shouldn't happen normally, but better to be prepared than not
    //        for (int i = blockCollection.Count - 1; i >= wantedSize; i--)
    //        {
    //            Destroy(blockCollection[i]);
    //            blockCollection.RemoveAt(i);
    //        }
    //}

    //void FreeAllBlocks()
    //{
    //    foreach(MapBlock block in blockCollection)
    //    {
    //        block.gameObject.SetActive(false);
    //    }
    //}

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
        if (materials == null)
            materials = new Dictionary<MatPairStruct, RemoteFortressReader.MaterialDefinition>();
        materials.Clear();
        foreach(RemoteFortressReader.MaterialDefinition material in connectionState.net_material_list.material_list)
        {
            materials[material.mat_pair] = material;
        }
        stopwatch.Stop();
        Debug.Log(materials.Count + " materials gotten, took " + stopwatch.Elapsed.Milliseconds + " ms.");
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

    //MapBlock getFreeBlock()
    //{
    //    for (int i = 0; i < blockCollection.Count; i++)
    //    {
    //        if (blockCollection[i].gameObject.activeSelf == false)
    //            return blockCollection[i];
    //    }
    //    return null;
    //}

    public MapTile GetTile(int x, int y, int z)
    {
        return tiles[x, y, z];
    }

    void GetTiletypeList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.TiletypeListCall.execute(null, out connectionState.net_tiletype_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_tiletype_list.tiletype_list.Count + " tiletypes gotten, took " + stopwatch.Elapsed.Milliseconds + " ms.");
    }

    void CopyTiles(RemoteFortressReader.MapBlock DFBlock)
    {
       for(int xx = 0; xx < 16; xx++)
           for(int yy = 0; yy < 16; yy++)
           {
               if(tiles[DFBlock.map_x + xx, DFBlock.map_y + yy, DFBlock.map_z] == null)
               {
                   tiles[DFBlock.map_x + xx, DFBlock.map_y + yy, DFBlock.map_z] = new MapTile();
               }
               MapTile tile = tiles[DFBlock.map_x + xx, DFBlock.map_y + yy, DFBlock.map_z];
               tile.tileType = DFBlock.tiles[xx + (yy * 16)];
               tile.material = DFBlock.materials[xx + (yy * 16)];
               tile.base_material = DFBlock.base_materials[xx + (yy * 16)];
               tile.layer_material = DFBlock.layer_materials[xx + (yy * 16)];
               tile.vein_material = DFBlock.vein_materials[xx + (yy * 16)];
           }
    }

    bool GenerateTiles(int block_x, int block_y, int block_z)
    {
        int bufferIndex = 0;
        for (int xx = (block_x * blockSize); xx < (block_x + 1) * blockSize; xx++)
            for (int yy = (block_y * blockSize); yy < (block_y + 1) * blockSize; yy++)
            {
                if(meshBuffer == null)
                {
                    meshBuffer = new MeshCombineUtility.MeshInstance[blockSize * blockSize];
                    //meshBuffer = new CombineInstance[blockSize * blockSize];
                }

                meshBuffer[bufferIndex].mesh = tileSelector.GetMesh(this, xx, yy, block_z);
                meshBuffer[bufferIndex].transform = Matrix4x4.TRS(DFtoUnityCoord(xx, yy, block_z), Quaternion.identity, Vector3.one);
                if (tiles[xx, yy, block_z] != null)
                {
                    Color newColor = contentLoader.colorConfiguration[tiles[xx, yy, block_z].material];
                    if (newColor == Color.black)
                    {
                        MaterialDefinition mattie;
                        if (materials.TryGetValue(tiles[xx, yy, block_z].material, out mattie))
                        {
                            ColorDefinition color = mattie.state_color;
                            if (color == null)
                                newColor = Color.cyan;
                            else
                                newColor = new Color(color.red / 255.0f, color.green / 255.0f, color.blue / 255.0f, 1);
                        }
                    }
                    meshBuffer[bufferIndex].color = newColor;
                }
                bufferIndex++;
            }
        if(blocks[block_x, block_y, block_z] == null)
        {
            GameObject block = Instantiate(defaultMapBlock) as GameObject;
            block.SetActive(true);
            block.transform.parent = this.transform;
            blocks[block_x, block_y, block_z] = block.GetComponent<MeshFilter>();
        }
        MeshFilter mf = blocks[block_x, block_y, block_z];
        if (mf == null)
            Debug.LogError("MF is null");
        if(mf.mesh == null)
            mf.mesh = new Mesh();
        mf.mesh.Clear();
        //mf.mesh.CombineMeshes(meshBuffer);
        return MeshCombineUtility.ColorCombine(mf.mesh, meshBuffer);
        //Debug.Log("Generated a mesh with " + (mf.mesh.triangles.Length / 3) + " tris");

    }

    void UpdateMeshes()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        int count = 0;
        int failed = 0;
        for (int zz = (posZ - rangeZdown); zz < (posZ + rangeZup); zz++)
            for (int yy = ((posY - rangeY) * 16 / blockSize); yy <= ((posY + rangeY) * 16 / blockSize); yy++)
                for (int xx = ((posX - rangeX) * 16 / blockSize); xx <= ((posX + rangeX) * 16 / blockSize); xx++)
                {
                    if (xx < 0 || yy < 0 || zz < 0 || xx >= blocks.GetLength(0) || yy >= blocks.GetLength(1) || zz >= blocks.GetLength(2))
                    {
                        //Debug.Log(xx + ", " + yy + ", " + zz + " is outside of " + blocks.GetLength(0) + ", " + blocks.GetLength(1) + ", " + blocks.GetLength(2));
                        continue;
                    }
                    //Debug.Log("Generating tiles at " + xx + ", " + yy + ", " + zz);
                    if (GenerateTiles(xx, yy, zz))
                        count++;
                    else
                        failed++;
                }
        watch.Stop();
        genStatus.text = (watch.ElapsedMilliseconds / count).ToString() + "ms per embark tile generated. " + failed + " generation failures";
        //Debug.Log("Generating " + count + " meshes took " + watch.ElapsedMilliseconds + " ms");
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
        //watch.Start();
        //FreeAllBlocks();
        if ((connectionState.net_block_list.map_x != map_x) || (connectionState.net_block_list.map_y != map_y))
            ClearMap();
        map_x = connectionState.net_block_list.map_x;
        map_y = connectionState.net_block_list.map_y;
        for (int i = 0; i < connectionState.net_block_list.map_blocks.Count; i++)
        {
            //MapBlock newBlock = getFreeBlock();
            //if (newBlock == null)
            //    break;
            //newBlock.gameObject.SetActive(true);
            //newBlock.SetAllTiles(connectionState.net_block_list.map_blocks[i], connectionState.net_block_list, connectionState.net_tiletype_list);
            //newBlock.Regenerate();
            //newBlock.name = "MapBlock(" + newBlock.coordString + ")";
            CopyTiles(connectionState.net_block_list.map_blocks[i]);
        }
        UpdateMeshes();
        //watch.Stop();
        //Debug.Log("Generating " + connectionState.net_block_list.map_blocks.Count + " Meshes took " + watch.Elapsed.TotalSeconds + " seconds");
    }
    void GetUnitList()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        connectionState.UnitListCall.execute(null, out connectionState.net_unit_list);
        stopwatch.Stop();
        Debug.Log(connectionState.net_unit_list.creature_list.Count + " units gotten, took " + stopwatch.Elapsed.Milliseconds + " ms.");

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

    void GetMapInfo()
    {
        connectionState.MapInfoCall.execute(null, out connectionState.net_map_info);
    }

    void ClearMap()
    {
        foreach(MeshFilter MF in blocks)
        {
            if (MF)
                MF.mesh.Clear();
        }
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                tile.tileType = 0;
                tile.material.mat_index = -1;
                tile.material.mat_type = -1;
            }
        }
    }

    void HideMeshes()
    {
        for(int zz = 0; zz < blocks.GetLength(2); zz++)
        for(int yy = 0; yy < blocks.GetLength(1); yy++)
        for(int xx = 0; xx < blocks.GetLength(0); xx++)
        {
            if (blocks[xx, yy, zz] != null)
            {
                if (zz > connectionState.net_view_info.view_pos_z)
                    blocks[xx, yy, zz].gameObject.layer = 8;
                else
                    blocks[xx, yy, zz].gameObject.layer = 0;
            }
        }

    }

    void ShowCursorInfo()
    {
        int cursX = connectionState.net_view_info.cursor_pos_x;
        int cursY = connectionState.net_view_info.cursor_pos_y;
        int cursZ = connectionState.net_view_info.cursor_pos_z;
        cursorProperties.text = "";
        cursorProperties.text += "Cursor: ";
        cursorProperties.text += cursX + ",";
        cursorProperties.text += cursY + ",";
        cursorProperties.text += cursZ + "\n";
        if(
            cursX >= 0 &&
            cursY >= 0 &&
            cursZ >= 0 &&
            cursX < tiles.GetLength(0) &&
            cursY < tiles.GetLength(1) &&
            cursZ < tiles.GetLength(2) &&
            tiles[cursX, cursY, cursZ] != null)
        {
            cursorProperties.text += "Tiletype: ";
            cursorProperties.text += connectionState.net_tiletype_list.tiletype_list[tiles[cursX, cursY, cursZ].tileType].name + "\n";
            var mat = tiles[cursX, cursY, cursZ].material;
            cursorProperties.text += "Material: ";
            cursorProperties.text += mat.mat_type + ",";
            cursorProperties.text += mat.mat_index + "\n";

            if (materials.ContainsKey(mat))
            {
                cursorProperties.text += "Material Name: ";
                cursorProperties.text += materials[mat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Material\n";

            cursorProperties.text += "\n";

            var basemat = tiles[cursX, cursY, cursZ].base_material;
            cursorProperties.text += "Base Material: ";
            cursorProperties.text += basemat.mat_type + ",";
            cursorProperties.text += basemat.mat_index + "\n";

            if (materials.ContainsKey(basemat))
            {
                cursorProperties.text += "Base Material Name: ";
                cursorProperties.text += materials[basemat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Base Material\n";

            cursorProperties.text += "\n";

            var layermat = tiles[cursX, cursY, cursZ].layer_material;
            cursorProperties.text += "Layer Material: ";
            cursorProperties.text += layermat.mat_type + ",";
            cursorProperties.text += layermat.mat_index + "\n";

            if (materials.ContainsKey(layermat))
            {
                cursorProperties.text += "Layer Material Name: ";
                cursorProperties.text += materials[layermat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Layer Material\n";

            cursorProperties.text += "\n";

            var veinmat = tiles[cursX, cursY, cursZ].vein_material;
            cursorProperties.text += "Vein Material: ";
            cursorProperties.text += veinmat.mat_type + ",";
            cursorProperties.text += veinmat.mat_index + "\n";

            if (materials.ContainsKey(veinmat))
            {
                cursorProperties.text += "Vein Material Name: ";
                cursorProperties.text += materials[veinmat].id + "\n";
            }
            else
                cursorProperties.text += "Unknown Vein Material\n";
        }
    }
}
