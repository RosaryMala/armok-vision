using Building;
using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel : MonoBehaviour, IClickable
{
    public Item originalItem;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        foreach (var item in GetComponentsInChildren<Collider>())
        {
            if (item.GetComponent<BuildingSelect>() == null)
            {
                item.gameObject.AddComponent<BuildingSelect>().root = this;
            }
        }
    }

    public void UpdateMaterial(Item itemInput)
    {
        originalItem = itemInput;

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (originalMaterial == null)
            originalMaterial = meshRenderer.sharedMaterial;

        Color partColor = ContentLoader.GetColor(itemInput);
        float textureIndex = ContentLoader.GetPatternIndex(itemInput.material);

        meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, partColor.a);

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", partColor);
        prop.SetFloat("_MatIndex", textureIndex);
        meshRenderer.SetPropertyBlock(prop);


        UpdateImprovements(gameObject, itemInput);
    }

    public static void UpdateImprovements(GameObject GO, Item itemInput)
    {
    List<RemoteFortressReader.ItemImprovement> images = new List<RemoteFortressReader.ItemImprovement>();
    List<RemoteFortressReader.ItemImprovement> ringSpikeBands = new List<RemoteFortressReader.ItemImprovement>();
    List<RemoteFortressReader.ItemImprovement> covereds = new List<RemoteFortressReader.ItemImprovement>();

    foreach (var improvement in itemInput.improvements)
    {
        switch (improvement.type)
        {
            case ImprovementType.ART_IMAGE:
                images.Add(improvement);
                break;
            case ImprovementType.COVERED:
                covereds.Add(improvement);
                break;
            case ImprovementType.RINGS_HANGING:
            case ImprovementType.BANDS:
            case ImprovementType.SPIKES:
                ringSpikeBands.Add(improvement);
                break;
            case ImprovementType.THREAD:
            case ImprovementType.CLOTH:
                //Handled already, in various ways.
                break;
            case ImprovementType.ITEMSPECIFIC:
            case ImprovementType.SEWN_IMAGE:
            case ImprovementType.PAGES:
            case ImprovementType.ILLUSTRATION:
            case ImprovementType.INSTRUMENT_PIECE:
            case ImprovementType.WRITING:
            default:
                Debug.LogWarning(string.Format("Unhandled improvement {0} on {1}", improvement.type, GO.name));
                break;
        }
    }

    var imps = GO.GetComponentsInChildren<ItemImprovement>();
        for (int i = 0; i < imps.Length; i++)
        {
            var imp = imps[i];
            if (imp.isImage && imp.index < images.Count)
            {
                imp.UpdateImprovement(images[imp.index]);
            }
            else if (imp.isRings && imp.index < ringSpikeBands.Count && ringSpikeBands[imp.index].type == ImprovementType.RINGS_HANGING)
            {
                imp.UpdateImprovement(ringSpikeBands[imp.index]);
            }
            else if (imp.isSpikes && imp.index < ringSpikeBands.Count && ringSpikeBands[imp.index].type == ImprovementType.SPIKES)
            {
                imp.UpdateImprovement(ringSpikeBands[imp.index]);
            }
            else if (imp.isBands && imp.index < ringSpikeBands.Count && ringSpikeBands[imp.index].type == ImprovementType.BANDS)
            {
                imp.UpdateImprovement(ringSpikeBands[imp.index]);
            }
            else if (imp.isCovered && covereds.Count > 0)
            {
                Random.InitState(i);
                imp.UpdateImprovement(covereds[Random.Range(0, covereds.Count)]);
            }
            else
                imp.gameObject.SetActive(false);
        }
    }

    public void HandleClick()
    {
        PrintItemInfo(originalItem);
    }

    public static void PrintItemInfo(Item item)
    {
        string mat = ((MatPairStruct)(item.material)).ToString();
        if (GameMap.materials.ContainsKey(item.material))
            mat = GameMap.materials[item.material].id;
        if (item.stack_size > 1)
            Debug.Log(string.Format("{0} {1} [{2}]", mat, GameMap.items[item.type].id, item.stack_size));
        else
            Debug.Log(string.Format("{0} {1}", mat, GameMap.items[item.type].id));

        foreach (var imp in item.improvements)
        {
            mat = ((MatPairStruct)(imp.material)).ToString();
            if (GameMap.materials.ContainsKey(imp.material))
                mat = GameMap.materials[imp.material].id;
            Debug.Log(string.Format("    {0} {1}", mat, imp.type));
        }
    }
}
