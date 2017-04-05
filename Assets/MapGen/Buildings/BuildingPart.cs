using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;
using TokenLists;

namespace Building
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class BuildingPart : MonoBehaviour
    {
        MeshRenderer meshRenderer;

        public string item;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        internal void SetMaterial(BuildingInstance buildingInput)
        {
            ColorDefinition dye = null;
            MatPairStruct mat = new MatPairStruct(-1,-1);
            if (string.IsNullOrEmpty(item) || ItemTokenList.ItemLookup == null || !ItemTokenList.ItemLookup.ContainsKey(item))
                mat = buildingInput.material;
            else
            {
                MatPairStruct itemCode = ItemTokenList.ItemLookup[item].mat_pair;

                foreach (var item in buildingInput.items)
                {
                    //skip items that are just stored in the building.
                    //though they should be later in the list anyway.
                    if (item.mode == 0)
                        continue;
                    //if our setting is a generic item, like any weapon, then any subtype can match.
                    if ((itemCode.mat_index == -1 && itemCode.mat_type == item.item.type.mat_type)
                        || (item.item.type == itemCode))
                    {
                        mat = item.item.material;
                        dye = item.item.dye;
                        break;
                    }
                }
            }
            Color partColor = Color.gray;
            ColorContent colorContent;
            if (ContentLoader.Instance.MaterialColors.TryGetValue(mat, out colorContent))
                partColor = colorContent.color;

            if (dye != null)
                partColor *= (Color)new Color32((byte)dye.red, (byte)dye.green, (byte)dye.blue, 255);

            int textureIndex = 0;
            TextureContent textureContent;
            if (ContentLoader.Instance.MaterialTextures.TryGetValue(mat, out textureContent))
                textureIndex = textureContent.StorageIndex;

            meshRenderer.sharedMaterial.SetTexture("_MatTex", ContentLoader.Instance.materialTextureStorage.AtlasTexture);
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            prop.SetColor("_MatColor", partColor);
            prop.SetFloat("_MatIndex", textureIndex);
            meshRenderer.SetPropertyBlock(prop);
        }
    }
}