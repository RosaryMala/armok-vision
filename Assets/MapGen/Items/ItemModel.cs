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

        foreach (var imp in GetComponentsInChildren<ItemImprovement>())
        {
            if(imp.index >= itemInput.improvements.Count)
            {
                imp.gameObject.SetActive(false);
                continue;
            }
            imp.UpdateImprovement(itemInput.improvements[imp.index]);
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
