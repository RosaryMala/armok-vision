using System.Collections.Generic;
using System;
using RemoteFortressReader;
using UnityEngine;
using UnitFlags;
using DF.Enums;

public class CreatureBody : MonoBehaviour
{
    static Material skinMat = null;

    public enum BodyCategory
    {
        None, //No body at all.
        Humanoid,
        Quadruped,
        Avian,
        Bug,
        Fish
    }
    public BodyCategory bodyCategory;
    public UnitDefinition unit;
    public CreatureRaw race;
    public CasteRaw caste;
    public BodyPart rootPart;
    public int stanceCount;
    public Bounds bounds;
    public Vector3 bodyScale;
    public CreatureRawFlags flags;
    public Transform riderPosition;

    public static BodyCategory FindBodyCategory(CasteRaw caste)
    {
        if (caste.body_parts.Count == 0)
            return BodyCategory.None;
        var rootPart = caste.body_parts[0];
        int stanceCount = 0;
        bool hasArms = false;
        foreach (var part in caste.body_parts)
        {
            if (part.parent < 0)
                rootPart = part;
            if (part.flags[(int)BodyPartFlags.BodyPartRawFlags.STANCE])
                stanceCount++;
            if (part.category.StartsWith("ARM"))
                hasArms = true;
        }
        if (stanceCount > 4)
            return BodyCategory.Bug;
        else if (stanceCount > 2)
            return BodyCategory.Quadruped;
        else if(stanceCount == 2)
        {
            if (hasArms)
                return BodyCategory.Humanoid;
            else
                return BodyCategory.Avian;
        }
            return BodyCategory.Fish;

    }

    Dictionary<int, BodyPart> spawnedParts = new Dictionary<int, BodyPart>();

