using DF.Enums;
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
        float shapeIndex = ContentLoader.GetShapeIndex(improvement.material);
        image = improvement.image;

        if (actualModel != null)
        {
            Destroy(actualModel);
            actualModel = null;
        }


        GameObject prefab = null;

        switch ((ImprovementType)improvement.type)
        {
            case ImprovementType.ArtImage:
                prefab = DecorationManager.Instance.Image;
                break;
            case ImprovementType.Bands:
            case ImprovementType.Covered:
                prefab = DecorationManager.Instance.GetShape(improvement.shape);
                break;
            case ImprovementType.RingsHanging:
                prefab = DecorationManager.Instance.Ring;
                break;
            case ImprovementType.Spikes:
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
        if ((ImprovementType)improvement.type == ImprovementType.ArtImage)
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
        prop.SetFloat("_ShapeIndex", shapeIndex);
        meshRenderer.SetPropertyBlock(prop);
        gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.HSVToRGB(index / 20f, 1, 1);
        Gizmos.DrawSphere(transform.position, (isImage ? 0.05f : 0.025f) * transform.lossyScale.x);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 0.025f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 0.025f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 0.025f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.HSVToRGB(index / 20f, 1, 1);
        Gizmos.DrawSphere(transform.position, (isImage ? 0.05f : 0.025f) * transform.lossyScale.x * 1.1f);
    }
}
