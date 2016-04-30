using DFHack;
using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RegionMaker : MonoBehaviour
{
    public float scale = 0.01f;
    public int width;
    public int height;
    public string worldNameEnglish;
    int[,] elevation;
    int[,] rainfall;
    int[,] vegetation;
    int[,] temperature;
    int[,] evilness;
    int[,] drainage;
    int[,] volcanism;
    int[,] savagery;
    int[,] salinity;

    public Vector3 offset;
    Vector3 embarkTileOffset;

    DFCoord regionOrigin;

    Mesh terrainMesh;

    int CoordToIndex(int x, int y)
    {
        return x + y * width;
    }

    MeshFilter meshFilter;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    DFCoord fortOrigin = new DFCoord(-1, -1, -1);

    public void Update()
    {
        if(fortOrigin != DFConnection.Instance.EmbarkMapPosition)
        {
            fortOrigin = DFConnection.Instance.EmbarkMapPosition;
            GenerateMesh();
        }
    }

    public void SetPosition(WorldMap mainMap)
    {
        if (mainMap != null)
        {
            offset = (new Vector3(-mainMap.center_x, 0, mainMap.center_y) + embarkTileOffset) * 48 * GameMap.tileWidth;
        }
        else
        {
            offset = embarkTileOffset * 48 * GameMap.tileWidth;
        }

        transform.position = offset * scale;
    }

    public void CopyFromRemote(WorldMap remoteMap, WorldMap mainMap)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        width = remoteMap.world_width;
        height = remoteMap.world_height;
        worldNameEnglish = remoteMap.name_english;
        embarkTileOffset = new Vector3((remoteMap.map_x * 16), 0, -(remoteMap.map_y * 16));
        regionOrigin = new DFCoord(remoteMap.map_x * 16, remoteMap.map_y * 16, 0);
        SetPosition(mainMap);
        InitArrays();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = y * width + x;
                elevation[x, y] = remoteMap.elevation[index];
                rainfall[x, y] = remoteMap.rainfall[index];
                vegetation[x, y] = remoteMap.vegetation[index];
                temperature[x, y] = remoteMap.temperature[index];
                evilness[x, y] = remoteMap.evilness[index];
                drainage[x, y] = remoteMap.drainage[index];
                volcanism[x, y] = remoteMap.volcanism[index];
                savagery[x, y] = remoteMap.savagery[index];
                salinity[x, y] = remoteMap.salinity[index];
            }
        GenerateMesh();
    }

    void InitArrays()
    {
        elevation = new int[width, height];
        rainfall = new int[width, height];
        vegetation = new int[width, height];
        temperature = new int[width, height];
        evilness = new int[width, height];
        drainage = new int[width, height];
        volcanism = new int[width, height];
        savagery = new int[width, height];
        salinity = new int[width, height];
    }

    //// Does about what you'd think it does.
    //void Start()
    //{
    //    enabled = false;

    //    DFConnection.RegisterConnectionCallback(this.OnConnectToDF);
    //}

    //void OnConnectToDF()
    //{
    //    enabled = true;
    //    CopyFromRemote(DFConnection.Instance.NetWorldMap, DFConnection.Instance.NetMapInfo);
    //}

    bool IsInCoords(DFCoord min, DFCoord max, int x, int y)
    {
        if (x < min.x)
            return false;
        if (y < min.y)
            return false;
        if (x >= max.x)
            return false;
        if (y >= max.y)
            return false;
        return true;
    }

    void GenerateMesh()
    {
        int h = height - 1;
        int w = width - 1;
        int length = width * height * 4;
        List<Vector3> vertices = new List<Vector3>(length);
        List<Color> colors = new List<Color>(length);
        List<Vector2> uvs = new List<Vector2>(length);
        List<Vector2> uv2s = new List<Vector2>(length);
        List<int> triangles = new List<int>(length);

        DFCoord fortMin = DFConnection.Instance.EmbarkMapPosition - regionOrigin;
        DFCoord fortMax = fortMin + (DFConnection.Instance.EmbarkMapSize / 3);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (IsInCoords(fortMin, fortMax, x, y))
                    continue;
                int index = vertices.Count;
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                    {
                        vertices.Add(new Vector3(
                    (x + i) * 48 * GameMap.tileWidth,
                    elevation[x, y] * GameMap.tileHeight,
                    -(y + j) * 48 * GameMap.tileWidth) * scale);

                        colors.Add(Color.white);

                        uv2s.Add(new Vector2(rainfall[x, y], 100 - drainage[x, y]) / 100);
                        uvs.Add(new Vector2(x + i, y + j));
                    }
                triangles.Add(index);
                triangles.Add(index + 2);
                triangles.Add(index + 1);

                triangles.Add(index + 1);
                triangles.Add(index + 2);
                triangles.Add(index + 3);

                Vector2 biome = new Vector2(rainfall[x, y], 100 - drainage[x, y]) / 100;

                int north = 0;
                if (y > 0 && !IsInCoords(fortMin, fortMax, x, y - 1))
                    north = elevation[x, y - 1];
                if (north < elevation[x, y])
                {
                    Vector3 vert1 = new Vector3(x * 48 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -y * 48 * GameMap.tileWidth) * scale;
                    Vector3 vert2 = new Vector3((x + 1) * 48 * GameMap.tileWidth, north * GameMap.tileHeight, -y * 48 * GameMap.tileWidth) * scale;

                    AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }

                int south = 0;
                if (y < h - 1 && !IsInCoords(fortMin, fortMax, x, y + 1))
                    south = elevation[x, y + 1];
                if (south < elevation[x, y])
                {
                    Vector3 vert1 = new Vector3((x + 1) * 48 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -(y + 1) * 48 * GameMap.tileWidth) * scale;
                    Vector3 vert2 = new Vector3(x * 48 * GameMap.tileWidth, south * GameMap.tileHeight, -(y + 1) * 48 * GameMap.tileWidth) * scale;

                    AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }

                int east = 0;
                if (x < w - 1 && !IsInCoords(fortMin, fortMax, x + 1, y))
                    east = elevation[x + 1, y];
                if (east < elevation[x, y])
                {
                    index = vertices.Count;
                    Vector3 vert1 = new Vector3((x + 1) * 48 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -y * 48 * GameMap.tileWidth) * scale;
                    Vector3 vert2 = new Vector3((x + 1) * 48 * GameMap.tileWidth, east * GameMap.tileHeight, -(y + 1) * 48 * GameMap.tileWidth) * scale;

                    AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }

                int west = 0;
                if (x > 0 && !IsInCoords(fortMin, fortMax, x - 1, y))
                    west = elevation[x - 1, y];
                if (west < elevation[x, y])
                {
                    index = vertices.Count;
                    Vector3 vert1 = new Vector3(x * 48 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -(y + 1) * 48 * GameMap.tileWidth) * scale;
                    Vector3 vert2 = new Vector3(x * 48 * GameMap.tileWidth, west * GameMap.tileHeight, -y * 48 * GameMap.tileWidth) * scale;

                    AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }
                if (vertices.Count >= (65535 - 20))
                    break;
            }
            if (vertices.Count >= (65535 - 20))
                break;
        }
        terrainMesh = new Mesh();
        terrainMesh.vertices = vertices.ToArray();
        terrainMesh.colors = colors.ToArray();
        terrainMesh.uv = uvs.ToArray();
        terrainMesh.uv2 = uv2s.ToArray();
        terrainMesh.triangles = triangles.ToArray();

        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateTangents();

        meshFilter.mesh = terrainMesh;
    }

    public static void AddVerticalQuad(Vector3 vert1, Vector3 vert2, Vector2 biome, Color color, List<Vector3> vertices, List<Color> colors, List<Vector2> uvs, List<Vector2> uv2s, List<int> triangles)
    {
        int index = vertices.Count;
        vertices.Add(new Vector3(vert1.x, vert1.y, vert1.z));
        vertices.Add(new Vector3(vert2.x, vert1.y, vert2.z));
        vertices.Add(new Vector3(vert1.x, vert2.y, vert1.z));
        vertices.Add(new Vector3(vert2.x, vert2.y, vert2.z));

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        uv2s.Add(biome);
        uv2s.Add(biome);
        uv2s.Add(biome);
        uv2s.Add(biome);
        uvs.Add(new Vector2(vert1.x, vert1.y));
        uvs.Add(new Vector2(vert2.x, vert1.y));
        uvs.Add(new Vector2(vert1.x, vert2.y));
        uvs.Add(new Vector2(vert2.x, vert2.y));

        triangles.Add(index);
        triangles.Add(index + 2);
        triangles.Add(index + 1);

        triangles.Add(index + 1);
        triangles.Add(index + 2);
        triangles.Add(index + 3);
    }
}