    public void MakeBody()
    {
        flags = new CreatureRawFlags(race.flags);
        bodyCategory = FindBodyCategory(caste);
        if(spawnedParts.Count > 0)
        {
            foreach (var part in spawnedParts)
            {
                Destroy(part.Value.gameObject);
            }
            spawnedParts.Clear();
        }
        float scale = caste.adult_size / (float)caste.total_relsize * 10;
        if(unit != null && unit.size_info != null)
            scale = unit.size_info.size_cur / (float)caste.total_relsize * 10;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        bodyScale = BodyDefinition.GetBodyScale(bodyCategory, race, caste);

        Vector3 unitShape = Vector3.one;
        for (int i = 0; i < caste.body_appearance_modifiers.Count; i++)
        {
            var mod = caste.body_appearance_modifiers[i];
            int value = 100;
            if (unit != null && unit.appearance != null)
                value = unit.appearance.body_modifiers[i];
            else
                value = UnityEngine.Random.Range(mod.mod_min, mod.mod_max);
            switch (mod.type)
            {
                case "BROADNESS":
                    unitShape.x = value / 100f;
                    break;
                case "HEIGHT":
                    unitShape.y = value / 100f;
                    break;
                case "LENGTH":
                    unitShape.z = value / 100f;
                    break;
                default:
                    break;
            }
        }

        bodyScale = BodyPart.MultiplyScales(bodyScale, unitShape);

        for (int i = 0; i < caste.body_parts.Count; i++)
        {
            var part = caste.body_parts[i];
            if (part.flags[(int)BodyPartFlags.BodyPartRawFlags.INTERNAL])
                continue;

            var spawnedPart = new GameObject().AddComponent<BodyPart>();
            spawnedPart.name = string.Format("{0} ({1})", part.token, part.category);
            spawnedPart.token = part.token;
            spawnedPart.category = part.category;
            spawnedPart.flags = new BodyPartFlags(part.flags);
            spawnedPart.volume = part.relsize * scale;
            spawnedPart.layers = part.layers;



            var model = BodyDefinition.GetPart(bodyCategory, race, caste, part);
            if (model != null)
            {
                var placedModel = Instantiate(model);
                placedModel.transform.SetParent(spawnedPart.transform);
                placedModel.CollectEquipment();
                spawnedPart.modeledPart = placedModel;
            }

            if (spawnedPart.modeledPart == null)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<VolumeKeeper>();
                if ((Application.isPlaying))
                    Destroy(cube.GetComponent<BoxCollider>());
                else
                    DestroyImmediate(cube.GetComponent<BoxCollider>());

                cube.name = spawnedPart.name + " cube";
                cube.transform.SetParent(spawnedPart.transform);
                cube.volume = spawnedPart.volume;
                cube.FixVolume();
                cube.gameObject.AddComponent<BodyLayer>();
                spawnedPart.placeholder = cube;
                if (skinMat == null)
                    skinMat = Resources.Load<Material>("Skin");
                cube.GetComponent<MeshRenderer>().sharedMaterial = skinMat;
            }
            if (spawnedPart.flags.upperbody)
            {
                upperBody = spawnedPart;
                foreach (var item in spawnedPart.GetComponentsInChildren<BodyPartChildPlaceholder>())
                {
                    if (item.category == ":ATTACH:")
                    {
                        riderPosition = item.transform;
                        break;
                    }
                }
            }
            if (spawnedPart.flags.lowerbody)
                lowerBody = spawnedPart;
            var modeledLayers = spawnedPart.GetComponentsInChildren<BodyLayer>();
            foreach (var layer in part.layers)
            {
                var matchedLayers = Array.FindAll(modeledLayers, x => x.layerName == layer.layer_name);
                if (matchedLayers.Length == 0)
                {
                    matchedLayers = Array.FindAll(modeledLayers, x => string.IsNullOrEmpty(x.layerName));
                    if (matchedLayers.Length < 0)
                        spawnedPart.layerModels.Add(null);
                    else
                    {
                        bool matchedAny = false;
                        foreach (var matchedLayer in matchedLayers)
                        {
                            if (!matchedLayer.placed)
                            {
                                matchedLayer.layerRaw = layer;
                                spawnedPart.layerModels.Add(matchedLayer);
                                matchedLayer.placed = true;
                                matchedAny = true;
                                break;
                            }
                        }
                        if (!matchedAny)
                            spawnedPart.layerModels.Add(null);
                    }
                }
                else
                {
                    bool matchedAny = false;
                    foreach (var matchedLayer in matchedLayers)
                    {
                        if (!matchedLayer.placed)
                        {
                            matchedLayer.layerRaw = layer;
                            spawnedPart.layerModels.Add(matchedLayer);
                            matchedLayer.placed = true;
                            matchedAny = true;
                            break;
                        }
                    }
                    if (!matchedAny)
                        spawnedPart.layerModels.Add(null);
                }
            }
            foreach (var modeledLayer in modeledLayers)
            {
                if (!modeledLayer.placed)
                    modeledLayer.gameObject.SetActive(false);
            }

            foreach (var layerModel in spawnedPart.layerModels)
            {
                if (layerModel == null)
                    continue;
                var renderer = layerModel.GetComponentInChildren<MeshRenderer>();
                if(renderer != null)
                {
                    var tissue = race.tissues[layerModel.layerRaw.tissue_id];
                    var color = ContentLoader.GetColor(tissue.material);
                    var index = ContentLoader.GetPatternIndex(tissue.material);
                    propertyBlock.SetColor("_MatColor", color);
                    propertyBlock.SetFloat("_MatIndex", index);
                    renderer.SetPropertyBlock(propertyBlock);
                }
            }
            spawnedParts[i] = spawnedPart;
        }
        for (int modNum = 0; modNum < caste.modifier_idx.Count; modNum++)
        {
            if (!spawnedParts.ContainsKey(caste.part_idx[modNum]))
                continue;
            var mod = caste.modifiers[caste.modifier_idx[modNum]];
            var part = spawnedParts[caste.part_idx[modNum]];
            if(caste.layer_idx[modNum] >= 0)
            {
                var layer = part.layerModels[caste.layer_idx[modNum]];
                if (layer != null)
                {
                    layer.mods.Add(new BodyPart.ModValue(mod, modNum));
                }
            }
            else
            {
                part.mods.Add(new BodyPart.ModValue(mod, modNum));
            }
        }

