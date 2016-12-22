using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExtension;

public class BlockMeshSet
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
    public Mesh voxelBlocks;
    /// <summary>
    /// top face of procedurally generated terrain blocks.
    /// </summary>
    public Mesh topVoxelBlocks;
    /// <summary>
    /// Procedurally generated grass.
    /// </summary>
    public Mesh grassBlocks;
    /// <summary>
    /// Procedural grass blocks.
    /// </summary>
    public MeshCollider collisionBlocks;

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
            if (voxelBlocks == null)
            {
                voxelBlocks = new Mesh();
                voxelBlocks.name = string.Format("block_voxel_{0}", suffix);
            }
            voxelBlocks.Clear();
            newMeshes.terrainMesh.CopyToMesh(voxelBlocks);
            voxelBlocks.RecalculateNormals();
            voxelBlocks.RecalculateTangents();
        }
        if(newMeshes.topTerrainMesh != null)
        {
            if(topVoxelBlocks == null)
            {
                topVoxelBlocks = new Mesh();
                topVoxelBlocks.name = string.Format("block_voxel_top_{0}", suffix);
            }
            topVoxelBlocks.Clear();
            newMeshes.topTerrainMesh.CopyToMesh(topVoxelBlocks);
            topVoxelBlocks.RecalculateNormals();
            topVoxelBlocks.RecalculateTangents();
        }
        if (newMeshes.water != null)
        {
            if (liquidBlocks == null)
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
        ClearMesh(voxelBlocks);
        ClearMesh(topVoxelBlocks);
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

    internal bool Render(bool phantom, Matrix4x4 LocalTransform, bool top,
        Material basicTerrainMaterial,
        Material stencilTerrainMaterial,
        Material transparentTerrainMaterial, 
        Material voxelTerrainMaterial, 
        Material waterMaterial,
        Material magmaMaterial,
        MaterialPropertyBlock properties
        )
    {
        UnityEngine.Profiling.Profiler.BeginSample("DrawSingleBlock");
        bool drewBlock = false;
        if (voxelBlocks != null && voxelBlocks.vertexCount > 0)
        {
            Graphics.DrawMesh(voxelBlocks, LocalTransform, voxelTerrainMaterial, 0, null, 0, properties, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }
        if (topVoxelBlocks != null && topVoxelBlocks.vertexCount > 0 && top)
        {
            Graphics.DrawMesh(topVoxelBlocks, LocalTransform, voxelTerrainMaterial, 0, null, 0, properties, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }

        if (blocks != null && blocks.vertexCount > 0)
        {
            Graphics.DrawMesh(blocks, LocalTransform, basicTerrainMaterial, 0, null, 0, properties, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }
        if (topBlocks != null && topBlocks.vertexCount > 0 && top)
        {
            Graphics.DrawMesh(topBlocks, LocalTransform, basicTerrainMaterial, 0, null, 0, properties, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }

        if (stencilBlocks != null && stencilBlocks.vertexCount > 0)
        {
            Graphics.DrawMesh(stencilBlocks, LocalTransform, stencilTerrainMaterial, 0, null, 0, properties, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }
        if (topStencilBlocks != null && topStencilBlocks.vertexCount > 0 && top)
        {
            Graphics.DrawMesh(topStencilBlocks, LocalTransform, stencilTerrainMaterial, 0, null, 0, properties, phantom ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On);
            drewBlock = true;
        }

        if (transparentBlocks != null && transparentBlocks.vertexCount > 0 && !phantom)
        {
            Graphics.DrawMesh(transparentBlocks, LocalTransform, transparentTerrainMaterial, 0, null, 0, properties);
            drewBlock = true;
        }
        if (topTransparentBlocks != null && topTransparentBlocks.vertexCount > 0 && !phantom && top)
        {
            Graphics.DrawMesh(topTransparentBlocks, LocalTransform, transparentTerrainMaterial, 0, null, 0, properties);
            drewBlock = true;
        }
        if (liquidBlocks != null)
        {
            if (liquidBlocks[MapDataStore.WATER_INDEX] != null && liquidBlocks[MapDataStore.WATER_INDEX].vertexCount > 0 && !phantom)
            {
                Graphics.DrawMesh(liquidBlocks[MapDataStore.WATER_INDEX], LocalTransform, waterMaterial, 4);
                drewBlock = true;
            }

            if (liquidBlocks[MapDataStore.MAGMA_INDEX] != null && liquidBlocks[MapDataStore.MAGMA_INDEX].vertexCount > 0 && !phantom)
            {
                Graphics.DrawMesh(liquidBlocks[MapDataStore.MAGMA_INDEX], LocalTransform, magmaMaterial, 4);
                drewBlock = true;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
        return drewBlock;

    }
}
