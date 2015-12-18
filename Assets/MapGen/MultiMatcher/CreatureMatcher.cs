using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnityEngine;

static class CreatureTokenList
{
    static List<CreatureRaw> _creatureRawList;
    public static List<CreatureRaw> CreatureRawList
    {
        set
        {
            if(_creatureRawList != value)
            {
                _creatureRawList = value;
                PopulateWordLists();
            }
        }
    }

    private static Dictionary<string, Dictionary<string, MatPairStruct>> _casteIds;

    static void AddCreature(string race, string caste, MatPairStruct id)
    {
        if (_casteIds == null)
            _casteIds = new Dictionary<string, Dictionary<string, MatPairStruct>>();
        if (!_casteIds.ContainsKey(race))
            _casteIds[race] = new Dictionary<string, MatPairStruct>();
        _casteIds[race][caste] = id;
    }

    static private void PopulateWordLists()
    {
        foreach (var race in _creatureRawList)
        {
            foreach (var caste in race.caste)
            {
                MatPairStruct id = new MatPairStruct(race.index, caste.index);

            }
        }
    }
}

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

    void Setwords(string race, Dictionary<string, MaterialDefinition> wordList, RaceMatch match)
    {
        if (race == "*")
        {
            match.difference |= 4;
            foreach (MaterialDefinition item in wordList.Values)
            {
                TrySetMatch(match, item.mat_pair);
            }
        }
        else
        {
            if (wordList.ContainsKey(race))
                TrySetMatch(match, wordList[race].mat_pair);
        }
    }
    void Setwords(string race, string caste, Dictionary<string, Dictionary<string, MaterialDefinition>> wordList, RaceMatch match)
    {
        if (caste == "*")
        {
            match.difference |= 2;
            foreach (var item in wordList.Values)
            {
                Setwords(race, item, match);
            }
        }
        else
        {
            if (wordList.ContainsKey(caste))
                Setwords(race, wordList[caste], match);
        }
    }


    public T this[string token]
    {
        set
        {
            string[] parts = token.Split(':');
        }
    }
}
