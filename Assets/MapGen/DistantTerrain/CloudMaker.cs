﻿using UnityEngine;
using UnityExtension;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class CloudMaker : MonoBehaviour 
{
    int width, height;
    int CoordToIndex(int x, int y)
    {
        return x + y * width;
    }
    public Vector3 offset;
    public float scale;
    MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void GenerateMesh(bool[,] cloudArray)
    {
        width = cloudArray.GetLength(0);
        height = cloudArray.GetLength(1);
        if (width * height > 65535)
        {
            width = Mathf.Clamp(width, 0, 255);
            height = Mathf.Clamp(height, 0, 255);
        }
        Vector3[] vertexPositions = new Vector3[width * height];
        Color[] vertexColors = new Color[width * height];
        Vector2[] vertexUV = new Vector2[width * height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = CoordToIndex(x, y);
                vertexPositions[index] = (new Vector3(
                    x * 16 * 48 * GameMap.tileWidth,
                    0,
                    -y * 16 * 48 * GameMap.tileWidth) + offset) * scale;
                vertexColors[index] = Color.white;
                if (cloudArray[x, y])
                    vertexColors[index].a = 1;
                else
                    vertexColors[index].a = 0;
                vertexUV[index] = new Vector2(x, y);
            }

        List<int> triangles = new List<int>();
        for (int x = 0; x < width - 1; x++)
            for (int y = 0; y < height - 1; y++)
            {
                if (vertexColors[CoordToIndex(x, y)].a > 0
                    || vertexColors[CoordToIndex(x + 1, y)].a > 0
                    || vertexColors[CoordToIndex(x + 1, y + 1)].a > 0)
                {
                    triangles.Add(CoordToIndex(x, y));
                    triangles.Add(CoordToIndex(x + 1, y));
                    triangles.Add(CoordToIndex(x + 1, y + 1));
                }
                if (vertexColors[CoordToIndex(x, y)].a > 0
                    || vertexColors[CoordToIndex(x + 1, y + 1)].a > 0
                    || vertexColors[CoordToIndex(x, y + 1)].a > 0)
                {
                    triangles.Add(CoordToIndex(x, y));
                    triangles.Add(CoordToIndex(x + 1, y + 1));
                    triangles.Add(CoordToIndex(x, y + 1));
                }
            }
        Mesh terrainMesh = new Mesh();
        terrainMesh.vertices = vertexPositions;
        terrainMesh.colors = vertexColors;
        terrainMesh.uv = vertexUV;
        terrainMesh.triangles = triangles.ToArray();

        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateTangents();

        meshFilter.mesh = terrainMesh;
    }
}
