using UnityEngine;
using System.Collections;
using DFHack;

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
    public int tileType;
    public MatPairStruct material;
    public MatPairStruct base_material;
    public MatPairStruct layer_material;
    public MatPairStruct vein_material;
}
