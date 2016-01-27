using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnityEngine;

static class MaterialTokenList
{
    static List<MaterialDefinition> _matTokenList;
    public static List<MaterialDefinition> MaterialTokens
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
    static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> _tripleWords;
    public static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> tripleWords
    {
        get
        {
            return _tripleWords;
        }
    }

    static void AddMat(string prefix, string word, string suffix, MaterialDefinition token)
    {
        if (_tripleWords == null)
            _tripleWords = new Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>>();
        if (!_tripleWords.ContainsKey(prefix))
            _tripleWords[prefix] = new Dictionary<string, Dictionary<string, MaterialDefinition>>();
        if (!_tripleWords[prefix].ContainsKey(suffix))
            _tripleWords[prefix][suffix] = new Dictionary<string, MaterialDefinition>();
        _tripleWords[prefix][suffix][word] = token;
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
}

public class MaterialMatcher<T>
{
    struct MaterialMatch
    {
        public T item;
        public int difference;
    }

    Dictionary<MatPairStruct, MaterialMatch> matList;

    void TrySetMatch(MaterialMatch match, MatPairStruct mat)
    {
        if (matList == null)
            matList = new Dictionary<MatPairStruct, MaterialMatch>();
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
            foreach (var item in MaterialTokenList.tripleWords.Values)
            {
                Setwords(word, suffix, item, match);
            }
        }
        else
        {
            if (MaterialTokenList.tripleWords.ContainsKey(prefix))
                Setwords(word, suffix, MaterialTokenList.tripleWords[prefix], match);
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
        }
    }
    public T this[MatPairStruct mat]
    {
        //get
        //{
        //    T output;
        //    if (!TryGetValue(mat, out output))
        //        output = default(T);
        //    return output;
        //}
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
    public bool TryGetValue(MatPairStruct mat, out T value)
    {
        if (matList != null)
        {
            MaterialMatch output;
            if (matList.TryGetValue(mat, out output))
            {
                value = output.item;
                return true;
            }
            mat = new MatPairStruct(mat.mat_type, -1); //Try once more with a more generic value.
            if (matList.TryGetValue(mat, out output))
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
