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
    public MeshFilter blocks;
    MeshRenderer blocksRenderer;
    /// <summary>
    /// Cout-out terrain built from prefab meshes
    /// Includes foliage, and things like floor grates.
    /// </summary>
    public MeshFilter stencilBlocks;
    MeshRenderer stencilRenderer;
    /// <summary>
    /// Semitransparent terrain built from prefab meshes.
    /// </summary>
    public MeshFilter transparentBlocks;
    MeshRenderer transparentRenderer;
    /// <summary>
    /// Water and magma meshes.
    /// W dimension is liquid type.
    /// </summary>
    public MeshFilter waterBlocks;
    MeshRenderer waterRenderer;

    public MeshFilter lavaBlocks;
    MeshRenderer lavaRenderer;

    /// <summary>
    /// Procedurally generated terrain blocks.
    /// </summary>
    public MeshFilter voxelBlocks;
    MeshRenderer voxelRenderer;
    /// <summary>
    /// Procedural grass blocks.
    /// </summary>
    public MeshCollider collisionBlocks;

    private void Awake()
    {
        blocksRenderer = blocks.GetComponent<MeshRenderer>();
        stencilRenderer = stencilBlocks.GetComponent<MeshRenderer>();
        transparentRenderer = transparentBlocks.GetComponent<MeshRenderer>();
        waterRenderer = waterBlocks.GetComponent<MeshRenderer>();
        lavaRenderer = lavaBlocks.GetComponent<MeshRenderer>();
        voxelRenderer = voxelBlocks.GetComponent<MeshRenderer>();
    }

    public enum Visibility
    {
        None,
        Shadows,
        All
    }

    public void UpdateVisibility(Visibility vis)
    {
        switch (vis)
        {
            case Visibility.None:
                gameObject.SetActive(false);
                break;
            case Visibility.Shadows:
                gameObject.SetActive(true);
                blocksRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                stencilRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                transparentRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                waterRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                lavaRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                voxelRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                break;
            case Visibility.All:
                gameObject.SetActive(true);
                blocksRenderer.shadowCastingMode = ShadowCastingMode.On;
                stencilRenderer.shadowCastingMode = ShadowCastingMode.On;
                transparentRenderer.shadowCastingMode = ShadowCastingMode.On;
                waterRenderer.shadowCastingMode = ShadowCastingMode.On;
                lavaRenderer.shadowCastingMode = ShadowCastingMode.On;
                voxelRenderer.shadowCastingMode = ShadowCastingMode.On;
                break;
            default:
                break;
        }
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
        blocksRenderer.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Opaque, matFlags);
        stencilRenderer.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Stencil, matFlags);
        transparentRenderer.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Transparent, matFlags);
        voxelRenderer.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.SplatMap, matFlags);
    }

    public void SetTerrainMap(Texture2D terrainSplatLayer, Texture2D terrainTintLayer)
    {
        matProperties.SetTexture(terrainSplatID, terrainSplatLayer);
        matProperties.SetTexture(terrainTintID, terrainTintLayer);

        blocksRenderer.SetPropertyBlock(matProperties);
        stencilRenderer.SetPropertyBlock(matProperties);
        transparentRenderer.SetPropertyBlock(matProperties);
        voxelRenderer.SetPropertyBlock(matProperties);
    }

    public void SetGrassMap(Texture2D grassSplatLayer, Texture2D grassTintLayer)
    {
        matProperties.SetTexture(grassSplatID, grassSplatLayer);
        matProperties.SetTexture(grassTintID, grassTintLayer);

        blocksRenderer.SetPropertyBlock(matProperties);
        stencilRenderer.SetPropertyBlock(matProperties);
        transparentRenderer.SetPropertyBlock(matProperties);
        voxelRenderer.SetPropertyBlock(matProperties);

        matFlags |= MaterialManager.MaterialFlags.Grass;

        SetupMaterials();
    }

    public void SetSpatterMap(Texture2D splatterLayer)
    {
        matProperties.SetTexture(spatterID, splatterLayer);

        blocksRenderer.SetPropertyBlock(matProperties);
        stencilRenderer.SetPropertyBlock(matProperties);
        transparentRenderer.SetPropertyBlock(matProperties);
        voxelRenderer.SetPropertyBlock(matProperties);

        matFlags |= MaterialManager.MaterialFlags.Contaminants;

        SetupMaterials();
    }

    internal void LoadMeshes(BlockMesher.Result newMeshes, string suffix)
    {
        if (newMeshes.tiles != null)
        {
            if (blocks == null)
            {
                blocks.mesh = new Mesh();
                blocks.mesh.name = string.Format("block_solid_{0}", suffix);
            }
            blocks.mesh.Clear();
            newMeshes.tiles.CopyToMesh(blocks.mesh);
        }
        if (newMeshes.stencilTiles != null)
        {
            if (stencilBlocks.mesh == null)
            {
                stencilBlocks.mesh = new Mesh();
                stencilBlocks.mesh.name = string.Format("block_stencil_{0}", suffix);
            }
            stencilBlocks.mesh.Clear();
            newMeshes.stencilTiles.CopyToMesh(stencilBlocks.mesh);
        }
        if (newMeshes.transparentTiles != null)
        {
            if (transparentBlocks.mesh == null)
            {
                transparentBlocks.mesh = new Mesh();
                transparentBlocks.mesh.name = string.Format("block_transparent_{0}", suffix);
            }
            transparentBlocks.mesh.Clear();
            newMeshes.transparentTiles.CopyToMesh(transparentBlocks.mesh);
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
        if (newMeshes.water != null)
        {
            if (waterBlocks.mesh == null)
            {
                waterBlocks.mesh = new Mesh();
                waterBlocks.mesh.name = string.Format("liquid_water_{0}", suffix);
            }
            waterBlocks.mesh.Clear();
            newMeshes.water.CopyToMesh(waterBlocks.mesh);
        }
        if (newMeshes.magma != null)
        {
            if (lavaBlocks.mesh == null)
            {
                lavaBlocks.mesh = new Mesh();
                lavaBlocks.mesh.name = string.Format("liquid_magma_{0}", suffix);
            }
            lavaBlocks.mesh.Clear();
            newMeshes.magma.CopyToMesh(lavaBlocks.mesh);
        }
    }

    internal void Clear()
    {
        ClearMesh(blocks.mesh);
        ClearMesh(stencilBlocks.mesh);
        ClearMesh(transparentBlocks.mesh);
        ClearMesh(voxelBlocks.mesh);
        ClearMesh(waterBlocks.mesh);
        ClearMesh(lavaBlocks.mesh);
        if (collisionBlocks != null)
        {
            Destroy(collisionBlocks);
            collisionBlocks = null;
        }
    }

    void ClearMesh(Mesh mesh)
    {
        if (mesh != null)
            mesh.Clear();
    }
}
