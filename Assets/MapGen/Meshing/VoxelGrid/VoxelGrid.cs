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
                break;
            case 2:
                AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                break;
            case 4:
                AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                break;
            case 8:
                AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                break;
            case 3:
                AddQuad(a.position, a.yEdgePosition, b.yEdgePosition, b.position);
                break;
            case 5:
                AddQuad(a.position, c.position, c.xEdgePosition, a.xEdgePosition);
                break;
            case 10:
                AddQuad(a.xEdgePosition, c.xEdgePosition, d.position, b.position);
                break;
            case 12:
                AddQuad(a.yEdgePosition, c.position, d.position, b.yEdgePosition);
                break;
            case 15:
                AddQuad(a.position, c.position, d.position, b.position);
                break;
            case 7:
                AddPentagon(a.position, c.position, c.xEdgePosition, b.yEdgePosition, b.position);
                break;
            case 11:
                AddPentagon(b.position, a.position, a.yEdgePosition, c.xEdgePosition, d.position);
                break;
            case 13:
                AddPentagon(c.position, d.position, b.yEdgePosition, a.xEdgePosition, a.position);
                break;
            case 14:
                AddPentagon(d.position, b.position, a.xEdgePosition, a.yEdgePosition, c.position);
                break;

            case 6:
                if (true)
                    AddHexagon(b.position, a.xEdgePosition, a.yEdgePosition, c.position, c.xEdgePosition, b.yEdgePosition);
                else
                {
                    AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                    AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                }

                break;
            case 9:
                if (true)
                    AddHexagon(a.position, a.yEdgePosition, c.xEdgePosition, d.position, b.yEdgePosition, a.xEdgePosition);
                else
                {
                    AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                    AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                }
                break;
        }
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
}