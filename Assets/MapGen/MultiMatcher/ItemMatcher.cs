using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class ItemMatcher<T>
{
    Dictionary<IntPair, T> itemList;
    public T this[string token]
    {
        set
        {
            if(!ItemTokenList.ItemLookup.ContainsKey(token))
            {
                Debug.Log("Invalid item: " + token);
                return;
            }
            if (itemList == null)
                itemList = new Dictionary<IntPair, T>();
            itemList[ItemTokenList.ItemLookup[token].mat_pair] = value;
        }
    }
    public T this[IntPair mat]
    {
        set
        {
            if (itemList == null)
                itemList = new Dictionary<IntPair, T>();
            itemList[mat] = value;
        }
    }
    public bool Get(IntPair mat, out T value)
    {
        if(itemList != null)
        {
            T output;
            if (itemList.TryGetValue(mat, out output))
            {
                value = output;
                return true;
            }
            mat = new IntPair(mat.mat_index, -1); //Try once more with a more generic value.
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
