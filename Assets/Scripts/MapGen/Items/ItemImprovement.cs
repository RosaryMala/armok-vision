using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class ItemImprovement : MonoBehaviour
{
    public int index;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Material originalMaterial;
    private GameObject actualModel = null;

    public bool isImage;

    public ArtImage image;

    internal void UpdateImprovement(RemoteFortressReader.ItemImprovement improvement)
    {
        Color matColor = ContentLoader.GetColor(improvement.material);
        float textureIndex = ContentLoader.GetPatternIndex(improvement.material);

        image = improvement.image;

        if (actualModel != null)
        {
            Destroy(actualModel);
            actualModel = null;
        }


        GameObject prefab = null;

        switch (improvement.type)
        {
            case ImprovementType.ART_IMAGE:
                prefab = DecorationManager.Instance.Image;
                break;
            case ImprovementType.BANDS:
            case ImprovementType.COVERED:
                prefab = DecorationManager.Instance.GetShape(improvement.shape);
                break;
            case ImprovementType.RINGS_HANGING:
                prefab = DecorationManager.Instance.Ring;
                break;
            case ImprovementType.SPIKES:
                prefab = DecorationManager.Instance.Spike;
                break;
            default:
                break;
        }

        if(prefab == null)
        {
            gameObject.SetActive(false);
            return;
        }

        actualModel = Instantiate(prefab, transform, false);

        meshFilter = actualModel.GetComponentInChildren<MeshFilter>();
        meshRenderer = actualModel.GetComponentInChildren<MeshRenderer>();
        if (improvement.type == ImprovementType.ART_IMAGE)
        {
            meshFilter.sharedMesh = ImageManager.Instance.CreateMesh(improvement.image, ImageManager.Direction.Front);
        }
        else
        {
            originalMaterial = meshRenderer.sharedMaterial;
            meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, matColor.a);
        }

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", matColor);
        prop.SetFloat("_MatIndex", textureIndex);
        meshRenderer.SetPropertyBlock(prop);
        gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, (isImage ? 0.05f : 0.025f) * transform.lossyScale.x);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 0.025f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 0.025f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 0.025f);
    }
}
