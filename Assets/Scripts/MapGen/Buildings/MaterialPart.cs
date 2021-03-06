﻿using RemoteFortressReader;
using UnityEngine;
using TokenLists;
using MaterialStore;
using System.Collections.Generic;

namespace Building
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class MaterialPart : MonoBehaviour, IBuildingPart
    {
        MeshRenderer meshRenderer;

        public string item;
        public int index = -1;
        public bool storedItem = false;
        [Tooltip("Used to disallow the last item in a building from being used, such as with traps.")]
        public int endOffset = 0;


        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public static string GetTreeName(GameObject go)
        {
            string name = go.name;
            for(Transform parent = go.transform.parent; parent != null; parent = parent.transform.parent)
            {
                name = parent.gameObject.name + "/" + name;
            }
            return name;
        }

        Material originalMaterial = null;

        public void UpdatePart(BuildingInstance buildingInput)
        {
            Color partColor = new Color32(128,128,128,128);
            float textureIndex = 0;
            float shapeIndex = 0;
            if (string.IsNullOrEmpty(item) || ItemTokenList.ItemLookup == null)
            {
                if (index < 0)
                {
                    partColor = ContentLoader.GetColor(buildingInput.material);
                    textureIndex = ContentLoader.GetPatternIndex(buildingInput.material);
                    shapeIndex = ContentLoader.GetShapeIndex(buildingInput.material);
                }
                else if (index >= buildingInput.items.Count - endOffset)
                {
                    gameObject.SetActive(false);
                    return;
                }
                else
                {
                    var buildingItem = buildingInput.items[index];
                    //skip items that are just stored in the building.
                    //though they should be later in the list anyway.
                    if ((buildingItem.mode == 0) != storedItem)
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                    partColor = ContentLoader.GetColor(buildingItem.item);
                    textureIndex = ContentLoader.GetPatternIndex(buildingItem.item.material);
                    shapeIndex = ContentLoader.GetShapeIndex(buildingItem.item.material);
                    ItemModel.UpdateImprovements(gameObject, buildingItem.item);
                }
            }
            else if (!ItemTokenList.ItemLookup.ContainsKey(item))
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                MatPairStruct itemCode = ItemTokenList.ItemLookup[item].mat_pair;
                bool set = false;
                foreach (var buildingItem in buildingInput.items)
                {
                    //skip items that are just stored in the building.
                    //though they should be later in the list anyway.
                    if (buildingItem.mode == 0 && !storedItem)
                        continue;
                    //if our setting is a generic item, like any weapon, then any subtype can match.
                    if ((itemCode.mat_index == -1 && itemCode.mat_type == buildingItem.item.type.mat_type)
                        || (buildingItem.item.type == itemCode))
                    {
                        partColor = ContentLoader.GetColor(buildingItem.item);
                        textureIndex = ContentLoader.GetPatternIndex(buildingItem.item.material);
                        shapeIndex = ContentLoader.GetShapeIndex(buildingItem.item.material);
                        ItemModel.UpdateImprovements(gameObject, buildingItem.item);
                        set = true;
                        break;
                    }
                }
                if (!set)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
            gameObject.SetActive(true);


            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();

            if (originalMaterial == null)
                originalMaterial = meshRenderer.sharedMaterial;

            meshRenderer.sharedMaterial = ContentLoader.getFinalMaterial(originalMaterial, partColor.a);

            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_MatColor", partColor);
            prop.SetFloat("_MatIndex", textureIndex);
            prop.SetFloat("_ShapeIndex", shapeIndex);
            meshRenderer.SetPropertyBlock(prop);
        }
    }
}