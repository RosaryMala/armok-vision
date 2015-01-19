using UnityEngine;
using System.Collections;

public class MeshCombineUtility
{

    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
        public Color color;
        public int uv1Index;
        public int uv2Index;
    }

    public static bool ColorCombine(Mesh mesh, MeshInstance[] combines)
    {
        int vertexCount = 0;
        int triangleCount = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                vertexCount += combine.mesh.vertexCount;
            }
        }

        if(vertexCount > 65535)
        {
            //Debug.LogError("Combined mesh would have " + vertexCount + " vertices. Should not be more than 65535");
            return false;
        }

        // Precomputed how many triangles we need instead
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
            }
        }

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector4[] tangents = new Vector4[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        Vector2[] uv1 = new Vector2[vertexCount];
        Color[] colors = new Color[vertexCount];

        int[] triangles = new int[triangleCount];

        int offset;

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 invTranspose = combine.transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
            }

        }
        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 invTranspose = combine.transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
            }

        }
        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 transform = Matrix4x4.TRS(new Vector2(((combine.uv1Index % 16) / 16.0f), ((15 - (combine.uv2Index / 16)) / 16.0f)), Quaternion.identity, new Vector2(1.0f / 16.0f, 1.0f / 16.0f));
                Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset, transform);
            }
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 transform = Matrix4x4.TRS(new Vector2( ((combine.uv2Index % 16) / 16.0f),  ((15 - (combine.uv2Index / 16)) / 16.0f)), Quaternion.identity, new Vector2(1.0f / 16.0f, 1.0f / 16.0f));
                Copy(combine.mesh.vertexCount, combine.mesh.uv, uv1, ref offset, transform);
            }
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                CopyColors(combine.mesh.vertexCount, combine.mesh.colors, colors, ref offset, combine.color);
        }


        int triangleOffset = 0;
        int vertexOffset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {

                int[] inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
                for (int i = 0; i < inputtriangles.Length; i++)
                {
                    triangles[i + triangleOffset] = inputtriangles[i] + vertexOffset;
                }
                triangleOffset += inputtriangles.Length;

                vertexOffset += combine.mesh.vertexCount;
            }
        }
        mesh.Clear();
        mesh.name = "Combined Mesh";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.uv1 = uv1;
        mesh.tangents = tangents;
        mesh.triangles = triangles;
        return true;
    }

    static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyPoint(src[i]);
        offset += vertexcount;
    }
    static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyPoint(src[i]);
        offset += vertexcount;
    }

    static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
        offset += vertexcount;
    }

    static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = src[i];
        offset += vertexcount;
    }

    static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset, Color color)
    {
        if(src.Length > 0)
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = src[i] * color;
        else
            for (int i = 0; i < vertexcount; i++)
                dst[i + offset] = color;
        offset += vertexcount;
    }

    static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
        {
            Vector4 p4 = src[i];
            Vector3 p = new Vector3(p4.x, p4.y, p4.z);
            p = transform.MultiplyVector(p).normalized;
            dst[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
        }

        offset += vertexcount;
    }
}
