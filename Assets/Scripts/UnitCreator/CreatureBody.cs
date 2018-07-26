using System.Collections.Generic;
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

    static Dictionary<BodyCategory, BodyDefinition> defaultBodyParts;

    static void LoadDefaultBodyParts()
    {
        defaultBodyParts = new Dictionary<BodyCategory, BodyDefinition>();
        LoadDefaultBodyParts(BodyCategory.Humanoid);
        LoadDefaultBodyParts(BodyCategory.Quadruped);
        LoadDefaultBodyParts(BodyCategory.Avian);
        LoadDefaultBodyParts(BodyCategory.Bug);
        LoadDefaultBodyParts(BodyCategory.Fish);
    }

    static void LoadDefaultBodyParts(BodyCategory category)
    {
        var part = Resources.Load<BodyDefinition>("BodyDefinitions/Default/" + category);
        if (part != null)
            defaultBodyParts[category] = part;
    }

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
        bodyCategory = FindBodyCategory(caste);
        var spawnedParts = new Dictionary<int, BodyPart>();
        float scale = caste.adult_size / (float)caste.total_relsize * 10;
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

            if (defaultBodyParts == null)
                LoadDefaultBodyParts();
            if (defaultBodyParts.ContainsKey(bodyCategory))
            {
                var model = defaultBodyParts[bodyCategory].GetPart(part);
                if (model != null)
                {
                    var placedModel = Instantiate(model);
                    placedModel.transform.SetParent(spawnedPart.transform);
                    placedModel.volume = spawnedPart.volume;
                    placedModel.FixVolume();
                    spawnedPart.modeledPart = placedModel;
                }

            }
            if (spawnedPart.modeledPart == null)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<VolumeKeeper>();
                cube.name = spawnedPart.name + " cube";
                cube.transform.SetParent(spawnedPart.transform);
                cube.volume = spawnedPart.volume;
                cube.FixVolume();
                spawnedPart.placeholder = cube;
            }
            spawnedParts[i] = spawnedPart;
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

        rootPart.Arrange(this);
        bounds = rootPart.GetComponentInChildren<MeshRenderer>().bounds;
        foreach (var item in rootPart.GetComponentsInChildren<MeshRenderer>())
        {
            bounds.Encapsulate(item.bounds);
        }
        rootPart.transform.localPosition = new Vector3(0, -bounds.min.y, 0);

    }
}
