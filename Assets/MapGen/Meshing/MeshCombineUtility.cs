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
        int maxVertices = 0;
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                int count = combines[combIndex].meshData.vertexCount;
                vertexCount += count;
                if (maxVertices < count)
                    maxVertices = count;
            }
        }
        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                triangleCount += combines[combIndex].meshData.triangles.Length;
            }
        }

        List<Vector3> vertices = new List<Vector3>(vertexCount);
        List<Vector3> normals = new List<Vector3>(vertexCount);
        List<Vector4> tangents = new List<Vector4>(vertexCount);
        List<Vector2> uvs = new List<Vector2>(vertexCount);
        List<Vector2> uv2s = new List<Vector2>(vertexCount);
        List<Color> colors = new List<Color>(vertexCount);

        Dictionary<int, int> indexTranslation = new Dictionary<int, int>(maxVertices);

        List<int> triangles = new List<int>(triangleCount);

        for (int combIndex = 0; combIndex < length; combIndex++)
        {
            if (combines[combIndex].meshData != null)
            {
                int[] inputtriangles = combines[combIndex].meshData.triangles;
                var inputVertices = combines[combIndex].meshData.vertices;
                var inputNormals = combines[combIndex].meshData.normals;
                var inputUVs = combines[combIndex].meshData.uv;
                var inputColors = combines[combIndex].meshData.colors;
                var inputTangents = combines[combIndex].meshData.tangents;
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

                    int newVert0 = getIndex(indexTranslation, combines[combIndex], vert0, vertices, normals, tangents, uvs, uv2s, colors, inputVertices, inputNormals, inputTangents, inputUVs, inputColors);
                    int newVert1 = getIndex(indexTranslation, combines[combIndex], vert1, vertices, normals, tangents, uvs, uv2s, colors, inputVertices, inputNormals, inputTangents, inputUVs, inputColors);
                    int newVert2 = getIndex(indexTranslation, combines[combIndex], vert2, vertices, normals, tangents, uvs, uv2s, colors, inputVertices, inputNormals, inputTangents, inputUVs, inputColors);

                    if (newVert0 > 65531 || newVert1 > 65531 || newVert2 > 65531)
                        goto failure;

                    triangles.Add(newVert0);
                    triangles.Add(newVert1);
                    triangles.Add(newVert2);
                    copiedTriangles += 3;
                }
            }
            indexTranslation.Clear();
        }
        success = true;
        return new MeshData(
            vertices: vertices.ToArray(),
            normals: normals.ToArray(),
            tangents: tangents.ToArray(),
            uv: uvs.ToArray(),
            uv2: uv2s.ToArray(),
            colors: colors.ToArray(),
            triangles: triangles.ToArray()
            );
        failure:
        success = false;
        return new MeshData(
            vertices: vertices.ToArray(),
            normals: normals.ToArray(),
            tangents: tangents.ToArray(),
            uv: uvs.ToArray(),
            uv2: uv2s.ToArray(),
            colors: colors.ToArray(),
            triangles: triangles.ToArray()
            );
    }

    private static int getIndex(Dictionary<int, int> indexTranslation, MeshInstance meshInstance, int inputVert, List<Vector3> vertices, List<Vector3> normals, List<Vector4> tangents, List<Vector2> uvs, List<Vector2> uv2s, List<Color> colors, Vector3[] inputVertices, Vector3[] inputNormals, Vector4[] inputTangents, Vector2[] inputUVs, Color[] inputColors)
    {
        int newVert;
        if (indexTranslation.ContainsKey(inputVert))
        {
            newVert = indexTranslation[inputVert];
        }
        else
        {
            newVert = vertices.Count;
            indexTranslation[inputVert] = newVert;
            vertices.Add(meshInstance.transform.MultiplyPoint(inputVertices[inputVert]));

            Matrix4x4 invTranspose = meshInstance.transform;
            invTranspose = invTranspose.inverse.transpose;
            normals.Add(invTranspose.MultiplyVector(inputNormals[inputVert]).normalized);
            Vector4 p4 = inputTangents[inputVert];
            Vector3 p = new Vector3(p4.x, p4.y, p4.z);
            p = invTranspose.MultiplyVector(p).normalized;
            tangents.Add(invTranspose.MultiplyVector(new Vector4(p.x, p.y, p.z, p4.w)));
            uvs.Add(meshInstance.uv1Transform.MultiplyPoint(inputUVs[inputVert]));
            uv2s.Add(meshInstance.uv2Transform.MultiplyPoint(inputUVs[inputVert]));
            if (inputColors.Length > 0)
                colors.Add(meshInstance.color * inputColors[inputVert]);
            else
                colors.Add(meshInstance.color);
        }
        return newVert;
    }
}
