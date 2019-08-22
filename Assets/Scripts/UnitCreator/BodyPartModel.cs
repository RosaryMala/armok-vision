using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class BodyPartModel : MonoBehaviour
{
    public float canonVolume = 1;
    public float volume;

    internal void FixVolume()
    {
        if (volume == 0)
            volume = transform.localScale.x * transform.localScale.y * transform.localScale.z * canonVolume;
        float factor = transform.localScale.x * transform.localScale.y * transform.localScale.z;
        if (factor == 0)
            factor = 0.000001f;
        factor = volume / factor / canonVolume;
        transform.localScale *= Mathf.Pow(factor, 1 / 3.0f);
    }

    Transform censorField;

    Dictionary<MatPairStruct, ItemModel> specialEquipment;

    class GenericClothingLayer
    {
        public Mesh mesh;
        public Vector3 pos;
        public Vector3 scale;
        public Quaternion rotation;
    }

    GenericClothingLayer dressClothing;
    GenericClothingLayer pantClothing;

    public List<ItemModel> ClothingLayers { get; private set; }

    internal void CollectEquipment()
    {
        censorField = transform.Find("[CENSOR]");
        specialEquipment = new Dictionary<MatPairStruct, ItemModel>();
        foreach (var item in GetComponentsInChildren<ItemModel>())
        {
            var generic = item.GetComponent<GenericClothingItem>();
            if (generic == null)
            {
                if(ItemRaws.Instance.ContainsKey(item.name))
                    specialEquipment[ItemRaws.Instance[item.name].mat_pair] = item;
                item.gameObject.SetActive(false);
            }
            else
            {
                var meshFilter = item.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    continue;
                GenericClothingLayer layer = new GenericClothingLayer
                {
                    mesh = meshFilter.sharedMesh,
                    pos = item.transform.localPosition,
                    scale = item.transform.localScale,
                    rotation = item.transform.localRotation
                };
                switch (generic.isDress)
                {
                    case GenericClothingItem.DressUsage.Any:
                        dressClothing = layer;
                        pantClothing = layer;
                        break;
                    case GenericClothingItem.DressUsage.DressOnly:
                        dressClothing = layer;
                        break;
                    case GenericClothingItem.DressUsage.NotDress:
                        pantClothing = layer;
                        break;
                }
                item.gameObject.SetActive(false);
                //if (Application.isPlaying)
                //    Destroy(item.gameObject);
                //else
                //    DestroyImmediate(item.gameObject);
            }
        }
    }

    List<ItemModel> instantiatedClothingLayers = new List<ItemModel>();

    internal void ApplyEquipment(List<BodyPart.Equip> inventory, RemoteFortressReader.UnitDefinition unit)
    {
        int wornItems = 0;
        //first of all disable any layers that are not used anymore, but not need to remove them entirely.
        for (int i = inventory.Count; i < instantiatedClothingLayers.Count; i++)
        {
            if (instantiatedClothingLayers[i] != null)
                instantiatedClothingLayers[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            //If it's not a worn clothing item, hide the respective layers.
            if (item.item.mode != RemoteFortressReader.InventoryMode.Worn)
            {
                if (instantiatedClothingLayers.Count > i && instantiatedClothingLayers[i] != null)
                    instantiatedClothingLayers[i].gameObject.SetActive(false);
            }
            if (specialEquipment.ContainsKey(item.item.item.type))
            {
                var model = specialEquipment[item.item.item.type];
                model.gameObject.SetActive(true);
                model.UpdateMaterial(item.item.item, unit);
            }
            else if (item.item.mode == RemoteFortressReader.InventoryMode.Worn)
            {
                wornItems++;
                var clothingTexture = ClothingTexture.GetTexture(item.item.item.type);
                if (clothingTexture == null || clothingTexture.baseMat == null)
                {
                    Debug.Log("Could not load texture for " + ItemRaws.Instance[item.item.item.type].id);
                    continue;
                }
                GenericClothingLayer clothing;
                if (clothingTexture.isDress)
                    clothing = dressClothing;
                else
                    clothing = pantClothing;
                if (clothing == null)
                    continue;
                if (i >= instantiatedClothingLayers.Count)
                {
                    instantiatedClothingLayers.AddRange(Enumerable.Repeat<ItemModel>(null, i - instantiatedClothingLayers.Count + 1));
                }
                if (instantiatedClothingLayers[i] == null)
                {
                    GameObject newLayer = new GameObject();
                    newLayer.transform.parent = transform;
                    newLayer.transform.localPosition = clothing.pos;
                    newLayer.transform.localRotation = clothing.rotation;
                    newLayer.transform.localScale = clothing.scale;
                    var mf = newLayer.AddComponent<MeshFilter>();
                    mf.sharedMesh = clothing.mesh;
                    var mr = newLayer.AddComponent<MeshRenderer>();
                    var mat = clothingTexture.GetMaterial(i);
                    mr.sharedMaterial = mat;
                    var im = newLayer.AddComponent<ItemModel>();
                    instantiatedClothingLayers[i] = im;
                }
                if (instantiatedClothingLayers[i] == null)
                    continue;
                instantiatedClothingLayers[i].gameObject.SetActive(true);
                instantiatedClothingLayers[i].UpdateMaterial(item.item.item, unit);
                if (Application.isEditor)
                    instantiatedClothingLayers[i].name = item.itemDef.id;
            }
        }
        if (censorField != null && GameSettings.Instance.units.censorField)
            censorField.gameObject.SetActive(wornItems == 0);
    }
}
