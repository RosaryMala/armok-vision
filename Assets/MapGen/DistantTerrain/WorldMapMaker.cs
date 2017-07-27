using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;
using DFHack;
using System;
using MaterialStore;

public class WorldMapMaker : MonoBehaviour
{
    public float scale = 0.01f;
    public int width;
    public int height;
    public string worldName;
    public string worldNameEnglish;
    RegionTile[,] regionTiles;
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

    RegionMaps regionMaps;
    WorldMap worldMap;

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

    public MeshFilter waterPrefab;
    public MeshFilter regionWaterPrefab;
    List<MeshFilter> waterChunks = new List<MeshFilter>();

    public Material terrainMat;

    int CoordToIndex(int x, int y)
    {
        return x + y * width;
    }

    const float EarthRadius = 6371000;

    public static Vector3 ConvertToSphere(Vector3 value)
    {
        float distSquare = (EarthRadius * EarthRadius) - (value.x * value.x) - (value.z * value.z);
        distSquare = Mathf.Max(distSquare, 0);
        var y = Mathf.Sqrt(distSquare);
        y = y - EarthRadius + value.y;
        return new Vector3(value.x, y, value.z);
    }

    void CopyFromRemote(WorldMap remoteMap)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        if (GameSettings.Instance.rendering.distantTerrainDetail == GameSettings.LandscapeDetail.Off)
            return;
        width = remoteMap.world_width;
        height = remoteMap.world_height;
        worldName = remoteMap.name;
        worldNameEnglish = remoteMap.name_english;
        transform.position = new Vector3(
        ((-remoteMap.center_x * 48) - 0.5f) * GameMap.tileWidth,
        0,
        ((remoteMap.center_y * 48) + 0.5f) * GameMap.tileWidth);

        terrainMat.SetFloat("_Scale", scale);
        terrainMat.SetFloat("_SeaLevel", (99 * GameMap.tileHeight) + transform.position.y);

        InitArrays();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = y * remoteMap.world_width + x;
                regionTiles[x,y] = remoteMap.region_tiles[index];
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
        if(ContentLoader.Instance != null)
            GenerateMesh();


