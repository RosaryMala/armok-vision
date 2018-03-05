using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Flags]
    public enum MaterialFlags
    {
        None = 0,
        Contaminants = 1,
        Grass = 2
    }
    public enum MaterialType
    {
        Opaque,
        Stencil,
        Transparent,
        SplatMap
    }

    [SerializeField]
    Material baseStandardMaterial;

    [SerializeField]
    Material baseStandardMaterialCutout;

    [SerializeField]
    Material baseStandardMaterialTransparent;

    [SerializeField]
    Material baseSplatMaterial;

    public static MaterialManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeAllTextures();
    }

    Dictionary<MaterialType, Dictionary<MaterialFlags, Material>> _cachedMaterials = new Dictionary<MaterialType, Dictionary<MaterialFlags, Material>>();
    private void InitializeAllTextures()
    {
        InitializeTextureCategory(MaterialType.Opaque);
        InitializeTextureCategory(MaterialType.SplatMap);
        InitializeTextureCategory(MaterialType.Stencil);
        InitializeTextureCategory(MaterialType.Transparent);
    }

    private void InitializeTextureCategory(MaterialType type)
    {
        InitizalizeTexture(type, MaterialFlags.None);
        InitizalizeTexture(type, MaterialFlags.Contaminants);
        InitizalizeTexture(type, MaterialFlags.Grass);
        InitizalizeTexture(type, MaterialFlags.Contaminants | MaterialFlags.Grass);
    }

    private void InitizalizeTexture(MaterialType type, MaterialFlags flags)
    {
        if (!_cachedMaterials.ContainsKey(type))
        {
            _cachedMaterials[type] = new Dictionary<MaterialFlags, Material>();
        }
        if (!_cachedMaterials[type].ContainsKey(flags))
        {
            switch (type)
            {
                case MaterialType.Opaque:
                    _cachedMaterials[type][flags] = new Material(baseStandardMaterial);
                    break;
                case MaterialType.Stencil:
                    _cachedMaterials[type][flags] = new Material(baseStandardMaterialCutout);
                    break;
                case MaterialType.Transparent:
                    _cachedMaterials[type][flags] = new Material(baseStandardMaterialTransparent);
                    break;
                case MaterialType.SplatMap:
                    _cachedMaterials[type][flags] = new Material(baseSplatMaterial);
                    break;
                default:
                    break;
            }
            if ((flags & MaterialFlags.Contaminants) == MaterialFlags.Contaminants)
                _cachedMaterials[type][flags].EnableKeyword("CONTAMINANTS");
            else
                _cachedMaterials[type][flags].DisableKeyword("CONTAMINANTS");

            if ((flags & MaterialFlags.Grass) == MaterialFlags.Grass)
                _cachedMaterials[type][flags].EnableKeyword("GRASS");
            else
                _cachedMaterials[type][flags].DisableKeyword("GRASS");

            foreach (var item in vectorProperties)
            {
                _cachedMaterials[type][flags].SetVector(item.Key, item.Value);
            }
            foreach (var item in textureProperties)
            {
                _cachedMaterials[type][flags].SetTexture(item.Key, item.Value);
            }
        }
    }

    public Material GetMaterial(MaterialType type, MaterialFlags flags)
    {
        return _cachedMaterials[type][flags];
    }

    Dictionary<string, Vector4> vectorProperties = new Dictionary<string, Vector4>();
    public void SetVector(string property, Vector4 vector)
    {
        vectorProperties[property] = vector;
        foreach (var item in _cachedMaterials)
        {
            foreach (var mat in item.Value)
            {
                mat.Value.SetVector(property, vector);
            }
        }
    }

    Dictionary<string, Texture> textureProperties = new Dictionary<string, Texture>();
    public void SetTexture(string property, Texture texture)
    {
        textureProperties[property] = texture;
        foreach (var item in _cachedMaterials)
        {
            foreach (var mat in item.Value)
            {
                mat.Value.SetTexture(property, texture);
            }
        }
    }
}
