using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSubPart : MonoBehaviour {
    private MeshRenderer meshRenderer;
    private Material originalMaterial;

    public int partIndex = 0;

    internal void UpdateImprovement(RemoteFortressReader.ItemImprovement improvement)
    {

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (originalMaterial == null)
            originalMaterial = meshRenderer.sharedMaterial;

        Color partColor = ContentLoader.GetColor(improvement.material);
        float textureIndex = ContentLoader.GetPatternIndex(improvement.material);
        float shapeIndex = ContentLoader.GetShapeIndex(improvement.material);

        meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, partColor.a);

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", partColor);
        prop.SetFloat("_MatIndex", textureIndex);
        prop.SetFloat("_ShapeIndex", shapeIndex);
        meshRenderer.SetPropertyBlock(prop);
    }
}
