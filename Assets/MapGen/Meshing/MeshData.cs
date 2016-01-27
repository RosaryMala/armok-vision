using UnityEngine;
using UnityExtension;

// Stores data like a Mesh, but doesn't talk to the graphics card.
public class MeshData
{
    // Everything is readonly because this will be accessed from
    // multiple threads, and we don't want race conditions
    public readonly int vertexCount;
    public readonly Vector3[] vertices;
    public readonly Vector3[] normals;
    public readonly Vector4[] tangents;
    public readonly Vector2[] uv;
    public readonly Vector2[] uv2;
    public readonly Color[] colors;
    public readonly int[] triangles;
    public readonly string name;

    public MeshData (Mesh target)
    {
        vertexCount = target.vertexCount;
        vertices = target.vertices;
        normals = target.normals;
        tangents = target.tangents;
        uv = target.uv;
        uv2 = target.uv2;
        colors = target.colors;
        if(target.GetTopology(0) == MeshTopology.Triangles)
            triangles = target.GetTriangles(0); // Will the submesh always be 0?
        else
            triangles = new int[0];
        name = target.name;
    }

    public MeshData (
     Vector3[] vertices,
     Vector3[] normals,
     Vector4[] tangents,
     Vector2[] uv,
     Vector2[] uv2,
     Color[] colors,
     int[] triangles,
     string name = null)
    {
        this.vertexCount = vertices.Length;
        this.vertices = vertices;
        this.normals = normals;
        this.tangents = tangents;
        this.uv = uv;
        this.uv2 = uv2;
        this.colors = colors;
        this.triangles = triangles;
        this.name = name;
    }

    public void CopyToMesh(Mesh target) {
        target.vertices = vertices;
        target.normals = normals;
        target.tangents = tangents;
        target.uv = uv;
        target.uv2 = uv2;
        target.colors = colors;
        target.triangles = triangles;
        target.RecalculateBounds();
        target.RecalculateTangents();
        target.name = name;
    }
}
