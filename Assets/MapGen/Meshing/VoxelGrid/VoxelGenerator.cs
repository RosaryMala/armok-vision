using UnityEngine;
using System;
using System.Collections.Generic;
using RemoteFortressReader;
using DFHack;

public class VoxelGenerator
{
    MapDataStore map;
    public VoxelGenerator(MapDataStore map)
    {
        this.map = map;
    }

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

    public CPUMesh TerrainMesh
    {
        get
        {
            Triangulate();
            return new CPUMesh(vertices.ToArray(), null, null, uvs.ToArray(), null, null, null, triangles.ToArray());
        }
    }

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

    private void Triangulate()
    {
        vertices.Clear();
        uvs.Clear();
        colors.Clear();
        triangles.Clear();

        TriangulateCellRows();
    }

    private void TriangulateCellRows()
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
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x - 1, y - 1, z))),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x, y - 1, z))),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x, y, z))),
                    GameMap.DFtoUnityCoord(map.WorldToLocalSpace(new DFCoord(x - 1, y, z))),
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
        Vector3 northWestPoint,
        Vector3 northEastPoint,
        Vector3 southEastPoint,
        Vector3 southWestPoint,
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
                    northWestPoint,
                    northEastPoint,
                    southEastPoint,
                    southWestPoint,
                    wallFloors, edges, walls, corner);
                break;
            case Directions.NorthEast:
            case Directions.East:
            case Directions.North | Directions.SouthEast:
            case Directions.NorthEast | Directions.SouthWest:
                AddRotatedCell(
                    northEastPoint,
                    southEastPoint,
                    southWestPoint,
                    northWestPoint,
                  RotateCCW(wallFloors), RotateCCW(edges), RotateCCW(walls), corner);
                break;
            case Directions.SouthEast:
            case Directions.South:
            case Directions.NorthEast | Directions.South:
                AddRotatedCell(
                    southEastPoint,
                    southWestPoint,
                    northWestPoint,
                    northEastPoint,
                    Rotate180(wallFloors), Rotate180(edges), Rotate180(walls), corner);
                break;
            case Directions.SouthWest:
            case Directions.West:
            case Directions.West | Directions.SouthEast:
                AddRotatedCell(
                    southWestPoint,
                    northWestPoint,
                    northEastPoint,
                    southEastPoint,
                    RotateCW(wallFloors), RotateCW(edges), RotateCW(walls), corner);
                break;
        }
    }

    private void AddRotatedCell(
        Vector3 northWest,
        Vector3 northEast,
        Vector3 southEast,
        Vector3 southWest,
        Directions neighbors,
        Directions edges,
        Directions walls,
        CornerType corner)
    {
        Vector3 north = (northWest + northEast) / 2;
        Vector3 east = (northEast + southEast) / 2;
        Vector3 south = (southEast + southWest) / 2;
        Vector3 west = (southWest + northWest) / 2;
        Vector3 center = (north + south) / 2;
        switch (neighbors)
        {
            case Directions.NorthWest:
                AddOuterCornerCell(west, northWest, north, center, corner, edges, walls);
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

    enum WallType
    {
        None,
        Floor,
        Wall,
        Both
    }

    void AddOuterCornerCell(Vector3 west, Vector3 northWest, Vector3 north, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        if ((edges & Directions.NorthWest) != Directions.NorthWest)
        {
            if ((walls & Directions.NorthWest) == Directions.NorthWest)
            {
                AddHorizontalPoly(WallType.Both, northWest, GetCornerPoints(west, north, center, corner));
                AddWallMesh(WallType.Both, GetCornerPoints(west, north, center, corner));
            }
            else
            {
                AddHorizontalPoly(WallType.Floor, northWest, GetCornerPoints(west, north, center, corner));
                AddWallMesh(WallType.Floor, GetCornerPoints(west, north, center, corner));
            }
        }
    }

    void AddStraightCell(Vector3 west, Vector3 north, Vector3 east, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
        switch (edges & Directions.North)
        {
            case Directions.North:
                break;
            case Directions.NorthEast:
                switch (walls)
                {
                    case Directions.NorthWest:
                        if (corner == CornerType.Square)
                            AddCorner(west, north, center, corner, WallType.Wall);
                        else
                        {
                            AddCorner(west, north, center, corner, WallType.Wall);
                            AddCorner(north, west, center, corner, WallType.None);
                            AddCorner(west, north, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.North:
                        AddCorner(west, north, center, CornerType.Square, WallType.Both);
                        break;
                    default:
                        AddCorner(west, north, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.NorthWest:
                switch (walls)
                {
                    case Directions.NorthEast:
                        if (corner == CornerType.Square)
                            AddCorner(north, east, center, corner, WallType.Both);
                        else
                        {
                            AddCorner(north, east, center, corner, WallType.Both);
                            AddCorner(east, north, center, corner, WallType.Both);
                            AddCorner(north, east, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.North:
                        AddCorner(north, east, center, CornerType.Square, WallType.Both);
                        break;
                    default:
                        AddCorner(north, east, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            default:
                switch (walls)
                {
                    case Directions.North:
                        AddStraight(west, east, WallType.Both);
                        break;
                    case Directions.NorthWest:
                        //Todo: convert these to simple segments if the corners are square
                        AddStraight(west, east, WallType.Floor);
                        AddCorner(north, west, center, corner, WallType.None);
                        AddCorner(west, north, center, corner, WallType.Wall);
                        break;
                    case Directions.NorthEast:
                        AddStraight(west, east, WallType.Floor);
                        AddCorner(east, north, center, corner, WallType.None);
                        AddCorner(north, east, center, corner, WallType.Wall);
                        break;
                    default:
                        AddStraight(west, east, WallType.Floor);
                        break;
                }
                break;
        }

    }

    void AddInnerCornerCell(Vector3 north, Vector3 east, Vector3 south, Vector3 west, Vector3 center, CornerType corner, Directions edges, Directions walls)
    {
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
                            AddCorner(south, west, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddCorner(south, east, center, corner, WallType.Floor);
                            AddStraight(east, west, WallType.Floor);
                            AddCorner(west, south, center, corner, WallType.None);
                            AddCorner(south, west, center, corner, WallType.Wall);
                        }
                        break;
                    case Directions.West:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(south, west, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddCorner(south, west, center, CornerType.Square, WallType.Both);
                            AddCorner(east, south, center, CornerType.Square, WallType.Floor);
                            AddCorner(south, east, center, corner, WallType.Floor);
                        }
                        break;
                    case Directions.West | Directions.North:
                        AddCorner(south, east, center, corner, WallType.Both);
                        AddStraight(east, west, WallType.Both);
                        break;
                    case Directions.NorthWest:
                    case Directions.NorthEast:
                    case Directions.North:
                    default:
                        AddCorner(south, east, center, corner, WallType.Floor);
                        AddStraight(east, west, WallType.Floor);
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
                            AddCorner(north, east, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddStraight(north, south, WallType.Floor);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            AddCorner(east, north, center, corner, WallType.None);
                            AddCorner(north, east, center, corner, WallType.Wall);
                        }
                        break;
                    case Directions.North:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(north, east, center, corner, WallType.Both);
                        }
                        else
                        {
                            AddCorner(south, east, center, corner, WallType.Floor);
                            AddCorner(east, south, center, CornerType.Square, WallType.None);
                            AddCorner(north, east, center, CornerType.Square, WallType.Both);
                        }
                        break;
                    case Directions.North | Directions.West:
                        AddStraight(north, south, WallType.Both);
                        AddCorner(south, east, center, corner, WallType.Both);
                        break;
                    case Directions.NorthWest:
                    case Directions.SouthWest:
                    case Directions.West:
                    default:
                        AddStraight(north, south, WallType.Floor);
                        AddCorner(south, east, center, corner, WallType.Floor);
                        break;
                }
                break;
            case Directions.North | Directions.West:
                switch (walls)
                {
                    case Directions.North | Directions.West:
                        if (corner != CornerType.Square)
                        {
                            AddCorner(south, east, center, corner, WallType.Both);
                            AddCorner(east, south, center, CornerType.Square, WallType.Both);
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
                            AddCorner(south, east, center, corner, WallType.Floor);
                            AddCorner(east, south, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                }
                break;
            case Directions.North | Directions.East:
                switch (walls)
                {
                    case Directions.West:
                    case Directions.North | Directions.West:
                        AddCorner(south, west, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.SouthWest:
                    case Directions.NorthEast | Directions.SouthWest:
                        if (corner == CornerType.Square)
                            AddCorner(south, west, center, CornerType.Square, WallType.Both);
                        else
                        {
                            AddCorner(south, west, center, corner, WallType.Wall);
                            AddCorner(west, south, center, corner, WallType.None);
                            AddCorner(south, west, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.NorthWest:
                    case Directions.NorthEast:
                    case Directions.North:
                    default:
                        AddCorner(south, west, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.East | Directions.South:
                switch (walls)
                {
                    case Directions.North | Directions.West:
                    case Directions.North:
                    case Directions.West:
                        AddCorner(west, north, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.NorthWest:
                        if (corner == CornerType.Square)
                            AddCorner(west, north, center, CornerType.Square, WallType.Both);
                        else
                        {
                            AddCorner(west, north, center, corner, WallType.Wall);
                            AddCorner(north, west, center, corner, WallType.None);
                            AddCorner(west, north, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.NorthEast:
                    case Directions.SouthWest:
                    case Directions.NorthEast | Directions.SouthWest:
                    default:
                        AddCorner(west, north, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.West | Directions.South:
                switch (walls)
                {
                    case Directions.North | Directions.West:
                    case Directions.North:
                        AddCorner(north, east, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.NorthEast:
                    case Directions.NorthEast | Directions.SouthWest:
                        if (corner == CornerType.Square)
                            AddCorner(north, east, center, corner, WallType.Both);
                        else
                        {
                            AddCorner(north, east, center, corner, WallType.Both);
                            AddCorner(east, north, center, corner, WallType.None);
                            AddCorner(north, east, center, CornerType.Square, WallType.Floor);
                        }
                        break;
                    case Directions.NorthWest:
                    case Directions.SouthWest:
                    case Directions.West:
                    default:
                        AddCorner(north, east, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            default:
                {
                    switch (walls)
                    {
                        case Directions.NorthWest:
                            AddCorner(west, north, center, corner, WallType.Wall);
                            AddCorner(north, west, center, corner, WallType.None);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            break;
                        case Directions.NorthEast:
                            AddCorner(north, east, center, corner, WallType.Wall);
                            AddCorner(east, north, center, corner, WallType.None);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            break;
                        case Directions.SouthWest:
                            AddCorner(south, west, center, corner, WallType.Wall);
                            AddCorner(west, south, center, corner, WallType.None);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            break;
                        case Directions.North:
                            AddStraight(west, east, WallType.Wall);
                            AddStraight(east, west, WallType.None);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            break;
                        case Directions.West:
                            AddStraight(south, north, WallType.Wall);
                            AddStraight(north, south, WallType.None);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            break;
                        case Directions.NorthEast | Directions.SouthWest:
                            AddCorner(north, east, center, corner, WallType.Wall);
                            AddCorner(south, west, center, corner, WallType.Wall);
                            AddCorner(west, south, center, corner, WallType.None);
                            AddCorner(south, east, center, corner, WallType.Floor);
                            AddCorner(east, north, center, corner, WallType.None);
                            break;
                        case Directions.North | Directions.West:
                            AddCorner(south, east, center, corner, WallType.Both);
                            break;
                        default:
                            AddCorner(south, east, center, corner, WallType.Floor);
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
                        AddCorner(east, south, center, corner, WallType.Both);
                        break;
                    default:
                        AddCorner(east, south, center, corner, WallType.Floor);
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
                        AddCorner(west, north, center, corner, WallType.Both);
                        break;
                    default:
                        AddCorner(west, north, center, corner, WallType.Floor);
                        break;
                }
                break;
            default:
                switch (walls)
                {
                    case Directions.NorthWest | Directions.SouthEast:
                        AddCorner(east, south, center, corner, WallType.Both);
                        AddCorner(west, north, center, corner, WallType.Both);
                        break;
                    case Directions.NorthWest:
                        AddCorner(east, south, center, corner, WallType.Floor);
                        AddCorner(west, north, center, corner, WallType.Both);
                        break;
                    case Directions.SouthEast:
                        AddCorner(east, south, center, corner, WallType.Both);
                        AddCorner(west, north, center, corner, WallType.Floor);
                        break;
                    default:
                        AddCorner(east, south, center, corner, WallType.Floor);
                        AddCorner(west, north, center, corner, WallType.Floor);
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
        switch (edges)
        {
            case Directions.North:
                switch (walls)
                {
                    case Directions.West | Directions.North:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(south, west, center, corner, WallType.Both);
                            AddCorner(east, south, center, corner, WallType.Floor);
                        }
                        else
                        {
                            AddCorner(south, east, center, corner, WallType.Wall);
                            AddStraight(east, west, WallType.Both);
                            AddCorner(east, south, center, corner, WallType.None);
                        }
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(east, south, center, corner, WallType.Wall);
                        AddCorner(south, east, center, corner, WallType.None);
                        AddStraight(east, west, WallType.Floor);
                        break;
                    case Directions.All:
                        AddStraight(east, west, WallType.Both);
                        break;
                    case Directions.North:
                    case Directions.NorthWest:
                    default:
                        AddStraight(east, west, WallType.Floor);
                        break;
                }
                break;
            case Directions.South:
                switch (walls)
                {
                    case Directions.NorthWest:
                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddCorner(north, west, center, corner, WallType.None);
                        AddStraight(west, east, WallType.Floor);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddCorner(north, west, center, corner, WallType.None);
                        AddStraight(west, east, WallType.Floor);
                        break;
                    case Directions.West | Directions.North:
                    case Directions.North:
                    case Directions.All:
                        AddStraight(west, east, WallType.Both);
                        break;
                    default:
                        AddStraight(west, east, WallType.Floor);
                        break;
                }
                break;
            case Directions.East:
                switch (walls)
                {
                    case Directions.NorthWest:
                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddStraight(south, north, WallType.Floor);
                        AddCorner(north, west, center, corner, WallType.None);
                        break;
                    case Directions.North:
                        AddCorner(west, north, center, CornerType.Square, WallType.Both);
                        AddCorner(south, west, center, CornerType.Square, WallType.Floor);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddStraight(south, north, WallType.Floor);
                        AddCorner(north, west, center, corner, WallType.None);
                        break;
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddStraight(south, north, WallType.Both);
                        break;
                    default:
                        AddStraight(south, north, WallType.Floor);
                        break;
                }
                break;
            case Directions.West:
                switch (walls)
                {
                    case Directions.North:
                        AddCorner(north, east, center, CornerType.Square, WallType.Both);
                        AddCorner(east, south, center, CornerType.Square, WallType.Floor);
                        break;
                    case Directions.West | Directions.North:
                        AddStraight(north, south, WallType.Both);
                        AddCorner(south, east, center, corner, WallType.Wall);
                        AddCorner(east, south, center, corner, WallType.None);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;
                        AddCorner(east, south, center, corner, WallType.Wall);
                        AddCorner(south, east, center, corner, WallType.None);
                        AddStraight(north, south, WallType.Floor);
                        break;
                    case Directions.All:
                        AddStraight(north, south, WallType.Both);
                        break;
                    case Directions.NorthWest:
                    default:
                        AddStraight(north, south, WallType.Floor);
                        break;
                }
                break;
            case Directions.West | Directions.North:
                switch (walls)
                {
                    case Directions.West | Directions.North:
                        AddCorner(east, south, center, CornerType.Square, WallType.Both);
                        AddCorner(south, east, center, corner, WallType.None);
                        AddCorner(east, south, center, corner, WallType.Floor);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (corner == CornerType.Square)
                        {
                            AddCorner(east, south, center, corner, WallType.Wall);
                        }
                        else
                        {
                            AddCorner(east, south, center, CornerType.Square, WallType.Floor);
                            AddCorner(south, east, center, corner, WallType.None);
                            AddCorner(east, south, center, corner, WallType.Wall);
                        }
                        break;
                    case Directions.All:
                        AddCorner(east, south, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.North:
                    case Directions.NorthWest:
                    default:
                        AddCorner(east, south, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.North | Directions.East:
                switch (walls)
                {
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddCorner(south, west, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.North:
                    case Directions.NorthWest:
                    case Directions.NorthWest | Directions.SouthEast:
                    default:
                        AddCorner(south, west, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.East | Directions.South:
                switch (walls)
                {
                    case Directions.NorthWest | Directions.SouthEast:
                    case Directions.NorthWest:
                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddCorner(north, west, center, corner, WallType.None);
                        AddCorner(west, north, center, CornerType.Square, WallType.Floor);
                        break;
                    case Directions.North:
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddCorner(west, north, center, CornerType.Square, WallType.Both);
                        break;
                    default:
                        AddCorner(west, north, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            case Directions.South | Directions.West:
                switch (walls)
                {
                    case Directions.North:
                    case Directions.West | Directions.North:
                    case Directions.All:
                        AddCorner(north, east, center, CornerType.Square, WallType.Both);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                    case Directions.NorthWest:
                    default:
                        AddCorner(north, east, center, CornerType.Square, WallType.Floor);
                        break;
                }
                break;
            default:
                switch (walls)
                {
                    case Directions.NorthWest:
                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddCorner(north, west, center, corner, WallType.None);
                        break;
                    case Directions.North:
                        AddStraight(west, east, WallType.Wall);
                        AddStraight(east, west, WallType.None);
                        break;
                    case Directions.West | Directions.North:
                        AddCorner(south, east, center, corner, WallType.Wall);
                        AddCorner(east, south, center, corner, WallType.None);
                        break;
                    case Directions.NorthWest | Directions.SouthEast:
                        if (OpenedDiagonals)
                            corner = CornerType.Diamond;

                        AddCorner(west, north, center, corner, WallType.Wall);
                        AddCorner(north, west, center, corner, WallType.None);

                        AddCorner(east, south, center, corner, WallType.Wall);
                        AddCorner(south, east, center, corner, WallType.None);
                        break;
                    default:
                        break;
                }
                break;
        }
    }

    private Vector3[] GetCornerPoints(Vector3 start, Vector3 end, Vector3 center, CornerType type)
    {
        switch (type)
        {
            case CornerType.Square:
                return new Vector3[]
                {
                    start,
                    center,
                    end
                };
            case CornerType.Rounded:
                return new Vector3[] 
                {
                    start,
                    (start + center) / 2,
                    (end + center) / 2,
                    end
                };
            default:
                return new Vector3[]
                {
                    start,
                    end
                };
        }
    }

    private void AddCorner(Vector3 start, Vector3 end, Vector3 center, CornerType type, WallType wallType = WallType.None)
    {
        switch (type)
        {
            case CornerType.Diamond:
                break;
            case CornerType.Square:
                AddWallMesh(wallType, start, center, end);
                return;
            case CornerType.Rounded:
                AddWallMesh(wallType,
                    start,
                    (start + center) / 2,
                    (end + center) / 2,
                    end);
                return;
            default:
                break;
        }
        AddWallMesh(wallType, start, end);
    }

    private void AddStraight(Vector3 a, Vector3 b, WallType wallType = WallType.None)
    {
        AddWallMesh(wallType, a, b);
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

    void AddHorizontalPoly(WallType type, Vector3 origin, params Vector3[] points)
    {
        Vector3[] newPoints = new Vector3[points.Length + 1];
        newPoints[0] = origin;
        Array.Copy(points, 0, newPoints, 1, points.Length);
        AddHorizontalPoly(type, newPoints);
    }

    void AddHorizontalPoly(WallType type, params Vector3[] points)
    {
        switch (type)
        {
            case WallType.Floor:
                AddHorizontalPoly(GameMap.floorHeight, false, points);
                AddHorizontalPoly(bottomless ? bottomlessDepth : 0.0f, true, points);
                break;
            case WallType.Wall:
                AddHorizontalPoly(GameMap.tileHeight, false, points);
                break;
            case WallType.Both:
                AddHorizontalPoly(GameMap.tileHeight, false, points);
                AddHorizontalPoly(bottomless ? bottomlessDepth : 0.0f, true, points);
                break;
            default:
                break;
        }
    }


    void AddHorizontalPoly(float height, bool flip, params Vector3[] points)
    {
        if (points.Length < 3)
            return;
        int vertIndex = vertices.Count;
        vertices.Add(new Vector3(points[0].x, height, points[0].z));
        uvs.Add(new Vector2(points[0].x / GameMap.tileWidth, -points[0].z / GameMap.tileWidth));
        vertices.Add(new Vector3(points[1].x, height, points[1].z));
        uvs.Add(new Vector2(points[1].x / GameMap.tileWidth, -points[1].z / GameMap.tileWidth));
        for(int i = 2; i < points.Length;i++)
        {
            vertices.Add(new Vector3(points[i].x, height, points[i].z));
            uvs.Add(new Vector2(points[i].x / GameMap.tileWidth, -points[i].z / GameMap.tileWidth));
            triangles.Add(vertIndex);
            if (flip)
            {
                triangles.Add(vertIndex + i - 1);
                triangles.Add(vertIndex + i);
            }
            else
            {
                triangles.Add(vertIndex + i);
                triangles.Add(vertIndex + i - 1);
            }
        }
    }

}
