using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;
using DFHack;

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

    public MeshFilter terrainPrefab;
    List<MeshFilter> terrainChunks = new List<MeshFilter>();

    public Material terrainMat;

    int CoordToIndex(int x, int y)
    {
        return x + y * width;
    }

    TimeHolder timeHolder;

    void Awake()
    {
        timeHolder = FindObjectOfType<TimeHolder>();
    }

    void CopyFromRemote(WorldMap remoteMap)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        timeHolder.realTime = new DFTime(remoteMap.cur_year, remoteMap.cur_year_tick);
        if (!GameSettings.Instance.rendering.drawDistantTerrain)
            return;
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
        -remoteMap.center_x * 48 * GameMap.tileWidth,
        0,
        remoteMap.center_y * 48 * GameMap.tileWidth);
        terrainMat.SetFloat("_Scale", scale);
        terrainMat.SetFloat("_SeaLevel", (99 * GameMap.tileHeight) + offset.y);
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
                if (GameSettings.Instance.rendering.drawClouds)
                {
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
            }
        GenerateMesh();


        if (GameSettings.Instance.rendering.drawClouds)
            GenerateClouds();
        //Debug.Log("Loaded World: " + worldNameEnglish);
    }

    DFTime lastUpdateTime;

    void CopyClouds(WorldMap remoteMap)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        timeHolder.realTime = new DFTime(remoteMap.cur_year, remoteMap.cur_year_tick);
        if (timeHolder.realTime - lastUpdateTime < new System.TimeSpan(1, 0, 0))
            return;
        lastUpdateTime = timeHolder.realTime;
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
        worldMap = DFConnection.Instance.PopWorldMapUpdate();
        if (regionMaps != null && worldMap != null)
        {
            GenerateRegionMeshes();
        }
        if (worldMap != null)
        {
            CopyFromRemote(worldMap);
        }
    }

    void Update()
    {
        regionMaps = DFConnection.Instance.PopRegionMapUpdate();
        worldMap = DFConnection.Instance.PopWorldMapUpdate();
        if (regionMaps != null && worldMap != null)
        {
            GenerateRegionMeshes();
        }
        if (worldMap != null)
        {
            if (DFConnection.Instance.HasWorldMapPositionChanged())
            {
                CopyFromRemote(worldMap);
                UpdateRegionPositions(worldMap);
            }
            else
            {
                if(GameSettings.Instance.rendering.drawClouds)
                    CopyClouds(worldMap);
            }
        }
    }

    void UpdateRegionPositions(WorldMap map)
    {
        if (!GameSettings.Instance.rendering.drawDistantTerrain)
            return;

        foreach (var item in DetailRegions)
        {
            RegionMaker region = item.Value;
            region.SetPosition(map);
        }
    }

    void GenerateRegionMeshes()
    {
        if (!GameSettings.Instance.rendering.drawDistantTerrain)
            return;

        foreach (WorldMap map in regionMaps.world_maps)
        {
            DFCoord2d pos = new DFCoord2d(map.map_x, map.map_y);
            if (DetailRegions.ContainsKey(pos))
            {
                continue;
            }
            RegionMaker region = Instantiate<RegionMaker>(regionPrefab);
            region.CopyFromRemote(map, worldMap);
            region.name = region.worldNameEnglish;
            region.transform.parent = transform;
            DetailRegions[pos] = region;
        }
    }

    void GenerateMesh()
    {
        if (width == 1 && height == 1)
            return;
        int length = width * height * 4;
        List<Vector3> vertices = new List<Vector3>(length);
        List<Color> colors = new List<Color>(length);
        List<Vector2> uvs = new List<Vector2>(length);
        List<Vector2> uv2s = new List<Vector2>(length);
        List<int> triangles = new List<int>(length);

        foreach(MeshFilter mf in terrainChunks)
        {
            if (mf.mesh != null)
                mf.mesh.Clear();
        }

        int chunkIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (DetailRegions.ContainsKey(new DFCoord2d(x, y)))
                    continue;

                //If the vertex lists are already full, make a chunk with what we have, and keep going
                if (vertices.Count >= (65535 - 20))
                {
                    if(terrainChunks.Count <= chunkIndex)
                    {
                        terrainChunks.Add(Instantiate<MeshFilter>(terrainPrefab));
                        terrainChunks[chunkIndex].transform.parent = transform;
                        terrainChunks[chunkIndex].gameObject.name = "TerrainChunk" + chunkIndex;

                    }
                    MeshFilter mf = terrainChunks[chunkIndex];

                    if (mf.mesh == null)
                        mf.mesh = new Mesh();

                    Mesh terrainMesh = mf.mesh;

                    terrainMesh.vertices = vertices.ToArray();
                    terrainMesh.colors = colors.ToArray();
                    terrainMesh.uv = uvs.ToArray();
                    terrainMesh.uv2 = uv2s.ToArray();
                    terrainMesh.triangles = triangles.ToArray();

                    terrainMesh.RecalculateNormals();
                    terrainMesh.RecalculateTangents();

                    mf.mesh = terrainMesh;

                    vertices.Clear();
                    colors.Clear();
                    uvs.Clear();
                    uv2s.Clear();
                    triangles.Clear();
                    chunkIndex++;
                }

                int index = vertices.Count;
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                    {
                        vertices.Add((new Vector3(
                    (x + i) * 48 * 16 * GameMap.tileWidth,
                    elevation[x, y] * GameMap.tileHeight,
                    -(y + j) * 48 * 16 * GameMap.tileWidth) + offset) * scale);

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
                if (y > 0 && !DetailRegions.ContainsKey(new DFCoord2d(x, y - 1)))
                    north = elevation[x, y - 1];
                if (north < elevation[x, y])
                {
                    Vector3 vert1 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth) + offset) * scale;
                    Vector3 vert2 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, north * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth) + offset) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }

                int south = 0;
                if (y < height - 1 && !DetailRegions.ContainsKey(new DFCoord2d(x, y + 1)))
                    south = elevation[x, y + 1];
                if (south < elevation[x, y])
                {
                    index = vertices.Count;
                    Vector3 vert1 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth) + offset) * scale;
                    Vector3 vert2 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, south * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth) + offset) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }

                int east = 0;
                if (x < width - 1 && !DetailRegions.ContainsKey(new DFCoord2d(x + 1, y)))
                    east = elevation[x + 1, y];
                if (east < elevation[x, y])
                {
                    Vector3 vert1 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth) + offset) * scale;
                    Vector3 vert2 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, east * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth) + offset) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }
                int west = 0;
                if (x > 0 && !DetailRegions.ContainsKey(new DFCoord2d(x-1, y)))
                    west = elevation[x - 1, y];
                if (west < elevation[x, y])
                {
                    Vector3 vert1 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, elevation[x, y] * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth) + offset) * scale;
                    Vector3 vert2 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, west * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth) + offset) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, Color.white, vertices, colors, uvs, uv2s, triangles);
                }
            }
        }
        {
            if (terrainChunks.Count <= chunkIndex)
            {
                terrainChunks.Add(Instantiate<MeshFilter>(terrainPrefab));
                terrainChunks[chunkIndex].transform.parent = transform;
                terrainChunks[chunkIndex].gameObject.name = "TerrainChunk" + chunkIndex;
            }
            MeshFilter mf = terrainChunks[chunkIndex];

            if (mf.mesh == null)
                mf.mesh = new Mesh();

            Mesh terrainMesh = mf.mesh;

            terrainMesh.vertices = vertices.ToArray();
            terrainMesh.colors = colors.ToArray();
            terrainMesh.uv = uvs.ToArray();
            terrainMesh.uv2 = uv2s.ToArray();
            terrainMesh.triangles = triangles.ToArray();

            terrainMesh.RecalculateNormals();
            terrainMesh.RecalculateTangents();

            mf.mesh = terrainMesh;
        }
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
