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

    public static bool ColorCombine(Mesh mesh, MeshInstance[] combines, int length = -1)
    {
        if (length < 0)
            length = combines.Length;
        int vertexCount = 0;
        int triangleCount = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {
                vertexCount += combines[combIndex].mesh.vertexCount;
            }
        }

        if(vertexCount > 65535)
        {
            //Debug.LogError("Combined mesh would have " + vertexCount + " vertices. Should not be more than 65535");
            return false;
        }

        // Precomputed how many triangles we need instead
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {
                triangleCount += combines[combIndex].mesh.GetTriangles(combines[combIndex].subMeshIndex).Length;
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
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
                Copy(combines[combIndex].mesh.vertexCount, combines[combIndex].mesh.vertices, vertices, ref offset, combines[combIndex].transform);
        }

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {
                Matrix4x4 invTranspose = combines[combIndex].transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyNormal(combines[combIndex].mesh.vertexCount, combines[combIndex].mesh.normals, normals, ref offset, invTranspose);
            }

        }
        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {
                Matrix4x4 invTranspose = combines[combIndex].transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyTangents(combines[combIndex].mesh.vertexCount, combines[combIndex].mesh.tangents, tangents, ref offset, invTranspose);
            }

        }
        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {
                Matrix4x4 transform = Matrix4x4.TRS(new Vector2(((combines[combIndex].uv1Index % 16) / 16.0f), ((15 - (combines[combIndex].uv2Index / 16)) / 16.0f)), Quaternion.identity, new Vector2(1.0f / 16.0f, 1.0f / 16.0f));
                Copy(combines[combIndex].mesh.vertexCount, combines[combIndex].mesh.uv, uv, ref offset, transform);
            }
        }

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {
                Matrix4x4 transform = Matrix4x4.TRS(new Vector2( ((combines[combIndex].uv2Index % 16) / 16.0f),  ((15 - (combines[combIndex].uv2Index / 16)) / 16.0f)), Quaternion.identity, new Vector2(1.0f / 16.0f, 1.0f / 16.0f));
                Copy(combines[combIndex].mesh.vertexCount, combines[combIndex].mesh.uv, uv1, ref offset, transform);
            }
        }

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
                CopyColors(combines[combIndex].mesh.vertexCount, combines[combIndex].mesh.colors, colors, ref offset, combines[combIndex].color);
        }


        int triangleOffset = 0;
        int vertexOffset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].mesh)
            {

                int[] inputtriangles = combines[combIndex].mesh.GetTriangles(combines[combIndex].subMeshIndex);
                for (int i = 0; i < inputtriangles.Length; i++)
                {
                    triangles[i + triangleOffset] = inputtriangles[i] + vertexOffset;
                }
                triangleOffset += inputtriangles.Length;

                vertexOffset += combines[combIndex].mesh.vertexCount;
            }
        }
        mesh.Clear();
        mesh.name = "Combined Mesh";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.uv2 = uv1;
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
