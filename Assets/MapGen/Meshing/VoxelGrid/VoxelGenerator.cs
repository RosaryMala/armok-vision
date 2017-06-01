using UnityEngine;
using System;
using System.Collections.Generic;
using RemoteFortressReader;
using Poly2Tri;
using DFHack;

public class VoxelGenerator
{

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Color> colors = new List<Color>();
    private List<int> triangles = new List<int>();

    bool OpenedDiagonals { get { return false; } }

    const float bottomlessDepth = -1000000;

    public bool bottomless = false;

    public enum CornerType
    {
        Diamond,
        Square,
        Rounded
    }

    CornerType UsedCornerType { get { return GameSettings.Instance.meshing.cornerType; } }

    [Flags]
    enum Directions
    {
        None = 0,
        NorthWest = 1,
        NorthEast = 2,
        SouthWest = 4,
        SouthEast = 8,

        North = NorthWest | NorthEast,
        South = SouthWest | SouthEast,
        East = NorthEast | SouthEast,
        West = NorthWest | SouthWest,

        All = NorthWest | NorthEast | SouthWest | SouthEast
    }

    Directions RotateCW(Directions dir)
    {
        Directions output = Directions.None;
        if ((dir & Directions.NorthWest) == Directions.NorthWest)
            output |= Directions.NorthEast;
        if ((dir & Directions.NorthEast) == Directions.NorthEast)
            output |= Directions.SouthEast;
        if ((dir & Directions.SouthEast) == Directions.SouthEast)
            output |= Directions.SouthWest;
        if ((dir & Directions.SouthWest) == Directions.SouthWest)
            output |= Directions.NorthWest;
        return output;
    }

    Directions Rotate180(Directions dir)
    {
        return RotateCW(RotateCW(dir));
    }

    Directions RotateCCW(Directions dir)
    {
        return RotateCW(RotateCW(RotateCW(dir)));
    }

    public CPUMesh Triangulate(MapDataStore map)
    {
        vertices.Clear();
        uvs.Clear();
        colors.Clear();
        triangles.Clear();

        wallPolygons.Clear();
        floorPolygons.Clear();

        TriangulateCellRows(map);

        ConvertToMesh(wallPolygons.Polygons, GameMap.tileHeight, true);
        ConvertToMesh(floorPolygons.Polygons, GameMap.floorHeight, true);
        ConvertToMesh(wallPolygons.Polygons, 0, false);
        ConvertToMesh(floorPolygons.Polygons, 0, false);

        return new CPUMesh(vertices.ToArray(), null, null, uvs.ToArray(), null, null, null, triangles.ToArray());

    }

