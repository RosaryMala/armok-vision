using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaterialSetter : MonoBehaviour
{
    public Color materialColor;
    public Texture2DArray materialPattern;
    public int index;

    public void SetMaterials()
    {
        foreach (var part in FindObjectsOfType<Building.MaterialPart>())
        {
            var renderer = part.GetComponent<MeshRenderer>();
            renderer.sharedMaterial.SetTexture("_MatTex", materialPattern);
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_MatColor", materialColor.gamma);
            prop.SetFloat("_MatIndex", index);
            renderer.SetPropertyBlock(prop);
        }
    }

    private void OnValidate()
    {
        SetMaterials();
    }
}
