using MaterialStore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMaterialSetter : MonoBehaviour
{
    public MaterialTag currentMaterialTag;

    public void SetMaterials(MaterialTextureSet set)
    {
        var materialPattern = Resources.Load<Texture2DArray>("patternTextures");
        Shader.SetGlobalTexture("_MatTexArray", materialPattern);
        foreach (var part in FindObjectsOfType<Building.MaterialPart>())
        {
            var renderer = part.GetComponent<MeshRenderer>();
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_MatColor", set.color);
            prop.SetFloat("_MatIndex", set.patternIndex);
            renderer.SetPropertyBlock(prop);
        }
        foreach (var part in FindObjectsOfType<ItemModel>())
        {
            var renderers = part.GetComponentsInChildren<MeshRenderer>();
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_MatColor", set.color);
            prop.SetFloat("_MatIndex", set.patternIndex);
            foreach (var renderer in renderers)
            {
                renderer.SetPropertyBlock(prop);
            }
        }
    }
}
