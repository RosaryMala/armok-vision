using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class ItemImprovement : MonoBehaviour
{
    public int index;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;

    internal void UpdateImprovement(RemoteFortressReader.ItemImprovement improvement)
    {
        Color matColor = ContentLoader.GetColor(improvement.material);
        float textureIndex = ContentLoader.GetPatternIndex(improvement.material);

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (originalMaterial == null)
            originalMaterial = meshRenderer.sharedMaterial;

        meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, matColor.a);

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", matColor);
        prop.SetFloat("_MatIndex", textureIndex);
        meshRenderer.SetPropertyBlock(prop);
    }
}
