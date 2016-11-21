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
                CreateVoxel(i, x, y);
            }
        }
        SetVoxelColors();

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "VoxelGrid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Refresh();
    }

    private void CreateVoxel(int i, int x, int y)
    {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, 0.01f, (y + 0.5f) * -voxelSize);
        o.transform.localScale = Vector3.one * voxelSize * 0.1f;
        voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
        voxels[i] = new Voxel(x, y, voxelSize);
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
            voxelMaterials[i].color = voxels[i].state ? Color.black : Color.white;
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
        if (yNeighbor != null)
        {
            TriangulateGapRow();
        }
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
                TriangulateCell(
                    voxels[i],
                    voxels[i + 1],
                    voxels[i + resolution],
                    voxels[i + resolution + 1]);
            }
            if (xNeighbor != null)
            {
                TriangulateGapCell(i);
            }
        }
    }

    private void TriangulateGapCell(int i)
    {
        Voxel dummySwap = dummyT;
        dummySwap.BecomeXDummyOf(xNeighbor.voxels[i + 1], gridSize);
        dummyT = dummyX;
        dummyX = dummySwap;
        TriangulateCell(voxels[i], dummyT, voxels[i + resolution], dummyX);
    }

    private void TriangulateGapRow()
    {
        dummyY.BecomeYDummyOf(yNeighbor.voxels[0], gridSize);
        int cells = resolution - 1;
        int offset = cells * resolution;

        for (int x = 0; x < cells; x++)
        {
            Voxel dummySwap = dummyT;
            dummySwap.BecomeYDummyOf(yNeighbor.voxels[x + 1], gridSize);
            dummyT = dummyY;
            dummyY = dummySwap;
            TriangulateCell(voxels[x + offset], voxels[x + offset + 1], dummyT, dummyY);
        }
        if (xNeighbor != null)
        {
            dummyT.BecomeXYDummyOf(xyNeighbor.voxels[0], gridSize);
            TriangulateCell(voxels[voxels.Length - 1], dummyX, dummyY, dummyT);
        }
    }

    private void TriangulateCell(Voxel a, Voxel b, Voxel c, Voxel d)
    {
        CornerType cornerType = CornerType.Square;
        bool saddleCrossing = true;
        int cellType = 0;
        if (a.state)
        {
            cellType |= 1;
        }
        if (b.state)
        {
            cellType |= 2;
        }
        if (c.state)
        {
            cellType |= 4;
        }
        if (d.state)
        {
            cellType |= 8;
        }

        switch (cellType)
        {
            case 0:
                return;
            case 1:
                AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                AddCorner(a.yEdgePosition, a.xEdgePosition, a.cornerPosition, cornerType);
                break;
            case 2:
                AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                AddCorner(a.xEdgePosition, b.yEdgePosition, a.cornerPosition, cornerType);
                break;
            case 4:
                AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                AddCorner(c.xEdgePosition, a.yEdgePosition, a.cornerPosition, cornerType);
                break;
            case 8:
                AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                AddCorner(b.yEdgePosition, c.xEdgePosition, a.cornerPosition, cornerType);
                break;
            case 3:
                AddQuad(a.position, a.yEdgePosition, b.yEdgePosition, b.position);
                AddStraight(a.yEdgePosition, b.yEdgePosition);
                break;
            case 5:
                AddQuad(a.position, c.position, c.xEdgePosition, a.xEdgePosition);
                AddStraight(c.xEdgePosition, a.xEdgePosition);
                break;
            case 10:
                AddQuad(a.xEdgePosition, c.xEdgePosition, d.position, b.position);
                AddStraight(a.xEdgePosition, c.xEdgePosition);
                break;
            case 12:
                AddQuad(a.yEdgePosition, c.position, d.position, b.yEdgePosition);
                AddStraight(b.yEdgePosition, a.yEdgePosition);
                break;
            case 15:
                AddQuad(a.position, c.position, d.position, b.position);
                break;
            case 7:
                AddPentagon(a.position, c.position, c.xEdgePosition, b.yEdgePosition, b.position);
                AddCorner(c.xEdgePosition, b.yEdgePosition, a.cornerPosition, cornerType);
                break;
            case 11:
                AddPentagon(b.position, a.position, a.yEdgePosition, c.xEdgePosition, d.position);
                AddCorner(a.yEdgePosition, c.xEdgePosition, a.cornerPosition, cornerType);
                break;
            case 13:
                AddPentagon(c.position, d.position, b.yEdgePosition, a.xEdgePosition, a.position);
                AddCorner(b.yEdgePosition, a.xEdgePosition, a.cornerPosition, cornerType);
                break;
            case 14:
                AddPentagon(d.position, b.position, a.xEdgePosition, a.yEdgePosition, c.position);
                AddCorner(a.xEdgePosition, a.yEdgePosition, a.cornerPosition, cornerType);
                break;

            case 6:
                if (saddleCrossing)
                {
                    AddHexagon(b.position, a.xEdgePosition, a.yEdgePosition, c.position, c.xEdgePosition, b.yEdgePosition);
                }
                else
                {
                    AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                    AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                }

                break;
            case 9:
                if (saddleCrossing)
                {
                    AddHexagon(a.position, a.yEdgePosition, c.xEdgePosition, d.position, b.yEdgePosition, a.xEdgePosition);
                }
                else
                {
                    AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                    AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                }
                break;
        }
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
                break;
            default:
                break;
        }
        AddLineSegment(start, end);
    }

    private void AddStraight(PolygonPoint a, PolygonPoint b, CornerType type = CornerType.Diamond)
    {
        AddLineSegment(a, b);
    }


    private void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
    }

    private void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 2);
    }

    private void AddPentagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        vertices.Add(e);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 4);
        triangles.Add(vertexIndex + 3);
    }

    private void AddHexagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e, Vector3 f)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        vertices.Add(e);
        vertices.Add(f);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 4);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 5);
        triangles.Add(vertexIndex + 4);
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