using UnityEngine;
using System.Collections;
using isoworldremote;
using DFHack;

[ExecuteInEditMode]
public class LocalEmbarkTile : MonoBehaviour
{

    public MapBlock baseBlock;
    public MapBlock[] blockList;
    public int minZ = 0;
    public int maxZ = 0;
    public BasicShape defaultShape = BasicShape.OPEN;
    public bool NeedsUpdate = false;
    DFCoord2d worldPosition;

    public float embarkTileWidth
    {
        get
        {
            return MapBlock.blockWidth * MapBlock.tileWidth;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (NeedsUpdate)
        {
            FillBlocks(defaultShape);
            NeedsUpdate = false;
        }
    }

    void FillBlocks(BasicShape shape = BasicShape.NONE)
    {
        if (blockList != null)
        {
            foreach (MapBlock layer in blockList)
            {
                if (Application.isEditor)
                    DestroyImmediate(layer.gameObject);
                else
                    Destroy(layer.gameObject);
            }
        }
        blockList = new MapBlock[maxZ - minZ];
        for (int i = minZ; i < maxZ; i++)
        {
            Vector3 layerPosition = transform.position;
            layerPosition.y += ((float)i * MapBlock.getBlockHeight());
            blockList[LevelToIndex(i)] = Instantiate(baseBlock, layerPosition, transform.rotation) as MapBlock;
            blockList[LevelToIndex(i)].transform.parent = transform;
            blockList[LevelToIndex(i)].name = "Layer " + i;
            blockList[LevelToIndex(i)].SetAllTiles(shape);

        }
    }

    int LevelToIndex(int input)
    {
        return input - minZ;
    }

    public void UpdatePosition(isoworldremote.MapReply region)
    {
        transform.position = new Vector3(worldPosition.x * embarkTileWidth, 0, worldPosition.y * embarkTileWidth);
    }

    public void makeTile(isoworldremote.EmbarkTile input, isoworldremote.MapReply region)
    {
        worldPosition.x = input.world_x;
        worldPosition.y = input.world_y;
        minZ = input.world_z;
        maxZ = input.tile_layer.Count + minZ;
        UpdatePosition(region);
        FillBlocks();
        for(int i = 0; i < input.tile_layer.Count; i++)
        {
            blockList[i].SetAllTiles(input.tile_layer[i]);
            blockList[i].Regenerate();
        }
    }
}
