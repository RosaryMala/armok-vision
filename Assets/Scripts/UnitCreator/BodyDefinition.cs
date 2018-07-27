using RemoteFortressReader;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu]
public class BodyDefinition : ScriptableObject
{
    [System.Serializable]
    public class BodyPartSelection
    {
        public string category;
        public bool categoryRegex;
        public string token;
        public bool tokenRegex;
        public BodyPartModel model;
    }
    public Vector3 bodyScale = Vector3.one;

    public List<BodyPartSelection> bodyParts;

    internal BodyPartModel GetPart(BodyPartRaw part)
    {
        foreach (var partModel in bodyParts)
        {
            if(!string.IsNullOrEmpty(partModel.category))
            {
                if(partModel.categoryRegex)
                {
                    if (!Regex.IsMatch(part.category, partModel.category))
                        continue;
                }
                else
                {
                    if (partModel.category != part.category)
                        continue;
                }
            }
            if (!string.IsNullOrEmpty(partModel.token))
            {
                if (partModel.tokenRegex)
                {
                    if (!Regex.IsMatch(part.token, partModel.token))
                        continue;
                }
                else
                {
                    if (partModel.token != part.token)
                        continue;
                }
            }
            return partModel.model;
        }
        return null;
    }

    static Dictionary<CreatureBody.BodyCategory, BodyDefinition> categoryParts = new Dictionary<CreatureBody.BodyCategory, BodyDefinition>();
    static Dictionary<string, BodyDefinition> raceParts = new Dictionary<string, BodyDefinition>();
    static Dictionary<string, Dictionary<string, BodyDefinition>> casteParts = new Dictionary<string, Dictionary<string, BodyDefinition>>();

    static void LoadDefaultBodyParts()
    {
        categoryParts = new Dictionary<CreatureBody.BodyCategory, BodyDefinition>();
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Humanoid);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Quadruped);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Avian);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Bug);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Fish);
    }

    static void LoadDefaultBodyParts(CreatureBody.BodyCategory category)
    {
        var part = Resources.Load<BodyDefinition>("BodyDefinitions/Default/" + category);
        if (part != null)
            categoryParts[category] = part;
    }

    public static BodyPartModel GetPart(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste, BodyPartRaw part)
    {
        if (categoryParts == null)
            LoadDefaultBodyParts();

        BodyPartModel partModel = null;
        var bodyDef = GetBodyDefinition(race, caste);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        if (partModel != null)
            return partModel;
        bodyDef = GetBodyDefinition(race);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        if (partModel != null)
            return partModel;
        bodyDef = GetBodyDefinition(category);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        return partModel;
    }

    public static Vector3 GetBodyScale(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste)
    {
        var body = GetBodyDefinition(category, race, caste);
        if (body == null)
            return Vector3.one;
        return body.bodyScale;
    }

    static BodyDefinition GetBodyDefinition(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste)
    {
        BodyDefinition body = null;
        body = GetBodyDefinition(race, caste);
        if (body == null)
            body = GetBodyDefinition(race);
        if (body == null)
            body = GetBodyDefinition(category);
        return body;
    }
    static BodyDefinition GetBodyDefinition(CreatureRaw race, CasteRaw caste)
    {
        if (!casteParts.ContainsKey(race.creature_id))
            casteParts[race.creature_id] = new Dictionary<string, BodyDefinition>();
        if(!casteParts[race.creature_id].ContainsKey(caste.caste_id))
            casteParts[race.creature_id][caste.caste_id] = Resources.Load<BodyDefinition>("BodyDefinitions/" + race.creature_id + "/" + caste.caste_id);
        return casteParts[race.creature_id][caste.caste_id];
    }
    static BodyDefinition GetBodyDefinition(CreatureRaw race)
    {
        if (!raceParts.ContainsKey(race.creature_id))
            raceParts[race.creature_id] = Resources.Load<BodyDefinition>("BodyDefinitions/" + race.creature_id + "/Default");
        return raceParts[race.creature_id];
    }
    static BodyDefinition GetBodyDefinition(CreatureBody.BodyCategory category)
    {
        if (!categoryParts.ContainsKey(category))
            categoryParts[category] = Resources.Load<BodyDefinition>("BodyDefinitions/Default/" + category);
        return categoryParts[category];
    }
}
