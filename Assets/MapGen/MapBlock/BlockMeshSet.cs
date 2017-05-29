using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExtension;

public class BlockMeshSet : MonoBehaviour
{
    // The actual unity meshes used to draw things on screen.
    /// <summary>
    /// Opaque terrain built from prefab meshes
    /// </summary>
    public Mesh blocks;
    /// <summary>
    /// Cout-out terrain built from prefab meshes
    /// Includes foliage, and things like floor grates.
    /// </summary>
    public Mesh stencilBlocks;
    /// <summary>
    /// Semitransparent terrain built from prefab meshes.
    /// </summary>
    public Mesh transparentBlocks;
    /// <summary>
    /// Top face of opaque terrain, only rendered on the top level.
    /// </summary>
    public Mesh topBlocks;
    /// <summary>
    /// Top face of cutout terrain.
    /// </summary>
    public Mesh topStencilBlocks;
    /// <summary>
    /// Top face of partially transparent terrain
    /// includes glass, etc.
    /// </summary>
    public Mesh topTransparentBlocks;
    /// <summary>
    /// Water and magma meshes.
    /// W dimension is liquid type.
    /// </summary>
    public Mesh[] liquidBlocks;

    /// <summary>
    /// Procedurally generated terrain blocks.
    /// </summary>
    public MeshFilter voxelBlocks;
    MeshRenderer voxelRenderer;
    /// <summary>
    /// top face of procedurally generated terrain blocks.
    /// </summary>
    public MeshFilter topVoxelBlocks;
    MeshRenderer topVoxelRenderer;
    /// <summary>
    /// Procedurally generated grass.
    /// </summary>
    public Mesh grassBlocks;
    /// <summary>
    /// Procedural grass blocks.
    /// </summary>
    public MeshCollider collisionBlocks;

    private void Awake()
    {
        voxelRenderer = voxelBlocks.GetComponent<MeshRenderer>();
        topVoxelRenderer = topVoxelBlocks.GetComponent<MeshRenderer>();
    }

    MaterialManager.MaterialFlags matFlags = MaterialManager.MaterialFlags.None;

    MaterialPropertyBlock matProperties;

    int spatterID;
    int terrainSplatID;
    int terrainTintID;
    int grassSplatID;
    int grassTintID;

    public void Init()
    {
        SetupMaterials();
        spatterID = Shader.PropertyToID("_SpatterTex");
        terrainSplatID = Shader.PropertyToID("_Control");
        terrainTintID = Shader.PropertyToID("_Tint");
        grassSplatID = Shader.PropertyToID("_GrassControl");
        grassTintID = Shader.PropertyToID("_GrassTint");
        matProperties = new MaterialPropertyBlock();
    }

