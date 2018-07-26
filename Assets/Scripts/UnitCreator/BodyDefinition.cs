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
}