    private void TriangulateCellRows(MapDataStore map)
    {
        for (int y = map.MinCoord.y; y < map.MaxCoord.y - 1; y++)
        {
            for (int x = map.MinCoord.x; x < map.MaxCoord.x - 1; x++)
            {
                int z = map.SliceOrigin.z;
                Directions edges = Directions.None;
                if (x == map.MinCoord.x)
                    edges |= Directions.West;
                if (x == map.MaxCoord.x - 2)
                    edges |= Directions.East;
                if (y == map.MinCoord.y)
                    edges |= Directions.North;
                if (y == map.MaxCoord.y - 2)
                    edges |= Directions.South;

                TriangulateCell(
                    map[x, y, z],
                    map[x + 1, y, z],
                    map[x, y + 1, z],
                    map[x + 1, y + 1, z],
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x - 1, y - 1, z))) + new Vector3(GameMap.tileWidth / 2, 0, 0),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x, y - 1, z))) + new Vector3(0, 0, -GameMap.tileWidth / 2),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x - 1, y, z))) + new Vector3(GameMap.tileWidth / 2, 0, 0),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x - 1, y - 1, z))) + new Vector3(0, 0, -GameMap.tileWidth / 2),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x - 1, y - 1, z))) + new Vector3(GameMap.tileWidth / 2, 0, -GameMap.tileWidth / 2),
                    edges);
            }
        }
    }


    public static bool UseBoth(MapDataStore.Tile tile)
    {
        if (tile == null)
            return true; //means it's air/empty
        switch (tile.shape)
        {
            case TiletypeShape.PEBBLES:
            case TiletypeShape.BOULDER:
            case TiletypeShape.SAPLING:
            case TiletypeShape.SHRUB:
                break;
            default:
                return false;
        }
        return true;
    }

    public static bool IsNatural(MapDataStore.Tile tile)
    {
        switch (tile.tiletypeMaterial)
        {
            case TiletypeMaterial.NO_MATERIAL:
            case TiletypeMaterial.AIR:
            case TiletypeMaterial.SOIL:
            case TiletypeMaterial.STONE:
            case TiletypeMaterial.FEATURE:
            case TiletypeMaterial.LAVA_STONE:
            case TiletypeMaterial.MINERAL:
            case TiletypeMaterial.GRASS_LIGHT:
            case TiletypeMaterial.GRASS_DARK:
            case TiletypeMaterial.GRASS_DRY:
            case TiletypeMaterial.GRASS_DEAD:
            case TiletypeMaterial.PLANT:
            case TiletypeMaterial.POOL:
            case TiletypeMaterial.BROOK:
            case TiletypeMaterial.RIVER:
            case TiletypeMaterial.ROOT:
            case TiletypeMaterial.HFS:
                return true;
            default:
                return false;
        }
    }

    public static bool HandleShape(MapDataStore.Tile tile)
    {
        switch (tile.shape)
        {
            case TiletypeShape.NO_SHAPE:
            case TiletypeShape.EMPTY:
            case TiletypeShape.FLOOR:
            case TiletypeShape.WALL:
            case TiletypeShape.BROOK_TOP:
            case TiletypeShape.ENDLESS_PIT:
                return true;
            default:
                return false;
        }

    }

    public static bool Handled(MapDataStore.Tile tile)
    {
        if (UseBoth(tile))
            return true;
        if (tile == null)
            return true; //means it's air/empty
        if (tile.Hidden)
            return false;
        return (HandleShape(tile) && IsNatural(tile));
    }

    public static bool IsFloor(MapDataStore.Tile tile)
    {
        switch (tile.shape)
        {
            case TiletypeShape.FLOOR:
            case TiletypeShape.BOULDER:
            case TiletypeShape.PEBBLES:
            case TiletypeShape.SAPLING:
            case TiletypeShape.SHRUB:
                return true;
            default:
                return false;
        }
    }

    private void TriangulateCell(
        MapDataStore.Tile northWest,
        MapDataStore.Tile northEast,
        MapDataStore.Tile southWest, 
        MapDataStore.Tile southEast,
        Vector3 northPoint, 
        Vector3 eastPoint,
        Vector3 southPoint,
        Vector3 westPoint,
        Vector3 centerPoint,
        Directions edges)
    {
        var corner = UsedCornerType;
        if (!Handled(northWest)
            || !Handled(northEast)
            || !Handled(southWest)
            || !Handled(southEast)
            )
            corner = CornerType.Square;

        Directions walls = Directions.None;
        if (northWest != null && northWest.shape == TiletypeShape.WALL && Handled(northWest))
        {
            walls |= Directions.NorthWest;
        }
        if (northEast != null && northEast.shape == TiletypeShape.WALL && Handled(northEast))
        {
            walls |= Directions.NorthEast;
        }
        if (southWest != null && southWest.shape == TiletypeShape.WALL && Handled(southWest))
        {
            walls |= Directions.SouthWest;
        }
        if (southEast != null && southEast.shape == TiletypeShape.WALL && Handled(southEast))
        {
            walls |= Directions.SouthEast;
        }

        Directions wallFloors = walls;
        if (northWest != null && IsFloor(northWest) && Handled(northWest))
        {
            wallFloors |= Directions.NorthWest;
        }
        if (northEast != null && IsFloor(northEast) && Handled(northEast))
        {
            wallFloors |= Directions.NorthEast;
        }
        if (southWest != null && IsFloor(southWest) && Handled(southWest))
        {
            wallFloors |= Directions.SouthWest;
        }
        if (southEast != null && IsFloor(southEast) && Handled(southEast))
        {
            wallFloors |= Directions.SouthEast;
        }

        switch (wallFloors)
        {
            case Directions.None:
                return;
            case Directions.NorthWest:
            case Directions.North:
            case Directions.North | Directions.SouthWest:
            case Directions.NorthWest | Directions.SouthEast:
            case Directions.All:
                AddRotatedCell(
                    northPoint,
                    eastPoint,
                    southPoint,
                    westPoint,
                    centerPoint,
                    wallFloors, edges, walls, corner);
                break;
            case Directions.NorthEast:
            case Directions.East:
            case Directions.North | Directions.SouthEast:
            case Directions.NorthEast | Directions.SouthWest:
                AddRotatedCell(
                    eastPoint,
                    southPoint,
                    westPoint,
                    northPoint,
                    centerPoint,
                    RotateCCW(wallFloors), RotateCCW(edges), RotateCCW(walls), corner);
                break;
            case Directions.SouthEast:
            case Directions.South:
            case Directions.NorthEast | Directions.South:
                AddRotatedCell(
                    southPoint,
                    westPoint,
                    northPoint,
                    eastPoint,
                    centerPoint,
                    Rotate180(wallFloors), Rotate180(edges), Rotate180(walls), corner);
                break;
            case Directions.SouthWest:
            case Directions.West:
            case Directions.West | Directions.SouthEast:
                AddRotatedCell(
                    westPoint,
                    northPoint,
                    eastPoint,
                    southPoint,
                    centerPoint,
                    RotateCW(wallFloors), RotateCW(edges), RotateCW(walls), corner);
                break;
        }
    }

    private void AddRotatedCell(
        Vector3 north,
        Vector3 east,
        Vector3 south,
        Vector3 west,
        Vector3 center,
        Directions neighbors,
        Directions edges,
        Directions walls,
        CornerType corner)
    {
        switch (neighbors)
        {
            case Directions.NorthWest:
                AddOuterCornerCell(west, north, center, corner, edges, walls);
                break;
            case Directions.North:
                AddStraightCell(west, north, east, center, corner, edges, walls);
                break;
            case Directions.North | Directions.West:
                AddInnerCornerCell(north, east, south, west, center, corner, edges, walls);
                break;
            case Directions.NorthWest | Directions.SouthEast:
                AddSaddleCell(north, east, south, west, center, corner, edges, walls);
                break;
            case Directions.All:
                AddCenterCell(north, east, south, west, center, corner, edges, walls);
                break;
            default:
                break;
        }
    }

    //The triangulator doesn't like points in the same place. This makes things just far enough apart that they work.
    Vector3 nudge(Vector3 a, Vector3 b, Vector3 center)
    {
        Vector3 average = (a + b) / 2;
        Vector3 direction = center - average;
        return center - (direction * 0.001f);
    }

    enum WallType
    {
        None,
        Floor,
        Wall,
        Both
    }

    void AddOuterCornerCell(Vector3 west, Vector3 north, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        if ((edges & Directions.NorthWest) != Directions.NorthWest)
        {
            if ((walls & Directions.NorthWest) == Directions.NorthWest)
                AddCorner(wallPolygons, west, north, center, corner, WallType.Both);
            else
                AddCorner(floorPolygons, west, north, center, corner, WallType.Floor);

        }
    }

    void AddStraightCell(Vector3 west, Vector3 north, Vector3 east, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        var eastPoint = nudge(center, center, east);
        var westPoint = nudge(center, center, west);
        var northPoint = nudge(center, center, north);
        switch (edges & Directions.North)
        {
            case Directions.North:
                break;
            case Directions.NorthEast:
                switch (walls)
                {
                    case Directions.NorthWest:
                        if (corner == CornerType.Square)
                            AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        else
                        {
                            AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, northPoint, westPoint, center, corner, WallType.None);
                            AddCorner(floorPolygons, westPoint, northPoint, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.North:
                        AddCorner(wallPolygons, west, north, center, CornerType.Square, WallType.Both);
                        break;
                    default:
                        AddCorner(floorPolygons, west, north, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.NorthWest:
                switch (walls)
                {
                    case Directions.NorthEast:
                        if (corner == CornerType.Square)
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Both);
                        else
                        {
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Both);
                            AddCorner(floorPolygons, eastPoint, northPoint, center, corner, WallType.Both);
                            AddCorner(floorPolygons, northPoint, eastPoint, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.North:
                        AddCorner(wallPolygons, north, east, center, CornerType.Square, WallType.Both);
                        break;
                    default:
                        AddCorner(floorPolygons, north, east, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            default:
                switch (walls)
                {
                    case Directions.North:
                        AddStraight(wallPolygons, west, east, WallType.Both);
                        break;
                    case Directions.NorthWest:
                        //Todo: convert these to simple segments if the corners are square
                        AddStraight(floorPolygons, westPoint, east, WallType.Floor);
                        AddCorner(floorPolygons, north, westPoint, center, corner, WallType.None);
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        break;
                    case Directions.NorthEast:
                        AddStraight(floorPolygons, west, eastPoint, WallType.Floor);
                        AddCorner(floorPolygons, eastPoint, north, center, corner, WallType.None);
                        AddCorner(wallPolygons, north, east, center, corner, WallType.Wall);
                        break;
                    default:
                        AddStraight(floorPolygons, west, east, WallType.Floor);
                        break;
                }
                break;
        }

    }

    void AddInnerCornerCell(Vector3 north, Vector3 east, Vector3 south, Vector3 west, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        var eastPoint = nudge(center, center, east);
        var westPoint = nudge(center, center, west);
        var northPoint = nudge(center, center, north);
        var southPoint = nudge(center, center, south);
        switch (edges)
        {
            case Directions.East:
                AddStraightCell(south, west, north, center, (walls == Directions.North) ? CornerType.Square : corner, RotateCW(edges), RotateCW(walls) & Directions.North);
                break;
            case Directions.South:
                AddStraightCell(west, north, east, center, (walls == Directions.West) ? CornerType.Square : corner, edges, walls & Directions.North);
                break;
            case Directions.North:
                switch (walls)
                {
                    case Directions.SouthWest:
                    case Directions.SouthWest | Directions.NorthEast:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddCorner(floorPolygons, south, eastPoint, center, corner, WallType.Floor);
                            AddStraight(floorPolygons, eastPoint, westPoint, WallType.Floor);
                            AddCorner(floorPolygons, westPoint, south, center, corner, WallType.None);
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Wall);
                        }
                        break;
                    case Directions.West:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddCorner(wallPolygons, south, west, center, CornerType.Square, WallType.Both);
                            AddCorner(floorPolygons, eastPoint, southPoint, center, CornerType.Square, WallType.Floor);
                            AddCorner(floorPolygons, southPoint, eastPoint, center, corner, WallType.Floor);
                        }
                        break;
                    case Directions.West | Directions.North:
                        AddCorner(wallPolygons, south, eastPoint, center, corner, WallType.Both);
                        AddStraight(wallPolygons, eastPoint, west, WallType.Both);
                        break;
                    case Directions.NorthWest:
                    case Directions.NorthEast:
                    case Directions.North:
                    default:
                        AddCorner(floorPolygons, south, eastPoint, center, corner, WallType.Floor);
                        AddStraight(floorPolygons, eastPoint, west, WallType.Floor);
                        break;
                }
                break;
            case Directions.West:
                switch (walls)
                {
                    case Directions.NorthEast:
                    case Directions.NorthEast | Directions.SouthWest:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddStraight(floorPolygons, northPoint, southPoint, WallType.Floor);
                            AddCorner(floorPolygons, southPoint, eastPoint, center, corner, WallType.Floor);
                            AddCorner(floorPolygons, eastPoint, northPoint, center, corner, WallType.None);
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Wall);
                        }
                        break;
                    case Directions.North:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddCorner(floorPolygons, southPoint, eastPoint, center, corner, WallType.Floor);
                            AddCorner(floorPolygons, eastPoint, southPoint, center, CornerType.Square, WallType.None);
                            AddCorner(wallPolygons, north, east, center, CornerType.Square, WallType.Both);
                        }
                        break;
                    case Directions.North | Directions.West:
                        AddStraight(wallPolygons, north, southPoint, WallType.Both);
                        AddCorner(wallPolygons, southPoint, east, center, corner, WallType.Both);
                        break;
                    case Directions.NorthWest:
                    case Directions.SouthWest:
                    case Directions.West:
                    default:
                        AddStraight(floorPolygons, north, southPoint, WallType.Floor);
                        AddCorner(floorPolygons, southPoint, east, center, corner, WallType.Floor);
                        break;
                }
                break;
            case Directions.North | Directions.West:
                switch (walls)
                {
                    case Directions.North | Directions.West:
                        if (corner != CornerType.Square)
                        {
                            AddCorner(wallPolygons, southPoint, eastPoint, center, corner, WallType.Both);
                            AddCorner(wallPolygons, eastPoint, southPoint, center, CornerType.Square, WallType.Both);
                        }
                        break;
                    case Directions.NorthWest:
                    case Directions.NorthEast:
                    case Directions.SouthWest:
                    case Directions.North:
                    case Directions.West:
                    case Directions.NorthEast | Directions.SouthWest:
                    default:
                        if (corner != CornerType.Square)
                        {
                            AddCorner(floorPolygons, southPoint, eastPoint, center, corner, WallType.Floor);
                            AddCorner(floorPolygons, eastPoint, southPoint, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                }
                break;
            case Directions.North | Directions.East:
                switch (walls)
                {
                    case Directions.West:
                    case Directions.North | Directions.West:
                        AddCorner(wallPolygons, south, west, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.SouthWest:
                    case Directions.NorthEast | Directions.SouthWest:
                        if (corner == CornerType.Square)
                            AddCorner(wallPolygons, south, west, center, CornerType.Square, WallType.Both);
                        else
                        {
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, westPoint, southPoint, center, corner, WallType.None);
                            AddCorner(floorPolygons, southPoint, westPoint, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.NorthWest:
                    case Directions.NorthEast:
                    case Directions.North:
                    default:
                        AddCorner(floorPolygons, south, west, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.East | Directions.South:
                switch (walls)
                {
                    case Directions.North | Directions.West:
                    case Directions.North:
                    case Directions.West:
                        AddCorner(wallPolygons, west, north, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.NorthWest:
                        if (corner == CornerType.Square)
                            AddCorner(wallPolygons, west, north, center, CornerType.Square, WallType.Both);
                        else
                        {
                            AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, northPoint, westPoint, center, corner, WallType.None);
                            AddCorner(floorPolygons, westPoint, northPoint, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.NorthEast:
                    case Directions.SouthWest:
                    case Directions.NorthEast | Directions.SouthWest:
                    default:
                        AddCorner(floorPolygons, west, north, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.West | Directions.South:
                switch (walls)
                {
                    case Directions.North | Directions.West:
                    case Directions.North:
                        AddCorner(wallPolygons, north, east, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.NorthEast:
                    case Directions.NorthEast | Directions.SouthWest:
                        if (corner == CornerType.Square)
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Both);
                        else
                        {
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Both);
                            AddCorner(floorPolygons, eastPoint, northPoint, center, corner, WallType.None);
                            AddCorner(floorPolygons, northPoint, eastPoint, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.NorthWest:
                    case Directions.SouthWest:
                    case Directions.West:
                    default:
                        AddCorner(floorPolygons, north, east, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            default:
                {
                    switch (walls)
                    {
                        case Directions.NorthWest:
                            AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, north, west, center, corner, WallType.None);
                            AddCorner(floorPolygons, south, east, center, corner, WallType.Floor);
                            break;
                        case Directions.NorthEast:
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, eastPoint, north, center, corner, WallType.None);
                            AddCorner(floorPolygons, south, eastPoint, center, corner, WallType.Floor);
                            break;
                        case Directions.SouthWest:
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, west, southPoint, center, corner, WallType.None);
                            AddCorner(floorPolygons, southPoint, east, center, corner, WallType.Floor);
                            break;
                        case Directions.North:
                            AddStraight(wallPolygons, west, east, WallType.Wall);
                            AddStraight(floorPolygons, eastPoint, west, WallType.None);
                            AddCorner(floorPolygons, south, eastPoint, center, corner, WallType.Floor);
                            break;
                        case Directions.West:
                            AddStraight(wallPolygons, south, north, WallType.Wall);
                            AddStraight(floorPolygons, north, southPoint, WallType.None);
                            AddCorner(floorPolygons, southPoint, east, center, corner, WallType.Floor);
                            break;
                        case Directions.NorthEast | Directions.SouthWest:
                            AddCorner(wallPolygons, north, east, center, corner, WallType.Wall);
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Wall);
                            AddCorner(floorPolygons, west, southPoint, center, corner, WallType.None);
                            AddCorner(floorPolygons, southPoint, eastPoint, center, corner, WallType.Floor);
                            AddCorner(floorPolygons, eastPoint, north, center, corner, WallType.None);
                            break;
                        case Directions.North | Directions.West:
                            AddCorner(wallPolygons, south, east, center, corner, WallType.Both);
                            break;
                        default:
                            AddCorner(floorPolygons, south, east, center, corner, WallType.Floor);
                            break;
                    }
                }
                break;
        }
    }

    void AddSaddleCell(Vector3 north, Vector3 east, Vector3 south, Vector3 west, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        if (OpenedDiagonals)
            corner = CornerType.Diamond;
        switch (edges)
        {
            case Directions.East | Directions.North:
            case Directions.West | Directions.South:
                break;
            case Directions.North:
            case Directions.West:
            case Directions.North | Directions.West:
                switch (walls)
                {
                    case Directions.NorthWest | Directions.SouthEast:
                    case Directions.SouthEast:
                        AddCorner(wallPolygons, east, south, center, corner, WallType.Both);
                        break;
                    default:
                        AddCorner(floorPolygons, east, south, center, corner, WallType.Floor);
                        break;
                }
                break;
            case Directions.East:
            case Directions.South:
            case Directions.East | Directions.South:
                switch (walls)
                {
                    case Directions.NorthWest | Directions.SouthEast:
                    case Directions.NorthWest:
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Both);
                        break;
                    default:
                        AddCorner(floorPolygons, west, north, center, corner, WallType.Floor);
                        break;
                }
                break;
            default:
                switch (walls)
                {
                    case Directions.NorthWest | Directions.SouthEast:
                        AddCorner(wallPolygons, east, south, center, corner, WallType.Both);
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Both);
                        break;
                    case Directions.NorthWest:
                        AddCorner(floorPolygons, east, south, center, corner, WallType.Floor);
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Both);
                        break;
                    case Directions.SouthEast:
                        AddCorner(wallPolygons, east, south, center, corner, WallType.Both);
                        AddCorner(floorPolygons, west, north, center, corner, WallType.Floor);
                        break;
                    default:
                        AddCorner(floorPolygons, east, south, center, corner, WallType.Floor);
                        AddCorner(floorPolygons, west, north, center, corner, WallType.Floor);
                        break;
                }
                break;
        }
    }

    void AddCenterCell(Vector3 north, Vector3 east, Vector3 south, Vector3 west, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        switch (walls)
        {
            case Directions.None:
            case Directions.NorthWest:
            case Directions.North:
            case Directions.North | Directions.SouthWest:
            case Directions.NorthWest | Directions.SouthEast:
            case Directions.All:
                AddRotatedCenterCell(north, east, south, west, center, corner, edges, walls);
                break;
            case Directions.NorthEast:
            case Directions.East:
            case Directions.North | Directions.SouthEast:
            case Directions.NorthEast | Directions.SouthWest:
                AddRotatedCenterCell(east, south, west, north, center, corner, RotateCCW(edges), RotateCCW(walls));
                break;
            case Directions.SouthEast:
            case Directions.South:
            case Directions.NorthEast | Directions.South:
                AddRotatedCenterCell(south, west, north, east, center, corner, Rotate180(edges), Rotate180(walls));
                break;
            case Directions.SouthWest:
            case Directions.West:
            case Directions.West | Directions.SouthEast:
                AddRotatedCenterCell(west, north, east, south, center, corner, RotateCW(edges), RotateCW(walls));
                break;
        }
    }

    void AddRotatedCenterCell(Vector3 north, Vector3 east, Vector3 south, Vector3 west, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        var eastPoint = nudge(center, center, east);
        var westPoint = nudge(center, center, west);
        var northPoint = nudge(center, center, north);
        var southPoint = nudge(center, center, south);

        switch (edges)
        {
            case Directions.North:
                switch (walls)
                {
                    case Directions.West | Directions.North:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(wallPolygons, south, west, center, corner, WallType.Both);
                            AddCorner(floorPolygons, east, south, center, corner, WallType.Floor);
                        }
                        else
                        {
                            AddCorner(wallPolygons, south, eastPoint, center, corner, WallType.Wall);
                            AddStraight(wallPolygons, eastPoint, west, WallType.Both);
                            AddCorner(floorPolygons, east, south, center, corner, WallType.None);
                        }
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(wallPolygons, east, south, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, south, eastPoint, center, corner, WallType.None);
                        AddStraight(floorPolygons, eastPoint, west, WallType.Floor);
                        break;
                    case Directions.All:
                        AddStraight(wallPolygons, east, west, WallType.Both);
                        break;
                    case Directions.North:
                    case Directions.NorthWest:
                    default:
                        AddStraight(floorPolygons, east, west, WallType.Floor);
                        break;
                }
                break;
            case Directions.South:
                switch (walls)
                {
                    case Directions.NorthWest:
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, north, westPoint, center, corner, WallType.None);
                        AddStraight(floorPolygons, westPoint, east, WallType.Floor);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, north, westPoint, center, corner, WallType.None);
                        AddStraight(floorPolygons, westPoint, east, WallType.Floor);
                        break;
                    case Directions.West | Directions.North:
                    case Directions.North:
                    case Directions.All:
                        AddStraight(wallPolygons, west, east, WallType.Both);
                        break;
                    default:
                        AddStraight(floorPolygons, west, east, WallType.Floor);
                        break;
                }
                break;
            case Directions.East:
                switch (walls)
                {
                    case Directions.NorthWest:
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddStraight(floorPolygons, south, northPoint, WallType.Floor);
                        AddCorner(floorPolygons, northPoint, west, center, corner, WallType.None);
                        break;
                    case Directions.North:
                        AddCorner(wallPolygons, west, north, center, CornerType.Square, WallType.Both);
                        AddCorner(floorPolygons, south, west, center, CornerType.Square, WallType.Floor);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddStraight(floorPolygons, south, northPoint, WallType.Floor);
                        AddCorner(floorPolygons, northPoint, west, center, corner, WallType.None);
                        break;
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddStraight(wallPolygons, south, north, WallType.Both);
                        break;
                    default:
                        AddStraight(floorPolygons, south, north, WallType.Floor);
                        break;
                }
                break;
            case Directions.West:
                switch (walls)
                {
                    case Directions.North:
                        AddCorner(wallPolygons, north, east, center, CornerType.Square, WallType.Both);
                        AddCorner(floorPolygons, east, south, center, CornerType.Square, WallType.Floor);
                        break;
                    case Directions.West | Directions.North:
                        AddStraight(wallPolygons, north, southPoint, WallType.Both);
                        AddCorner(wallPolygons, southPoint, east, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, east, south, center, corner, WallType.None);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;
                        AddCorner(wallPolygons, east, south, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, south, east, center, corner, WallType.None);
                        AddStraight(floorPolygons, north, south, WallType.Floor);
                        break;
                    case Directions.All:
                        AddStraight(wallPolygons, north, south, WallType.Both);
                        break;
                    case Directions.NorthWest:
                    default:
                        AddStraight(floorPolygons, north, south, WallType.Floor);
                        break;
                }
                break;
            case Directions.West | Directions.North:
                switch (walls)
                {
                    case Directions.West | Directions.North:
                        AddCorner(wallPolygons, eastPoint, southPoint, center, CornerType.Square, WallType.Both);
                        AddCorner(wallPolygons, southPoint, eastPoint, center, corner, WallType.None);
                        AddCorner(floorPolygons, east, south, center, corner, WallType.Floor);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(wallPolygons, east, south, center, corner, WallType.Wall);
                        }
                        else
                        {
                            AddCorner(floorPolygons, eastPoint, southPoint, center, CornerType.Square, WallType.Floor);
                            AddCorner(floorPolygons, southPoint, eastPoint, center, corner, WallType.None);
                            AddCorner(wallPolygons, east, south, center, corner, WallType.Wall);
                        }
                        break;
                    case Directions.All:
                        AddCorner(wallPolygons, east, south, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.North:
                    case Directions.NorthWest:
                    default:
                        AddCorner(floorPolygons, east, south, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.North | Directions.East:
                switch (walls)
                {
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddCorner(wallPolygons, south, west, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.North:
                    case Directions.NorthWest:
                    case Directions.NorthWest | Directions.SouthEast:
                    default:
                        AddCorner(floorPolygons, south, west, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.East | Directions.South:
                switch (walls)
                {
                    case Directions.NorthWest | Directions.SouthEast:
                    case Directions.NorthWest:
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, northPoint, westPoint, center, corner, WallType.None);
                        AddCorner(floorPolygons, westPoint, northPoint, center, CornerType.Square, WallType.Floor);
                        break;
                    case Directions.North:
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddCorner(wallPolygons, west, north, center, CornerType.Square, WallType.Both);
                        break;
                    default:
                        AddCorner(floorPolygons, west, north, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.South | Directions.West:
                switch (walls)
                {
                    case Directions.North:
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddCorner(wallPolygons, north, east, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                    case Directions.NorthWest:
                    default:
                        AddCorner(floorPolygons, north, east, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            default:
                switch (walls)
                {
                    case Directions.NorthWest:
                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, north, west, center, corner, WallType.None);
                        break;
                    case Directions.North:
                        AddStraight(wallPolygons, west, east, WallType.Wall);
                        AddStraight(floorPolygons, east, west, WallType.None);
                        break;
                    case Directions.West | Directions.North:
                        AddCorner(wallPolygons, south, east, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, east, south, center, corner, WallType.None);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(wallPolygons, west, north, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, north, west, center, corner, WallType.None);

                        AddCorner(wallPolygons, east, south, center, corner, WallType.Wall);
                        AddCorner(floorPolygons, south, east, center, corner, WallType.None);
                        break;
                    default:
                        break;
                }
                break;
        }
    }

    private void AddCorner(ComplexPoly poly, Vector3 start, Vector3 end, Vector3 center, CornerType type, WallType wallType = WallType.None)
    {
        center = nudge(start, end, center);
        switch (type)
        {
            case CornerType.Diamond:
                break;
            case CornerType.Square:
                poly.AddLineSegment(start, center, end);
                AddWallMesh(wallType, start, center, end);
                return;
            case CornerType.Rounded:
                poly.AddLineSegment(
                    start,
                    (start + center) / 2,
                    (end + center) / 2,
                    end);
                AddWallMesh(wallType,
                    start,
                    (start + center) / 2,
                    (end + center) / 2,
                    end);
                return;
            default:
                break;
        }
        poly.AddLineSegment(start, end);
        AddWallMesh(wallType, start, end);
    }

    private void AddStraight(ComplexPoly poly, Vector3 a, Vector3 b, WallType wallType = WallType.None)
    {
        poly.AddLineSegment(a, b);
        AddWallMesh(wallType, a, b);
    }

    ComplexPoly wallPolygons = new ComplexPoly();
    ComplexPoly floorPolygons = new ComplexPoly();


    void ConvertToMesh(PolygonSet polySet, float height, bool up)
    {
        Dictionary<TriangulationPoint, int> pointIndices = new Dictionary<TriangulationPoint, int>();
        P2T.Triangulate(polySet);
        foreach (var polygon in polySet.Polygons)
        {
            foreach (var triangle in polygon.Triangles)
            {
                for (int i = 0; i < 3; i++)
                {
                    var point = triangle.Points[up ? 2 - i : i];
                    int index;
                    if (!pointIndices.ContainsKey(point))
                    {
                        index = vertices.Count;
                        pointIndices[point] = index;
                        vertices.Add(new Vector3(point.Xf, height, point.Yf));
                        uvs.Add(new Vector2(point.Xf / GameMap.tileWidth + 0.5f, point.Yf / GameMap.tileWidth + 0.5f));
                    }
                    else
                        index = pointIndices[point];
                    triangles.Add(index);
                }
            }
        }
    }

    void AddWallMesh(WallType wallType, params Vector3[] points)
    {
        switch (wallType)
        {
            case WallType.Floor:
                AddWallMesh(GameMap.floorHeight, bottomless ? bottomlessDepth : 0, points);
                break;
            case WallType.Wall:
                AddWallMesh(GameMap.tileHeight, GameMap.floorHeight, points);
                break;
            case WallType.Both:
                AddWallMesh(GameMap.tileHeight, bottomless ? bottomlessDepth : 0, points);
                break;
            default:
                break;
        }
    }

    void AddWallMesh(float top, float bottom, params Vector3[] points)
    {
        float uvTop = top / GameMap.tileHeight;
        float uvBottom = bottom / GameMap.tileHeight;

        float length = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            length += (points[i] - points[i + 1]).magnitude;
        }

        int vertIndex = vertices.Count;
        float runningLength = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            vertices.Add(new Vector3(points[i].x, top, points[i].z));
            vertices.Add(new Vector3(points[i].x, bottom, points[i].z));
            vertices.Add(new Vector3(points[i + 1].x, top, points[i + 1].z));
            vertices.Add(new Vector3(points[i + 1].x, bottom, points[i + 1].z));

            float thisLength = (points[i] - points[i + 1]).magnitude;

            uvs.Add(new Vector2(Mathf.Lerp(-0.5f, 0.5f, (runningLength / length)), uvTop));
            uvs.Add(new Vector2(Mathf.Lerp(-0.5f, 0.5f, (runningLength / length)), uvBottom));
            uvs.Add(new Vector2(Mathf.Lerp(-0.5f, 0.5f, ((runningLength + thisLength) / length)), uvTop));
            uvs.Add(new Vector2(Mathf.Lerp(-0.5f, 0.5f, ((runningLength + thisLength) / length)), uvBottom));

            runningLength += thisLength;

            triangles.Add(vertIndex + (i * 4) + 0);
            triangles.Add(vertIndex + (i * 4) + 2);
            triangles.Add(vertIndex + (i * 4) + 1);
            triangles.Add(vertIndex + (i * 4) + 1);
            triangles.Add(vertIndex + (i * 4) + 2);
            triangles.Add(vertIndex + (i * 4) + 3);
        }
    }
}
