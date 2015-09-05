using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;
using DFHack;

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
    bool[,] cumulusMedium;
    bool[,] cumulusMulti;
    bool[,] cumulusNimbus;
    bool[,] stratusAlto;
    bool[,] stratusProper;
    bool[,] stratusNimbus;
    bool[,] cirrus;
    bool[,] fogMist;
    bool[,] fogNormal;
    bool[,] fogThick;

    public Vector3 offset;
    public Vector3 worldPosition;

    RemoteFortressReader.RegionMaps regionMaps;
    RemoteFortressReader.WorldMap worldMap;

    public CloudMaker cloudPrafab;

    CloudMaker cumulusMediumClouds;
    CloudMaker cumulusMultiClouds;
    Dictionary<int, CloudMaker> cumulusNimbusClouds;
    CloudMaker stratusAltoClouds;
    CloudMaker stratusProperClouds;
    Dictionary<int, CloudMaker> stratusNimbusClouds;
    CloudMaker cirrusClouds;

    Dictionary<DFCoord2d, RegionMaker> DetailRegions = new Dictionary<DFCoord2d, RegionMaker>();
    public RegionMaker regionPrefab;

    Mesh terrainMesh;

    bool madeTerrain = false;

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
        if(width * height > 65535)
        {
            width = Mathf.Clamp(width, 0, 255);
            height = Mathf.Clamp(height, 0, 255);
        }
        worldName = remoteMap.name;
        worldNameEnglish = remoteMap.name_english;
        offset = new Vector3(
            -mapInfo.block_pos_x * 48 * GameMap.tileWidth,
            -mapInfo.block_pos_z * GameMap.tileHeight,
            mapInfo.block_pos_y * 48 * GameMap.tileWidth);
        worldPosition = new Vector3(mapInfo.block_pos_x, mapInfo.block_pos_y, mapInfo.block_pos_z);
        meshRenderer.material.SetFloat("_Scale", scale);
        meshRenderer.material.SetFloat("_SeaLevel", (99 * GameMap.tileHeight) + offset.y);
        InitArrays();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = y * remoteMap.world_width + x;
                elevation[x, y] = remoteMap.elevation[index];
                rainfall[x, y] = remoteMap.rainfall[index];
                vegetation[x, y] = remoteMap.vegetation[index];
                temperature[x, y] = remoteMap.temperature[index];
                evilness[x, y] = remoteMap.evilness[index];
                drainage[x, y] = remoteMap.drainage[index];
                volcanism[x, y] = remoteMap.volcanism[index];
                savagery[x, y] = remoteMap.savagery[index];
                salinity[x, y] = remoteMap.salinity[index];
                cumulusMedium[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_MEDIUM;
                cumulusMulti[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_MULTI;
                cumulusNimbus[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_NIMBUS;
                stratusAlto[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_ALTO;
                stratusProper[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_PROPER;
                stratusNimbus[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_NIMBUS;
                cirrus[x, y] = remoteMap.clouds[index].cirrus;
                fogMist[x, y] = remoteMap.clouds[index].fog == FogType.FOG_MIST;
                fogNormal[x, y] = remoteMap.clouds[index].fog == FogType.FOG_NORMAL;
                fogThick[x, y] = remoteMap.clouds[index].fog == FogType.F0G_THICK;
            }
        GenerateMesh();

        GenerateClouds();
        madeTerrain = true;
        Debug.Log("Loaded World: " + worldNameEnglish);
    }

    void CopyClouds(WorldMap remoteMap)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = y * remoteMap.world_width + x;
                cumulusMedium[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_MEDIUM;
                cumulusMulti[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_MULTI;
                cumulusNimbus[x, y] = remoteMap.clouds[index].cumulus == CumulusType.CUMULUS_NIMBUS;
                stratusAlto[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_ALTO;
                stratusProper[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_PROPER;
                stratusNimbus[x, y] = remoteMap.clouds[index].stratus == StratusType.STRATUS_NIMBUS;
                cirrus[x, y] = remoteMap.clouds[index].cirrus;
                fogMist[x, y] = remoteMap.clouds[index].fog == FogType.FOG_MIST;
                fogNormal[x, y] = remoteMap.clouds[index].fog == FogType.FOG_NORMAL;
                fogThick[x, y] = remoteMap.clouds[index].fog == FogType.F0G_THICK;
            }
        GenerateClouds();
    }

    void InitArrays()
    {
        if (elevation == null)
            elevation = new int[width, height];

        if (rainfall == null)
            rainfall = new int[width, height];

        if (vegetation == null)
            vegetation = new int[width, height];

        if (temperature == null)
            temperature = new int[width, height];

        if (evilness == null)
            evilness = new int[width, height];

        if (drainage == null)
            drainage = new int[width, height];

        if (volcanism == null)
            volcanism = new int[width, height];

        if (savagery == null)
            savagery = new int[width, height];

        if (salinity == null)
            salinity = new int[width, height];

        if (cumulusMedium == null)
            cumulusMedium = new bool[width, height];

        if (cumulusMulti == null)
            cumulusMulti = new bool[width, height];

        if (cumulusNimbus == null)
            cumulusNimbus = new bool[width, height];

        if (stratusAlto == null)
            stratusAlto = new bool[width, height];

        if (stratusProper == null)
            stratusProper = new bool[width, height];

        if (stratusNimbus == null)
            stratusNimbus = new bool[width, height];

        if (cirrus == null)
            cirrus = new bool[width, height];

        if (fogMist == null)
            fogMist = new bool[width, height];

        if (fogNormal == null)
            fogNormal = new bool[width, height];

        if (fogThick == null)
            fogThick = new bool[width, height];

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
        regionMaps = DFConnection.Instance.PopRegionMapUpdate();
        if (regionMaps != null)
        {
            GenerateRegionMeshes();
        }
        worldMap = DFConnection.Instance.PopWorldMapUpdate();
        if (worldMap != null)
        {
            CopyFromRemote(worldMap, DFConnection.Instance.NetMapInfo);
        }
    }

    void Update()
    {
        worldMap = DFConnection.Instance.PopWorldMapUpdate();
        if (worldMap != null)
            if (madeTerrain)
                CopyClouds(worldMap);
            else
                CopyFromRemote(worldMap, DFConnection.Instance.NetMapInfo);
    }

    void GenerateRegionMeshes()
    {
        foreach (WorldMap map in regionMaps.world_maps)
        {
            DFCoord2d pos = new DFCoord2d(map.map_x, map.map_y);
            if (DetailRegions.ContainsKey(pos))
            {
                //Debug.Log("Region exists: " + pos.x + ", " + pos.y);
                continue;
            }
            RegionMaker region = Instantiate<RegionMaker>(regionPrefab);
            region.CopyFromRemote(map, DFConnection.Instance.NetMapInfo);
            region.name = region.worldNameEnglish;
            region.transform.parent = transform;
            DetailRegions[pos] = region;
        }
    }

    void GenerateMesh()
    {
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
                if (DetailRegions.ContainsKey(new DFCoord2d(x, y)))
                    continue;
                triangles.Add(CoordToIndex(x, y));
                triangles.Add(CoordToIndex(x + 1, y));
                triangles.Add(CoordToIndex(x + 1, y + 1));

                triangles.Add(CoordToIndex(x, y));
                triangles.Add(CoordToIndex(x + 1, y + 1));
                triangles.Add(CoordToIndex(x, y + 1));
            }

        terrainMesh = new Mesh();
        terrainMesh.vertices = vertexPositions;
        terrainMesh.colors = vertexColors;
        terrainMesh.uv = vertexUV;
        terrainMesh.uv2 = vertexUV2;
        terrainMesh.triangles = triangles.ToArray();

        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateTangents();

        meshFilter.mesh = terrainMesh;
        madeTerrain = true;
    }

    void GenerateClouds()
    {
        cumulusMediumClouds = MakeCloud(cumulusMediumClouds, 1250, cumulusMedium, "cumulusMedium");
        cumulusMultiClouds = MakeCloud(cumulusMultiClouds, 5000, cumulusMulti, "cumulusMulti");
        if (cumulusNimbusClouds == null) cumulusNimbusClouds = new Dictionary<int, CloudMaker>();
        for (int i = 1875; i <= 6250; i += 300)
            cumulusNimbusClouds[i] = MakeCloud(cumulusNimbusClouds.ContainsKey(i) ? cumulusNimbusClouds[i] : null, i, cumulusNimbus, "cumulusNimbus");
        stratusAltoClouds = MakeCloud(stratusAltoClouds, 6250, stratusAlto, "stratusAlto");
        stratusProperClouds = MakeCloud(stratusProperClouds, 1875, stratusProper, "stratusProper");
        if (stratusNimbusClouds == null) stratusNimbusClouds = new Dictionary<int, CloudMaker>();
        for (int i = 625; i <= 1875; i += 300)
            stratusNimbusClouds[i] = MakeCloud(stratusNimbusClouds.ContainsKey(i) ? stratusNimbusClouds[i] : null, i, stratusNimbus, "stratusNimbus");
        cirrusClouds = MakeCloud(cirrusClouds, 6250, cirrus, "cirrus");
    }

    CloudMaker MakeCloud(CloudMaker original, float height, bool[,] cloudMap, string name)
    {
        if (original == null)
        {
            original = Instantiate<CloudMaker>(cloudPrafab);
            original.transform.position = new Vector3(0, height * GameMap.tileHeight * scale);
            original.offset = offset;
            original.scale = scale;
            original.GenerateMesh(cloudMap);
            original.name = name;
            original.transform.parent = transform;
        }
        else
            original.UpdateClouds(cloudMap);
        return original;
    }
}
