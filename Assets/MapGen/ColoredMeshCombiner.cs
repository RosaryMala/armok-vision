using UnityEngine;
using System.Collections;

public struct ColorCombineInstance
{
    public Mesh mesh;
    public Matrix4x4 transform;
    public int submeshindex;
    public Matrix4x4 uvTransform;
    public Color32 color;
}

public static class ColoredMeshCombiner
{
    public static void CombineMeshesColored(this Mesh mesh, ColorCombineInstance[] combine, bool mergeSubMeshes = true, bool useMatrices = true)
    {
        int totalVertices = 0;
        int totalTriangles = 0;
        for(int i = 0; i < combine.Length; i++)
        {
            totalVertices += combine[i].mesh.vertices.Length;
            totalTriangles += combine[i].mesh.triangles.Length;
        }
        mesh.Clear();
        mesh.vertices = new Vector3[totalVertices];
        mesh.triangles = new int[totalTriangles];
        int startingVertex = 0;
        for(int i = 0; i < combine.Length; i++)
        {
            ColorCombineInstance instance = combine[i];

        }
    }
}
