using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMatcher<T>
{
    public static List<MaterialDefinition> _matTokenList;
    public List<MaterialDefinition> matTokenList
    {
        set
        {
            if (_matTokenList != value)
            {
                _matTokenList = value;
                PopulateWordLists();
            }
        }
    }
    struct MaterialMatch
    {
        public T item;
        public int difference;
    }
    Dictionary<MatPairStruct, MaterialMatch> matList;
    static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> tripleWords;

    static void AddMat(string prefix, string word, string suffix, MaterialDefinition token)
    {
        if (tripleWords == null)
            tripleWords = new Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>>();
        if (!tripleWords.ContainsKey(prefix))
            tripleWords[prefix] = new Dictionary<string, Dictionary<string, MaterialDefinition>>();
        if (!tripleWords[prefix].ContainsKey(suffix))
            tripleWords[prefix][suffix] = new Dictionary<string, MaterialDefinition>();
        tripleWords[prefix][suffix][word] = token;
    }

    static void PopulateWordLists()
    {
        foreach (MaterialDefinition token in _matTokenList)
        {
            var parts = token.id.Split(':');
            switch (parts.Length)
            {
                case 1:
                    AddMat(parts[0], "", "", token);
                    break;
                case 2:
                    AddMat(parts[0], parts[1], "", token);
                    break;
                case 3:
                    AddMat(parts[0], parts[1], parts[2], token);
                    break;
                default:
                    Debug.LogError("Found a wrong number of material token parts");
                    break;
            }
        }
    }

    void TrySetMatch(MaterialMatch match, MatPairStruct mat)
    {
        if (matList.ContainsKey(mat))
        {
            if (matList[mat].difference < match.difference)//overwrite existing exact matches
                return; //the comparitor can be changed to <= if that behavior is not desired.
        }
        matList[mat] = match;
    }
    void Setwords(string word, Dictionary<string, MaterialDefinition> wordList, MaterialMatch match)
    {
        if (word == "*")
        {
            match.difference |= 4;
            foreach (MaterialDefinition item in wordList.Values)
            {
                TrySetMatch(match, item.mat_pair);
            }
        }
        else
        {
            if (wordList.ContainsKey(word))
                TrySetMatch(match, wordList[word].mat_pair);
        }
    }
    void Setwords(string word, string suffix, Dictionary<string, Dictionary<string, MaterialDefinition>> wordList, MaterialMatch match)
    {
        if (suffix == "*")
        {
            match.difference |= 2;
            foreach (var item in wordList.Values)
            {
                Setwords(word, item, match);
            }
        }
        else
        {
            if (wordList.ContainsKey(suffix))
                Setwords(word, wordList[suffix], match);
        }
    }
    void Setwords(string prefix, string word, string suffix, MaterialMatch match)
    {
        if (prefix == "*")
        {
            match.difference |= 1;
            foreach (var item in tripleWords.Values)
            {
                Setwords(word, suffix, item, match);
            }
        }
        else
        {
            if (tripleWords.ContainsKey(prefix))
                Setwords(word, suffix, tripleWords[prefix], match);
        }
    }

    public T this[string token]
    {
        set
        {
            string[] parts = token.Split(':');
            MaterialMatch newItem;
            newItem.item = value;
            newItem.difference = 0;
            switch (parts.Length)
            {
                case 1:
                    Setwords(parts[0], "", "", newItem);
                    break;
                case 2:
                    Setwords(parts[0], parts[1], "", newItem);
                    break;
                case 3:
                    Setwords(parts[0], parts[1], parts[2], newItem);
                    break;
                default:
                    break;
            }
            //if (_matTokenList == null)
            //    throw new ArgumentNullException("matTokenList");

            //foreach (MaterialDefinition material in _matTokenList)
            //{
            //    int match = WildMatch.ScoreMatch(material.id, token); //gives a lower positive score for better matches
            //    if (match < 0)
            //        continue;
            //    if (matList == null)
            //        matList = new Dictionary<MatPairStruct, MaterialMatch>();
            //    MatPairStruct pair = material.mat_pair;
            //    if (matList.ContainsKey(pair))
            //    {
            //        if (matList[pair].difference < match)//overwrite existing exact matches
            //            continue; //the comparitor can be changed to <= if that behavior is not desired.
            //    }
            //    MaterialMatch newItem;
            //    newItem.item = value;
            //    newItem.difference = match;
            //    matList[pair] = newItem;
            //}
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
            mat.mat_type = -1; //Try once more with a more generic value.
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
