using DFHack;
using RemoteFortressReader;
using System.Collections.Generic;

public class MapTile
{
    MapTile[, ,] _container;
    public MapTile[, ,] container
    {
        set
        {
            _container = value;
        }
    }
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

    public TiletypeShape shape { get { return tiletypeTokenList[tileType].shape; } }
    public TiletypeMaterial tiletypeMaterial { get { return tiletypeTokenList[tileType].material; } }
    public TiletypeSpecial special { get { return tiletypeTokenList[tileType].special; } }
    public TiletypeVariant variant { get { return tiletypeTokenList[tileType].variant; } }
    public string direction { get { return tiletypeTokenList[tileType].direction; } }

    public MapTile north
    {
        get
        {
            if (position.y > 0)
                return _container[position.x, position.y - 1, position.z];
            else return null;
        }
    }
    public MapTile south
    {
        get
        {
            if (position.y < _container.GetLength(1) - 1)
                return _container[position.x, position.y + 1, position.z];
            else return null;
        }
    }
    public MapTile east
    {
        get
        {
            if (position.x < _container.GetLength(0) - 1)
                return _container[position.x + 1, position.y, position.z];
            else return null;
        }
    }
    public MapTile west
    {
        get
        {
            if (position.x > 0)
                return _container[position.x - 1, position.y, position.z];
            else return null;
        }
    }
    public MapTile up
    {
        get
        {
            if (position.z < _container.GetLength(2) - 1)
                return _container[position.x, position.y, position.z + 1];
            else return null;
        }
    }
    public MapTile down
    {
        get
        {
            if (position.z > 0)
                return _container[position.x, position.y, position.z - 1];
            else return null;
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
