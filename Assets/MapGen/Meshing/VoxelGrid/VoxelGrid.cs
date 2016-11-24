using Poly2Tri;
using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;

    public GameObject voxelPrefab;
    public int resolution;

    private Voxel[] voxels;
    private float voxelSize, gridSize;

    private Voxel dummyX, dummyY, dummyT;

    private Material[] voxelMaterials;

    public VoxelGrid xNeighbor, yNeighbor, xyNeighbor;

    public enum CornerType
    {
        Diamond,
        Square,
        Rounded
    }

    CornerType _cornerType = CornerType.Diamond;

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

    bool _filledGaps = false;

    public bool filledGaps
    {
        get { return _filledGaps; }
        set
        {
            _filledGaps = value;
            Refresh();
        }
    }

    bool _saddleCrossing = false;

    public bool saddleCrossing
    {
        get { return _saddleCrossing; }
        set
        {
            _saddleCrossing = value;
            Refresh();
        }
    }

    public void Initialize(int resolution, float size)
    {
        this.resolution = resolution;
        gridSize = size;
        voxelSize = size / resolution;
        voxels = new Voxel[resolution * resolution];
        voxelMaterials = new Material[voxels.Length];

        dummyX = new Voxel();
        dummyY = new Voxel();
        dummyT = new Voxel();

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                CreateVoxel(i, x, y, x == 0 || x == resolution - 1 || y == 0 || y == resolution - 1);
            }
        }
        SetVoxelColors();

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "VoxelGrid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        Refresh();
    }

    private void CreateVoxel(int i, int x, int y, bool edge)
    {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, GameMap.tileHeight + 0.01f, (y + 0.5f) * -voxelSize);
        o.transform.localScale = Vector3.one * voxelSize * 0.1f;
        voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
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
        SetVoxelColors();
        Refresh();
    }

    private void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            switch (voxels[i].state)
            {
                case Voxel.State.Empty:
                    voxelMaterials[i].color = Color.white;
                    break;
                case Voxel.State.Wall:
                    voxelMaterials[i].color = Color.blue;
                    break;
                case Voxel.State.Floor:
                    voxelMaterials[i].color = Color.yellow;
                    break;
                case Voxel.State.Intruded:
                    voxelMaterials[i].color = Color.red;
                    break;
                default:
                    voxelMaterials[i].color = Color.white;
                    break;
            }
        }
    }

    private void Refresh()
    {
        SetVoxelColors();
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

    internal void Invert()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            switch (voxels[i].state)
            {
                case Voxel.State.Empty:
                    voxels[i].state = Voxel.State.Wall;
                    break;
                case Voxel.State.Wall:
                    voxels[i].state = Voxel.State.Empty;
                    break;
                case Voxel.State.Intruded:
                    break;
                default:
                    break;
            }
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


    private void TriangulateCell(Voxel northWest, Voxel northEast, Voxel southWest, Voxel southEast, Voxel.State state = Voxel.State.Wall)
    {
        var corner = _cornerType;
        bool intruded = false;
        if (northWest.state == Voxel.State.Intruded
            || northEast.state == Voxel.State.Intruded
            || southWest.state == Voxel.State.Intruded
            || southEast.state == Voxel.State.Intruded
            )
            intruded = true;
        if (intruded)
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
                    northWest, northWest.eastEdge,
                    northEast, northEast.southEdge,
                    southEast, southWest.eastEdge,
                    southWest, northWest.southEdge,
                    northWest.cornerPosition,
                    wallFloors, edges, walls, corner, intruded, _saddleCrossing);
                break;
            case Directions.NorthEast:
            case Directions.East:
            case Directions.North | Directions.SouthEast:
            case Directions.NorthEast | Directions.SouthWest:
                AddRotatedCell(
                    northEast, northEast.southEdge,
                    southEast, southWest.eastEdge,
                    southWest, northWest.southEdge,
                    northWest, northWest.eastEdge,
                    northWest.cornerPosition,
                    RotateCCW(wallFloors), RotateCCW(edges), RotateCCW(walls), corner, intruded, _saddleCrossing);
                break;
            case Directions.SouthEast:
            case Directions.South:
            case Directions.NorthEast | Directions.South:
                AddRotatedCell(
                    southEast, southWest.eastEdge,
                    southWest, northWest.southEdge,
                    northWest, northWest.eastEdge,
                    northEast, northEast.southEdge,
                    northWest.cornerPosition,
                    Rotate180(wallFloors), Rotate180(edges), Rotate180(walls), corner, intruded, _saddleCrossing);
                break;
            case Directions.SouthWest:
            case Directions.West:
            case Directions.West | Directions.SouthEast:
                AddRotatedCell(
                    southWest, northWest.southEdge,
                    northWest, northWest.eastEdge,
                    northEast, northEast.southEdge,
                    southEast, southWest.eastEdge,
                    northWest.cornerPosition,
                    RotateCW(wallFloors), RotateCW(edges), RotateCW(walls), corner, intruded, _saddleCrossing);
                break;
        }
    }

    private void AddRotatedCell(
        Voxel northWest,
        Vector3 north,
        Voxel northEast,
        Vector3 east,
        Voxel southEast,
        Vector3 south,
        Voxel southWest,
        Vector3 west,
        Vector3 center,
        Directions neighbors,
        Directions edges,
        Directions walls,
        CornerType corner,
        bool intruded, bool saddleCrossing)
    {
        switch (neighbors)
        {
            case Directions.NorthWest:
                if ((neighbors & edges) != neighbors)
                {
                    if(neighbors == walls)
                        AddCorner(wallPolygons, west, north, center, corner);
                    else
                        AddCorner(floorPolygons, west, north, center, corner);

                }
                break;
            case Directions.North:
                switch (edges)
                {
                    case Directions.North:
                    case Directions.North | Directions.SouthWest:
                    case Directions.North | Directions.SouthEast:
                        break;
                    case Directions.East:
                    case Directions.NorthEast | Directions.South:
                        if((walls & Directions.NorthWest) == Directions.NorthWest)
                            AddCorner(wallPolygons, west, north, center, CornerType.Square);
                        else
                            AddCorner(floorPolygons, west, north, center, CornerType.Square);
                        break;
                    case Directions.West:
                    case Directions.West | Directions.SouthEast:
                        if((walls & Directions.NorthEast) == Directions.NorthEast)
                            AddCorner(wallPolygons, north, east, center, CornerType.Square);
                        else
                            AddCorner(floorPolygons, north, east, center, CornerType.Square);
                        break;
                    default:
                        switch (walls)
                        {
                            case Directions.North:
                                AddStraight(wallPolygons, west, east);
                                break;
                            case Directions.NorthWest:
                                AddStraight(floorPolygons, west, east);
                                AddCorner(floorPolygons, north, west, center, corner);
                                AddCorner(wallPolygons, west, north, center, corner);
                                break;
                            case Directions.NorthEast:
                                AddStraight(floorPolygons, west, east);
                                AddCorner(floorPolygons, east, north, center, corner);
                                AddCorner(wallPolygons, north, east, center, corner);
                                break;
                            default:
                                AddStraight(floorPolygons, west, east);
                                break;
                        }
                        break;
                }
                break;
            case Directions.North | Directions.SouthWest:
                switch (edges)
                {
                    case Directions.East:
                        AddStraight(wallPolygons, south, north);
                        break;
                    case Directions.South:
                        AddStraight(wallPolygons, west, east);
                        break;
                    case Directions.North:
                        AddStraight(wallPolygons, east, west);
                        AddCorner(wallPolygons, south, east, center, corner);
                        break;
                    case Directions.West:
                        AddStraight(wallPolygons, north, south);
                        AddCorner(wallPolygons, south, east, center, corner);
                        break;
                    case Directions.North | Directions.SouthWest:
                        if (corner != CornerType.Square)
                        {
                            AddCorner(wallPolygons, south, east, center, corner);
                            AddCorner(wallPolygons, east, south, center, CornerType.Square);
                        }
                        break;
                    case Directions.North | Directions.SouthEast:
                        AddCorner(wallPolygons, south, west, center, CornerType.Square);
                        break;
                    case Directions.NorthEast | Directions.South:
                        AddCorner(wallPolygons, west, north, center, CornerType.Square);
                        break;
                    case Directions.NorthWest | Directions.South:
                        AddCorner(wallPolygons, north, east, center, CornerType.Square);
                        break;
                    default:
                        AddCorner(wallPolygons, south, east, center, corner);
                        break;
                }
                break;
            case Directions.NorthWest | Directions.SouthEast:
                switch (edges)
                {
                    case Directions.North:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing)
                            {
                                Vector3 point = type == CornerType.Diamond ? nudge(east, south, west) : west;
                                AddStraight(wallPolygons, east, point);
                                AddCorner(wallPolygons, point, south, center, type);
                            }
                            else
                            {
                                AddCorner(wallPolygons, east, south, center, type);
                            }
                        }
                        break;
                    case Directions.East:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing)
                            {
                                Vector3 point = type == CornerType.Diamond ? nudge(west, north, south) : south;
                                AddCorner(wallPolygons, west, point, center, type);
                                AddStraight(wallPolygons, point, north);
                            }
                            else
                            {
                                AddCorner(wallPolygons, west, north, center, type);
                            }
                        }
                        break;
                    case Directions.South:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing)
                            {
                                Vector3 point = type == CornerType.Diamond ? nudge(west, north, east) : east;
                                AddStraight(wallPolygons, west, point);
                                AddCorner(wallPolygons, point, north, center, type);
                            }
                            else
                            {
                                AddCorner(wallPolygons, west, north, center, type);
                            }
                        }
                        break;
                    case Directions.West:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing)
                            {
                                Vector3 point = type == CornerType.Diamond ? nudge(east, south, north) : north;
                                AddCorner(wallPolygons, east, point, center, type);
                                AddStraight(wallPolygons, point, south);
                            }
                            else
                            {
                                AddCorner(wallPolygons, east, south, center, type);
                            }
                        }
                        break;
                    case Directions.North | Directions.SouthWest:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing)
                            {
                                AddCorner(wallPolygons, east, south, center, CornerType.Square);
                            }
                            else
                            {
                                AddCorner(wallPolygons, east, south, center, type);
                            }
                        }
                        break;
                    case Directions.NorthEast | Directions.South:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing)
                            {
                                AddCorner(wallPolygons, west, north, center, CornerType.Square);
                            }
                            else
                            {
                                AddCorner(wallPolygons, west, north, center, type);
                            }
                        }
                        break;
                    case Directions.North | Directions.SouthEast:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing && type != CornerType.Square)
                            {
                                AddCorner(wallPolygons, west, south, center, type);
                                AddCorner(wallPolygons, south, west, center, CornerType.Square);
                            }
                        }
                        break;
                    case Directions.NorthWest | Directions.South:
                        {
                            var type = corner;
                            if (!_filledGaps)
                                type = CornerType.Diamond;
                            if (saddleCrossing && type != CornerType.Square)
                            {
                                AddCorner(wallPolygons, east, north, center, type);
                                AddCorner(wallPolygons, north, east, center, CornerType.Square);
                            }
                        }
                        break;
                    default:
                        AddSaddle(wallPolygons, north, east, south, west, center, saddleCrossing, corner);
                        break;
                }
                break;
            case Directions.All:
                switch (edges)
                {
                    case Directions.North | Directions.SouthWest:
                        AddCorner(wallPolygons, east, south, center, CornerType.Square);
                        break;
                    case Directions.North | Directions.SouthEast:
                        AddCorner(wallPolygons, south, west, center, CornerType.Square);
                        break;
                    case Directions.West | Directions.SouthEast:
                        AddCorner(wallPolygons, north, east, center, CornerType.Square);
                        break;
                    case Directions.NorthEast | Directions.South:
                        AddCorner(wallPolygons, west, north, center, CornerType.Square);
                        break;
                    case Directions.North:
                        AddStraight(wallPolygons, east, west);
                        break;
                    case Directions.West:
                        AddStraight(wallPolygons, north, south);
                        break;
                    case Directions.East:
                        AddStraight(wallPolygons, south, north);
                        break;
                    case Directions.South:
                        AddStraight(wallPolygons, west, east);
                        break;
                    default:
                        break;
                }
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

    private void AddCorner(ComplexPoly poly, PolygonPoint start, PolygonPoint end, PolygonPoint center, CornerType type = CornerType.Diamond)
    {
        switch (type)
        {
            case CornerType.Diamond:
                break;
            case CornerType.Square:
                poly.AddLineSegment(start, center, end);
                return;
            case CornerType.Rounded:
                poly.AddLineSegment(
                    start,
                    new PolygonPoint((start.X + center.X) / 2, (start.Y + center.Y) / 2),
                    new PolygonPoint((end.X + center.X) / 2, (end.Y + center.Y) / 2),
                    end);
                return;
            default:
                break;
        }
        poly.AddLineSegment(start, end);
    }

    private void AddStraight(ComplexPoly poly, PolygonPoint a, PolygonPoint b)
    {
        poly.AddLineSegment(a, b);
    }


    private void AddSaddle(ComplexPoly poly, PolygonPoint north, PolygonPoint east, PolygonPoint south, PolygonPoint west, Vector3 center, bool saddleCrossing = true, CornerType type = CornerType.Diamond)
    {
        if (!_filledGaps)
            type = CornerType.Diamond;
        if(saddleCrossing)
        {
            AddCorner(poly, east, north, type == CornerType.Square ? nudge(east, north, center) : center, type);
            AddCorner(poly, west, south, type == CornerType.Square ? nudge(west, south, center) : center, type);
        }
        else
        {
            AddCorner(poly, east, south, type == CornerType.Square ? nudge(east, south, center) : center, type);
            AddCorner(poly, west, north, type == CornerType.Square ? nudge(west, north, center) : center, type);
        }
    }

    ComplexPoly wallPolygons = new ComplexPoly();
    ComplexPoly floorPolygons = new ComplexPoly();

    private void OnDrawGizmos()
    {
        wallPolygons.DrawGizmos(transform, Color.green, Color.blue);
        floorPolygons.DrawGizmos(transform, Color.magenta, Color.yellow);
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
                        uvs.Add(new Vector2(point.Xf / GameMap.tileWidth, -point.Yf / GameMap.tileWidth));
                    }
                    else
                        index = pointIndices[point];
                    triangles.Add(index);
                }
            }
        }
    }
}