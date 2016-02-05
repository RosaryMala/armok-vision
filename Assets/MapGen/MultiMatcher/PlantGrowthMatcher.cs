using RemoteFortressReader;
using System.Collections.Generic;
using TokenLists;
using System;

public class PlantGrowthMatcher<T>
{
    struct GrowthMatch
    {
        public T item;
        public int difference;
    }

    Dictionary<BuildingStruct, GrowthMatch> matList;

    void TrySetMatch(GrowthMatch match, BuildingStruct growth)
    {
        if (matList == null)
            matList = new Dictionary<BuildingStruct, GrowthMatch>();
        if (matList.ContainsKey(growth))
        {
            if (matList[growth].difference < match.difference)//overwrite existing exact matches
                return; //the comparitor can be changed to <= if that behavior is not desired.
        }
        matList[growth] = match;
    }
    void Setwords(string word, Dictionary<string, BuildingStruct> wordList, GrowthMatch match)
    {
        if (word == "*")
        {
            match.difference |= 4;
            foreach (BuildingStruct item in wordList.Values)
            {
                TrySetMatch(match, item);
            }
        }
        else
        {
            if (wordList.ContainsKey(word))
                TrySetMatch(match, wordList[word]);
        }
    }
    void Setwords(string word, string suffix, Dictionary<string, Dictionary<string, BuildingStruct>> wordList, GrowthMatch match)
    {
        if (word == "*")
        {
            match.difference |= 2;
            foreach (var item in wordList.Values)
            {
                Setwords(suffix, item, match);
            }
        }
        else
        {
            if (wordList.ContainsKey(word))
                Setwords(suffix, wordList[word], match);
        }
    }
    void Setwords(string plant, string growth, string color, GrowthMatch match)
    {
        if (plant == "*")
        {
            match.difference |= 1;
            foreach (var item in PlantTokenList.GrowthIDs.Values)
            {
                Setwords(growth, color, item, match);
            }
        }
        else
        {
            if (PlantTokenList.GrowthIDs.ContainsKey(plant))
                Setwords(growth, color, PlantTokenList.GrowthIDs[plant], match);
        }
    }

    public T this[string token]
    {
        set
        {
            string[] parts = token.Split(':');
            GrowthMatch newItem;
            newItem.item = value;
            newItem.difference = 0;
            if (parts.Length == 3)
                Setwords(parts[0], parts[1], parts[2], newItem);
        }
    }
    public T this[BuildingStruct growth]
    {
        get
        {
            T output;
            if (!TryGetValue(growth, out output))
            {
                throw new KeyNotFoundException();
            }
            return output;
        }
        set
        {
            if (matList == null)
                matList = new Dictionary<BuildingStruct, GrowthMatch>();
            GrowthMatch newItem;
            newItem.item = value;
            newItem.difference = 0; //a material pair will always be an exact match.
            matList[growth] = newItem; //actually, is that desired?

        }
    }
    public bool TryGetValue(BuildingStruct growth, out T value)
    {
        if (matList != null)
        {
            GrowthMatch output;
            if (matList.TryGetValue(growth, out output))
            {
                value = output.item;
                return true;
            }
            growth = new BuildingStruct(growth.building_type, growth.building_subtype, -1); //Try once more with a more generic value.
            if (matList.TryGetValue(growth, out output))
            {
                value = output.item;
                return true;
            }
        }
        value = default(T);
        return false;

    }
    public void Clear()
    {
        matList.Clear();
    }

}
