using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExtension;

public class BlockMeshSet : MonoBehaviour
{
    enum MeshType
    {
        Tiles,
        TopTiles,
        StencilTiles,
        TopStencilTiles,
        TransparentTiles,
        TopTransparentTiles,
        Water,
        Magma,
        Terrain,
        TopTerrain
    }

    bool IsTop(MeshType type)
    {
        switch (type)
        {
            case MeshType.TopTiles:
            case MeshType.TopStencilTiles:
            case MeshType.TopTransparentTiles:
            case MeshType.TopTerrain:
                return true;
            default:
                return false;
        }
    }

    Dictionary<MeshType, MeshFilter> meshFilters = new Dictionary<MeshType, MeshFilter>();
    Dictionary<MeshType, MeshRenderer> meshRenderers = new Dictionary<MeshType, MeshRenderer>();

    public MeshCollider collisionBlocks;

    public enum Visibility
    {
        None,
        Shadows,
        Walls,
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
                foreach (var renderer in meshRenderers)
                {
                    renderer.Value.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
                collisionBlocks.gameObject.layer = 2;
                break;
            case Visibility.Walls:
                gameObject.SetActive(true);
                foreach (var renderer in meshRenderers)
                {
                    if (IsTop(renderer.Key))
                        renderer.Value.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    else renderer.Value.shadowCastingMode = ShadowCastingMode.On;
                }
                collisionBlocks.gameObject.layer = 0;
                break;
            case Visibility.All:
                gameObject.SetActive(true);
                foreach (var renderer in meshRenderers)
                {
                    renderer.Value.shadowCastingMode = ShadowCastingMode.On;
                }
                collisionBlocks.gameObject.layer = 0;
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
        foreach (var item in meshRenderers)
        {
            switch (item.Key)
            {
                case MeshType.Tiles:
                case MeshType.TopTiles:
                    item.Value.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Opaque, matFlags);
                    break;
                case MeshType.StencilTiles:
                case MeshType.TopStencilTiles:
                    item.Value.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Stencil, matFlags);
                    break;
                case MeshType.TransparentTiles:
                case MeshType.TopTransparentTiles:
                    item.Value.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.Transparent, matFlags);
                    break;
                case MeshType.Water:
                    item.Value.sharedMaterial = GameMap.Instance.waterMaterial;
                    break;
                case MeshType.Magma:
                    item.Value.sharedMaterial = GameMap.Instance.magmaMaterial;
                    break;
                case MeshType.Terrain:
                case MeshType.TopTerrain:
                    item.Value.sharedMaterial = MaterialManager.Instance.GetMaterial(MaterialManager.MaterialType.SplatMap, matFlags);
                    break;
                default:
                    break;
            }
        }
    }

    void UpdatePropertyBlock()
    {
        foreach (var item in meshRenderers)
        {
            item.Value.SetPropertyBlock(matProperties);
        }
    }

    public void SetTerrainMap(Texture2D terrainSplatLayer, Texture2D terrainTintLayer)
    {
        matProperties.SetTexture(terrainSplatID, terrainSplatLayer);
        matProperties.SetTexture(terrainTintID, terrainTintLayer);

        UpdatePropertyBlock();
    }

    public void SetGrassMap(Texture2D grassSplatLayer, Texture2D grassTintLayer)
    {
        matProperties.SetTexture(grassSplatID, grassSplatLayer);
        matProperties.SetTexture(grassTintID, grassTintLayer);

        matFlags |= MaterialManager.MaterialFlags.Grass;

        UpdatePropertyBlock();


        SetupMaterials();
    }

    public void SetSpatterMap(Texture2D splatterLayer)
    {
        matProperties.SetTexture(spatterID, splatterLayer);

        matFlags |= MaterialManager.MaterialFlags.Contaminants;

        UpdatePropertyBlock();

        SetupMaterials();
    }



    internal void LoadMeshes(BlockMesher.Result newMeshes, string suffix)
    {
        bool madeNew = false;
        if (newMeshes.tiles != null)
        {
            madeNew |= CopyMesh(MeshType.Tiles, newMeshes.tiles, suffix);
        }
        if(newMeshes.topTiles != null)
        {
            madeNew |= CopyMesh(MeshType.TopTiles, newMeshes.topTiles, suffix);
        }
        if (newMeshes.stencilTiles != null)
        {
            madeNew |= CopyMesh(MeshType.StencilTiles, newMeshes.stencilTiles, suffix);
        }
        if (newMeshes.topStencilTiles != null)
        {
            madeNew |= CopyMesh(MeshType.TopStencilTiles, newMeshes.topStencilTiles, suffix);
        }
        if (newMeshes.transparentTiles != null)
        {
            madeNew |= CopyMesh(MeshType.TransparentTiles, newMeshes.transparentTiles, suffix);
        }
        if (newMeshes.topTransparentTiles != null)
        {
            madeNew |= CopyMesh(MeshType.TopTransparentTiles, newMeshes.topTransparentTiles, suffix);
        }
        if (newMeshes.terrainMesh != null)
        {
            madeNew |= CopyMesh(MeshType.Terrain, newMeshes.terrainMesh, suffix);
            meshFilters[MeshType.Terrain].mesh.RecalculateNormals();
            meshFilters[MeshType.Terrain].mesh.RecalculateTangents();
        }
        if (newMeshes.topTerrainMesh != null)
        {
            madeNew |= CopyMesh(MeshType.TopTerrain, newMeshes.topTerrainMesh, suffix);
            meshFilters[MeshType.TopTerrain].mesh.RecalculateNormals();
            meshFilters[MeshType.TopTerrain].mesh.RecalculateTangents();
        }
        if (newMeshes.water != null)
        {
            madeNew |= CopyMesh(MeshType.Water, newMeshes.water, suffix);
        }
        if (newMeshes.magma != null)
        {
            madeNew |= CopyMesh(MeshType.Magma, newMeshes.magma, suffix);
        }
        if (madeNew)
        {
            UpdatePropertyBlock();
            SetupMaterials();
        }
    }

    private bool CopyMesh(MeshType type, CPUMesh mesh, string suffix)
    {
        bool madeNew = false;
        if(!meshFilters.ContainsKey(type))
        {
            var newGameObject = new GameObject();
            newGameObject.transform.parent = transform;
            newGameObject.transform.LocalReset();
            newGameObject.name = type.ToString();
            meshFilters[type] = newGameObject.AddComponent<MeshFilter>();
            meshRenderers[type] = newGameObject.AddComponent<MeshRenderer>();
            meshFilters[type].mesh = new Mesh();
            meshFilters[type].mesh.name = type.ToString() + "_" + suffix;
            madeNew = true;
        }
        meshFilters[type].mesh.Clear();
        mesh.CopyToMesh(meshFilters[type].mesh);
        return madeNew;
    }

    internal void Clear()
    {
        foreach (var item in meshFilters)
        {
            ClearMesh(item.Value.mesh);
        }
        if (collisionBlocks != null)
        {
            collisionBlocks.sharedMesh = null;
        }
    }

    void ClearMesh(Mesh mesh)
    {
        if (mesh != null)
            mesh.Clear();
    }
}
