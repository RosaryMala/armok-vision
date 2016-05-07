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

    [Flags]
    enum Sides
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8
    }

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

                Sides riverSides = 0;

                if (rivers[x, y] != null)
                {
                    if (rivers[x, y].north.min_pos >= 0)
                        riverSides |= Sides.North;
                    if (rivers[x, y].east.min_pos >= 0)
                        riverSides |= Sides.East;
                    if (rivers[x, y].south.min_pos >= 0)
                        riverSides |= Sides.South;
                    if (rivers[x, y].west.min_pos >= 0)
                        riverSides |= Sides.West;
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
                else
                {
                    AddRiverTile(riverSides, rivers[x,y], vert1, biome, north * GameMap.tileHeight, east * GameMap.tileHeight, south * GameMap.tileHeight, west * GameMap.tileHeight);
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

    private void AddRiverTile(Sides riverSides, RiverTile riverTile, Vector3 vert1, Vector2 biome, float north, float east, float south, float west)
    {
        Vector3[,] verts = new Vector3[4,4];

        int waterLevel = int.MaxValue;

        if (riverTile.north.min_pos >= 0)
            waterLevel = Mathf.Min(riverTile.north.elevation, waterLevel);
        if (riverTile.east.min_pos >= 0)
            waterLevel = Mathf.Min(riverTile.east.elevation, waterLevel);
        if (riverTile.south.min_pos >= 0)
            waterLevel = Mathf.Min(riverTile.south.elevation, waterLevel);
        if (riverTile.west.min_pos >= 0)
            waterLevel = Mathf.Min(riverTile.west.elevation, waterLevel);

        //setup a few vertices that are used everywhere.
        verts[0, 0] = vert1;
        verts[3, 3] = vert1 + new Vector3(48 * GameMap.tileWidth, 0, -48 * GameMap.tileWidth);
        verts[0, 3] = new Vector3(verts[0, 0].x, verts[0, 0].y, verts[3, 3].z);
        verts[3, 0] = new Vector3(verts[3, 3].x, verts[0, 0].y, verts[0, 0].z);

        verts[1, 0] = verts[0, 0] + new Vector3(riverTile.north.min_pos * GameMap.tileWidth, 0, 0);
        verts[2, 0] = verts[0, 0] + new Vector3((riverTile.north.max_pos + 1) * GameMap.tileWidth, 0, 0);

        verts[1, 3] = verts[0, 3] + new Vector3(riverTile.south.min_pos * GameMap.tileWidth, 0, 0);
        verts[2, 3] = verts[0, 3] + new Vector3((riverTile.south.max_pos + 1) * GameMap.tileWidth, 0, 0);

        verts[0, 1] = verts[0, 0] + new Vector3(0, 0, -riverTile.west.min_pos * GameMap.tileWidth);
        verts[0, 2] = verts[0, 0] + new Vector3(0, 0, -(riverTile.west.max_pos + 1) * GameMap.tileWidth);

        verts[3, 1] = verts[3, 0] + new Vector3(0, 0, -riverTile.east.min_pos * GameMap.tileWidth);
        verts[3, 2] = verts[3, 0] + new Vector3(0, 0, -(riverTile.east.max_pos + 1) * GameMap.tileWidth);

        int index = vertices.Count;
        int waterIndex = waterVerts.Count;

        switch (riverSides)
        {
            case Sides.North | Sides.South:
                {
                    //first add the land
                    vertices.Add(verts[0, 0]);
                    vertices.Add(verts[1, 0]);
                    vertices.Add(verts[0, 3]);
                    vertices.Add(verts[1, 3]);

                    vertices.Add(verts[3, 0]);
                    vertices.Add(verts[3, 3]);
                    vertices.Add(verts[2, 0]);
                    vertices.Add(verts[2, 3]);

                    vertices.Add(new Vector3(verts[1, 0].x, (waterLevel - 1) * GameMap.tileHeight, verts[1, 0].z));
                    vertices.Add(new Vector3(verts[2, 0].x, (waterLevel - 1) * GameMap.tileHeight, verts[2, 0].z));
                    vertices.Add(new Vector3(verts[1, 3].x, (waterLevel - 1) * GameMap.tileHeight, verts[1, 3].z));
                    vertices.Add(new Vector3(verts[2, 3].x, (waterLevel - 1) * GameMap.tileHeight, verts[2, 3].z));

                    for (int i = 0; i < 12; i++)
                    {
                        uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
                        uvCoords2.Add(biome);
                        colors.Add(Color.white);
                    }

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);

                    triangles.Add(index + 1);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);
                    
                    triangles.Add(index + 4);
                    triangles.Add(index + 5);
                    triangles.Add(index + 6);

                    triangles.Add(index + 5);
                    triangles.Add(index + 7);
                    triangles.Add(index + 6);

                    triangles.Add(index + 8);
                    triangles.Add(index + 9);
                    triangles.Add(index + 10);

                    triangles.Add(index + 9);
                    triangles.Add(index + 11);
                    triangles.Add(index + 10);

                    //now the river banks
                    AddVerticalQuad(verts[1, 0], new Vector3(verts[1, 3].x, (waterLevel - 1) * GameMap.tileHeight, verts[1, 3].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    AddVerticalQuad(verts[2, 3], new Vector3(verts[2, 0].x, (waterLevel - 1) * GameMap.tileHeight, verts[2, 0].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);

                    //Tile edges
                    if (north < vert1.y)
                    {
                        AddVerticalQuad(verts[0, 0], new Vector3(verts[1, 0].x, north, verts[1, 0].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                        AddVerticalQuad(verts[2, 0], new Vector3(verts[3, 0].x, north, verts[3, 0].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }

                    if (south < vert1.y)
                    {
                        AddVerticalQuad(verts[3, 3], new Vector3(verts[2, 3].x, south, verts[2, 3].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                        AddVerticalQuad(verts[1, 3], new Vector3(verts[0, 3].x, south, verts[0, 3].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }

                    if (east < vert1.y)
                    {
                        AddVerticalQuad(verts[3, 0], new Vector3(verts[3, 3].x, east, verts[3, 3].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }

                    if (west < vert1.y)
                    {
                        AddVerticalQuad(verts[0, 3], new Vector3(verts[0, 0].x, west, verts[0, 0].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }


                    //and now for the river
                    waterVerts.Add(new Vector3(verts[1, 0].x, (waterLevel - 0) * GameMap.tileHeight, verts[1, 0].z));
                    waterVerts.Add(new Vector3(verts[2, 0].x, (waterLevel - 0) * GameMap.tileHeight, verts[2, 0].z));
                    waterVerts.Add(new Vector3(verts[1, 3].x, (waterLevel - 0) * GameMap.tileHeight, verts[1, 3].z));
                    waterVerts.Add(new Vector3(verts[2, 3].x, (waterLevel - 0) * GameMap.tileHeight, verts[2, 3].z));

                    for (int i = 0; i < 4; i++)
                        waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

                    waterTris.Add(waterIndex + 0);
                    waterTris.Add(waterIndex + 1);
                    waterTris.Add(waterIndex + 2);

                    waterTris.Add(waterIndex + 1);
                    waterTris.Add(waterIndex + 3);
                    waterTris.Add(waterIndex + 2);

                }
                break;
            case Sides.East | Sides.West:
                {
                    //first add the land
                    vertices.Add(verts[0, 0]);
                    vertices.Add(verts[3, 0]);
                    vertices.Add(verts[0, 1]);
                    vertices.Add(verts[3, 1]);

                    vertices.Add(verts[0, 2]);
                    vertices.Add(verts[3, 2]);
                    vertices.Add(verts[0, 3]);
                    vertices.Add(verts[3, 3]);

                    vertices.Add(new Vector3(verts[3, 1].x, (waterLevel - 1) * GameMap.tileHeight, verts[3, 1].z));
                    vertices.Add(new Vector3(verts[3, 2].x, (waterLevel - 1) * GameMap.tileHeight, verts[3, 2].z));
                    vertices.Add(new Vector3(verts[0, 1].x, (waterLevel - 1) * GameMap.tileHeight, verts[0, 1].z));
                    vertices.Add(new Vector3(verts[0, 2].x, (waterLevel - 1) * GameMap.tileHeight, verts[0, 2].z));

                    for (int i = 0; i < 12; i++)
                    {
                        uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
                        uvCoords2.Add(biome);
                        colors.Add(Color.white);
                    }

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);

                    triangles.Add(index + 1);
                    triangles.Add(index + 3);
                    triangles.Add(index + 2);

                    triangles.Add(index + 4);
                    triangles.Add(index + 5);
                    triangles.Add(index + 6);

                    triangles.Add(index + 5);
                    triangles.Add(index + 7);
                    triangles.Add(index + 6);

                    triangles.Add(index + 8);
                    triangles.Add(index + 9);
                    triangles.Add(index + 10);

                    triangles.Add(index + 9);
                    triangles.Add(index + 11);
                    triangles.Add(index + 10);

                    //now the river banks
                    AddVerticalQuad(verts[3, 1], new Vector3(verts[0, 1].x, (waterLevel - 1) * GameMap.tileHeight, verts[0, 1].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    AddVerticalQuad(verts[0, 2], new Vector3(verts[3, 2].x, (waterLevel - 1) * GameMap.tileHeight, verts[3, 2].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);

                    //Tile edges
                    if (north < vert1.y)
                    {
                        AddVerticalQuad(verts[0, 0], new Vector3(verts[3, 0].x, north, verts[3, 0].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }

                    if (south < vert1.y)
                    {
                        AddVerticalQuad(verts[3, 3], new Vector3(verts[0, 3].x, south, verts[0, 3].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }

                    if (east < vert1.y)
                    {
                        AddVerticalQuad(verts[3, 0], new Vector3(verts[3, 1].x, east, verts[3, 1].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                        AddVerticalQuad(verts[3, 2], new Vector3(verts[3, 3].x, east, verts[3, 3].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }

                    if (west < vert1.y)
                    {
                        AddVerticalQuad(verts[0, 3], new Vector3(verts[0, 2].x, west, verts[0, 2].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                        AddVerticalQuad(verts[0, 1], new Vector3(verts[0, 0].x, west, verts[0, 0].z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
                    }


                    //and now for the river
                    waterVerts.Add(new Vector3(verts[3, 1].x, (waterLevel - 0) * GameMap.tileHeight, verts[3, 1].z));
                    waterVerts.Add(new Vector3(verts[3, 2].x, (waterLevel - 0) * GameMap.tileHeight, verts[3, 2].z));
                    waterVerts.Add(new Vector3(verts[0, 1].x, (waterLevel - 0) * GameMap.tileHeight, verts[0, 1].z));
                    waterVerts.Add(new Vector3(verts[0, 2].x, (waterLevel - 0) * GameMap.tileHeight, verts[0, 2].z));

                    for (int i = 0; i < 4; i++)
                        waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

                    waterTris.Add(waterIndex + 0);
                    waterTris.Add(waterIndex + 1);
                    waterTris.Add(waterIndex + 2);

                    waterTris.Add(waterIndex + 1);
                    waterTris.Add(waterIndex + 3);
                    waterTris.Add(waterIndex + 2);
                }
                break;
            case Sides.North | Sides.East:
                    AddRiverCorner(verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[0, 3], biome, waterLevel, north, east, south, west);
                break;
            case Sides.East | Sides.South:
                    AddRiverCorner(verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 0], biome, waterLevel, east, south, west, north);
                break;
            case Sides.South | Sides.West:
                    AddRiverCorner(verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[3, 0], biome, waterLevel, south, west, north, east);
                break;
            case Sides.West | Sides.North:
                    AddRiverCorner(verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 3], biome, waterLevel, west, north, east, south);
                break;
            case Sides.North | Sides.East | Sides.South:
                    AddRiverTee(verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], biome, waterLevel, north, east, south, west);
                break;
            default:
                break;
        }
    }

    private void AddRiverTee(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector3 vert5, Vector3 vert6, Vector3 vert7, Vector3 vert8, Vector3 vert9, Vector3 vert10, Vector2 biome, int waterLevel, float north, float east, float south, float west)
    {
        int index = vertices.Count;
        int waterIndex = waterVerts.Count;

        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert3);
        vertices.Add(vert4);
        vertices.Add(vert5);
        vertices.Add(vert6);
        vertices.Add(vert7);
        vertices.Add(vert8);
        vertices.Add(vert9);
        vertices.Add(vert10);

        vertices.Add(new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z));
        vertices.Add(new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z));
        vertices.Add(new Vector3(vert5.x, (waterLevel - 1) * GameMap.tileHeight, vert5.z));
        vertices.Add(new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z));
        vertices.Add(new Vector3(vert8.x, (waterLevel - 1) * GameMap.tileHeight, vert8.z));
        vertices.Add(new Vector3(vert9.x, (waterLevel - 1) * GameMap.tileHeight, vert9.z));

        for (int i = 0; i < 16; i++)
        {
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords2.Add(biome);
            colors.Add(Color.white);
        }

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 9);

        triangles.Add(index + 1);
        triangles.Add(index + 8);
        triangles.Add(index + 9);

        triangles.Add(index + 2);
        triangles.Add(index + 3);
        triangles.Add(index + 4);

        triangles.Add(index + 5);
        triangles.Add(index + 6);
        triangles.Add(index + 7);
    }

    private void AddRiverCorner(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector3 vert5, Vector3 vert6, Vector3 vert7, Vector3 vert8, Vector2 biome, int waterLevel, float north, float east, float south, float west)
    {
        int index = vertices.Count;
        int waterIndex = waterVerts.Count;


        //first add the land
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert6);
        vertices.Add(vert7);
        vertices.Add(vert8);

        vertices.Add(vert3);
        vertices.Add(vert4);
        vertices.Add(vert5);

        vertices.Add(new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z));
        vertices.Add(new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z));
        vertices.Add(new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z));
        vertices.Add(new Vector3(vert5.x, (waterLevel - 1) * GameMap.tileHeight, vert5.z));


        for (int i = 0; i < 12; i++)
        {
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords2.Add(biome);
            colors.Add(Color.white);
        }

        triangles.Add(index + 4);
        triangles.Add(index + 0);
        triangles.Add(index + 1);

        triangles.Add(index + 4);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 4);
        triangles.Add(index + 2);
        triangles.Add(index + 3);


        triangles.Add(index + 5);
        triangles.Add(index + 6);
        triangles.Add(index + 7);

        triangles.Add(index + 8);
        triangles.Add(index + 9);
        triangles.Add(index + 10);

        triangles.Add(index + 9);
        triangles.Add(index + 11);
        triangles.Add(index + 10);


        //now the river banks
        AddVerticalQuad(vert5, new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert2, new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);

        //Tile edges
        if (north < vert1.y)
        {
            AddVerticalQuad(vert1, new Vector3(vert2.x, north, vert2.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert3, new Vector3(vert4.x, north, vert4.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            AddVerticalQuad(vert7, new Vector3(vert8.x, south, vert8.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            AddVerticalQuad(vert4, new Vector3(vert5.x, east, vert5.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert6, new Vector3(vert7.x, east, vert7.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            AddVerticalQuad(vert8, new Vector3(vert1.x, west, vert1.z), biome, Color.white, vertices, colors, uvCoords, uvCoords2, triangles);
        }


        //and now for the river
        waterVerts.Add(new Vector3(vert2.x, (waterLevel - 0) * GameMap.tileHeight, vert2.z));
        waterVerts.Add(new Vector3(vert3.x, (waterLevel - 0) * GameMap.tileHeight, vert3.z));
        waterVerts.Add(new Vector3(vert6.x, (waterLevel - 0) * GameMap.tileHeight, vert6.z));
        waterVerts.Add(new Vector3(vert5.x, (waterLevel - 0) * GameMap.tileHeight, vert5.z));

        for (int i = 0; i < 4; i++)
            waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

        waterTris.Add(waterIndex + 0);
        waterTris.Add(waterIndex + 1);
        waterTris.Add(waterIndex + 2);

        waterTris.Add(waterIndex + 1);
        waterTris.Add(waterIndex + 3);
        waterTris.Add(waterIndex + 2);
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
