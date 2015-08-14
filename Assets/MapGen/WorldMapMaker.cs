using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldMapMaker : MonoBehaviour
{
    public float scale = 0.01f;
    public int width;
    public int height;
    public string worldName;
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
    bool[,] CumulusMedium;
    bool[,] CumulusMulti;
    bool[,] CumulusNimbus;
    bool[,] StratusAlto;
    bool[,] StratusProper;
    bool[,] StratusNimbus;
    bool[,] FogMist;
    bool[,] FogNormal;
    bool[,] FogThick;

    public Vector3 offset;
    public Vector3 worldPosition;

    Mesh terrainMesh;

    int CoordToIndex(int x, int y)
    {
        return x + y * width;
    }

    Vector3[] vertexPositions;
    Vector2[] vertexUV;
    Vector2[] vertexUV2;
    Color[] vertexColors;
    List<int> triangles;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void CopyFromRemote(WorldMap remoteMap, MapInfo mapInfo)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        width = remoteMap.world_width;
        height = remoteMap.world_height;
        worldName = remoteMap.name;
        worldNameEnglish = remoteMap.name_english;
        offset = new Vector3(-mapInfo.block_pos_x * 48 * GameMap.tileWidth, -mapInfo.block_pos_z * GameMap.tileHeight, mapInfo.block_pos_y * 48 * GameMap.tileWidth);
        worldPosition = new Vector3(mapInfo.block_pos_x, mapInfo.block_pos_y, mapInfo.block_pos_z);
        meshRenderer.material.SetFloat("_Scale", scale);
        meshRenderer.material.SetFloat("_SeaLevel", (99 * GameMap.tileHeight) + offset.y);
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
                CumulusMedium[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_MEDIUM;
                CumulusMulti[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_MULTI;
                CumulusNimbus[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_NIMBUS;
                StratusAlto[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_ALTO;
                StratusProper[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_PROPER;
                StratusNimbus[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_NIMBUS;
                FogMist[x, y] = remoteMap.clouds[index].fog == FogType.FOG_MIST;
                FogNormal[x, y] = remoteMap.clouds[index].fog == FogType.FOG_NORMAL;
                FogThick[x, y] = remoteMap.clouds[index].fog == FogType.F0G_THICK;
            }
        GenerateMesh();
        Debug.Log("Loaded World: " + worldNameEnglish);
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
        CumulusMedium = new bool[width, height];
        CumulusMulti = new bool[width, height];
        CumulusNimbus = new bool[width, height];
        StratusAlto = new bool[width, height];
        StratusProper = new bool[width, height];
        StratusNimbus = new bool[width, height];
        FogMist = new bool[width, height];
        FogNormal = new bool[width, height];
        FogThick = new bool[width, height];
    }

    // Does about what you'd think it does.
    void Start()
    {
        enabled = false;

        DFConnection.RegisterConnectionCallback(this.OnConnectToDF);
    }

    void OnConnectToDF()
    {
        enabled = true;
        CopyFromRemote(DFConnection.Instance.NetWorldMap, DFConnection.Instance.NetMapInfo);
    }

    void GenerateMesh()
    {
        width = Mathf.Clamp(width, 0, 255);
        height = Mathf.Clamp(height, 0, 255);
        vertexPositions = new Vector3[width * height];
        vertexColors = new Color[width * height];
        vertexUV = new Vector2[width * height];
        vertexUV2 = new Vector2[width * height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = CoordToIndex(x, y);
                vertexPositions[index] = (new Vector3(
                    x * 16 * 48 * GameMap.tileWidth,
                    elevation[x, y] * GameMap.tileHeight,
                    -y * 16 * 48 * GameMap.tileWidth) + offset) * scale;
                //vertexColors[index] = cloudStriped[x, y] ? Color.white : Color.black;// * cloudFog[x, y] / 3.0f;
                vertexColors[index] = Color.white;
                vertexUV2[index] = new Vector2(rainfall[x, y], 100 - drainage[x, y]) / 100;
                vertexUV[index] = new Vector2(x, y);
            }

        if (triangles == null)
            triangles = new List<int>();
        triangles.Clear();
        for (int x = 0; x < width - 1; x++)
            for (int y = 0; y < height - 1; y++)
            {
                triangles.Add(CoordToIndex(x, y));
                triangles.Add(CoordToIndex(x + 1, y));
                triangles.Add(CoordToIndex(x + 1, y + 1));

                triangles.Add(CoordToIndex(x, y));
                triangles.Add(CoordToIndex(x + 1, y + 1));
                triangles.Add(CoordToIndex(x, y + 1));
            }

        Debug.Log(vertexPositions.Length);
        terrainMesh = new Mesh();
        terrainMesh.vertices = vertexPositions;
        terrainMesh.colors = vertexColors;
        terrainMesh.uv = vertexUV;
        terrainMesh.uv2 = vertexUV2;
        terrainMesh.triangles = triangles.ToArray();

        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateTangents();

        meshFilter.mesh = terrainMesh;
    }
}
