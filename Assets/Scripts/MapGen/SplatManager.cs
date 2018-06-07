using MaterialStore;
using RemoteFortressReader;
using System;
using UnityEngine;

public class SplatManager : MonoBehaviour
{
    bool[,,] spatterDirtyBits;
    bool[,,] terrainDirtyBits;
    bool[,,] grassDirtyBits;

    Texture2D[] spatterLayers;
    Texture2D[] terrainSplatLayers;
    Texture2D[] terrainTintLayers;
    Texture2D[] grassSplatLayers;
    Texture2D[] grassTintLayers;

    Color[][] spatterColor;
    Color[][] terrainSplatColor;
    Color[][] terrainTintColor;
    Color[][] grassSplatColor;
    Color[][] grassTintColor;

    const int timeout = 10;

    public static SplatManager Instance { get; private set; }

    public int SizeZ
    {
        get
        {
            return spatterLayers.Length;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateSplatTextures();
    }

    internal void DirtyLayer(int x, int y, int z)
    {
        terrainDirtyBits[x, y, z] = true;
    }

    internal void DirtyGrass(int x, int y, int z)
    {
        grassDirtyBits[x, y, z] = true;
    }

    internal void DirtySpatter(int x, int y, int z)
    {
        spatterDirtyBits[x, y, z] = true;
    }

    internal void Init(int x, int y, int z)
    {
        spatterDirtyBits = new bool[x, y, z];
        terrainDirtyBits = new bool[x, y, z];
        grassDirtyBits = new bool[x, y, z];

        spatterLayers = new Texture2D[z];
        terrainSplatLayers = new Texture2D[z];
        terrainTintLayers = new Texture2D[z];
        grassSplatLayers = new Texture2D[z];
        grassTintLayers = new Texture2D[z];

        spatterColor = new Color[z][];
        terrainSplatColor = new Color[z][];
        terrainTintColor = new Color[z][];
        grassSplatColor = new Color[z][];
        grassTintColor = new Color[z][];
    }

    void UpdateSplatTextures()
    {
        if (ContentLoader.Instance == null)
            return;
        UnityEngine.Profiling.Profiler.BeginSample("UpdateTerrainTextures", this);
        var timer = System.Diagnostics.Stopwatch.StartNew();
        for (int z = terrainDirtyBits.GetLength(2) - 1; z >= 0; z--)
        {
            for (int y = 0; y < terrainDirtyBits.GetLength(1); y++)
                for (int x = 0; x < terrainDirtyBits.GetLength(0); x++)
                {
                    if (terrainDirtyBits[x, y, z])
                    {
                        GenerateTerrainTexture(x, y, z);
                        UpdateTerrainSplatTexture(x, y, z);
                        UpdateTerrainTintTexture(x, y, z);
                        terrainDirtyBits[x, y, z] = false;
                        if (timer.ElapsedMilliseconds > timeout)
                            break;
                    }
                    if (grassDirtyBits[x, y, z])
                    {
                        if(GenerateGrassTexture(x, y, z))
                            UpdateGrassTexture(x, y, z);
                        grassDirtyBits[x, y, z] = false;
                        if (timer.ElapsedMilliseconds > timeout)
                            break;
                    }
                    if (spatterDirtyBits[x, y, z])
                    {
                        GenerateSpatterTexture(x, y, z);
                        UpdateSplatterTexture(x, y, z);
                        spatterDirtyBits[x, y, z] = false;
                        if (timer.ElapsedMilliseconds > timeout)
                            break;
                    }
                    if (timer.ElapsedMilliseconds > timeout)
                        break;
                }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void GenerateTerrainTexture(int blockX, int blockY, int blockZ)
    {
        if (ContentLoader.Instance == null)
            return;
        UnityEngine.Profiling.Profiler.BeginSample("GenerateTerrainTexture", this);

        if (terrainSplatColor[blockZ] == null || terrainTintColor[blockZ] == null || terrainSplatColor[blockZ].Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
        {
            CreateBlankTerrainTextures(blockZ);
        }

        var terrainIndices = terrainSplatColor[blockZ];
        var terrainColors = terrainTintColor[blockZ];

        UnityEngine.Profiling.Profiler.BeginSample("Update Terrain Color Array", this);
        for (int y = blockY * GameMap.blockSize; y < (blockY+1) * GameMap.blockSize; y++)
            for (int x = blockX * GameMap.blockSize; x < (blockX + 1) * GameMap.blockSize; x++)
            {
                int index = x + (y * MapDataStore.MapSize.x);
                var tile = MapDataStore.Main[x, y, blockZ];
                if (IsNullOrEmpty(tile))
                {
                    if (!IsNullOrEmpty(MapDataStore.Main[x - 1, y, blockZ]))
                        tile = MapDataStore.Main[x - 1, y, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x, y - 1, blockZ]))
                        tile = MapDataStore.Main[x, y - 1, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x + 1, y, blockZ]))
                        tile = MapDataStore.Main[x + 1, y, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x, y + 1, blockZ]))
                        tile = MapDataStore.Main[x, y + 1, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x - 1, y - 1, blockZ]))
                        tile = MapDataStore.Main[x - 1, y - 1, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x - 1, y + 1, blockZ]))
                        tile = MapDataStore.Main[x - 1, y + 1, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x + 1, y - 1, blockZ]))
                        tile = MapDataStore.Main[x + 1, y - 1, blockZ];
                    else if (!IsNullOrEmpty(MapDataStore.Main[x + 1, y + 1, blockZ]))
                        tile = MapDataStore.Main[x + 1, y + 1, blockZ];
                    else
                    {
                        continue;
                    }
                }
                if (tile.shape == TiletypeShape.RAMP_TOP && tile.Down != null)
                    tile = tile.Down;

                var layer = MeshLayer.BaseMaterial;
                if (tile.tiletypeMaterial == TiletypeMaterial.GRASS_DARK
                || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DEAD
                || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DRY
                || tile.tiletypeMaterial == TiletypeMaterial.GRASS_LIGHT
                || tile.tiletypeMaterial == TiletypeMaterial.PLANT
                )
                    layer = MeshLayer.LayerMaterial;
                if (tile.special == TiletypeSpecial.NORMAL || layer == MeshLayer.LayerMaterial)
                {
                    //If it's natural terrain or grass, use the material normal maps.
                    terrainIndices[index].g = ContentLoader.GetShapeIndex(tile.GetMaterial(layer)) / 255f;
                }
                else
                {
                    NormalContent norms;
                    ContentLoader.Instance.ShapeTextureConfiguration.GetValue(tile, layer, out norms);
                    terrainIndices[index].g = norms.StorageIndex / 255f;
                }
                terrainIndices[index].r = ContentLoader.GetPatternIndex(tile.GetMaterial(layer)) / 255f;
                terrainColors[index] = ContentLoader.GetColor(tile.GetMaterial(layer));
            }
        UnityEngine.Profiling.Profiler.EndSample();
        UnityEngine.Profiling.Profiler.EndSample();
    }
    private void CreateBlankTerrainTextures(int blockZ)
    {
        terrainSplatColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
        terrainTintColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
        for (int i = 0; i < terrainTintColor[blockZ].Length; i++)
        {
            terrainSplatColor[blockZ][i].r = ContentLoader.Instance.DefaultMatTexIndex;
            terrainSplatColor[blockZ][i].g = ContentLoader.Instance.DefaultShapeTexIndex;
            terrainTintColor[blockZ][i] = Color.gray;
        }
        UpdateTerrainSplatTexture(blockZ);
        UpdateTerrainTintTexture(blockZ);
    }

    private void UpdateTerrainTintTexture(int blockX, int blockY, int blockZ)
    {
        if (terrainTintLayers[blockZ] == null)
        {
            terrainTintLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, false);
            terrainTintLayers[blockZ].filterMode = FilterMode.Point;
            terrainTintLayers[blockZ].wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < GameMap.Instance.mapMeshes.GetLength(0); x++)
                for (int y = 0; y < GameMap.Instance.mapMeshes.GetLength(1); y++)
                {
                    if (GameMap.Instance.mapMeshes[x, y, blockZ] != null)
                        GameMap.Instance.mapMeshes[x, y, blockZ].SetTerrainMap(terrainSplatLayers[blockZ], terrainTintLayers[blockZ]);
                }
            terrainTintLayers[blockZ].SetPixels(terrainTintColor[blockZ]);
        }
        if (terrainTintLayers[blockZ].width != MapDataStore.MapSize.x || terrainTintLayers[blockZ].height != MapDataStore.MapSize.y)
            terrainTintLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainTintLayers[blockZ].SetPixels(blockX * GameMap.blockSize, blockY * GameMap.blockSize, GameMap.blockSize, GameMap.blockSize, GetColorBlock(blockX, blockY, terrainTintColor[blockZ]));
        terrainTintLayers[blockZ].Apply();
    }

    private void UpdateTerrainTintTexture(int blockZ)
    {
        if (terrainTintLayers[blockZ] == null)
        {
            terrainTintLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, false);
            terrainTintLayers[blockZ].filterMode = FilterMode.Point;
            terrainTintLayers[blockZ].wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < GameMap.Instance.mapMeshes.GetLength(0); x++)
                for (int y = 0; y < GameMap.Instance.mapMeshes.GetLength(1); y++)
                {
                    if (GameMap.Instance.mapMeshes[x, y, blockZ] != null)
                        GameMap.Instance.mapMeshes[x, y, blockZ].SetTerrainMap(terrainSplatLayers[blockZ], terrainTintLayers[blockZ]);
                }
        }
        if (terrainTintLayers[blockZ].width != MapDataStore.MapSize.x || terrainTintLayers[blockZ].height != MapDataStore.MapSize.y)
            terrainTintLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainTintLayers[blockZ].SetPixels(terrainTintColor[blockZ]);
        terrainTintLayers[blockZ].Apply();
    }

    private void UpdateTerrainSplatTexture(int blockX, int blockY, int blockZ)
    {
        if (terrainSplatLayers[blockZ] == null)
        {
            terrainSplatLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGB24, false, true);
            terrainSplatLayers[blockZ].filterMode = FilterMode.Point;
            terrainSplatLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
            terrainSplatLayers[blockZ].SetPixels(terrainSplatColor[blockZ]);
        }
        if (terrainSplatLayers[blockZ].width != MapDataStore.MapSize.x || terrainSplatLayers[blockZ].height != MapDataStore.MapSize.y)
            terrainSplatLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainSplatLayers[blockZ].SetPixels(blockX * GameMap.blockSize, blockY * GameMap.blockSize, GameMap.blockSize, GameMap.blockSize, GetColorBlock(blockX, blockY, terrainSplatColor[blockZ]));
        terrainSplatLayers[blockZ].Apply();
    }

    private void UpdateTerrainSplatTexture(int blockZ)
    {
        if (terrainSplatLayers[blockZ] == null)
        {
            terrainSplatLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGB24, false, true);
            terrainSplatLayers[blockZ].filterMode = FilterMode.Point;
            terrainSplatLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
        }
        if (terrainSplatLayers[blockZ].width != MapDataStore.MapSize.x || terrainSplatLayers[blockZ].height != MapDataStore.MapSize.y)
            terrainSplatLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainSplatLayers[blockZ].SetPixels(terrainSplatColor[blockZ]);
        terrainSplatLayers[blockZ].Apply();
    }

    bool GenerateGrassTexture(int blockX, int blockY, int blockZ)
    {
        if (ContentLoader.Instance == null)
            return false;
        UnityEngine.Profiling.Profiler.BeginSample("GenerateGrassTexture", this);

        if (grassTintColor[blockZ] == null || grassSplatColor[blockZ] == null || grassTintColor[blockZ].Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
        {
            grassTintColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
            grassSplatColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
        }

        var grassIndices = grassSplatColor[blockZ];
        var grassColors = grassTintColor[blockZ];

        int grassTiles = 0;

        for (int y = blockY * GameMap.blockSize; y < (blockY + 1) * GameMap.blockSize; y++)
            for (int x = blockX * GameMap.blockSize; x < (blockX + 1) * GameMap.blockSize; x++)
            {
                int index = x + (y * MapDataStore.MapSize.x);
                var tile = MapDataStore.Main[x, y, blockZ];
                if (tile == null)
                {
                    continue;
                }
                if (!(tile.tiletypeMaterial == TiletypeMaterial.GRASS_DARK
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_LIGHT
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DEAD
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DRY
                    ))
                    continue;

                grassTiles++;

                grassIndices[index].r = ContentLoader.GetPatternIndex(tile.material) / 255f;
                grassIndices[index].g = ContentLoader.GetShapeIndex(tile.material) / 255f;
                grassColors[index] = ContentLoader.GetColor(tile.material);
                grassColors[index].a = tile.grassPercent / 100f;
            }
        if (grassTiles == 0)
        {
            UnityEngine.Profiling.Profiler.EndSample();
            return false;
        }
        UnityEngine.Profiling.Profiler.EndSample();
        return true;
    }

    void UpdateGrassTexture(int blockZ)
    {
        if (grassSplatLayers[blockZ] == null)
        {
            grassSplatLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGB24, false, true);
            grassSplatLayers[blockZ].filterMode = FilterMode.Point;
            grassSplatLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
        }
        if (grassSplatLayers[blockZ].width != MapDataStore.MapSize.x || terrainSplatLayers[blockZ].height != MapDataStore.MapSize.y)
            grassSplatLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassSplatLayers[blockZ].SetPixels(grassSplatColor[blockZ]);
        grassSplatLayers[blockZ].Apply();

        if (grassTintLayers[blockZ] == null)
        {
            grassTintLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, false);
            grassTintLayers[blockZ].filterMode = FilterMode.Point;
            grassTintLayers[blockZ].wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < GameMap.Instance.mapMeshes.GetLength(0); x++)
                for (int y = 0; y < GameMap.Instance.mapMeshes.GetLength(1); y++)
                {
                    if (GameMap.Instance.mapMeshes[x, y, blockZ] != null)
                        GameMap.Instance.mapMeshes[x, y, blockZ].SetGrassMap(grassSplatLayers[blockZ], grassTintLayers[blockZ]);
                }
        }
        if (grassTintLayers[blockZ].width != MapDataStore.MapSize.x || terrainTintLayers[blockZ].height != MapDataStore.MapSize.y)
            grassTintLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassTintLayers[blockZ].SetPixels(grassTintColor[blockZ]);
        grassTintLayers[blockZ].Apply();
    }

    void UpdateGrassTexture(int blockX, int blockY, int blockZ)
    {
        if (grassSplatLayers[blockZ] == null)
        {
            grassSplatLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGB24, false, true);
            grassSplatLayers[blockZ].filterMode = FilterMode.Point;
            grassSplatLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
            grassSplatLayers[blockZ].SetPixels(grassSplatColor[blockZ]);
        }
        if (grassSplatLayers[blockZ].width != MapDataStore.MapSize.x || terrainSplatLayers[blockZ].height != MapDataStore.MapSize.y)
            grassSplatLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassSplatLayers[blockZ].SetPixels(blockX * GameMap.blockSize, blockY * GameMap.blockSize, GameMap.blockSize, GameMap.blockSize, GetColorBlock(blockX, blockY, grassSplatColor[blockZ]));
        grassSplatLayers[blockZ].Apply();

        if (grassTintLayers[blockZ] == null)
        {
            grassTintLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, false);
            grassTintLayers[blockZ].filterMode = FilterMode.Point;
            grassTintLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
            grassTintLayers[blockZ].SetPixels(grassTintColor[blockZ]);

            for (int x = 0; x < GameMap.Instance.mapMeshes.GetLength(0); x++)
                for (int y = 0; y < GameMap.Instance.mapMeshes.GetLength(1); y++)
                {
                    if (GameMap.Instance.mapMeshes[x, y, blockZ] != null)
                        GameMap.Instance.mapMeshes[x, y, blockZ].SetGrassMap(grassSplatLayers[blockZ], grassTintLayers[blockZ]);
                }
        }
        if (grassTintLayers[blockZ].width != MapDataStore.MapSize.x || terrainTintLayers[blockZ].height != MapDataStore.MapSize.y)
            grassTintLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassTintLayers[blockZ].SetPixels(blockX * GameMap.blockSize, blockY * GameMap.blockSize, GameMap.blockSize, GameMap.blockSize, GetColorBlock(blockX, blockY, grassTintColor[blockZ]));
        grassTintLayers[blockZ].Apply();
    }

    void GenerateSpatterTexture(int blockX, int blockY, int blockZ)
    {
        UnityEngine.Profiling.Profiler.BeginSample("GenerateSpatterTexture", this);
        if (spatterColor[blockZ] == null || spatterColor[blockZ].Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
            spatterColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];

        var textureColors = spatterColor[blockZ];

        for (int y = blockY * GameMap.blockSize; y < (blockY + 1) * GameMap.blockSize; y++)
            for (int x = blockX * GameMap.blockSize; x < (blockX + 1) * GameMap.blockSize; x++)
            {
                Color totalColor = new Color(0, 0, 0, 0);
                int index = x + (y * MapDataStore.MapSize.x);

                var tile = MapDataStore.Main[x, y, blockZ];
                if (tile == null)
                {
                    textureColors[index] = totalColor;
                    continue;
                }
                if (tile.spatters == null || tile.spatters.Count == 0)
                {
                    textureColors[index] = totalColor;
                    continue;
                }

                if (tile.Hidden)
                {
                    textureColors[index] = totalColor;
                    continue;
                }
                float totalAmount = 0;

                foreach (var spatter in tile.spatters)
                {
                    if (spatter.amount == 0)
                        continue;

                    Color color = Color.white;

                    if (spatter.material.mat_type == (int)MatBasic.ICE && spatter.state == MatterState.Powder)
                        color = Color.white;
                    else
                    {
                        if (spatter.item != null)
                            color = ContentLoader.GetGrowthColor(ContentLoader.GrowthPeriod.Stale, spatter.item.mat_index, spatter.material);
                        else
                            color = ContentLoader.GetColor(spatter.material);
                    }

                    float amount = spatter.amount;
                    if (spatter.item != null)
                        amount /= 3000;
                    else
                        amount /= 100;
                    //amount = Mathf.Clamp01(amount);

                    color *= amount;

                    color.a = amount;

                    totalColor += color;
                    totalAmount += amount;
                }
                if (totalAmount > 1)
                {
                    totalColor /= totalAmount;
                }
                textureColors[index] = totalColor;
            }

        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void UpdateSplatterTexture(int blockZ)
    {
        if (spatterLayers[blockZ] == null)
        {
            spatterLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.ARGB32, false);
            spatterLayers[blockZ].wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < GameMap.Instance.mapMeshes.GetLength(0); x++)
                for (int y = 0; y < GameMap.Instance.mapMeshes.GetLength(1); y++)
                {
                    if (GameMap.Instance.mapMeshes[x, y, blockZ] != null)
                        GameMap.Instance.mapMeshes[x, y, blockZ].SetSpatterMap(spatterLayers[blockZ]);
                }
        }
        if (spatterLayers[blockZ].width != MapDataStore.MapSize.x || spatterLayers[blockZ].height != MapDataStore.MapSize.y)
            spatterLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        spatterLayers[blockZ].SetPixels(spatterColor[blockZ]);
        spatterLayers[blockZ].Apply();
    }

    private void UpdateSplatterTexture(int blockX, int blockY, int blockZ)
    {
        if (spatterLayers[blockZ] == null)
        {
            spatterLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.ARGB32, false);
            spatterLayers[blockZ].wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < GameMap.Instance.mapMeshes.GetLength(0); x++)
                for (int y = 0; y < GameMap.Instance.mapMeshes.GetLength(1); y++)
                {
                    if (GameMap.Instance.mapMeshes[x, y, blockZ] != null)
                        GameMap.Instance.mapMeshes[x, y, blockZ].SetSpatterMap(spatterLayers[blockZ]);
                }
            spatterLayers[blockZ].SetPixels(spatterColor[blockZ]);
        }
        if (spatterLayers[blockZ].width != MapDataStore.MapSize.x || spatterLayers[blockZ].height != MapDataStore.MapSize.y)
        {
            spatterLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);
            spatterLayers[blockZ].SetPixels(spatterColor[blockZ]);
        }

        spatterLayers[blockZ].SetPixels(blockX * GameMap.blockSize, blockY * GameMap.blockSize, GameMap.blockSize, GameMap.blockSize, GetColorBlock(blockX, blockY, spatterColor[blockZ]));
        spatterLayers[blockZ].Apply();
    }



    private bool IsNullOrEmpty(MapDataStore.Tile tile)
    {
        if (tile == null)
            return true;
        if (tile.shape == TiletypeShape.EMPTY)
            return true;
        if (tile.shape == TiletypeShape.ENDLESS_PIT)
            return true;
        return false;
    }

    internal bool HasTerrain(int block_z)
    {
        return terrainSplatLayers[block_z] != null;
    }
    internal bool HasGrass(int block_z)
    {
        return grassSplatLayers[block_z] != null;
    }
    internal bool HasSpatter(int block_z)
    {
        return spatterLayers[block_z] != null;
    }

    internal void ApplyTerrain(BlockMeshSet blockMeshSet, int block_z)
    {
        if (!HasTerrain(block_z))
            CreateBlankTerrainTextures(block_z);
        blockMeshSet.SetTerrainMap(terrainSplatLayers[block_z], terrainTintLayers[block_z]);
    }
    internal void ApplyGrass(BlockMeshSet blockMeshSet, int block_z)
    {
        if (HasGrass(block_z))
            blockMeshSet.SetGrassMap(grassSplatLayers[block_z], grassTintLayers[block_z]);
    }
    internal void ApplySpatter(BlockMeshSet blockMeshSet, int block_z)
    {
        if (HasSpatter(block_z))
            blockMeshSet.SetSpatterMap(spatterLayers[block_z]);
    }

    static Color[] tempColorBlock;

    static Color[] GetColorBlock(int blockX, int blockY, Color[] colors)
    {
        if (tempColorBlock == null)
            tempColorBlock = new Color[GameMap.blockSize * GameMap.blockSize];

        for (int y = 0; y < GameMap.blockSize; y++)
            for (int x = 0; x < GameMap.blockSize; x++)
            {
                tempColorBlock[x + (y * GameMap.blockSize)] = colors[(x + blockX * GameMap.blockSize) + ((y + blockY * GameMap.blockSize) * MapDataStore.MapSize.x)];
            }
        return tempColorBlock;
    }
}
