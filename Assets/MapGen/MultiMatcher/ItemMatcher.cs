using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnityEngine;

static class ItemTokenList
{
    static List<MaterialDefinition> _itemTokenList;
    public static List<MaterialDefinition> itemTokenList
    {
        set
        {
            if (_itemTokenList != value)
            {
                _itemTokenList = value;
                PopulateWordLists();
            }
        }
    }
    public static bool IsValid
    {
        get
        {
            return _itemTokenList != null;
        }
    }
    public static Dictionary<string, MaterialDefinition> itemLookup { get; private set; }

    static void PopulateWordLists()
    {
        if (itemLookup == null)
            itemLookup = new Dictionary<string, MaterialDefinition>();
        foreach (MaterialDefinition token in _itemTokenList)
        {
            itemLookup[token.id] = token;
        }
    }

}


public class ItemMatcher<T>
{
    Dictionary<MatPairStruct, T> itemList;
    public T this[string token]
    {
        set
        {
            if(!ItemTokenList.itemLookup.ContainsKey(token))
            {
                Debug.Log("Invalid item: " + token);
                return;
            }
            if (itemList == null)
                itemList = new Dictionary<MatPairStruct, T>();
            itemList[ItemTokenList.itemLookup[token].mat_pair] = value;
        }
    }
    public T this[MatPairStruct mat]
    {
        set
        {
            if (itemList == null)
                itemList = new Dictionary<MatPairStruct, T>();
            itemList[mat] = value;
        }
    }
    public bool Get(MatPairStruct mat, out T value)
    {
        if(itemList != null)
        {
            T output;
            if (itemList.TryGetValue(mat, out output))
            {
                value = output;
                return true;
            }
            mat.mat_type = -1; //Try once more with a more generic value.
            if (itemList.TryGetValue(mat, out output))
            {
                value = output;
                return true;
            }

        }
        value = default(T);
        return false;
    }
    public void Clear()
    {
        itemList.Clear();
    }
}
