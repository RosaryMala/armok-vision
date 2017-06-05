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


    static SplatManager _instance;
    public static SplatManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public int SizeZ
    {
        get
        {
            return spatterLayers.Length;
        }
    }

    private void Awake()
    {
        _instance = this;
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
            for (int y = 0; y < terrainDirtyBits.GetLength(1); y++)
                for (int x = 0; x < terrainDirtyBits.GetLength(0); x++)
                {
                    if (terrainDirtyBits[x, y, z])
                    {
                        GenerateTerrainTexture(x, y, z);
                        terrainDirtyBits[x, y, z] = false;
                        if(timer.ElapsedMilliseconds > 0.01)
                            goto exit;
                    }
                    if (grassDirtyBits[x, y, z])
                    {
                        GenerateGrassTexture(x, y, z);
                        grassDirtyBits[x, y, z] = false;
                        if (timer.ElapsedMilliseconds > 0.01)
                            goto exit;
                    }
                    if (spatterDirtyBits[x, y, z])
                    {
                        GenerateSpatterTexture(x, y, z);
                        spatterDirtyBits[x, y, z] = false;
                        if (timer.ElapsedMilliseconds > 0.01)
                            goto exit;
                    }
                }
        exit:
        UnityEngine.Profiling.Profiler.EndSample();
    }

    void GenerateTerrainTexture(int blockX, int blockY, int blockZ)
    {
        if (ContentLoader.Instance == null)
            return;
        UnityEngine.Profiling.Profiler.BeginSample("GenerateTerrainTexture", this);

        if (terrainSplatColor[blockZ] == null || terrainTintColor[blockZ] == null || terrainSplatColor[blockZ].Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
        {
            terrainSplatColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
            terrainTintColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];
        }

        var terrainIndices = terrainSplatColor[blockZ];
        var terrainColors = terrainTintColor[blockZ];

        UnityEngine.Profiling.Profiler.BeginSample("Update Terrain Color Array", this);
        for (int y = blockY * GameMap.blockSize; y < (blockY+1) * GameMap.blockSize; y++)
            for (int x = blockX * GameMap.blockSize; x < (blockX + 1) * GameMap.blockSize; x++)
            {
                int index = x + (y * MapDataStore.MapSize.x);
                var tile = MapDataStore.Main[x, y, blockZ];
                UnityEngine.Profiling.Profiler.BeginSample("Find nearest tile", this);
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
                        terrainIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                        terrainIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                        terrainColors[index] = Color.gray;
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
                UnityEngine.Profiling.Profiler.EndSample();

                UnityEngine.Profiling.Profiler.BeginSample("Normal texture lookup", this);
                NormalContent normalContent;
                if (ContentLoader.Instance.TerrainShapeTextureConfiguration.GetValue(tile, layer, out normalContent))
                    terrainIndices[index].g = normalContent.StorageIndex;
                else
                    terrainIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                UnityEngine.Profiling.Profiler.EndSample();

                UnityEngine.Profiling.Profiler.BeginSample("Diffuse texture lookup", this);
                TextureContent materialContent;
                if (ContentLoader.Instance.MaterialTextureConfiguration.GetValue(tile, layer, out materialContent))
                    terrainIndices[index].r = materialContent.StorageIndex;
                else
                    terrainIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                UnityEngine.Profiling.Profiler.EndSample();

                UnityEngine.Profiling.Profiler.BeginSample("Diffuse color lookup", this);
                ColorContent colorContent;
                if (ContentLoader.Instance.ColorConfiguration.GetValue(tile, layer, out colorContent))
                    terrainColors[index] = colorContent.color;
                else
                    terrainColors[index] = Color.gray;
                UnityEngine.Profiling.Profiler.EndSample();

            }
        UnityEngine.Profiling.Profiler.EndSample();
        UnityEngine.Profiling.Profiler.BeginSample("Apply terrain textures.", this);

        if (terrainSplatLayers[blockZ] == null)
        {
            terrainSplatLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGHalf, false, true);
            terrainSplatLayers[blockZ].filterMode = FilterMode.Point;
            terrainSplatLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
        }
        if (terrainSplatLayers[blockZ].width != MapDataStore.MapSize.x || terrainSplatLayers[blockZ].height != MapDataStore.MapSize.y)
            terrainSplatLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        terrainSplatLayers[blockZ].SetPixels(terrainIndices);
        terrainSplatLayers[blockZ].Apply();

        if (terrainTintLayers[blockZ] == null)
        {
            terrainTintLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, true);
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

        terrainTintLayers[blockZ].SetPixels(terrainColors);
        terrainTintLayers[blockZ].Apply();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.EndSample();
    }

    void GenerateGrassTexture(int blockX, int blockY, int blockZ)
    {
        if (ContentLoader.Instance == null)
            return;
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
                    grassIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                    grassIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                    grassColors[index] = new Color(0, 0, 0, 0);
                    continue;
                }
                if (!(tile.tiletypeMaterial == TiletypeMaterial.GRASS_DARK
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_LIGHT
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DEAD
                    || tile.tiletypeMaterial == TiletypeMaterial.GRASS_DRY
                    ))
                    continue;

                grassTiles++;

                GrassContent grassTexture;
                if (ContentLoader.Instance.GrassTextureConfiguration.GetValue(tile, MeshLayer.StaticMaterial, out grassTexture))
                {
                    grassIndices[index].r = grassTexture.MaterialTexture.StorageIndex;
                    grassIndices[index].g = grassTexture.ShapeTexture.StorageIndex;
                }
                else
                {
                    grassIndices[index].r = ContentLoader.Instance.DefaultMatTexIndex;
                    grassIndices[index].g = ContentLoader.Instance.DefaultShapeTexIndex;
                }

                ColorContent colorContent;
                if (ContentLoader.Instance.ColorConfiguration.GetValue(tile, MeshLayer.StaticMaterial, out colorContent))
                    grassColors[index] = colorContent.color;
                else
                    grassColors[index] = Color.gray;

            }
        if (grassTiles == 0)
            return;

        if (grassSplatLayers[blockZ] == null)
        {
            grassSplatLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGHalf, false, true);
            grassSplatLayers[blockZ].filterMode = FilterMode.Point;
            grassSplatLayers[blockZ].wrapMode = TextureWrapMode.Clamp;
        }
        if (grassSplatLayers[blockZ].width != MapDataStore.MapSize.x || terrainSplatLayers[blockZ].height != MapDataStore.MapSize.y)
            grassSplatLayers[blockZ].Resize(MapDataStore.MapSize.x, MapDataStore.MapSize.y);

        grassSplatLayers[blockZ].SetPixels(grassIndices);
        grassSplatLayers[blockZ].Apply();

        if (grassTintLayers[blockZ] == null)
        {
            grassTintLayers[blockZ] = new Texture2D(MapDataStore.MapSize.x, MapDataStore.MapSize.y, TextureFormat.RGBA32, false, true);
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

        grassTintLayers[blockZ].SetPixels(grassColors);
        grassTintLayers[blockZ].Apply();

        UnityEngine.Profiling.Profiler.EndSample();
    }

    void GenerateSpatterTexture(int blockX, int blockY, int blockZ)
    {
        UnityEngine.Profiling.Profiler.BeginSample("GenerateSpatterTexture", this);
        if (spatterColor[blockZ] == null || spatterColor[blockZ].Length != MapDataStore.MapSize.x * MapDataStore.MapSize.y)
            spatterColor[blockZ] = new Color[MapDataStore.MapSize.x * MapDataStore.MapSize.y];

        var textureColors = spatterColor[blockZ];

        UnityEngine.Profiling.Profiler.BeginSample("UpdateSpatterArray", this);
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

                    ColorContent cont;

                    if (spatter.material.mat_type == (int)MatBasic.ICE && spatter.state == MatterState.Powder)
                    {
                        color = Color.white;
                    }
                    else if (ContentLoader.Instance.MaterialColors.TryGetValue(spatter.material, out cont))
                    {
                        color = cont.color;
                    }
                    else if (GameMap.materials.ContainsKey(spatter.material))
                    {
                        var colorDef = GameMap.materials[spatter.material].state_color;
                        color = new Color32((byte)colorDef.red, (byte)colorDef.green, (byte)colorDef.blue, 255);
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

        UnityEngine.Profiling.Profiler.BeginSample("Apply Image", this);
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

        spatterLayers[blockZ].SetPixels(textureColors);
        spatterLayers[blockZ].Apply();

        UnityEngine.Profiling.Profiler.EndSample();
        UnityEngine.Profiling.Profiler.EndSample();
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
        if(HasTerrain(block_z))
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
}
