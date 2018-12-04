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
                if (Application.isPlaying)
                    Destroy(item.gameObject);
                else
                    DestroyImmediate(item.gameObject);
            }
        }
    }

    List<ItemModel> instantiatedClothingLayers = new List<ItemModel>();

    internal void ApplyEquipment(List<BodyPart.Equip> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            if (specialEquipment.ContainsKey(item.item.item.type))
            {
                var model = specialEquipment[item.item.item.type];
                model.gameObject.SetActive(true);
                model.UpdateMaterial(item.item.item);
                if (instantiatedClothingLayers.Count > i && instantiatedClothingLayers[i] != null)
                    instantiatedClothingLayers[i].gameObject.SetActive(false);
            }
            else
            {
                if (dressClothing == null)
                    continue;
                if (i >= instantiatedClothingLayers.Count)
                {
                    instantiatedClothingLayers.AddRange(Enumerable.Repeat<ItemModel>(null, instantiatedClothingLayers.Count - i + 1));
                }
                if (instantiatedClothingLayers[i] == null)
                {
                    //TODO: Check which one to actually use.
                    var clothing = dressClothing;
                    GameObject newLayer = new GameObject();
                    newLayer.transform.parent = transform;
                    newLayer.transform.localPosition = clothing.pos;
                    newLayer.transform.localRotation = clothing.rotation;
                    newLayer.transform.localScale = clothing.scale;
                    var mf = newLayer.AddComponent<MeshFilter>();
                    mf.sharedMesh = clothing.mesh;
                    var mr = newLayer.AddComponent<MeshRenderer>();
                    mr.sharedMaterial = new Material(Shader.Find("Building/Cutout"));
                    var im = newLayer.AddComponent<ItemModel>();
                    instantiatedClothingLayers[i] = im;
                }
                if (instantiatedClothingLayers[i] == null)
                    continue;
                instantiatedClothingLayers[i].gameObject.SetActive(true);
                instantiatedClothingLayers[i].UpdateMaterial(item.item.item);
            }
        }
    }
}
