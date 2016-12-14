using Poly2Tri;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;

    public int resolution;

    private Voxel[] voxels;
    private float voxelSize, gridSize;

    private Voxel dummyX;

    public VoxelGrid xNeighbor, yNeighbor, xyNeighbor;

    public enum CornerType
    {
        Diamond,
        Square,
        Rounded
    }

    CornerType _cornerType = CornerType.Rounded;

    public CornerType cornerType
    {
        get
        {
            return _cornerType;
        }
        set
        {
            _cornerType = value;
            Refresh();
        }
    }

    bool _filledGaps = true;

    public bool filledGaps
    {
        get { return _filledGaps; }
        set
        {
            _filledGaps = value;
            Refresh();
        }
    }

    public void Initialize(int resolution, float size)
    {
        this.resolution = resolution;
        gridSize = size;
        voxelSize = size / resolution;
        voxels = new Voxel[resolution * resolution];

        dummyX = new Voxel();

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                CreateVoxel(i, x, y, x == 0 || x == resolution - 1 || y == 0 || y == resolution - 1);
            }
        }

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "VoxelGrid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        Refresh();
    }

    private void CreateVoxel(int i, int x, int y, bool edge)
    {
        voxels[i] = new Voxel(x, y, voxelSize);
        voxels[i].edge = edge;
    }

    public void Apply(VoxelStencil stencil)
    {
        int xStart = Mathf.Max(stencil.XStart, 0);
        int xEnd = Mathf.Min(stencil.XEnd, resolution - 1);
        int yStart = Mathf.Max(stencil.YStart, 0);
        int yEnd = Mathf.Min(stencil.YEnd, resolution - 1);

        for (int y = yStart; y <= yEnd; y++)
        {
            int i = y * resolution + xStart;
            for (int x = xStart; x <= xEnd; x++, i++)
            {
                voxels[i].state = stencil.Apply(x, y, voxels[i].state);
            }
        }
        Refresh();
    }


    private void Refresh()
    {
        Triangulate();
    }

    private void Triangulate()
    {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
        uvs.Clear();

        wallPolygons.Clear();
        floorPolygons.Clear();

        if (xNeighbor != null)
        {
            dummyX.BecomeXDummyOf(xNeighbor.voxels[0], gridSize);
        }
        TriangulateCellRows();

        ConvertToMesh(wallPolygons.Polygons, GameMap.tileHeight);
        ConvertToMesh(floorPolygons.Polygons, GameMap.floorHeight);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    private void TriangulateCellRows()
    {
        int cells = resolution - 1;
        for (int i = 0, y = 0; y < cells; y++, i++)
        {
            for (int x = 0; x < cells; x++, i++)
            {
                //if (x == 0)
                //    TriangulateLeftEdge(voxels[i], voxels[i + resolution]);
                //if (x == cells - 1)
                //    TriangulateRightEdge(voxels[i + 1], voxels[i + resolution + 1]);
                //if (y == 0)
                //    TriangulateTopEdge(voxels[i], voxels[i + 1]);
                //if (y == cells - 1)
                //    TriangulateBottomEdge(voxels[i + resolution], voxels[i + resolution + 1]);

                TriangulateCell(
                    voxels[i],
                    voxels[i + 1],
                    voxels[i + resolution],
                    voxels[i + resolution + 1]);
            }
        }
    }

    internal void Randomize()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            voxels[i].state = (Voxel.State)UnityEngine.Random.Range(0, 3);
        }
        Refresh();
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


    private void TriangulateCell(Voxel northWest, Voxel northEast, Voxel southWest, Voxel southEast)
    {
        var corner = _cornerType;
        if (northWest.state == Voxel.State.Intruded
            || northEast.state == Voxel.State.Intruded
            || southWest.state == Voxel.State.Intruded
            || southEast.state == Voxel.State.Intruded
            )
            corner = CornerType.Square;



        Directions walls = Directions.None;
        if (northWest.state == Voxel.State.Wall)
        {
            walls |= Directions.NorthWest;
        }
        if (northEast.state == Voxel.State.Wall)
        {
            walls |= Directions.NorthEast;
        }
        if (southWest.state == Voxel.State.Wall)
        {
            walls |= Directions.SouthWest;
        }
        if (southEast.state == Voxel.State.Wall)
        {
            walls |= Directions.SouthEast;
        }

        Directions wallFloors = walls;
        if (northWest.state == Voxel.State.Floor)
        {
            wallFloors |= Directions.NorthWest;
        }
        if (northEast.state == Voxel.State.Floor)
        {
            wallFloors |= Directions.NorthEast;
        }
        if (southWest.state == Voxel.State.Floor)
        {
            wallFloors |= Directions.SouthWest;
        }
        if (southEast.state == Voxel.State.Floor)
        {
            wallFloors |= Directions.SouthEast;
        }



        Directions edges = Directions.None;
        if (northWest.edge)
        {
            edges |= Directions.NorthWest;
        }
        if (northEast.edge)
        {
            edges |= Directions.NorthEast;
        }
        if (southWest.edge)
        {
            edges |= Directions.SouthWest;
        }
        if (southEast.edge)
        {
            edges |= Directions.SouthEast;
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
                    northWest.eastEdge,
                    northEast.southEdge,
                    southWest.eastEdge,
                    northWest.southEdge,
                    northWest.cornerPosition,
                    wallFloors, edges, walls, corner);
                break;
            case Directions.NorthEast:
            case Directions.East:
            case Directions.North | Directions.SouthEast:
            case Directions.NorthEast | Directions.SouthWest:
                AddRotatedCell(
                    northEast.southEdge,
                    southWest.eastEdge,
                    northWest.southEdge,
                    northWest.eastEdge,
                    northWest.cornerPosition,
                    RotateCCW(wallFloors), RotateCCW(edges), RotateCCW(walls), corner);
                break;
            case Directions.SouthEast:
            case Directions.South:
            case Directions.NorthEast | Directions.South:
                AddRotatedCell(
                    southWest.eastEdge,
                    northWest.southEdge,
                    northWest.eastEdge,
                    northEast.southEdge,
                    northWest.cornerPosition,
                    Rotate180(wallFloors), Rotate180(edges), Rotate180(walls), corner);
                break;
            case Directions.SouthWest:
            case Directions.West:
            case Directions.West | Directions.SouthEast:
                AddRotatedCell(
                    northWest.southEdge,
                    northWest.eastEdge,
                    northEast.southEdge,
                    southWest.eastEdge,
                    northWest.cornerPosition,
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
                        if(corner == CornerType.Square)
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
                AddStraightCell(south, west, north, center, (walls == Directions.North)? CornerType.Square : corner, RotateCW(edges), RotateCW(walls) & Directions.North);
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
                        if(corner == CornerType.Square)
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
                        if(corner == CornerType.Square)
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
        if (!_filledGaps)
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
                        if (!_filledGaps)
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
                        if (!_filledGaps)
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
                        if (!_filledGaps)
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
                        if (!_filledGaps)
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
                        if (!_filledGaps)
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

    private void OnDrawGizmos()
    {
        wallPolygons.DrawGizmos(transform, Color.green, Color.blue, GameMap.tileHeight);
        floorPolygons.DrawGizmos(transform, Color.magenta, Color.yellow, GameMap.floorHeight);
        foreach (var item in voxels)
        {
            switch (item.state)
            {
                case Voxel.State.Empty:
                    continue;
                case Voxel.State.Floor:
                    Gizmos.color = Color.yellow;
                    break;
                case Voxel.State.Wall:
                    Gizmos.color = Color.blue;
                    break;
                case Voxel.State.Intruded:
                    Gizmos.color = Color.red;
                    break;
                default:
                    break;
            }
            Gizmos.DrawCube(transform.localToWorldMatrix.MultiplyPoint(item.position), Vector3.one * 0.1f);
        }
    }


    void ConvertToMesh(PolygonSet polySet, float height)
    {
        Dictionary<TriangulationPoint, int> pointIndices = new Dictionary<TriangulationPoint, int>();
        P2T.Triangulate(polySet);
        foreach (var polygon in polySet.Polygons)
        {
            foreach (var triangle in polygon.Triangles)
            {
                for(int i = 2; i >= 0; i--)
                {
                    var point = triangle.Points[i];
                    int index;
                    if (!pointIndices.ContainsKey(point))
                    {
                        index = vertices.Count;
                        pointIndices[point] = index;
                        vertices.Add(new Vector3(point.Xf, height, point.Yf));
                        uvs.Add(new Vector2(point.Xf / GameMap.tileWidth, point.Yf / GameMap.tileWidth));
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
                AddWallMesh(GameMap.floorHeight, 0, points);
                break;
            case WallType.Wall:
                AddWallMesh(GameMap.tileHeight, GameMap.floorHeight, points);
                break;
            case WallType.Both:
                AddWallMesh(GameMap.tileHeight, 0, points);
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
        for(int i = 0; i < points.Length-1; i++)
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