using RemoteFortressReader;
using UnityEngine;
using TokenLists;
using MaterialStore;
using System.Collections.Generic;
using System;

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

        private void UpdateMaterialVersions()
        {
            if (setMaterials)
                return;
            setMaterials = true;
            var originalMat = meshRenderer.sharedMaterial;
            originalMaterialID = originalMat.GetInstanceID();
            if (!opaqueMaterialVersions.ContainsKey(originalMaterialID))
            {
                if (originalMat.shader.name == "Standard")
                {
                    Debug.LogWarning(GetTreeName(gameObject) + " Has a standard shader!");
                    opaqueMaterialVersions[originalMaterialID] = originalMat;
                    transparentMaterialVersions[originalMaterialID] = originalMat;
                }
                else
                {
                    opaqueMaterialVersions[originalMaterialID] = originalMat;
                    transparentMaterialVersions[originalMaterialID] = new Material(originalMat);
                    transparentMaterialVersions[originalMaterialID].shader = Shader.Find("Building/Transparent");
                }
            }
        }

        private static string GetTreeName(GameObject go)
        {
            string name = go.name;
            for(Transform parent = go.transform.parent; parent != null; parent = parent.transform.parent)
            {
                name = parent.gameObject.name + "/" + name;
            }
            return name;
        }

        bool setMaterials = false;
        int originalMaterialID;
        static Dictionary<int, Material> opaqueMaterialVersions = new Dictionary<int, Material>();
        static Dictionary<int, Material> transparentMaterialVersions = new Dictionary<int, Material>();

        Material OriginalMaterial
        {
            get
            {
                if (!setMaterials)
                    UpdateMaterialVersions();
                return opaqueMaterialVersions[originalMaterialID];
            }
        }
        Material TransparentMaterial
        {
            get
            {
                if (!setMaterials)
                    UpdateMaterialVersions();
                return transparentMaterialVersions[originalMaterialID];
            }
        }

        public void UpdatePart(BuildingInstance buildingInput)
        {
            ColorDefinition dye = null;
            MatPairStruct mat = new MatPairStruct(-1,-1);
            if (string.IsNullOrEmpty(item) || ItemTokenList.ItemLookup == null)
            {
                if (index < 0)
                    mat = buildingInput.material;
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
                    mat = buildingItem.item.material;
                    dye = buildingItem.item.dye;
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
                foreach (var item in buildingInput.items)
                {
                    //skip items that are just stored in the building.
                    //though they should be later in the list anyway.
                    if (item.mode == 0 && !storedItem)
                        continue;
                    //if our setting is a generic item, like any weapon, then any subtype can match.
                    if ((itemCode.mat_index == -1 && itemCode.mat_type == item.item.type.mat_type)
                        || (item.item.type == itemCode))
                    {
                        mat = item.item.material;
                        dye = item.item.dye;
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
            Color partColor = Color.gray;
            float textureIndex = 0;
            MaterialTextureSet textureContent;
            if (ContentLoader.Instance.MaterialTextures.TryGetValue(mat, out textureContent))
            {
                textureIndex = textureContent.patternIndex;
                partColor = textureContent.color;
            }

            if (dye != null)
                partColor *= (Color)new Color32((byte)dye.red, (byte)dye.green, (byte)dye.blue, 255);


            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            if (partColor.a < 0.5f)
                meshRenderer.sharedMaterial = TransparentMaterial;
            else
                meshRenderer.sharedMaterial = OriginalMaterial;
            meshRenderer.sharedMaterial.SetTexture("_MatTex", ContentLoader.Instance.PatternTextureArray);
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_MatColor", partColor);
            prop.SetFloat("_MatIndex", textureIndex);
            meshRenderer.SetPropertyBlock(prop);
        }
    }
}