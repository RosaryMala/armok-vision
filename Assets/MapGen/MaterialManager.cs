using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [Flags]
    public enum MaterialFlags
    {
        None = 0,
        Contaminants = 1
    }
    public enum TransparencyType
    {
        Opaque,
        Stencil,
        Transparent
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

    Material _voxelMaterial = null;
    Material _voxelMaterialContaminated = null;
    public Material GetSplatMaterial(MaterialFlags flags)
    {
        switch (flags)
        {
            case MaterialFlags.Contaminants:
                if (_voxelMaterialContaminated == null)
                {
                    _voxelMaterialContaminated = new Material(baseSplatMaterial);
                    _voxelMaterialContaminated.EnableKeyword("CONTAMINANTS");
                    foreach (var item in vectorProperties)
                    {
                        _voxelMaterialContaminated.SetVector(item.Key, item.Value);
                    }
                    foreach (var item in textureProperties)
                    {
                        _voxelMaterialContaminated.SetTexture(item.Key, item.Value);
                    }
                }
                return _voxelMaterialContaminated;
            default:
                if (_voxelMaterial == null)
                {
                    _voxelMaterial = new Material(baseSplatMaterial);
                    _voxelMaterial.DisableKeyword("CONTAMINANTS");
                    foreach (var item in vectorProperties)
                    {
                        _voxelMaterial.SetVector(item.Key, item.Value);
                    }
                    foreach (var item in textureProperties)
                    {
                        _voxelMaterial.SetTexture(item.Key, item.Value);
                    }
                }
                return _voxelMaterial;
        }
    }

    Dictionary<string, Vector4> vectorProperties = new Dictionary<string, Vector4>();
    public void SetVector(string property, Vector4 vector)
    {
        vectorProperties[property] = vector;
    }

    Dictionary<string, Texture> textureProperties = new Dictionary<string, Texture>();
    public void SetTexture(string property, Texture texture)
    {
        textureProperties[property] = texture;
    }
}
