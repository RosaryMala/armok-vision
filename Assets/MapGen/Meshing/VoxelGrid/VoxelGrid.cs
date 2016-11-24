using Poly2Tri;
using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
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
        Refresh();
    }

    private void CreateVoxel(int i, int x, int y, bool edge)
    {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, 0.01f, (y + 0.5f) * -voxelSize);
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
                case Voxel.State.Filled:
                    voxelMaterials[i].color = Color.black;
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

        PolygonSegmentsByEnd.Clear();
        PolygonSegmentsByStart.Clear();
        CompletedPolygons = new PolygonSet();

        if (xNeighbor != null)
        {
            dummyX.BecomeXDummyOf(xNeighbor.voxels[0], gridSize);
        }
        TriangulateCellRows();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
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
                    voxels[i].state = Voxel.State.Filled;
                    break;
                case Voxel.State.Filled:
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

    private void TriangulateLeftEdge(Voxel a, Voxel b)
    {
        if(a.state == Voxel.State.Filled)
        {
            if (b.state == Voxel.State.Filled)
            {
                AddLineSegment(a.position, b.position);
            }
            else
            {
                AddLineSegment(a.position, a.southEdge);
            }
        }
        else if(b.state == Voxel.State.Filled)
        {
            AddLineSegment(a.southEdge, b.position);
        }
    }

    private void TriangulateRightEdge(Voxel a, Voxel b)
    {
        if (a.state == Voxel.State.Filled)
        {
            if (b.state == Voxel.State.Filled)
            {
                AddLineSegment(b.position, a.position);
            }
            else
            {
                AddLineSegment(a.southEdge, a.position);
            }
        }
        else if (b.state == Voxel.State.Filled)
        {
            AddLineSegment(b.position, a.southEdge);
        }
    }

    private void TriangulateTopEdge(Voxel a, Voxel b)
    {
        if (a.state == Voxel.State.Filled)
        {
            if (b.state == Voxel.State.Filled)
            {
                AddLineSegment(b.position, a.position);
            }
            else
            {
                AddLineSegment(a.eastEdge, a.position);
            }
        }
        else if (b.state == Voxel.State.Filled)
        {
            AddLineSegment(b.position, a.eastEdge);
        }
    }

    private void TriangulateBottomEdge(Voxel a, Voxel b)
    {
        if (a.state == Voxel.State.Filled)
        {
            if (b.state == Voxel.State.Filled)
            {
                AddLineSegment(a.position, b.position);
            }
            else
            {
                AddLineSegment(a.position, a.eastEdge);
            }
        }
        else if (b.state == Voxel.State.Filled)
        {
            AddLineSegment(a.eastEdge, b.position);
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


    private void TriangulateCell(Voxel northWest, Voxel northEast, Voxel southWest, Voxel southEast)
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


        Directions cellType = Directions.None;
        if (northWest.state == Voxel.State.Filled)
        {
            cellType |= Directions.NorthWest;
        }
        if (northEast.state == Voxel.State.Filled)
        {
            cellType |= Directions.NorthEast;
        }
        if (southWest.state == Voxel.State.Filled)
        {
            cellType |= Directions.SouthWest;
        }
        if (southEast.state == Voxel.State.Filled)
        {
            cellType |= Directions.SouthEast;
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


        switch (cellType)
        {
            case Directions.None:
                return;
            case Directions.NorthWest:
            case Directions.North:
            case Directions.North | Directions.SouthWest:
            case Directions.NorthWest | Directions.SouthEast:
            case Directions.All:
                AddRotatedCell(
                    northWest.position, northWest.eastEdge,
                    northEast.position, northEast.southEdge,
                    southEast.position, southWest.eastEdge,
                    southWest.position, northWest.southEdge,
                    northWest.cornerPosition,
                    cellType, edges, corner, intruded, _saddleCrossing);
                break;
            case Directions.NorthEast:
            case Directions.East:
            case Directions.North | Directions.SouthEast:
            case Directions.NorthEast | Directions.SouthWest:
                AddRotatedCell(
                    northEast.position, northEast.southEdge,
                    southEast.position, southWest.eastEdge,
                    southWest.position, northWest.southEdge,
                    northWest.position, northWest.eastEdge,
                    northWest.cornerPosition,
                    RotateCCW(cellType), RotateCCW(edges), corner, intruded, _saddleCrossing);
                break;
            case Directions.SouthEast:
            case Directions.South:
            case Directions.NorthEast | Directions.South:
                AddRotatedCell(
                    southEast.position, southWest.eastEdge,
                    southWest.position, northWest.southEdge,
                    northWest.position, northWest.eastEdge,
                    northEast.position, northEast.southEdge,
                    northWest.cornerPosition,
                    Rotate180(cellType), Rotate180(edges), corner, intruded, _saddleCrossing);
                break;
            case Directions.SouthWest:
            case Directions.West:
            case Directions.West | Directions.SouthEast:
                AddRotatedCell(
                    southWest.position, northWest.southEdge,
                    northWest.position, northWest.eastEdge,
                    northEast.position, northEast.southEdge,
                    southEast.position, southWest.eastEdge,
                    northWest.cornerPosition,
                    RotateCW(cellType), RotateCW(edges), corner, intruded, _saddleCrossing);
                break;
        }
    }

    private void AddRotatedCell(
        Vector3 northWest,
        Vector3 north,
        Vector3 northEast,
        Vector3 east,
        Vector3 southEast,
        Vector3 south,
        Vector3 southWest,
        Vector3 west,
        Vector3 center,
        Directions neighbors,
        Directions edges,
        CornerType corner,
        bool intruded, bool saddleCrossing)
    {
        switch (neighbors)
        {
            case Directions.NorthWest:
                if ((neighbors & edges) != neighbors)
                    AddCorner(west, north, center, corner);
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
                        AddCorner(west, north, center, CornerType.Square);
                        break;
                    case Directions.West:
                    case Directions.West | Directions.SouthEast:
                        AddCorner(north, east, center, CornerType.Square);
                        break;
                    default:
                        AddStraight(west, east);
                        break;
                }
                break;
            case Directions.North | Directions.SouthWest:
                switch (edges)
                {
                    case Directions.East:
                        AddStraight(south, north);
                        break;
                    case Directions.South:
                        AddStraight(west, east);
                        break;
                    case Directions.North:
                        AddStraight(east, west);
                        AddCorner(south, east, center, corner);
                        break;
                    case Directions.West:
                        AddStraight(north, south);
                        AddCorner(south, east, center, corner);
                        break;
                    case Directions.North | Directions.SouthWest:
                        if (corner != CornerType.Square)
                        {
                            AddCorner(south, east, center, corner);
                            AddCorner(east, south, center, CornerType.Square);
                        }
                        break;
                    case Directions.North | Directions.SouthEast:
                        AddCorner(south, west, center, CornerType.Square);
                        break;
                    case Directions.NorthEast | Directions.South:
                        AddCorner(west, north, center, CornerType.Square);
                        break;
                    case Directions.NorthWest | Directions.South:
                        AddCorner(north, east, center, CornerType.Square);
                        break;
                    default:
                        AddCorner(south, east, center, corner);
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
                                AddStraight(east, point);
                                AddCorner(point, south, center, type);
                            }
                            else
                            {
                                AddCorner(east, south, center, type);
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
                                AddCorner(west, point, center, type);
                                AddStraight(point, north);
                            }
                            else
                            {
                                AddCorner(west, north, center, type);
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
                                AddStraight(west, point);
                                AddCorner(point, north, center, type);
                            }
                            else
                            {
                                AddCorner(west, north, center, type);
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
                                AddCorner(east, point, center, type);
                                AddStraight(point, south);
                            }
                            else
                            {
                                AddCorner(east, south, center, type);
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
                                AddCorner(east, south, center, CornerType.Square);
                            }
                            else
                            {
                                AddCorner(east, south, center, type);
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
                                AddCorner(west, north, center, CornerType.Square);
                            }
                            else
                            {
                                AddCorner(west, north, center, type);
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
                                AddCorner(west, south, center, type);
                                AddCorner(south, west, center, CornerType.Square);
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
                                AddCorner(east, north, center, type);
                                AddCorner(north, east, center, CornerType.Square);
                            }
                        }
                        break;
                    default:
                        AddSaddle(north, east, south, west, center, saddleCrossing, corner);
                        break;
                }
                break;
            case Directions.All:
                switch (edges)
                {
                    case Directions.North | Directions.SouthWest:
                        AddCorner(east, south, center, CornerType.Square);
                        break;
                    case Directions.North | Directions.SouthEast:
                        AddCorner(south, west, center, CornerType.Square);
                        break;
                    case Directions.West | Directions.SouthEast:
                        AddCorner(north, east, center, CornerType.Square);
                        break;
                    case Directions.NorthEast | Directions.South:
                        AddCorner(west, north, center, CornerType.Square);
                        break;
                    case Directions.North:
                        AddStraight(east, west);
                        break;
                    case Directions.West:
                        AddStraight(north, south);
                        break;
                    case Directions.East:
                        AddStraight(south, north);
                        break;
                    case Directions.South:
                        AddStraight(west, east);
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

    private void AddCorner(PolygonPoint start, PolygonPoint end, PolygonPoint center, CornerType type = CornerType.Diamond)
    {
        switch (type)
        {
            case CornerType.Diamond:
                break;
            case CornerType.Square:
                AddLineSegment(start, center, end);
                return;
            case CornerType.Rounded:
                AddLineSegment(
                    start,
                    new PolygonPoint((start.X + center.X) / 2, (start.Y + center.Y) / 2),
                    new PolygonPoint((end.X + center.X) / 2, (end.Y + center.Y) / 2),
                    end);
                return;
            default:
                break;
        }
        AddLineSegment(start, end);
    }

    private void AddStraight(PolygonPoint a, PolygonPoint b)
    {
        AddLineSegment(a, b);
    }


    private void AddSaddle(PolygonPoint north, PolygonPoint east, PolygonPoint south, PolygonPoint west, Vector3 center, bool saddleCrossing = true, CornerType type = CornerType.Diamond)
    {
        if (!_filledGaps)
            type = CornerType.Diamond;
        if(saddleCrossing)
        {
            AddCorner(east, north, type == CornerType.Square ? nudge(east, north, center) : center, type);
            AddCorner(west, south, type == CornerType.Square ? nudge(west, south, center) : center, type);
        }
        else
        {
            AddCorner(east, south, type == CornerType.Square ? nudge(east, south, center) : center, type);
            AddCorner(west, north, type == CornerType.Square ? nudge(west, north, center) : center, type);
        }
    }

    #region Polygon Stuff
    Dictionary<Point2D, List<PolygonPoint>> PolygonSegmentsByEnd = new Dictionary<Point2D, List<PolygonPoint>>();
    Dictionary<Point2D, List<PolygonPoint>> PolygonSegmentsByStart = new Dictionary<Point2D, List<PolygonPoint>>();
    PolygonSet CompletedPolygons = new PolygonSet();

    void AddLineSegment(params PolygonPoint[] values)
    {
        AddLineSegment(new List<PolygonPoint>(values));
    }

    void AddLineSegment(List<PolygonPoint> segment)
    {
        if (segment.Count < 2)
            return; //can't add a single point.

        //if both the start and end match, then we're bridging two together.
        if (PolygonSegmentsByEnd.ContainsKey(segment[0]) && PolygonSegmentsByStart.ContainsKey(segment[segment.Count - 1]))
        {
            //get the segment that this one completes.
            var startSegment = PolygonSegmentsByEnd[segment[0]];
            var endSegment = PolygonSegmentsByStart[segment[segment.Count - 1]];
            //remove both from both collections.
            PolygonSegmentsByEnd.Remove(segment[0]);
            PolygonSegmentsByStart.Remove(segment[segment.Count - 1]);

            if (startSegment == endSegment) // We're completing a closed polygon.
            {
                //remove the shared point between both
                startSegment.RemoveAt(startSegment.Count - 1);
                //join them together
                startSegment.AddRange(segment);
                //remove one of the now same two points that are at either ends.
                startSegment.RemoveAt(startSegment.Count - 1);
                Polygon newPoly = new Polygon(startSegment);
                newPoly.Simplify();
                Polygon potentialHole = null;
                foreach (var item in CompletedPolygons.Polygons)
                {
                    if (TryInsertHole(item, newPoly))
                        return;
                    else if (newPoly.IsPointInside(item[0]))
                    {
                        potentialHole = item;
                    }
                }
                if (potentialHole != null)
                {
                    CompletedPolygons.Remove(potentialHole);
                    newPoly.AddHole(potentialHole);
                }
                CompletedPolygons.Add(newPoly);
            }
            else
            {
                //remove the shared point between both
                startSegment.RemoveAt(startSegment.Count - 1);
                //join them together
                startSegment.AddRange(segment);
                //remove one of the now same two points that are at either ends.
                startSegment.RemoveAt(startSegment.Count - 1);
                //join them together
                startSegment.AddRange(endSegment);

                PolygonSegmentsByEnd[startSegment[startSegment.Count - 1]] = startSegment;
                PolygonSegmentsByStart[startSegment[0]] = startSegment;
            }
        }
        //If we have a segment who's end matches our beginning, we join them.
        else if(PolygonSegmentsByEnd.ContainsKey(segment[0]))
        {
            //get the segment we need to join to
            var oldSegment = PolygonSegmentsByEnd[segment[0]];

            //Remove the old segment from that list.
            //It will need a new address.
            PolygonSegmentsByEnd.Remove(segment[0]);

            //remove the last point from the old segment,
            //because it's the same as the first point from the new one.
            oldSegment.RemoveAt(oldSegment.Count - 1);

            //now join them together.
            oldSegment.AddRange(segment);

            //finally add the newly joined segment to the list of ends.
            PolygonSegmentsByEnd[oldSegment[oldSegment.Count - 1]] = oldSegment;

            //The other list doesn't need changeing because we keep the start point.
        }
        //likewise, if we have a segment who's beginning matches our end.
        else if(PolygonSegmentsByStart.ContainsKey(segment[segment.Count - 1]))
        {
            var oldSegment = PolygonSegmentsByStart[segment[segment.Count - 1]];
            PolygonSegmentsByStart.Remove(segment[segment.Count - 1]);
            segment.RemoveAt(segment.Count - 1);
            oldSegment.InsertRange(0, segment);
            PolygonSegmentsByStart[oldSegment[0]] = oldSegment;
        }
        //finally, if there's no existing connection, just add it to both lists.
        else
        {
            PolygonSegmentsByStart[segment[0]] = segment;
            PolygonSegmentsByEnd[segment[segment.Count - 1]] = segment;
        }
    }

    bool TryInsertHole(Polygon parent, Polygon hole)
    {
        //Can't go in.
        if (!parent.IsPointInside(hole[0]))
            return false;

        if(parent.Holes != null)
            foreach (var item in parent.Holes)
            {
                if(TryInsertHole(item, hole))
                    return true;
            }
        //it doesn't fit into any of the daughter holes.
        parent.AddHole(hole);
        return true;
    }

    private void OnDrawGizmos()
    {
        foreach (var item in PolygonSegmentsByEnd)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(item.Key), 0.3f);
            Gizmos.color = Color.green;
            DrawPolygonSegment(item.Value);
        }
        foreach (var item in PolygonSegmentsByStart)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(item.Key), 0.3f);
        }
        Gizmos.color = Color.blue;
        foreach (var item in CompletedPolygons.Polygons)
        {
            DrawPolygon(item);
        }
    }

    private void DrawPolygon(Polygon poly)
    {
        DrawPolygonSegment(poly.Points, true);
        if(poly.Holes != null)
        {
            foreach (var item in poly.Holes)
            {
                DrawPolygon(item);
            }
        }
    }

    private void DrawPolygonSegment(IList<TriangulationPoint> segment, bool closed = false)
    {
        for (int i = 0; i < segment.Count - 1; i++)
        {
            if (i > 0 || closed)
                Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(segment[i]), 0.2f);
            Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(segment[i]), transform.localToWorldMatrix.MultiplyPoint(segment[i + 1]));
        }
        if (closed)
        {
            Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(segment[segment.Count - 1]), transform.localToWorldMatrix.MultiplyPoint(segment[0]));
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(segment[segment.Count - 1]), 0.2f);
        }
    }


    private void DrawPolygonSegment(IList<PolygonPoint> segment, bool closed = false)
    {
        for (int i = 0; i < segment.Count - 1; i++)
        {
            if (i > 0 || closed)
                Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(segment[i]), 0.2f);
            Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(segment[i]), transform.localToWorldMatrix.MultiplyPoint(segment[i + 1]));
        }
        if (closed)
        {
            Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint(segment[segment.Count - 1]), transform.localToWorldMatrix.MultiplyPoint(segment[0]));
            Gizmos.DrawSphere(transform.localToWorldMatrix.MultiplyPoint(segment[segment.Count - 1]), 0.2f);
        }
    }

    #endregion
}