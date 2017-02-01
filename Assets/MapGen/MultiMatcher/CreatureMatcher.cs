using System.Collections.Generic;
using TokenLists;

public class CreatureRaceMatcher<T>
{
    struct RaceMatch
    {
        public T item;
        public int difference;
    }
    Dictionary<MatPairStruct, RaceMatch> creatureRaceList;

    void TrySetMatch(RaceMatch match, MatPairStruct mat)
    {
        if (creatureRaceList == null)
            creatureRaceList = new Dictionary<MatPairStruct, RaceMatch>();
        if (creatureRaceList.ContainsKey(mat))
        {
            if (creatureRaceList[mat].difference < match.difference)//overwrite existing exact matches
                return; //the comparitor can be changed to <= if that behavior is not desired.
        }
        creatureRaceList[mat] = match;
    }

    void Setwords(string caste, Dictionary<string, MatPairStruct> casteIDList, RaceMatch match)
    {
        if (caste == "*")
        {
            match.difference |= 2;
            foreach (var item in casteIDList.Values)
            {
                TrySetMatch(match, item);
            }
        }
        else
        {
            if (casteIDList.ContainsKey(caste))
                TrySetMatch(match, casteIDList[caste]);
        }
    }
    void Setwords(string race, string caste, RaceMatch match)
    {
        if (race == "*")
        {
            match.difference |= 1;
            foreach (var item in CreatureTokenList.CasteIDs.Values)
            {
                Setwords(caste, item, match);
            }
        }
        else
        {
            if (CreatureTokenList.CasteIDs.ContainsKey(race))
                Setwords(caste, CreatureTokenList.CasteIDs[race], match);
        }
    }

    /// <summary>
    /// sets an object to the specified token, using a static creature token list
    /// that is populated from DF the first time it's used.
    /// </summary>
    /// <param name="token">String token in the form of "RACE:CASTE"</param>
    /// <returns></returns>
    public T this[string token]
    {
        set
        {
            string[] parts = token.Split(':');
            RaceMatch newItem;
            newItem.item = value;
            newItem.difference = 0;
            switch (parts.Length)
            {
                case 1:
                    Setwords(parts[0], "*", newItem);
                    break;
                case 2:
                    Setwords(parts[0], parts[1], newItem);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// sets an object to the specified race/caste ID.
    /// This will always take precedence over objects set by string tokens.
    /// </summary>
    /// <param name="caste">numerical race/caste</param>
    /// <returns></returns>
    public T this[MatPairStruct caste]
    {
        set
        {
            if (creatureRaceList == null)
                creatureRaceList = new Dictionary<MatPairStruct, RaceMatch>();
            RaceMatch newItem;
            newItem.item = value;
            newItem.difference = 0;
            creatureRaceList[caste] = newItem;
        }
    }
    public bool TryGetValue(MatPairStruct caste, out T value)
    {
        if (creatureRaceList != null)
        {
            RaceMatch output;
            if (creatureRaceList.TryGetValue(caste, out output))
            {
                value = output.item;
                return true;
            }
            caste = new MatPairStruct(caste.mat_type, -1);
            if (creatureRaceList.TryGetValue(caste, out output))
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
        creatureRaceList.Clear();
    }
}
