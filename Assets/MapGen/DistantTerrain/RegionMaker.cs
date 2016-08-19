using DFHack;
using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RegionMaker : MonoBehaviour
{
    class RegionTile
    {
        public int elevation;
        public int water_elevation;
        public int rainfall;
        public int vegetation;
        public int temperature;
        public int evilness;
        public int drainage;
        public int volcanism;
        public int savagery;
        public int salinity;
        public RiverTile rivers;
        public MatPairStruct surfaceMaterial;
        public List<MatPairStruct> plantMaterials;
        public List<SiteRealizationBuilding> buildings;
        public List<MatPairStruct> stoneMaterials;
        public List<MatPairStruct> treeMaterials;

        public RegionTile(WorldMap remoteMap, int index)
        {
            elevation = remoteMap.elevation[index];
            if (remoteMap.water_elevation != null && remoteMap.water_elevation.Count > index)
                water_elevation = remoteMap.water_elevation[index];
            else
                water_elevation = 99;
            rainfall = remoteMap.rainfall[index];
            vegetation = remoteMap.vegetation[index];
            temperature = remoteMap.temperature[index];
            evilness = remoteMap.evilness[index];
            drainage = remoteMap.drainage[index];
            volcanism = remoteMap.volcanism[index];
            savagery = remoteMap.savagery[index];
            salinity = remoteMap.salinity[index];
            if (remoteMap.river_tiles != null && remoteMap.river_tiles.Count > index)
                rivers = remoteMap.river_tiles[index];
        }

        public RegionTile(RemoteFortressReader.RegionTile tile)
        {
            elevation = tile.elevation;
            water_elevation = tile.water_elevation;
            rainfall = tile.rainfall;
            vegetation = tile.vegetation;
            temperature = tile.temperature;
            evilness = tile.evilness;
            drainage = tile.drainage;
            volcanism = tile.volcanism;
            savagery = tile.savagery;
            salinity = tile.salinity;
            rivers = tile.river_tiles;
            surfaceMaterial = tile.surface_material;
            plantMaterials = new List<MatPairStruct>();
            foreach (var item in tile.plant_materials)
            {
                plantMaterials.Add(item);
            }
            buildings = tile.buildings;
            stoneMaterials = new List<MatPairStruct>();
            foreach (var item in tile.stone_materials)
            {
                stoneMaterials.Add(item);
            }
            treeMaterials = new List<MatPairStruct>();
            foreach (var item in tile.tree_materials)
            {
                treeMaterials.Add(item);
            }
        }
    }
    public float scale;
    public int width;
    public int height;
    public string worldNameEnglish;
    RegionTile[,] tiles;

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
        if(fortOrigin != DFConnection.Instance.EmbarkMapPosition && ContentLoader.Instance != null)
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

    public void CopyFromRemote(RegionMap remoteMap, WorldMap mainMap)
    {
        if (remoteMap == null)
        {
            Debug.Log("Didn't get world map!");
            return;
        }
        width = 17;
        height = 17;
        worldNameEnglish = remoteMap.name_english;
        embarkTileOffset = new Vector3((remoteMap.map_x * 16), 0, -(remoteMap.map_y * 16));
        regionOrigin = new DFCoord(remoteMap.map_x * 16, remoteMap.map_y * 16, 0);
        SetPosition(mainMap);
        InitArrays();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = y * width + x;
                tiles[x, y] = new RegionTile(remoteMap.tiles[index]);
            }

        if (ContentLoader.Instance != null)
            GenerateMesh();
    }

    void InitArrays()
    {
        tiles = new RegionTile[width, height];
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

    [System.Flags]
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
                RegionTile tile = tiles[x, y];
                if (tile == null)
                    continue;

                Vector2 biome = new Vector2(tile.rainfall, 100 - tile.drainage) / 100;

                Vector3 vert1 = new Vector3(x * 48 * GameMap.tileWidth, tile.elevation * GameMap.tileHeight, -y * 48 * GameMap.tileWidth);

                Sides riverSides = 0;

                if (tile.rivers != null)
                {
                    if (tile.rivers.north.min_pos >= 0)
                        riverSides |= Sides.North;
                    if (tile.rivers.east.min_pos >= 0)
                        riverSides |= Sides.East;
                    if (tile.rivers.south.min_pos >= 0)
                        riverSides |= Sides.South;
                    if (tile.rivers.west.min_pos >= 0)
                        riverSides |= Sides.West;
                }

                int north = 0;
                if (y > 0 && !IsInCoords(fortMin, fortMax, x, y - 1))
                    north = tiles[x, y - 1].elevation;

                int south = 0;
                if (y < h - 1 && !IsInCoords(fortMin, fortMax, x, y + 1))
                    south = tiles[x, y + 1].elevation;

                int east = 0;
                if (x < w - 1 && !IsInCoords(fortMin, fortMax, x + 1, y))
                    east = tiles[x + 1, y].elevation;

                int west = 0;
                if (x > 0 && !IsInCoords(fortMin, fortMax, x - 1, y))
                    west = tiles[x - 1, y].elevation;

                MapDataStore.Tile fakeTile = new MapDataStore.Tile(null, new DFCoord(0, 0, 0));

                fakeTile.material = tile.surfaceMaterial;

                ColorContent colorDef;
                ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);

                Color terrainColor = colorDef.value;
                Color plantColor = Color.black;
                Color stoneColor = Color.grey;
                foreach (var item in tile.plantMaterials)
                {
                    fakeTile.material = item;
                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                    plantColor += colorDef.value;
                }
                float plantBlend = Mathf.Pow(tile.vegetation / 100.0f, 0.25f);
                if (tile.plantMaterials.Count == 0)
                    plantBlend = 0;
                else
                    plantColor /= tile.plantMaterials.Count;

                if (tile.stoneMaterials.Count > 0)
                {
                    fakeTile.material = tile.stoneMaterials[0];
                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                    stoneColor = colorDef.value;
                }

                Color blendedColor = Color.Lerp(terrainColor, plantColor, plantBlend);

                if (riverSides == 0)
                    AddFlatTile(vert1, biome, north * GameMap.tileHeight, east * GameMap.tileHeight, south * GameMap.tileHeight, west * GameMap.tileHeight, tile.water_elevation, blendedColor);
                else
                {
                    AddRiverTile(riverSides, tile.rivers, vert1, biome, north * GameMap.tileHeight, east * GameMap.tileHeight, south * GameMap.tileHeight, west * GameMap.tileHeight, blendedColor);
                }

                if (GameSettings.Instance.rendering.distantTerrainDetail == GameSettings.LandscapeDetail.High)
                {
                    foreach (SiteRealizationBuilding building in tile.buildings)
                    {

                        Vector3 min, max;

                        switch (building.type)
                        {
                            case SiteRealizationBuildingType.cottage_plot:
                            case SiteRealizationBuildingType.castle_courtyard:
                            case SiteRealizationBuildingType.house:
                            case SiteRealizationBuildingType.temple:
                            case SiteRealizationBuildingType.shop_house:
                            case SiteRealizationBuildingType.warehouse:
                            case SiteRealizationBuildingType.well:
                            case SiteRealizationBuildingType.vault:
                            case SiteRealizationBuildingType.tree_house:
                            case SiteRealizationBuildingType.hillock_house:
                            case SiteRealizationBuildingType.mead_hall:
                            case SiteRealizationBuildingType.fortress_entrance:
                            case SiteRealizationBuildingType.library:
                            case SiteRealizationBuildingType.tavern:
                                {
                                    fakeTile.material = building.material;
                                    Color buildingColor;
                                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                                    if (colorDef != null)
                                        buildingColor = colorDef.value;
                                    else
                                    {
                                        Debug.LogError("No valid color for " + building.type);
                                        buildingColor = Color.magenta;
                                    }
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, 2 * GameMap.tileHeight, -(building.max_y + 1) * GameMap.tileWidth);
                                    AddBlock(vert1 + min, vert1 + max, biome, buildingColor);
                                    break;
                                }
                            case SiteRealizationBuildingType.courtyard:
                            case SiteRealizationBuildingType.market_square:
                            case SiteRealizationBuildingType.waste:
                                {
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, 1 * GameMap.tileHeight / 6.0f, -(building.max_y + 1) * GameMap.tileWidth);
                                    AddBlock(vert1 + min, vert1 + max, biome, terrainColor);
                                    break;
                                }
                            case SiteRealizationBuildingType.pasture:
                                {
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, 1 * GameMap.tileHeight / 6.0f, -(building.max_y + 1) * GameMap.tileWidth);
                                    AddBlock(vert1 + min, vert1 + max, biome, plantColor);
                                    break;
                                }
                            case SiteRealizationBuildingType.trenches:
                                {
                                    int mid_x = (building.min_x + building.max_x) / 2;
                                    int mid_y = (building.min_y + building.max_y) / 2;
                                    min = new Vector3((mid_x - 1) * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    max = new Vector3((mid_x + 2) * GameMap.tileWidth, 1 * GameMap.tileHeight / 6.0f, -(building.max_y + 1) * GameMap.tileWidth);
                                    AddBlock(vert1 + min, vert1 + max, biome, Color.black);
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -(mid_y - 1) * GameMap.tileWidth);
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, 1 * GameMap.tileHeight / 6.0f, -(mid_y + 2) * GameMap.tileWidth);
                                    AddBlock(vert1 + min, vert1 + max, biome, Color.black);
                                    break;
                                }
                            case SiteRealizationBuildingType.great_tower:
                                {
                                    fakeTile.material = building.material;
                                    Color buildingColor;
                                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                                    if (colorDef != null)
                                        buildingColor = colorDef.value;
                                    else
                                    {
                                        Debug.LogError("No valid color for " + building.type);
                                        buildingColor = Color.magenta;
                                    }
                                    min = new Vector3((building.min_x + 5) * GameMap.tileWidth, 0, -(building.min_y + 5) * GameMap.tileWidth);
                                    max = new Vector3((building.max_x - 4) * GameMap.tileWidth, 15 * GameMap.tileHeight, -(building.max_y - 4) * GameMap.tileWidth);
                                    AddCylinder(vert1 + min, vert1 + max, biome, buildingColor);
                                    break;
                                }
                            case SiteRealizationBuildingType.castle_tower:
                                {
                                    fakeTile.material = building.material;
                                    Color buildingColor;
                                    Color roofColor;
                                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                                    if (colorDef != null)
                                    {
                                        buildingColor = colorDef.value;
                                        roofColor = colorDef.value;
                                    }
                                    else
                                    {
                                        buildingColor = stoneColor;
                                        roofColor = terrainColor;
                                    }
                                    int top = 2;
                                    if (building.tower_info != null)
                                    {
                                        if (building.tower_info.goblin)
                                        {
                                            top = 11;
                                        }
                                        else
                                        {
                                            top = building.tower_info.roof_z;
                                            top -= tile.elevation;
                                        }
                                    }
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, top * GameMap.tileHeight, -(building.max_y + 1) * GameMap.tileWidth);
                                    if (building.tower_info != null && building.tower_info.round)
                                        AddCylinder(vert1 + min, vert1 + max, biome, buildingColor, roofColor);
                                    else
                                        AddBlock(vert1 + min, vert1 + max, biome, buildingColor, roofColor);
                                    break;
                                }
                            case SiteRealizationBuildingType.castle_wall:
                                {
                                    fakeTile.material = building.material;
                                    Color buildingColor;
                                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                                    if (colorDef != null)
                                        buildingColor = colorDef.value;
                                    else
                                    {
                                        buildingColor = Color.magenta;
                                    }
                                    if (building.wall_info != null)
                                    {
                                        min = new Vector3((building.wall_info.start_x + 1) * GameMap.tileWidth, (building.wall_info.start_z - tile.elevation) * GameMap.tileHeight, -(building.wall_info.start_y + 1) * GameMap.tileWidth);
                                        max = new Vector3((building.wall_info.end_x + 1) * GameMap.tileWidth, 0, -(building.wall_info.end_y + 1) * GameMap.tileWidth);
                                        AddWall(vert1 + min, vert1 + max, 4 * GameMap.tileWidth, biome, buildingColor);
                                    }
                                    else
                                    {
                                        min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                        max = new Vector3((building.max_x + 1) * GameMap.tileWidth, 2 * GameMap.tileHeight, -(building.max_y + 1) * GameMap.tileWidth);
                                        AddBlock(vert1 + min, vert1 + max, biome, Color.black);
                                    }
                                    break;
                                }
                            case SiteRealizationBuildingType.tomb:
                                {
                                    fakeTile.material = building.material;
                                    if (fakeTile.material.mat_type < 0)
                                        fakeTile.material = new MatPairStruct(0, fakeTile.material.mat_index);
                                    Color buildingColor;
                                    ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef);
                                    if (colorDef != null)
                                        buildingColor = colorDef.value;
                                    else
                                        buildingColor = Color.grey;
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    int tombHeight = (building.max_x - building.min_x + building.max_y - building.min_y) / 8;
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, tombHeight * GameMap.tileHeight, -(building.max_y + 1) * GameMap.tileWidth);
                                    AddPyramid(vert1 + min, vert1 + max, biome, buildingColor);
                                    break;
                                }

                            default:
                                {
                                    min = new Vector3(building.min_x * GameMap.tileWidth, 0, -building.min_y * GameMap.tileWidth);
                                    max = new Vector3((building.max_x + 1) * GameMap.tileWidth, 13 * GameMap.tileHeight / 6.0f, -(building.max_y + 1) * GameMap.tileWidth);
                                    AddBlock(vert1 + min, vert1 + max, biome, Color.magenta);
                                    break;
                                }
                        }

                    }

                    if (tile.buildings.Count == 0 && tile.treeMaterials.Count > 0)
                    {
                        float treeRadius = 4.5f * GameMap.tileWidth;
                        var treeCoords = UniformPoissonDiskSampler.SampleRectangle(new Vector2(vert1.x + treeRadius, vert1.z - 48 * GameMap.tileWidth + treeRadius), new Vector2(vert1.x + 48 * GameMap.tileWidth - treeRadius, vert1.z - treeRadius), (Mathf.Lerp(48.0f, 8.0f, Mathf.Pow(tile.vegetation / 100.0f, 0.5f))) * GameMap.tileWidth);

                        foreach (var coord in treeCoords)
                        {
                            var tree = tile.treeMaterials[Random.Range(0, tile.treeMaterials.Count - 1)];
                            int plantIndex = tree.mat_index;
                            if (tree.mat_type != 419
                                || DFConnection.Instance.NetPlantRawList == null
                                || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex)
                                continue;

                            foreach (var growth in DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths)
                            {
                                int currentTicks = TimeHolder.DisplayedTime.CurrentYearTicks;
                                if ((growth.timing_start != -1 && growth.timing_start > currentTicks) || (growth.timing_end != -1 && growth.timing_end < currentTicks))
                                    continue;
                                tree = growth.mat;
                                break;
                            }
                            Color treeColor = Color.green;

                            fakeTile.material = tree;
                            if (ContentLoader.Instance.ColorConfiguration.GetValue(fakeTile, MeshLayer.StaticMaterial, out colorDef))
                                treeColor = colorDef.value;

                            if (tree.mat_type == 419) // bare tree
                                AddTree(new Vector3(coord.x, tile.elevation * GameMap.tileHeight, coord.y), GameMap.tileWidth / 2, Random.Range(7.0f, 9.0f) * GameMap.tileHeight, treeColor, biome, Random.Range(0.0f, 360.0f));
                            else
                                AddTree(new Vector3(coord.x, tile.elevation * GameMap.tileHeight, coord.y), treeRadius, Random.Range(7.0f, 9.0f) * GameMap.tileHeight, treeColor, biome, Random.Range(0.0f, 360.0f));
                        }
                    }
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
                waterChunk = Instantiate(parentMap.regionWaterPrefab);
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

    private void AddRiverTile(Sides riverSides, RiverTile riverTile, Vector3 vert1, Vector2 biome, float north, float east, float south, float west, Color terrainColor)
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

        switch (riverSides)
        {
            case Sides.North | Sides.South:
                AddRiverStraight(verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], biome, waterLevel, north, east, south, west, riverTile.north.elevation, riverTile.south.elevation, terrainColor);
                break;
            case Sides.East | Sides.West:
                AddRiverStraight(verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], biome, waterLevel, east, south, west, north, riverTile.east.elevation, riverTile.west.elevation, terrainColor);
                break;
            case Sides.North:
                AddRiverEnd(verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 3], verts[0, 3], biome, waterLevel, north, east, south, west, terrainColor);
                break;
            case Sides.East:
                AddRiverEnd(verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[0, 3], verts[0, 0], biome, waterLevel, east, south, west, north, terrainColor);
                break;
            case Sides.South:
                AddRiverEnd(verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 0], verts[3, 0], biome, waterLevel, south, west, north, east, terrainColor);
                break;
            case Sides.West:
                AddRiverEnd(verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[3, 0], verts[3, 3], biome, waterLevel, west, north, east, south, terrainColor);
                break;
            case Sides.North | Sides.East:
                AddRiverCorner(verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[0, 3], biome, waterLevel, north, east, south, west, terrainColor);
                break;
            case Sides.East | Sides.South:
                AddRiverCorner(verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 0], biome, waterLevel, east, south, west, north, terrainColor);
                break;
            case Sides.South | Sides.West:
                AddRiverCorner(verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[3, 0], biome, waterLevel, south, west, north, east, terrainColor);
                break;
            case Sides.West | Sides.North:
                AddRiverCorner(verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 3], biome, waterLevel, west, north, east, south, terrainColor);
                break;
            case Sides.North | Sides.East | Sides.South:
                AddRiverTee(verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], biome, waterLevel, north, east, south, west, riverTile.north.elevation, riverTile.east.elevation, riverTile.south.elevation, terrainColor);
                break;
            case Sides.East | Sides.South | Sides.West:
                AddRiverTee(verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], biome, waterLevel, east, south, west, north, riverTile.east.elevation, riverTile.south.elevation, riverTile.west.elevation, terrainColor);
                break;
            case Sides.South | Sides.West | Sides.North:
                AddRiverTee(verts[3, 3], verts[2, 3], verts[1, 3], verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], biome, waterLevel, south, west, north, east, riverTile.south.elevation, riverTile.west.elevation, riverTile.north.elevation, terrainColor);
                break;
            case Sides.West | Sides.North | Sides.East:
                AddRiverTee(verts[0, 3], verts[0, 2], verts[0, 1], verts[0, 0], verts[1, 0], verts[2, 0], verts[3, 0], verts[3, 1], verts[3, 2], verts[3, 3], biome, waterLevel, west, north, east, south, riverTile.west.elevation, riverTile.north.elevation, riverTile.east.elevation, terrainColor);
                break;
            case Sides.North | Sides.East | Sides.South | Sides.West:
                break;
            default:
                break;
        }
    }

    Vector3 FindEllipseCorner(Vector3 center, Vector3 sideA, Vector3 sideB)
    {
        float a = sideA.x - center.x;
        float b = sideB.z - center.z;
        if(a*a + b*b < 0.00001)
        {
            a = sideB.x - center.x;
            b = sideA.z - center.z;
        }
        float x = a * a / Mathf.Sqrt((a * a) + (b * b));
        float z = Mathf.Sqrt((b * b * b * b) / ((a * a) + (b * b)));

        if (a < 1)
            x = -x;
        if (b < 1)
            z = -z;

        return center + new Vector3(x, 0, z);
    }

    private void AddRiverEnd(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector3 vert5, Vector3 vert6, Vector2 biome, int waterLevel, float north, float east, float south, float west, Color terrainColor)
    {
        int index = vertices.Count;
        int waterIndex = waterVerts.Count;

        Vector3 center = (vert1 + vert4 + vert5 + vert6) / 4;

        vertices.Add(center);
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert3);
        vertices.Add(vert4);
        vertices.Add(vert5);
        vertices.Add(vert6);

        vertices.Add(new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z));
        vertices.Add(new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z));
        vertices.Add(new Vector3(center.x, (waterLevel - 1) * GameMap.tileHeight, center.z));

        for (int i = 0; i < 10; i++)
        {
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords2.Add(biome);
            colors.Add(terrainColor);
        }

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 3);
        triangles.Add(index + 4);

        triangles.Add(index + 0);
        triangles.Add(index + 4);
        triangles.Add(index + 5);

        triangles.Add(index + 0);
        triangles.Add(index + 5);
        triangles.Add(index + 6);

        triangles.Add(index + 0);
        triangles.Add(index + 6);
        triangles.Add(index + 1);

        triangles.Add(index + 7 + 0);
        triangles.Add(index + 7 + 1);
        triangles.Add(index + 7 + 2);

        //now the river banks
        AddVerticalQuad(center, new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert2, new Vector3(center.x, (waterLevel - 1) * GameMap.tileHeight, center.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);

        //Tile edges
        if (north < vert1.y)
        {
            AddVerticalQuad(vert1, new Vector3(vert2.x, north, vert2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert3, new Vector3(vert4.x, north, vert4.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            AddVerticalQuad(vert5, new Vector3(vert6.x, south, vert6.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            AddVerticalQuad(vert4, new Vector3(vert5.x, east, vert5.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            AddVerticalQuad(vert6, new Vector3(vert1.x, west, vert1.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        //and now for the river
        waterVerts.Add(new Vector3(vert2.x, (waterLevel - 0) * GameMap.tileHeight, vert2.z));
        waterVerts.Add(new Vector3(vert3.x, (waterLevel - 0) * GameMap.tileHeight, vert3.z));
        waterVerts.Add(new Vector3(center.x, (waterLevel - 0) * GameMap.tileHeight, center.z));

        for (int i = 0; i < 3; i++)
            waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

        waterTris.Add(waterIndex + 0 + 0);
        waterTris.Add(waterIndex + 0 + 1);
        waterTris.Add(waterIndex + 0 + 2);

    }

    private void AddRiverStraight(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector3 vert5, Vector3 vert6, Vector3 vert7, Vector3 vert8, Vector2 biome, int waterLevel, float north, float east, float south, float west, int waterLevelNorth, int waterLevelSouth, Color terrainColor)
    {
        int index = vertices.Count;
        int waterIndex = waterVerts.Count;

        //first add the land
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert7);
        vertices.Add(vert8);

        vertices.Add(vert3);
        vertices.Add(vert4);
        vertices.Add(vert5);
        vertices.Add(vert6);

        vertices.Add(new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z));
        vertices.Add(new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z));
        vertices.Add(new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z));
        vertices.Add(new Vector3(vert7.x, (waterLevel - 1) * GameMap.tileHeight, vert7.z));


        for (int i = 0; i < 12; i++)
        {
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords2.Add(biome);
            colors.Add(terrainColor);
        }

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

        triangles.Add(index + 4);
        triangles.Add(index + 5);
        triangles.Add(index + 6);

        triangles.Add(index + 4);
        triangles.Add(index + 6);
        triangles.Add(index + 7);

        triangles.Add(index + 8 + 0);
        triangles.Add(index + 8 + 1);
        triangles.Add(index + 8 + 2);

        triangles.Add(index + 8 + 0);
        triangles.Add(index + 8 + 2);
        triangles.Add(index + 8 + 3);



        //now the river banks
        AddVerticalQuad(vert2, new Vector3(vert7.x, (waterLevel - 1) * GameMap.tileHeight, vert7.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert6, new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);

        //Tile edges
        if (north < vert1.y)
        {
            AddVerticalQuad(vert1, new Vector3(vert2.x, north, vert2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert3, new Vector3(vert4.x, north, vert4.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            AddVerticalQuad(vert5, new Vector3(vert6.x, south, vert6.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert7, new Vector3(vert8.x, south, vert8.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            AddVerticalQuad(vert4, new Vector3(vert5.x, east, vert5.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            AddVerticalQuad(vert8, new Vector3(vert1.x, west, vert1.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        //waterfalls
        if (waterLevelNorth > waterLevel)
        {
            AddVerticalQuad(new Vector3(vert3.x, (waterLevelNorth - 1) * GameMap.tileHeight, vert3.z), new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }
        if (waterLevelSouth > waterLevel)
        {
            AddVerticalQuad(new Vector3(vert7.x, (waterLevelSouth - 1) * GameMap.tileHeight, vert7.z), new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }


        //and now for the river
        waterVerts.Add(new Vector3(vert2.x, (waterLevel - 0) * GameMap.tileHeight, vert2.z));
        waterVerts.Add(new Vector3(vert3.x, (waterLevel - 0) * GameMap.tileHeight, vert3.z));
        waterVerts.Add(new Vector3(vert6.x, (waterLevel - 0) * GameMap.tileHeight, vert6.z));
        waterVerts.Add(new Vector3(vert7.x, (waterLevel - 0) * GameMap.tileHeight, vert7.z));

        for (int i = 0; i < 4; i++)
            waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

        waterTris.Add(waterIndex + 0 + 0);
        waterTris.Add(waterIndex + 0 + 1);
        waterTris.Add(waterIndex + 0 + 2);

        waterTris.Add(waterIndex + 0 + 0);
        waterTris.Add(waterIndex + 0 + 2);
        waterTris.Add(waterIndex + 0 + 3);

        //waterfalls again!
        if (waterLevelNorth > waterLevel)
        {
            AddQuad(
                new Vector3(vert2.x, waterLevelNorth * GameMap.tileHeight, vert2.z),
                new Vector3(vert3.x, waterLevelNorth * GameMap.tileHeight, vert3.z),
                new Vector3(vert3.x, waterLevel * GameMap.tileHeight, vert3.z) - ((vert1 - vert8) / 48),
                new Vector3(vert2.x, waterLevel * GameMap.tileHeight, vert2.z) - ((vert1 - vert8) / 48),
                biome, terrainColor, waterVerts, null, waterUvs, null, waterTris
                );
        }
        if (waterLevelSouth > waterLevel)
        {
            AddQuad(
                new Vector3(vert6.x, waterLevelSouth * GameMap.tileHeight, vert6.z),
                new Vector3(vert7.x, waterLevelSouth * GameMap.tileHeight, vert7.z),
                new Vector3(vert7.x, waterLevel * GameMap.tileHeight, vert7.z) - ((vert8 - vert1) / 48),
                new Vector3(vert6.x, waterLevel * GameMap.tileHeight, vert6.z) - ((vert8 - vert1) / 48),
                biome, terrainColor, waterVerts, null, waterUvs, null, waterTris
                );
        }

    }

    private void AddRiverCorner(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector3 vert5, Vector3 vert6, Vector3 vert7, Vector3 vert8, Vector2 biome, int waterLevel, float north, float east, float south, float west, Color terrainColor)
    {
        int index = vertices.Count;
        int waterIndex = waterVerts.Count;

        Vector3 corner1 = FindEllipseCorner(vert4, vert3, vert5);
        Vector3 corner2 = corner1 - (((vert3 - vert2) + (vert5 - vert6)) * 0.7071067811865475f);


        //first add the land
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(corner2);
        vertices.Add(vert6);
        vertices.Add(vert7);
        vertices.Add(vert8);

        vertices.Add(vert4);
        vertices.Add(vert5);
        vertices.Add(corner1);
        vertices.Add(vert3);

        vertices.Add(new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z));
        vertices.Add(new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z));
        vertices.Add(new Vector3(corner2.x, (waterLevel - 1) * GameMap.tileHeight, corner2.z));
        vertices.Add(new Vector3(corner1.x, (waterLevel - 1) * GameMap.tileHeight, corner1.z));
        vertices.Add(new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z));
        vertices.Add(new Vector3(vert5.x, (waterLevel - 1) * GameMap.tileHeight, vert5.z));


        for (int i = 0; i < 16; i++)
        {
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords2.Add(biome);
            colors.Add(terrainColor);
        }

        triangles.Add(index + 5);
        triangles.Add(index + 0);
        triangles.Add(index + 1);

        triangles.Add(index + 5);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 5);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

        triangles.Add(index + 5);
        triangles.Add(index + 3);
        triangles.Add(index + 4);

        triangles.Add(index + 6);
        triangles.Add(index + 7);
        triangles.Add(index + 8);

        triangles.Add(index + 6);
        triangles.Add(index + 8);
        triangles.Add(index + 9);

        triangles.Add(index + 10 + 0);
        triangles.Add(index + 10 + 1);
        triangles.Add(index + 10 + 2);

        triangles.Add(index + 10 + 1);
        triangles.Add(index + 10 + 3);
        triangles.Add(index + 10 + 2);

        triangles.Add(index + 10 + 2);
        triangles.Add(index + 10 + 3);
        triangles.Add(index + 10 + 4);

        triangles.Add(index + 10 + 3);
        triangles.Add(index + 10 + 5);
        triangles.Add(index + 10 + 4);


        //now the river banks
        AddVerticalQuad(corner1, new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert5, new Vector3(corner1.x, (waterLevel - 1) * GameMap.tileHeight, corner1.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(corner2, new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert2, new Vector3(corner2.x, (waterLevel - 1) * GameMap.tileHeight, corner2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);

        //Tile edges
        if (north < vert1.y)
        {
            AddVerticalQuad(vert1, new Vector3(vert2.x, north, vert2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert3, new Vector3(vert4.x, north, vert4.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            AddVerticalQuad(vert7, new Vector3(vert8.x, south, vert8.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            AddVerticalQuad(vert4, new Vector3(vert5.x, east, vert5.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert6, new Vector3(vert7.x, east, vert7.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            AddVerticalQuad(vert8, new Vector3(vert1.x, west, vert1.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }


        //and now for the river
        waterVerts.Add(new Vector3(vert2.x, (waterLevel - 0) * GameMap.tileHeight, vert2.z));
        waterVerts.Add(new Vector3(vert3.x, (waterLevel - 0) * GameMap.tileHeight, vert3.z));
        waterVerts.Add(new Vector3(corner2.x, (waterLevel - 0) * GameMap.tileHeight, corner2.z));
        waterVerts.Add(new Vector3(corner1.x, (waterLevel - 0) * GameMap.tileHeight, corner1.z));
        waterVerts.Add(new Vector3(vert6.x, (waterLevel - 0) * GameMap.tileHeight, vert6.z));
        waterVerts.Add(new Vector3(vert5.x, (waterLevel - 0) * GameMap.tileHeight, vert5.z));

        for (int i = 0; i < 6; i++)
            waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

        waterTris.Add(waterIndex + 0);
        waterTris.Add(waterIndex + 1);
        waterTris.Add(waterIndex + 2);

        waterTris.Add(waterIndex + 1);
        waterTris.Add(waterIndex + 3);
        waterTris.Add(waterIndex + 2);

        waterTris.Add(waterIndex + 2);
        waterTris.Add(waterIndex + 3);
        waterTris.Add(waterIndex + 4);

        waterTris.Add(waterIndex + 3);
        waterTris.Add(waterIndex + 5);
        waterTris.Add(waterIndex + 4);
    }

    private void AddRiverTee(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector3 vert5, Vector3 vert6, Vector3 vert7, Vector3 vert8, Vector3 vert9, Vector3 vert10, Vector2 biome, int waterLevel, float north, float east, float south, float west, int waterLevelNorth, int waterLevelEast, int waterLevelSouth, Color terrainColor)
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

        vertices.Add(new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z)); //10
        vertices.Add(new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z)); //11
        vertices.Add(new Vector3(vert5.x, (waterLevel - 1) * GameMap.tileHeight, vert5.z)); //12
        vertices.Add(new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z)); //13
        vertices.Add(new Vector3(vert8.x, (waterLevel - 1) * GameMap.tileHeight, vert8.z)); //14
        vertices.Add(new Vector3(vert9.x, (waterLevel - 1) * GameMap.tileHeight, vert9.z)); //15

        for (int i = 0; i < 16; i++)
        {
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords2.Add(biome);
            colors.Add(terrainColor);
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

        triangles.Add(index + 10 + 0);
        triangles.Add(index + 10 + 4);
        triangles.Add(index + 10 + 5);

        triangles.Add(index + 10 + 0);
        triangles.Add(index + 10 + 1);
        triangles.Add(index + 10 + 4);

        triangles.Add(index + 10 + 1);
        triangles.Add(index + 10 + 2);
        triangles.Add(index + 10 + 4);

        triangles.Add(index + 10 + 2);
        triangles.Add(index + 10 + 3);
        triangles.Add(index + 10 + 4);

        //now the river banks
        AddVerticalQuad(vert2, new Vector3(vert9.x, (waterLevel - 1) * GameMap.tileHeight, vert9.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert5, new Vector3(vert3.x, (waterLevel - 1) * GameMap.tileHeight, vert3.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(vert8, new Vector3(vert6.x, (waterLevel - 1) * GameMap.tileHeight, vert6.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);

        //waterfalls
        if (waterLevelNorth > waterLevel)
        {
            AddVerticalQuad(new Vector3(vert3.x, (waterLevelNorth - 1) * GameMap.tileHeight, vert3.z), new Vector3(vert2.x, (waterLevel - 1) * GameMap.tileHeight, vert2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }
        if (waterLevelEast > waterLevel)
        {
            AddVerticalQuad(new Vector3(vert6.x, (waterLevelEast - 1) * GameMap.tileHeight, vert6.z), new Vector3(vert5.x, (waterLevel - 1) * GameMap.tileHeight, vert5.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }
        if (waterLevelSouth > waterLevel)
        {
            AddVerticalQuad(new Vector3(vert9.x, (waterLevelSouth - 1) * GameMap.tileHeight, vert9.z), new Vector3(vert8.x, (waterLevel - 1) * GameMap.tileHeight, vert8.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        //Tile edges
        if (north < vert1.y)
        {
            AddVerticalQuad(vert1, new Vector3(vert2.x, north, vert2.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert3, new Vector3(vert4.x, north, vert4.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            AddVerticalQuad(vert7, new Vector3(vert8.x, south, vert8.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert9, new Vector3(vert10.x, south, vert10.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            AddVerticalQuad(vert4, new Vector3(vert5.x, east, vert5.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
            AddVerticalQuad(vert6, new Vector3(vert7.x, east, vert7.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            AddVerticalQuad(vert10, new Vector3(vert1.x, west, vert1.z), biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        //and now for the river
        waterVerts.Add(new Vector3(vert2.x, (waterLevel - 0) * GameMap.tileHeight, vert2.z)); //10
        waterVerts.Add(new Vector3(vert3.x, (waterLevel - 0) * GameMap.tileHeight, vert3.z)); //11
        waterVerts.Add(new Vector3(vert5.x, (waterLevel - 0) * GameMap.tileHeight, vert5.z)); //12
        waterVerts.Add(new Vector3(vert6.x, (waterLevel - 0) * GameMap.tileHeight, vert6.z)); //13
        waterVerts.Add(new Vector3(vert8.x, (waterLevel - 0) * GameMap.tileHeight, vert8.z)); //14
        waterVerts.Add(new Vector3(vert9.x, (waterLevel - 0) * GameMap.tileHeight, vert9.z)); //15

        for (int i = 0; i < 6; i++)
            waterUvs.Add(new Vector2(waterVerts[waterIndex + i].x, waterVerts[waterIndex + i].z));

        waterTris.Add(waterIndex + 0);
        waterTris.Add(waterIndex + 4);
        waterTris.Add(waterIndex + 5);

        waterTris.Add(waterIndex + 0);
        waterTris.Add(waterIndex + 1);
        waterTris.Add(waterIndex + 4);

        waterTris.Add(waterIndex + 1);
        waterTris.Add(waterIndex + 2);
        waterTris.Add(waterIndex + 4);

        waterTris.Add(waterIndex + 2);
        waterTris.Add(waterIndex + 3);
        waterTris.Add(waterIndex + 4);

        //waterfalls again!
        if (waterLevelNorth > waterLevel)
        {
            AddQuad(
                new Vector3(vert2.x, waterLevelNorth * GameMap.tileHeight, vert2.z),
                new Vector3(vert3.x, waterLevelNorth * GameMap.tileHeight, vert3.z),
                new Vector3(vert3.x, waterLevel * GameMap.tileHeight, vert3.z) - ((vert1 - vert10) / 48),
                new Vector3(vert2.x, waterLevel * GameMap.tileHeight, vert2.z) - ((vert1 - vert10) / 48),
                biome, terrainColor, waterVerts, null, waterUvs, null, waterTris
                );
        }
        if (waterLevelEast > waterLevel)
        {
            AddQuad(
                new Vector3(vert5.x, waterLevelEast * GameMap.tileHeight, vert5.z),
                new Vector3(vert6.x, waterLevelEast * GameMap.tileHeight, vert6.z),
                new Vector3(vert6.x, waterLevel * GameMap.tileHeight, vert6.z) - ((vert4 - vert1) / 48),
                new Vector3(vert5.x, waterLevel * GameMap.tileHeight, vert5.z) - ((vert4 - vert1) / 48),
                biome, terrainColor, waterVerts, null, waterUvs, null, waterTris
                );
        }
        if (waterLevelSouth > waterLevel)
        {
            AddQuad(
                new Vector3(vert8.x, waterLevelSouth * GameMap.tileHeight, vert8.z),
                new Vector3(vert9.x, waterLevelSouth * GameMap.tileHeight, vert9.z),
                new Vector3(vert9.x, waterLevel * GameMap.tileHeight, vert9.z) - ((vert10 - vert1) / 48),
                new Vector3(vert8.x, waterLevel * GameMap.tileHeight, vert8.z) - ((vert10 - vert1) / 48),
                biome, terrainColor, waterVerts, null, waterUvs, null, waterTris
                );
        }

    }

    private void AddWall(Vector3 start, Vector3 end, float width, Vector2 biome, Color color)
    {
        Vector3 front = new Vector3(end.x - start.x, 0, end.z - start.z).normalized * width / 2;
        Vector3 left = new Vector3(-front.z, 0, front.x);
        Vector3 down = new Vector3(0, end.y - start.y, 0);

        Vector3 v1 = new Vector3(start.x, start.y, start.z) + left - front;
        Vector3 v4 = new Vector3(start.x, start.y, start.z) - left - front;
        Vector3 v2 = new Vector3(end.x, start.y, end.z) + left + front;
        Vector3 v3 = new Vector3(end.x, start.y, end.z) - left + front;

        AddQuad(v1, v2, v3, v4, biome, color, vertices, colors, uvCoords, uvCoords2, triangles);

        AddVerticalQuad(v1, v2 + down, biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(v2, v3 + down, biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(v3, v4 + down, biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(v4, v1 + down, biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
    }

    private void AddBlock(Vector3 min, Vector3 max, Vector2 biome, Color color, Color? roofColor = null)
    {
        if (roofColor == null)
            AddHorizontalQuad(new Vector3(min.x, max.y, min.z), max, biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        else
            AddHorizontalQuad(new Vector3(min.x, max.y, min.z), max, biome, roofColor.Value, vertices, colors, uvCoords, uvCoords2, triangles);

        AddVerticalQuad(new Vector3(min.x, max.y, min.z), new Vector3(max.x, min.y, min.z), biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(new Vector3(max.x, max.y, min.z), new Vector3(max.x, min.y, max.z), biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(new Vector3(max.x, max.y, max.z), new Vector3(min.x, min.y, max.z), biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
        AddVerticalQuad(new Vector3(min.x, max.y, max.z), new Vector3(min.x, min.y, min.z), biome, color, vertices, colors, uvCoords, uvCoords2, triangles);
    }

    private void AddCylinder(Vector3 min, Vector3 max, Vector2 biome, Color color, Color? roofColor = null)
    {
        int index = vertices.Count;

        Color roof = color;
        if (roofColor != null)
            roof = roofColor.Value;

        float dia = ((max.x - min.x) + (max.y - min.y)) / 2;

        Vector2 offset = new Vector2(0.206931f, 0.5f) * dia;

        Vector3 centerTop = (max + new Vector3(min.x, max.y, min.z)) / 2;

        vertices.Add(new Vector3(centerTop.x - offset.x, centerTop.y, centerTop.z + offset.y));
        vertices.Add(new Vector3(centerTop.x + offset.x, centerTop.y, centerTop.z + offset.y));
        vertices.Add(new Vector3(centerTop.x + offset.y, centerTop.y, centerTop.z + offset.x));
        vertices.Add(new Vector3(centerTop.x + offset.y, centerTop.y, centerTop.z - offset.x));
        vertices.Add(new Vector3(centerTop.x + offset.x, centerTop.y, centerTop.z - offset.y));
        vertices.Add(new Vector3(centerTop.x - offset.x, centerTop.y, centerTop.z - offset.y));
        vertices.Add(new Vector3(centerTop.x - offset.y, centerTop.y, centerTop.z - offset.x));
        vertices.Add(new Vector3(centerTop.x - offset.y, centerTop.y, centerTop.z + offset.x));

        for (int i = 0; i < 8; i++)
        {
            colors.Add(roof);
            uvCoords2.Add(biome);
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
        }

        for (int i = 0; i < 6; i++)
        {
            triangles.Add(index + 0);
            triangles.Add(index + i + 1);
            triangles.Add(index + i + 2);
        }

        int newIndex = vertices.Count;

        for(int i = 0; i < 8; i++)
        {
            vertices.Add(vertices[index + i]);
            vertices.Add(new Vector3(vertices[index + i].x, min.y, vertices[index + i].z));
            colors.Add(color);
            uvCoords2.Add(biome);
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            colors.Add(color);
            uvCoords2.Add(biome);
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
        }

        for (int i = 0; i < 14; i+=2)
        {
            triangles.Add(newIndex + i);
            triangles.Add(newIndex + i + 1);
            triangles.Add(newIndex + i + 2);

            triangles.Add(newIndex + i + 1);
            triangles.Add(newIndex + i + 3);
            triangles.Add(newIndex + i + 2);
        }
        triangles.Add(newIndex + 14);
        triangles.Add(newIndex + 14 + 1);
        triangles.Add(newIndex + 0);

        triangles.Add(newIndex + 14 + 1);
        triangles.Add(newIndex + 1);
        triangles.Add(newIndex + 0);
    }

    private void AddPyramid(Vector3 min, Vector3 max, Vector2 biome, Color color)
    {
        int index = vertices.Count;

        vertices.Add(new Vector3((min.x + max.x) / 2, max.y, (min.z + max.z) / 2));
        vertices.Add(new Vector3(min.x, min.y, min.z));
        vertices.Add(new Vector3(max.x, min.y, min.z));
        vertices.Add(new Vector3(max.x, min.y, max.z));
        vertices.Add(new Vector3(min.x, min.y, max.z));

        for (int i = 0; i < 5; i++)
        {
            colors.Add(color);
            uvCoords2.Add(biome);
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
        }

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

        triangles.Add(index + 0);
        triangles.Add(index + 3);
        triangles.Add(index + 4);

        triangles.Add(index + 0);
        triangles.Add(index + 4);
        triangles.Add(index + 1);
    }

    private void AddTree(Vector3 root, float radius, float height, Color color, Vector2 biome, float rotation = 0)
    {
        int index = vertices.Count;
        vertices.Add(new Vector3(root.x, root.y + height, root.z));
        colors.Add(color);
        uvCoords2.Add(biome);
        uvCoords.Add(new Vector2(vertices[index].x, vertices[index].z));
        Vector2 spoke = new Vector2(radius, 0);
        spoke = spoke.Rotate(rotation);
        index = vertices.Count;
        for (int i = 0; i < 12; i += 2)
        {
            spoke = spoke.Rotate(360.0f / 6);
            vertices.Add(new Vector3(root.x + spoke.x, root.y + height - (0.577f * radius), root.z + spoke.y));
            vertices.Add(new Vector3(root.x + spoke.x, root.y , root.z + spoke.y));

            colors.Add(color);
            colors.Add(color);
            uvCoords2.Add(biome);
            uvCoords2.Add(biome);
            uvCoords.Add(new Vector2(vertices[index + i].x, vertices[index + i].z));
            uvCoords.Add(new Vector2(vertices[index + i + 1].x, vertices[index + i + 1].z));

            triangles.Add(index - 1);
            triangles.Add(index + (i + 2) % 12);
            triangles.Add(index + i);

            triangles.Add(index + (i + 0) % 12);
            triangles.Add(index + (i + 3) % 12);
            triangles.Add(index + (i + 1) % 12);

            triangles.Add(index + (i + 0) % 12);
            triangles.Add(index + (i + 2) % 12);
            triangles.Add(index + (i + 3) % 12);
        }
    }

    private void AddFlatTile(Vector3 vert1, Vector2 biome, float north, float east, float south, float west, int waterLevel, Color terrainColor)
    {
        Vector3 vert2 = vert1 + new Vector3(48 * GameMap.tileWidth, 0, -48 * GameMap.tileWidth);

        Vector3 vert3;
        Vector3 vert4;

        AddHorizontalQuad(vert1, vert2, biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);

        if (vert1.y < waterLevel * GameMap.tileHeight)
        {
            vert3 = new Vector3(vert1.x, waterLevel * GameMap.tileHeight, vert1.z);
            vert4 = new Vector3(vert2.x, waterLevel * GameMap.tileHeight, vert2.z);

            AddHorizontalQuad(vert3, vert4, biome, terrainColor, waterVerts, null, waterUvs, null, waterTris);
        }

        if (north < vert1.y)
        {
            vert3 = vert1;
            vert4 = new Vector3(vert2.x, north, vert1.z);

            AddVerticalQuad(vert3, vert4, biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (south < vert1.y)
        {
            vert3 = vert2;
            vert4 = new Vector3(vert1.x, south, vert2.z);

            AddVerticalQuad(vert3, vert4, biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (east < vert1.y)
        {
            vert3 = new Vector3(vert2.x, vert1.y, vert1.z);
            vert4 = new Vector3(vert2.x, east, vert2.z);

            AddVerticalQuad(vert3, vert4, biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }

        if (west < vert1.y)
        {
            vert3 = new Vector3(vert1.x, vert1.y, vert2.z);
            vert4 = new Vector3(vert1.x, west, vert1.z);

            AddVerticalQuad(vert3, vert4, biome, terrainColor, vertices, colors, uvCoords, uvCoords2, triangles);
        }
    }

    public static void AddQuad(Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 vert4, Vector2 biome, Color color, List<Vector3> vertices, List<Color> colors, List<Vector2> uvs, List<Vector2> uv2s, List<int> triangles)
    {
        int index = vertices.Count;
        vertices.Add(vert1);
        vertices.Add(vert2);
        vertices.Add(vert3);
        vertices.Add(vert4);

        for(int i = 0; i < 4; i++)
        {
            if (colors != null)
                colors.Add(color);
            if (uv2s != null)
                uv2s.Add(biome);
            if (uvs != null)
                uvs.Add(new Vector2(vertices[index+i].x, vertices[index + i].z));
        }

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 3);

        triangles.Add(index + 1);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

    }

    public static void AddHorizontalQuad(Vector3 vert1, Vector3 vert2, Vector2 biome, Color color, List<Vector3> vertices, List<Color> colors, List<Vector2> uvs, List<Vector2> uv2s, List<int> triangles)
    {
        AddQuad(new Vector3(vert1.x, vert1.y, vert1.z), new Vector3(vert2.x, vert1.y, vert1.z),  new Vector3(vert2.x, vert2.y, vert2.z), new Vector3(vert1.x, vert2.y, vert2.z), biome, color, vertices, colors, uvs, uv2s, triangles);
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
