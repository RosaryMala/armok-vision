using RemoteFortressReader;
using System;
using System.Collections.Generic;

public class MaterialMatcher<T>
{
    public List<MaterialDefinition> matTokenList { get; set; }
    struct MaterialMatch
    {
        public T item;
        public int difference;
    }
    Dictionary<MatPairStruct, MaterialMatch> matList;
    public T this[string token]
    {
        set
        {
            if (matTokenList == null)
                throw new ArgumentNullException("matTokenList");

            foreach (MaterialDefinition material in matTokenList)
            {
                int match = WildMatch.ScoreMatch(material.id, token); //gives a lower positive score for better matches
                if (match < 0)
                    continue;
                if (matList == null)
                    matList = new Dictionary<MatPairStruct, MaterialMatch>();
                MatPairStruct pair = material.mat_pair;
                if (matList.ContainsKey(pair))
                {
                    if (matList[pair].difference < match)//overwrite existing exact matches
                        continue; //the comparitor can be changed to <= if that behavior is not desired.
                }
                MaterialMatch newItem;
                newItem.item = value;
                newItem.difference = match;
                matList[pair] = newItem;
            }
        }
    }
    public T this[MatPairStruct mat]
    {
        get
        {
            MaterialMatch output;
            if (matList.TryGetValue(mat, out output))
            {
                return output.item;
            }
            return default(T);
        }
        set
        {
            if (matList == null)
                matList = new Dictionary<MatPairStruct, MaterialMatch>();
            MaterialMatch newItem;
            newItem.item = value;
            newItem.difference = 0; //a material pair will always be an exact match.
            matList[mat] = newItem; //actually, is that desired?

        }
    }
    public void Clear()
    {
        matList.Clear();
    }
}
