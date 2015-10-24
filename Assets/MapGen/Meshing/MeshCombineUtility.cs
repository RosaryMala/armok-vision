using UnityEngine;
using System.Collections.Generic;
using System;

public class MeshCombineUtility
{
    const float edgeThreshold = 0.01f;
    const float topThreshold = (GameMap.tileHeight / 2) - edgeThreshold;
    const float sideThreshold = (GameMap.tileWidth / 2) - edgeThreshold;

    [Flags]
    public enum HiddenFaces
    {
        None = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8,
        Up = 16,
        Down = 32,
        All = North | South | East | West | Up | Down
    }
    public struct MeshInstance
    {
        public MeshData meshData;
        public Matrix4x4 transform;
        public Color color;
        public Matrix4x4 uv1Transform;
        public Matrix4x4 uv2Transform;
        public HiddenFaces hiddenFaces;
    }

    public static MeshData ColorCombine(MeshInstance[] combines, out bool success)
    {
        
        int length = combines.Length;
        int vertexCount = 0;
        int triangleCount = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                vertexCount += combines[combIndex].meshData.vertexCount;
            }
        }

        if(vertexCount > 65535)
        {
            //Debug.LogError("Combined mesh would have " + vertexCount + " vertices. Should not be more than 65535");
            success = false;
            return null;
        }

        // Precomputed how many triangles we need instead
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                triangleCount += combines[combIndex].meshData.triangles.Length;
            }
        }

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector4[] tangents = new Vector4[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        Vector2[] uv2 = new Vector2[vertexCount];
        Color[] colors = new Color[vertexCount];

        List<int> triangles = new List<int>();
        triangles.Capacity = triangleCount;

        int offset;

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
                Copy(combines[combIndex].meshData.vertexCount, combines[combIndex].meshData.vertices, vertices, ref offset, combines[combIndex].transform);
        }

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                Matrix4x4 invTranspose = combines[combIndex].transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyNormal(combines[combIndex].meshData.vertexCount, combines[combIndex].meshData.normals, normals, ref offset, invTranspose);
            }

        }
        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                Matrix4x4 invTranspose = combines[combIndex].transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyTangents(combines[combIndex].meshData.vertexCount, combines[combIndex].meshData.tangents, tangents, ref offset, invTranspose);
            }

        }
        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                Copy(combines[combIndex].meshData.vertexCount, combines[combIndex].meshData.uv, uv, ref offset, combines[combIndex].uv1Transform);
            }
        }

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                Copy(combines[combIndex].meshData.vertexCount, combines[combIndex].meshData.uv, uv2, ref offset, combines[combIndex].uv2Transform);
            }
        }

        offset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
                CopyColors(combines[combIndex].meshData.vertexCount, combines[combIndex].meshData.colors, colors, ref offset, combines[combIndex].color);
        }


        int triangleOffset = 0;
        int vertexOffset = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {

                int[] inputtriangles = combines[combIndex].meshData.triangles;
                var inputVertices = combines[combIndex].meshData.vertices;
                int copiedTriangles = 0;
                for (int i = 0; i < inputtriangles.Length; i += 3)
                {
                    int vert0 = inputtriangles[i + 0];
                    int vert1 = inputtriangles[i + 1];
                    int vert2 = inputtriangles[i + 2];
                    if (((combines[combIndex].hiddenFaces & HiddenFaces.Up) == HiddenFaces.Up)
                        && (inputVertices[vert0].y > topThreshold)
                        && (inputVertices[vert1].y > topThreshold)
                        && (inputVertices[vert2].y > topThreshold))
                        continue;
                    if (((combines[combIndex].hiddenFaces & HiddenFaces.Down) == HiddenFaces.Down)
                        && (inputVertices[vert0].y < -topThreshold)
                        && (inputVertices[vert1].y < -topThreshold)
                        && (inputVertices[vert2].y < -topThreshold))
                        continue;
                    if (((combines[combIndex].hiddenFaces & HiddenFaces.North) == HiddenFaces.North)
                        && (inputVertices[vert0].z > sideThreshold)
                        && (inputVertices[vert1].z > sideThreshold)
                        && (inputVertices[vert2].z > sideThreshold))
                        continue;
                    if (((combines[combIndex].hiddenFaces & HiddenFaces.South) == HiddenFaces.South)
                        && (inputVertices[vert0].z < -sideThreshold)
                        && (inputVertices[vert1].z < -sideThreshold)
                        && (inputVertices[vert2].z < -sideThreshold))
                        continue;
                    if (((combines[combIndex].hiddenFaces & HiddenFaces.East) == HiddenFaces.East)
                        && (inputVertices[vert0].x > sideThreshold)
                        && (inputVertices[vert1].x > sideThreshold)
                        && (inputVertices[vert2].x > sideThreshold))
                        continue;
                    if (((combines[combIndex].hiddenFaces & HiddenFaces.West) == HiddenFaces.West)
                        && (inputVertices[vert0].x < -sideThreshold)
                        && (inputVertices[vert1].x < -sideThreshold)
                        && (inputVertices[vert2].x < -sideThreshold))
                        continue;
                    triangles.Add(vert0 + vertexOffset);
                    triangles.Add(vert1 + vertexOffset);
                    triangles.Add(vert2 + vertexOffset);
                    copiedTriangles += 3;
                }
                triangleOffset += copiedTriangles;

                vertexOffset += combines[combIndex].meshData.vertexCount;
            }
        }
        success = true;
        return new MeshData(
            vertices: vertices,
            normals: normals,
            tangents: tangents,
            uv: uv,
            uv2: uv2,
            colors: colors,
            triangles: triangles.ToArray()
            );
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