    public void SetupMaterials()
    {
        voxelRenderer.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.SplatMap, matFlags);
        topVoxelRenderer.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.SplatMap, matFlags);
    }

    public Texture2D terrainSplatLayer;
    public Texture2D terrainTintLayer;

    public void SetTerrainMap(Texture2D terrainSplatLayer, Texture2D terrainTintLayer)
    {
        if (matProperties == null)
            Debug.Log("What tyhe hell is wrong!");
        matProperties.SetTexture(terrainSplatID, terrainSplatLayer);
        matProperties.SetTexture(terrainTintID, terrainTintLayer);

        voxelRenderer.SetPropertyBlock(matProperties);
        topVoxelRenderer.SetPropertyBlock(matProperties);
    }

    internal void LoadMeshes(BlockMesher.Result newMeshes, string suffix)
    {
        if (newMeshes.tiles != null)
        {
            if (blocks == null)
            {
                blocks = new Mesh();
                blocks.name = string.Format("block_solid_{0}", suffix);
            }
            blocks.Clear();
            newMeshes.tiles.CopyToMesh(blocks);
        }
        if (newMeshes.topTiles != null)
        {
            if (topBlocks == null)
            {
                topBlocks = new Mesh();
                topBlocks.name = string.Format("block_solid_top_{0}", suffix);
            }
            topBlocks.Clear();
            newMeshes.topTiles.CopyToMesh(topBlocks);
        }
        if (newMeshes.stencilTiles != null)
        {
            if (stencilBlocks == null)
            {
                stencilBlocks = new Mesh();
                stencilBlocks.name = string.Format("block_stencil_{0}", suffix);
            }
            stencilBlocks.Clear();
            newMeshes.stencilTiles.CopyToMesh(stencilBlocks);
        }
        if (newMeshes.topStencilTiles != null)
        {
            if (topStencilBlocks == null)
            {
                topStencilBlocks = new Mesh();
                topStencilBlocks.name = string.Format("block_stencil_top_{0}", suffix);
            }
            topStencilBlocks.Clear();
            newMeshes.topStencilTiles.CopyToMesh(topStencilBlocks);
        }
        if (newMeshes.transparentTiles != null)
        {
            if (transparentBlocks == null)
            {
                transparentBlocks = new Mesh();
                transparentBlocks.name = string.Format("block_transparent_{0}", suffix);
            }
            transparentBlocks.Clear();
            newMeshes.transparentTiles.CopyToMesh(transparentBlocks);
        }
        if (newMeshes.topTransparentTiles != null)
        {
            if (topTransparentBlocks == null)
            {
                topTransparentBlocks = new Mesh();
                topTransparentBlocks.name = string.Format("block_transparent_top_{0}", suffix);
            }
            topTransparentBlocks.Clear();
            newMeshes.topTransparentTiles.CopyToMesh(topTransparentBlocks);
        }
        if (newMeshes.terrainMesh != null)
        {
            if (voxelBlocks.mesh == null)
            {
                voxelBlocks.mesh = new Mesh();
                voxelBlocks.mesh.name = string.Format("block_voxel_{0}", suffix);
            }
            voxelBlocks.mesh.Clear();
            newMeshes.terrainMesh.CopyToMesh(voxelBlocks.mesh);
            voxelBlocks.mesh.RecalculateNormals();
            voxelBlocks.mesh.RecalculateTangents();
        }
        if(newMeshes.topTerrainMesh != null)
        {
            if(topVoxelBlocks.mesh == null)
            {
                topVoxelBlocks.mesh = new Mesh();
                topVoxelBlocks.mesh.name = string.Format("block_voxel_top_{0}", suffix);
            }
            topVoxelBlocks.mesh.Clear();
            newMeshes.topTerrainMesh.CopyToMesh(topVoxelBlocks.mesh);
            topVoxelBlocks.mesh.RecalculateNormals();
            topVoxelBlocks.mesh.RecalculateTangents();
        }
        if (newMeshes.water != null)
        {
            if (liquidBlocks == null || liquidBlocks.Length == 0)
                liquidBlocks = new Mesh[2];
            if (liquidBlocks[MapDataStore.WATER_INDEX] == null)
            {
                liquidBlocks[MapDataStore.WATER_INDEX] = new Mesh();
                liquidBlocks[MapDataStore.WATER_INDEX].name = string.Format("liquid_water_{0}", suffix);
            }
            liquidBlocks[MapDataStore.WATER_INDEX].Clear();
            newMeshes.water.CopyToMesh(liquidBlocks[MapDataStore.WATER_INDEX]);
        }
        if (newMeshes.magma != null)
        {
            if (liquidBlocks == null)
                liquidBlocks = new Mesh[2];
            if (liquidBlocks[MapDataStore.MAGMA_INDEX] == null)
            {
                liquidBlocks[MapDataStore.MAGMA_INDEX] = new Mesh();
                liquidBlocks[MapDataStore.MAGMA_INDEX].name = string.Format("liquid_magma_{0}", suffix);
            }
            liquidBlocks[MapDataStore.MAGMA_INDEX].Clear();
            newMeshes.magma.CopyToMesh(liquidBlocks[MapDataStore.MAGMA_INDEX]);
        }
    }

    internal void Clear()
    {
        ClearMesh(blocks);
        ClearMesh(stencilBlocks);
        ClearMesh(transparentBlocks);
        ClearMesh(topBlocks);
        ClearMesh(topStencilBlocks);
        ClearMesh(topTransparentBlocks);
        foreach (var item in liquidBlocks)
        {
            ClearMesh(item);
        }
        ClearMesh(voxelBlocks.mesh);
        ClearMesh(topVoxelBlocks.mesh);
        ClearMesh(grassBlocks);
        if (collisionBlocks != null)
        {
            UnityEngine.Object.Destroy(collisionBlocks);
            collisionBlocks = null;
        }
    }

    void ClearMesh(Mesh mesh)
    {
        if (mesh != null)
            mesh.Clear();
    }
}
