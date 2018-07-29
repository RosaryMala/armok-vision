using System.Collections.Generic;
using MaterialStore;
using RemoteFortressReader;
using UnityEngine;

public class CreatureBody : MonoBehaviour
{
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

    public CreatureRaw race;
    public CasteRaw caste;
    public BodyPart rootPart;
    public int stanceCount;
    public Bounds bounds;
    public Vector3 bodyScale;
    public CreatureRawFlags flags;

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

    public void MakeBody()
    {
        flags = new CreatureRawFlags(race.flags);
        bodyCategory = FindBodyCategory(caste);
        var spawnedParts = new Dictionary<int, BodyPart>();
        float scale = caste.adult_size / (float)caste.total_relsize * 10;
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
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

            bodyScale = BodyDefinition.GetBodyScale(bodyCategory, race, caste);

            BodyPartLayerRaw usedLayer = null;
            foreach (var layer in part.layers)
            {
                if (usedLayer == null || usedLayer.layer_depth >= layer.layer_depth)
                    usedLayer = layer;
            }

            var tissue = race.tissues[usedLayer.tissue_id];

            var color = ContentLoader.GetColor(tissue.material);
            spawnedPart.material = MaterialRaws.Instance[tissue.material];

            propertyBlock.SetColor("_Color", color);

            var model = BodyDefinition.GetPart(bodyCategory, race, caste, part);
            if (model != null)
            {
                var placedModel = Instantiate(model);
                placedModel.transform.SetParent(spawnedPart.transform);
                spawnedPart.modeledPart = placedModel;
                placedModel.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(propertyBlock);
            }

            if (spawnedPart.modeledPart == null)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<VolumeKeeper>();
                cube.name = spawnedPart.name + " cube";
                cube.transform.SetParent(spawnedPart.transform);
                cube.volume = spawnedPart.volume;
                cube.FixVolume();
                spawnedPart.placeholder = cube;
                cube.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(propertyBlock);
            }
            spawnedParts[i] = spawnedPart;
        }
        foreach (var mod in caste.color_modifiers)
        {
            foreach (var partID in mod.body_part_id)
            {
                var part = spawnedParts[partID];
                var colorMod = mod.patterns[Mathf.Abs(GetInstanceID()) % mod.patterns.Count].colors[0];
                var color = new Color32((byte)colorMod.red, (byte)colorMod.green, (byte)colorMod.blue, 128);
                propertyBlock.SetColor("_Color", color);
                if (part.modeledPart != null)
                {
                    foreach (var renderer in part.modeledPart.GetComponentsInChildren<MeshRenderer>())
                    {
                        renderer.SetPropertyBlock(propertyBlock);
                    }
                }
                if (part.placeholder != null)
                {
                    foreach (var renderer in part.placeholder.GetComponentsInChildren<MeshRenderer>())
                    {
                        renderer.SetPropertyBlock(propertyBlock);
                    }
                }
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
                spawnedParts[i].transform.SetParent(spawnedParts[part.parent].transform);
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
}