        if (GameSettings.Instance.rendering.drawClouds)
            GenerateClouds();
        //Debug.Log("Loaded World: " + worldNameEnglish);
    }

    DFTime lastUpdateTime;
    private int chunkIndex;
    private int waterChunkIndex;

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
        if (regionTiles == null)
            regionTiles = new RegionTile[width, height];

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
        if (ContentLoader.Instance == null)
            return;
        regionMaps = DFConnection.Instance.PopRegionMapUpdate();
        worldMap = DFConnection.Instance.PopWorldMapUpdate();
        if (regionMaps != null && worldMap != null)
        {
            GenerateRegionMeshes();
            GenerateMesh();
        }
        if (worldMap != null)
        {
            if (DFConnection.Instance.HasWorldMapPositionChanged())
            {
                CopyFromRemote(worldMap);
            }
            else
            {
                if(GameSettings.Instance.rendering.drawClouds)
                    CopyClouds(worldMap);
            }
        }
    }

    void GenerateRegionMeshes()
    {
        if (GameSettings.Instance.rendering.distantTerrainDetail == GameSettings.LandscapeDetail.Off)
            return;

        foreach (RegionMap map in regionMaps.region_maps)
        {
            DFCoord2d pos = new DFCoord2d(map.map_x, map.map_y);
            RegionMaker region;
            if (!DetailRegions.ContainsKey(pos))
            {
                region = Instantiate(regionPrefab);
                region.transform.parent = transform;
                region.transform.localPosition = RegionToUnityCoords(map.map_x, map.map_y, 0);
                DetailRegions[pos] = region;
            }
            else
                region = DetailRegions[pos];
            region.CopyFromRemote(map, worldMap);
            region.name = region.worldNameEnglish;
        }
    }

    Vector3 RegionToUnityCoords(int x, int y, int z)
    {
        return new Vector3(
            x * 48 * 16 * GameMap.tileWidth,
            z * GameMap.tileHeight,
            -y * 48 * 16 * GameMap.tileWidth
            );
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
        List<Vector2> uv3s = new List<Vector2>(length);
        List<int> triangles = new List<int>(length);

        List<Vector3> waterVerts = new List<Vector3>();
        List<Vector2> waterUvs = new List<Vector2>();
        List<int> waterTris = new List<int>();

        foreach(MeshFilter mf in terrainChunks)
        {
            if (mf.mesh != null)
                mf.mesh.Clear();
        }
        foreach(MeshFilter mf in waterChunks)
        {
            if (mf.mesh != null)
                mf.mesh.Clear();
        }

        chunkIndex = 0;
        waterChunkIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (DetailRegions.ContainsKey(new DFCoord2d(x, y)))
                    continue;


                //If the vertex lists are already full, make a chunk with what we have, and keep going
                if (vertices.Count >= (65535 - 20))
                {
                    FinalizeGeometryChunk(vertices, colors, uvs, uv2s, uv3s, triangles);
                }

                //If the vertex lists are already full, make a chunk with what we have, and keep going
                if (waterVerts.Count >= (65535 - 20))
                {
                    FinalizeWaterGeometryChunk(waterVerts, waterUvs, waterTris);
                }


                MaterialTextureSet colorContent;
                ContentLoader.Instance.MaterialTextures.TryGetValue(regionTiles[x, y].surface_material, out colorContent);
                Color terrainColor = colorContent.color;

                Color plantColor = Color.black;
                float grassPercent = Mathf.Pow(regionTiles[x, y].vegetation / 100.0f, 0.25F);
                float treePercent = Mathf.Pow(regionTiles[x, y].vegetation / 100.0f, 0.5F);

                foreach (var item in regionTiles[x, y].plant_materials)
                {
                    ContentLoader.Instance.MaterialTextures.TryGetValue(item, out colorContent);
                    plantColor += colorContent.color;
                }
                if (regionTiles[x, y].plant_materials.Count == 0)
                    grassPercent = 0;
                else
                    plantColor /= regionTiles[x, y].plant_materials.Count;

                Color treeColor = Color.black;
                int treeCount = 0;
                foreach (var tree in regionTiles[x,y].tree_materials)
                {
                    int plantIndex = tree.mat_index;
                    if (tree.mat_type != 419
                        || DFConnection.Instance.NetPlantRawList == null
                        || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex)
                        continue;
                    var treeMat = tree;
                    foreach (var growth in DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths)
                    {
                        int currentTicks = TimeHolder.DisplayedTime.CurrentYearTicks;
                        if ((growth.timing_start != -1 && growth.timing_start > currentTicks) || (growth.timing_end != -1 && growth.timing_end < currentTicks))
                            continue;
                        treeMat = growth.mat;
                        break;
                    }
                    if (ContentLoader.Instance.MaterialTextures.TryGetValue(treeMat, out colorContent))
                    {
                        treeColor += colorContent.color;
                        treeCount++;
                    }
                }
                if (treeCount == 0)
                    treePercent = 0;
                else
                    treeColor /= treeCount;



                terrainColor = Color.Lerp(terrainColor, plantColor, grassPercent);
                terrainColor = Color.Lerp(terrainColor, treeColor, treePercent);

                Vector2 biome = new Vector2(regionTiles[x, y].rainfall, 100 - regionTiles[x, y].drainage) / 100;

                Vector3 vert1 = RegionToUnityCoords(x, y, regionTiles[x, y].elevation);
                Vector3 vert2 = RegionToUnityCoords(x + 1, y + 1, regionTiles[x, y].elevation);

                bool snow = regionTiles[x, y].snow > 0;

                RegionMaker.AddHorizontalQuad(vert1, vert2, biome, terrainColor, vertices, colors, uvs, uv2s, uv3s, triangles, snow);

                if (regionTiles[x, y].elevation < regionTiles[x, y].water_elevation)
                {
                    vert1 = RegionToUnityCoords(x, y, regionTiles[x, y].water_elevation);
                    vert2 = RegionToUnityCoords(x + 1, y + 1, regionTiles[x, y].water_elevation);

                    RegionMaker.AddHorizontalQuad(vert1, vert2, biome, terrainColor, waterVerts, null, waterUvs, null, null, waterTris, false);
                }

                int north = 0;
                if (y > 0 && !DetailRegions.ContainsKey(new DFCoord2d(x, y - 1)))
                    north = regionTiles[x, y - 1].elevation;
                if (north < regionTiles[x, y].elevation)
                {
                    vert1 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, regionTiles[x, y].elevation * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth)) * scale;
                    vert2 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, north * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth)) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, terrainColor, vertices, colors, uvs, uv2s, uv3s, triangles, snow);
                }

                int south = 0;
                if (y < height - 1 && !DetailRegions.ContainsKey(new DFCoord2d(x, y + 1)))
                    south = regionTiles[x, y + 1].elevation;
                if (south < regionTiles[x, y].elevation)
                {
                    vert1 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, regionTiles[x, y].elevation * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth)) * scale;
                    vert2 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, south * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth)) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, terrainColor, vertices, colors, uvs, uv2s, uv3s, triangles, snow);
                }

                int east = 0;
                if (x < width - 1 && !DetailRegions.ContainsKey(new DFCoord2d(x + 1, y)))
                    east = regionTiles[x + 1, y].elevation;
                if (east < regionTiles[x, y].elevation)
                {
                    vert1 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, regionTiles[x, y].elevation * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth)) * scale;
                    vert2 = (new Vector3((x + 1) * 48 * 16 * GameMap.tileWidth, east * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth)) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, terrainColor, vertices, colors, uvs, uv2s, uv3s, triangles, snow);
                }
                int west = 0;
                if (x > 0 && !DetailRegions.ContainsKey(new DFCoord2d(x - 1, y)))
                    west = regionTiles[x - 1, y].elevation;
                if (west < regionTiles[x, y].elevation)
                {
                    vert1 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, regionTiles[x, y].elevation * GameMap.tileHeight, -(y + 1) * 48 * 16 * GameMap.tileWidth)) * scale;
                    vert2 = (new Vector3(x * 48 * 16 * GameMap.tileWidth, west * GameMap.tileHeight, -y * 48 * 16 * GameMap.tileWidth)) * scale;

                    RegionMaker.AddVerticalQuad(vert1, vert2, biome, terrainColor, vertices, colors, uvs, uv2s, uv3s, triangles, snow);
                }
            }
        }

        FinalizeGeometryChunk(vertices, colors, uvs, uv2s, uv3s, triangles);

        FinalizeWaterGeometryChunk(waterVerts, waterUvs, waterTris);
    }

    private void FinalizeWaterGeometryChunk(List<Vector3> waterVerts, List<Vector2> waterUvs, List<int> waterTris)
    {
        if (waterChunks.Count <= waterChunkIndex)
        {
            waterChunks.Add(Instantiate(waterPrefab));
            waterChunks[waterChunkIndex].transform.parent = transform;
            waterChunks[waterChunkIndex].gameObject.name = "WaterChunk" + waterChunkIndex;
            waterChunks[waterChunkIndex].transform.localPosition = Vector3.zero;
        }
        MeshFilter mf = waterChunks[waterChunkIndex];

        if (mf.mesh == null)
            mf.mesh = new Mesh();

        Mesh waterMesh = mf.mesh;

        waterMesh.vertices = waterVerts.ToArray();
        waterMesh.uv = waterUvs.ToArray();
        waterMesh.triangles = waterTris.ToArray();

        waterMesh.RecalculateNormals();
        waterMesh.RecalculateTangents();

        mf.mesh = waterMesh;

        waterVerts.Clear();
        waterUvs.Clear();
        waterTris.Clear();
        waterChunkIndex++;
    }

    private void FinalizeGeometryChunk(List<Vector3> vertices, List<Color> colors, List<Vector2> uvs, List<Vector2> uv2s, List<Vector2> uv3s, List<int> triangles)
    {
        if (terrainChunks.Count <= chunkIndex)
        {
            terrainChunks.Add(Instantiate(terrainPrefab));
            terrainChunks[chunkIndex].transform.parent = transform;
            terrainChunks[chunkIndex].gameObject.name = "TerrainChunk" + chunkIndex;
            terrainChunks[chunkIndex].transform.localPosition = Vector3.zero;
        }
        MeshFilter mf = terrainChunks[chunkIndex];

        if (mf.mesh == null)
            mf.mesh = new Mesh();

        Mesh terrainMesh = mf.mesh;

        terrainMesh.vertices = vertices.ToArray();
        Linearize(colors);
        terrainMesh.colors = colors.ToArray();
        terrainMesh.uv = uvs.ToArray();
        terrainMesh.uv2 = uv2s.ToArray();
        terrainMesh.uv3 = uv3s.ToArray();
        terrainMesh.triangles = triangles.ToArray();

        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateTangents();

        mf.mesh = terrainMesh;

        vertices.Clear();
        colors.Clear();
        uvs.Clear();
        uv2s.Clear();
        uv3s.Clear();
        triangles.Clear();
        chunkIndex++;
    }

    public static void Linearize(List<Color> colors)
    {
        for (int i = 0; i < colors.Count; i++)
        {
            colors[i] = colors[i].linear;
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
            original = Instantiate(cloudPrafab);
            original.scale = scale;
            original.GenerateMesh(cloudMap);
            original.name = name;
            original.transform.parent = transform;
            original.transform.localPosition = new Vector3(0, height * GameMap.tileHeight * scale);
        }
        else
            original.UpdateClouds(cloudMap);
        return original;
    }
}
