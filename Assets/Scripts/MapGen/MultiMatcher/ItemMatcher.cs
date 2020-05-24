using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class ItemMatcher<T>
{
    public T this[string token]
    {
        set
        {
            if(!ItemTokenList.ItemLookup.ContainsKey(token))
            {
                Debug.Log("Invalid item: " + token);
                return;
            }
            BaseContainer[ItemTokenList.ItemLookup[token].mat_pair] = value;
        }
    }
    public T this[MatPairStruct mat]
    {
        set
        {
            BaseContainer[mat] = value;
        }
    }
    public bool Get(MatPairStruct mat, out T value)
    {
        if(BaseContainer != null)
        {
            T output;
            if (BaseContainer.TryGetValue(mat, out output))
            {
                value = output;
                return true;
            }
            mat = new MatPairStruct(mat.Type, -1); //Try once more with a more generic value.
            if (BaseContainer.TryGetValue(mat, out output))
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
        BaseContainer.Clear();
    }
    public Dictionary<MatPairStruct, T> BaseContainer { get; } = new Dictionary<MatPairStruct, T>();
}