        for (int modNum = 0; modNum < caste.color_modifiers.Count; modNum++)
        {
            var colorMod = caste.color_modifiers[modNum];
            //Temp fix until actual creatures are being read.
            if (colorMod.start_date > 0)
                continue;
            int seed = Mathf.Abs(GetInstanceID() * modNum) % colorMod.patterns.Count;
            for (int i = 0; i < colorMod.body_part_id.Count; i++)
            {
                var part = spawnedParts[colorMod.body_part_id[i]];
                if (part == null || !part.gameObject.activeSelf)
                    continue;
                var layer = part.layerModels[colorMod.tissue_layer_id[i]];
                if (layer == null || !layer.gameObject.activeSelf)
                    continue;
                ColorDefinition colorDef;
                if (unit != null && unit.appearance != null)
                    colorDef = colorMod.patterns[unit.appearance.colors[modNum]].colors[0];
                else
                    colorDef = colorMod.patterns[seed].colors[0];
                var color = new Color32((byte)colorDef.red, (byte)colorDef.green, (byte)colorDef.blue, 128);
                var index = ContentLoader.GetPatternIndex(race.tissues[layer.layerRaw.tissue_id].material);
                propertyBlock.SetColor("_MatColor", color);
                propertyBlock.SetFloat("_MatIndex", index);
                var renderer = layer.GetComponentInChildren<MeshRenderer>();
                if (renderer != null)
                    renderer.SetPropertyBlock(propertyBlock);
            }
        }
        for (int i = 0; i < caste.body_parts.Count; i++)
        {
            if (!spawnedParts.ContainsKey(i))
                continue;
            var part = caste.body_parts[i];
            if (!spawnedParts.ContainsKey(part.parent))
                spawnedParts[i].transform.SetParent(transform);
            else
            {
                spawnedParts[i].transform.SetParent(spawnedParts[part.parent].transform);
                spawnedParts[i].parent = spawnedParts[part.parent];
                spawnedParts[part.parent].children.Add(spawnedParts[i]);
            }
            if (part.parent < 0)
                rootPart = spawnedParts[i];
            if (spawnedParts[i].flags.stance)
                stanceCount++;
        }

        if (rootPart == null)
            return; //There's no root part, means there's no body.

        //Use this when we do body part mods.
        //for(int i = 0; i < caste.modifier_idx.Count; i++)
        //{
        //    var modifier = caste.modifiers[caste.modifier_idx[i]];
        //    var part = caste.body_parts[caste.part_idx[i]];
        //}


