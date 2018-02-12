using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSubPart : MonoBehaviour {
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private RemoteFortressReader.ItemImprovement improvement;

    public int partIndex = 0;

    internal void UpdateImprovement(RemoteFortressReader.ItemImprovement improvement)
    {
        this.improvement = improvement;

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (originalMaterial == null)
            originalMaterial = meshRenderer.sharedMaterial;

        Color partColor = ContentLoader.GetColor(improvement.material);
        float textureIndex = ContentLoader.GetPatternIndex(improvement.material);

        meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, partColor.a);

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", partColor);
        prop.SetFloat("_MatIndex", textureIndex);
        meshRenderer.SetPropertyBlock(prop);
    }

}
