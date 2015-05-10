using DFHack;
using RemoteFortressReader;
using System.Collections.Generic;

public class MapTile
{
    DFCoord _position;
    public DFCoord position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }
    public static List<Tiletype> tiletypeTokenList { private get; set; }
    public int tileType;
    public MatPairStruct material;
    public MatPairStruct base_material;
    public MatPairStruct layer_material;
    public MatPairStruct vein_material;

    public int[] liquid = new int[2];

    public TiletypeShape shape { get { return tiletypeTokenList[tileType].shape; } }
    public TiletypeMaterial tiletypeMaterial { get { return tiletypeTokenList[tileType].material; } }
    public TiletypeSpecial special { get { return tiletypeTokenList[tileType].special; } }
    public TiletypeVariant variant { get { return tiletypeTokenList[tileType].variant; } }
    public string direction { get { return tiletypeTokenList[tileType].direction; } }

    private MapTile() {}

    public MapTile(DFCoord coord) {
        _position = coord;
    }

    public void Reset() {
        tileType = default(int);
        material = default(MatPairStruct);
        base_material = default(MatPairStruct);
        layer_material = default(MatPairStruct);
        vein_material = default(MatPairStruct);
        liquid[0] = default(int);
        liquid[1] = default(int);
    }

    public MapTile north
    {
        get
        {
            return MapDataStore.GetTile(position.x, position.y - 1, position.z);
        }
    }
    public MapTile south
    {
        get
        {
            return MapDataStore.GetTile(position.x, position.y + 1, position.z);
        }
    }
    public MapTile east
    {
        get
        {
            return MapDataStore.GetTile(position.x + 1, position.y, position.z);
        }
    }
    public MapTile west
    {
        get
        {
            return MapDataStore.GetTile(position.x - 1, position.y, position.z);
        }
    }
    public MapTile up
    {
        get
        {
            return MapDataStore.GetTile(position.x, position.y, position.z + 1);
        }
    }
    public MapTile down
    {
        get
        {
            return MapDataStore.GetTile(position.x, position.y, position.z - 1);
        }
    }

    public bool isWall
    {
        get
        {
            switch (shape)
            {
                case TiletypeShape.WALL:
                case TiletypeShape.FORTIFICATION:
                case TiletypeShape.BROOK_BED:
                case TiletypeShape.TREE_SHAPE:
                    return true;
                default:
                    return false;
            }
        }
    }
    public bool isFloor
    {
        get
        {
            switch (shape)
            {
                case TiletypeShape.RAMP:
                case TiletypeShape.FLOOR:
                case TiletypeShape.BOULDER:
                case TiletypeShape.PEBBLES:
                case TiletypeShape.BROOK_TOP:
                case TiletypeShape.SAPLING:
                case TiletypeShape.SHRUB:
                case TiletypeShape.BRANCH:
                case TiletypeShape.TRUNK_BRANCH:
                    return true;
                default:
                    return false;
            }
        }
    }
}
