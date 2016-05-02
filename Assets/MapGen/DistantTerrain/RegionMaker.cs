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
    public float scale;
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
    RiverTile[,] rivers;

    public Vector3 offset;
    Vector3 embarkTileOffset;

    DFCoord regionOrigin;

    Mesh terrainMesh;

    int CoordToIndex(int x, int y)
    {
        return x + y * width;
    }

    MeshFilter meshFilter;
    WorldMapMaker parentMap;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        parentMap = FindObjectOfType<WorldMapMaker>();
    }

    MeshFilter waterChunk;


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

        offset += new Vector3(-0.5f, 0, 0.5f) * GameMap.tileWidth;

        transform.position = offset;
        transform.localScale = new Vector3(scale, scale, scale);
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
                if (remoteMap.river_tiles != null && remoteMap.river_tiles.Count > index)
                    rivers[x, y] = remoteMap.river_tiles[index];
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
        rivers = new RiverTile[width, height];
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

    List<Vector3> vertices = new List<Vector3>();
    List<Color> colors = new List<Color>();
    List<Vector2> uvCoords = new List<Vector2>();
    List<Vector2> uvCoords2 = new List<Vector2>();
    List<int> triangles = new List<int>();

    List<Vector3> waterVerts = new List<Vector3>();
    List<Vector2> waterUvs = new List<Vector2>();
    List<int> waterTris = new List<int>();


    void GenerateMesh()
    {
        int h = height - 1;
        int w = width - 1;
        vertices.Clear();
        colors.Clear();
        uvCoords.Clear();
        uvCoords2.Clear();
        triangles.Clear();
        waterVerts.Clear();
        waterUvs.Clear();
        waterTris.Clear();

        DFCoord fortMin = DFConnection.Instance.EmbarkMapPosition - regionOrigin;
        DFCoord fortMax = fortMin + (DFConnection.Instance.EmbarkMapSize / 3);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (IsInCoords(fortMin, fortMax, x, y))
                    continue;

                Vector2 biome = new Vector2(rainfall[x, y], 100 - drainage[x, y]) / 100;

                Vector3 vert1 = new Vector3(x * 48 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -y * 48 * GameMap.tileWidth);
                Vector3 vert2 = new Vector3((x + 1) * 48 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -(y + 1) * 48 * GameMap.tileWidth);

                byte riverSides = 0;

                if (rivers[x, y] != null)
                {
                    if (rivers[x, y].north.min_pos >= 0)
                        riverSides |= 1;
                    if (rivers[x, y].east.min_pos >= 0)
                        riverSides |= 2;
                    if (rivers[x, y].south.min_pos >= 0)
                        riverSides |= 4;
                    if (rivers[x, y].west.min_pos >= 0)
                        riverSides |= 8;
                }

                int north = 0;
                if (y > 0 && !IsInCoords(fortMin, fortMax, x, y - 1))
                    north = elevation[x, y - 1];

                int south = 0;
                if (y < h - 1 && !IsInCoords(fortMin, fortMax, x, y + 1))
                    south = elevation[x, y + 1];

                int east = 0;
                if (x < w - 1 && !IsInCoords(fortMin, fortMax, x + 1, y))
                    east = elevation[x + 1, y];

                int west = 0;
                if (x > 0 && !IsInCoords(fortMin, fortMax, x - 1, y))
                    west = elevation[x - 1, y];

                if (riverSides == 0)
                    AddFlatTile(vert1, biome, north * GameMap.tileHeight, east * GameMap.tileHeight, south * GameMap.tileHeight, west * GameMap.tileHeight);
                else;
                {

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
        terrainMesh.uv = uvCoords.ToArray();
        terrainMesh.uv2 = uvCoords2.ToArray();
        terrainMesh.triangles = triangles.ToArray();

        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateTangents();

        meshFilter.mesh = terrainMesh;

        if(waterVerts.Count > 0)
        {
            if (waterChunk ==  null)
            {
                waterChunk = Instantiate<MeshFilter>(parentMap.waterPrefab);
                waterChunk.transform.parent = transform;
                waterChunk.gameObject.name = "Water ";
                waterChunk.transform.localPosition = Vector3.zero;

            }

            if (waterChunk.mesh == null)
                waterChunk.mesh = new Mesh();

            Mesh waterMesh = waterChunk.mesh;
            waterMesh.Clear();

            waterMesh.vertices = waterVerts.ToArray();
            waterMesh.uv = waterUvs.ToArray();
            waterMesh.triangles = waterTris.ToArray();

            waterMesh.RecalculateNormals();
            waterMesh.RecalculateTangents();

            waterChunk.mesh = waterMesh;

            waterVerts.Clear();
            waterUvs.Clear();
            waterTris.Clear();
        }

    }

    private void AddFlatTile(Vector3 vert1, Vector2 biome, float north, float east, float south, float west)
    {
        Vector3 vert2 = vert1 + new Vector3(48 * GameMap.tileWidth, 0, -48 * GameMap.tileWidth);

        Vector3 vert3;
        Vector3 vert4;

        AddHorizontalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);

        if (vert1.y < 99 * GameMap.tileHeight)
        {
            vert3 = new Vector3(vert1.x, 99 * GameMap.tileHeight, vert1.z);
            vert4 = new Vector3(vert2.x, 99 * GameMap.tileHeight, vert2.z);

            AddHorizontalQuad(vert3, vert4, biome, Color.white, waterVerts, null, waterUvs, null, waterTris);
        }

        if (north < vert1.y)
        {
            vert3 = vert1;
            vert4 = new Vector3(vert2.x, north, vert1.z);

            AddVerticalQuad(vert3, vert4, biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            vert3 = vert2;
            vert4 = new Vector3(vert1.x, south, vert2.z);

            AddVerticalQuad(vert3, vert4, biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            vert3 = new Vector3(vert2.x, vert1.y, vert1.z);
            vert4 = new Vector3(vert2.x, east, vert2.z);

            AddVerticalQuad(vert3, vert4, biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            vert3 = new Vector3(vert1.x, vert1.y, vert2.z);
            vert4 = new Vector3(vert1.x, west, vert1.z);

            AddVerticalQuad(vert3, vert4, biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }
    }

    public static void AddHorizontalQuad(Vector3 vert1, Vector3 vert2, Vector2 biome, Color color, List<Vector3> vertices, List<Color> colors, List<Vector2> uvs, List<Vector2> uv2s, List<int> triangles)
    {
        int index = vertices.Count;
        vertices.Add(new Vector3(vert1.x, vert1.y, vert1.z));
        vertices.Add(new Vector3(vert2.x, vert1.y, vert1.z));
        vertices.Add(new Vector3(vert1.x, vert2.y, vert2.z));
        vertices.Add(new Vector3(vert2.x, vert2.y, vert2.z));

        if (colors != null)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        if (uv2s != null)
        {
            uv2s.Add(biome);
            uv2s.Add(biome);
            uv2s.Add(biome);
            uv2s.Add(biome);
        }
        if (uvs != null)
        {
            uvs.Add(new Vector2(vert1.x, vert1.z));
            uvs.Add(new Vector2(vert2.x, vert1.z));
            uvs.Add(new Vector2(vert1.x, vert2.z));
            uvs.Add(new Vector2(vert2.x, vert2.z));
        }

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 1);
        triangles.Add(index + 3);
        triangles.Add(index + 2);
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
