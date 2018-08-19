using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TokenLists;
using UnityEngine;

public class ItemRaws : ScriptableObject, IReadOnlyDictionary<MatPairStruct, MaterialDefinition>
{
    [SerializeField]
    List<MaterialDefinition> _itemList = new List<MaterialDefinition>();

    Dictionary<MatPairStruct, MaterialDefinition> itemLookup = new Dictionary<MatPairStruct, MaterialDefinition>();
    Dictionary<string, MaterialDefinition> stringLookup = new Dictionary<string, MaterialDefinition>();

    private void PopulateLookupTable()
    {
        ItemTokenList.ItemTokens = _itemList;
        itemLookup.Clear();
        foreach (var item in _itemList)
        {
            itemLookup[item.mat_pair] = item;
            stringLookup[item.id] = item;
            if(item.id.Contains("/")) //It's a sub-item.
            {
                stringLookup[item.id.Split('/').Last()] = item;
            }
        }
    }

    public MaterialDefinition FromToken(string token)
    {
        return stringLookup[token];
    }

    static ItemRaws _instance = null;

    public List<MaterialDefinition> ItemList
    {
        set
        {
            _itemList = value;
            PopulateLookupTable();
        }
        get
        {
            return _itemList;
        }
    }


    public static ItemRaws Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ItemRaws>("ItemRaws");
                if (_instance == null)
                    _instance = CreateInstance<ItemRaws>();
                Instance.PopulateLookupTable();
            }
            return _instance;
        }
    }

    public IEnumerable<MatPairStruct> Keys => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).Keys;

    public IEnumerable<MaterialDefinition> Values => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).Values;

    public int Count => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).Count;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        if (_instance == null)
            _instance = this;
    }

    public bool ContainsKey(MatPairStruct key)
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).ContainsKey(key);
    }

    public bool TryGetValue(MatPairStruct key, out MaterialDefinition value)
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<MatPairStruct, MaterialDefinition>> GetEnumerator()
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).GetEnumerator();
    }

    public MaterialDefinition this[MatPairStruct mat]
    {
        get
        {
            return itemLookup[mat];
        }
    }
}
