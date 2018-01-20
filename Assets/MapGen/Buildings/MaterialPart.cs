using RemoteFortressReader;
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
            if (string.IsNullOrEmpty(item) || ItemTokenList.ItemLookup == null)
            {
                if (index < 0)
                {
                    partColor = ContentLoader.GetColor(buildingInput.material);
                    textureIndex = ContentLoader.GetPatternIndex(buildingInput.material);
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
                    UpdateDecorations(buildingItem.item);
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
                        UpdateDecorations(buildingItem.item);
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
            meshRenderer.SetPropertyBlock(prop);
        }

        public void UpdateDecorations(Item item)
        {
            List<RemoteFortressReader.ItemImprovement> images = new List<RemoteFortressReader.ItemImprovement>();
            List<RemoteFortressReader.ItemImprovement> ringSpikeBands = new List<RemoteFortressReader.ItemImprovement>();
            List<RemoteFortressReader.ItemImprovement> covereds = new List<RemoteFortressReader.ItemImprovement>();

            foreach (var improvement in item.improvements)
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
                        //Handled already, it's dye.
                        break;
                    case ImprovementType.ITEMSPECIFIC:
                    case ImprovementType.SEWN_IMAGE:
                    case ImprovementType.PAGES:
                    case ImprovementType.ILLUSTRATION:
                    case ImprovementType.INSTRUMENT_PIECE:
                    case ImprovementType.WRITING:
                    default:
                        Debug.LogWarning(string.Format("Unhandled improvement {0} on {1}", improvement.type, gameObject.name));
                        break;
                }
            }

            var imps = GetComponentsInChildren<ItemImprovement>();
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
                else if (covereds.Count > 0)
                {
                    UnityEngine.Random.InitState(i);
                    imp.UpdateImprovement(covereds[UnityEngine.Random.Range(0, covereds.Count - 1)]);
                }
                else
                    imp.gameObject.SetActive(false);
            }

        }
    }
}