        rootPart.Arrange(this);
        bounds = rootPart.GetComponentInChildren<MeshRenderer>().bounds;
        foreach (var item in rootPart.GetComponentsInChildren<MeshRenderer>())
        {
            bounds.Encapsulate(item.bounds);
        }
        rootPart.transform.localPosition = new Vector3(0, -bounds.min.y, 0);

    }

    public bool onGround;
    private BodyPart upperBody;
    private BodyPart lowerBody;

    public void UpdateUnit(UnitDefinition unit)
    {
        if (((UnitFlags1)unit.flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
        {
            if (!onGround)
            {
                rootPart.transform.localRotation = Quaternion.Euler(90, 0, 0);
                rootPart.transform.localPosition = new Vector3(0, bounds.max.z, 0);
                onGround = true;
            }
        }
        else
        {
            if (onGround)
            {
                rootPart.transform.localRotation = Quaternion.identity;
                rootPart.transform.localPosition = new Vector3(0, -bounds.min.y, 0);
                onGround = false;
            }
        }
        if (unit.facing != null && GameMap.DFtoUnityDirection(unit.facing).sqrMagnitude > 0 && unit.rider_id < 0)
            transform.rotation = Quaternion.LookRotation(GameMap.DFtoUnityDirection(unit.facing));
        else if (unit.rider_id >= 0)
            transform.rotation = Quaternion.identity;

        if (InventoryChanged(unit.inventory))
        {
            foreach (var part in spawnedParts)
            {
                part.Value.inventory.Clear();
            }
            //Here we add pants first before shirts because otherwise the layering looks bad.
            foreach (var item in unit.inventory)
            {
                if((ItemType)item.item.type.mat_type == ItemType.Pants)
                AddInventoryItem(item);
            }
            foreach (var item in unit.inventory)
            {
                if ((ItemType)item.item.type.mat_type != ItemType.Pants)
                    AddInventoryItem(item);
            }
            foreach (var part in spawnedParts)
            {
                part.Value.UpdateItems();
            }
        }
    }

    private void AddInventoryItem(InventoryItem item)
    {
        if (spawnedParts.ContainsKey(item.body_part_id))
        {
            var itemDef = ItemRaws.Instance[item.item.type];
            var material = MaterialRaws.Instance[item.item.material];
            //If it's any item at all, it'll at least be on the body part itself, either carried or worn.
            var SpecifiedPart = spawnedParts[item.body_part_id];
            SpecifiedPart.inventory.Add(new BodyPart.Equip(item, itemDef, material));
            //If it's worn, then coverage rules apply.
            if (item.mode == InventoryMode.Worn)
            {
                switch ((ItemType)item.item.type.mat_type)
                {
                    case ItemType.Armor:
                        {
                            if (upperBody != SpecifiedPart)
                                upperBody.inventory.Add(new BodyPart.Equip(item, itemDef, material));
                            if (lowerBody != SpecifiedPart)
                                lowerBody.inventory.Add(new BodyPart.Equip(item, itemDef, material));
                            if (itemDef.down_step > 0)
                                ApplyItemDown(lowerBody, item, itemDef, material, itemDef.down_step - 1);
                            if (itemDef.up_step > 0)
                                ApplyItemDown(upperBody, item, itemDef, material, itemDef.up_step - 1);
                        }
                        break;
                    case ItemType.Gloves:
                    case ItemType.Shoes:
                    case ItemType.Shield:
                        {
                            if (itemDef.down_step > 0)
                                ApplyItemDown(SpecifiedPart, item, itemDef, material, itemDef.down_step - 1);
                            if (itemDef.up_step > 0)
                                ApplyItemUp(SpecifiedPart, item, itemDef, material, itemDef.up_step - 1);
                        }
                        break;
                    case ItemType.Pants:
                        {
                            if (itemDef.down_step > 0)
                                ApplyItemDown(SpecifiedPart, item, itemDef, material, itemDef.down_step - 1);
                        }
                        break;
                }
            }
        }
    }

    InventoryMode[] inventoryModes = new InventoryMode[0];

    private bool InventoryChanged(List<InventoryItem> inventory)
    {
        if(inventory.Count != inventoryModes.Length)
        {
            inventoryModes = new InventoryMode[inventory.Count];
            for(int i = 0; i < inventory.Count; i++)
            {
                inventoryModes[i] = inventory[i].mode;
            }
            return true;
        }
        else
        {
            bool changed = false;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventoryModes[i] != inventory[i].mode)
                    changed = true;
                inventoryModes[i] = inventory[i].mode;
            }
            return changed;
        }
    }

    private void ApplyItemUp(BodyPart part, InventoryItem item, MaterialDefinition itemDef, MaterialDefinition material, int up_step)
    {
        var parent = part.parent;
        //These are all flags for primary parts that take equipment. Other equipment doesn't cover it.
        if (parent.flags.grasp || parent.flags.upperbody || parent.flags.lowerbody || parent.flags.head || parent.flags.stance)
            return;
        parent.inventory.Add(new BodyPart.Equip(item, itemDef, material));
        if (up_step > 0)
            ApplyItemUp(parent, item, itemDef, material, up_step - 1);
    }

    private void ApplyItemDown(BodyPart part, InventoryItem item, MaterialDefinition itemDef, MaterialDefinition material, int down_step)
    {
        foreach (var child in part.children)
        {
            //These are all flags for primary parts that take equipment. Other equipment doesn't cover it.
            if (child.flags.grasp || child.flags.upperbody || child.flags.lowerbody || child.flags.head || child.flags.stance)
                continue;
            child.inventory.Add(new BodyPart.Equip(item, itemDef, material));
            if (down_step > 0)
                ApplyItemDown(child, item, itemDef, material, down_step - 1);
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}
