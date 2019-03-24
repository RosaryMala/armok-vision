using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TokenLists;
using UnityEngine;

public class ItemRaws : ScriptableObject, IReadOnlyDictionary<MatPairStruct, MaterialDefinition>, IReadOnlyDictionary<string, MaterialDefinition>
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

    static List<MaterialDefinition> TranslateProceduralNames(List<MaterialDefinition> items)
    {
        foreach (var item in items)
        {
            if (item.mat_pair.mat_index < 0)
                continue; //It's a top level category.
            switch (item.mat_pair.mat_type)
            {
                case 13: //Instrument
                    {
                        var idParts = item.id.Split('/');
                        idParts[idParts.Length-1] = Regex.Replace(idParts[idParts.Length - 1].Split(' ').Last(), @"\d", "");
                        item.id = string.Join("/", idParts);
                    }
                    break;
                default:
                    break;
            }
        }
        return items;
    }

    static ItemRaws _instance = null;

    public List<MaterialDefinition> ItemList
    {
        set
        {
            _itemList = TranslateProceduralNames(value);
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
                _instance.PopulateLookupTable();
            }
            return _instance;
        }
        set
        {
            if (_instance != null && _instance != value)
                DestroyImmediate(_instance);
            _instance = value;
            _instance.PopulateLookupTable();
        }
    }

    public IEnumerable<MatPairStruct> Keys => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).Keys;

    public IEnumerable<MaterialDefinition> Values => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).Values;

    public int Count => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)itemLookup).Count;

    IEnumerable<string> IReadOnlyDictionary<string, MaterialDefinition>.Keys => ((IReadOnlyDictionary<string, MaterialDefinition>)stringLookup).Keys;

    public MaterialDefinition this[string key] => ((IReadOnlyDictionary<string, MaterialDefinition>)stringLookup)[key];


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

    public bool ContainsKey(string key)
    {
        return ((IReadOnlyDictionary<string, MaterialDefinition>)stringLookup).ContainsKey(key);
    }

    public bool TryGetValue(string key, out MaterialDefinition value)
    {
        return ((IReadOnlyDictionary<string, MaterialDefinition>)stringLookup).TryGetValue(key, out value);
    }

    IEnumerator<KeyValuePair<string, MaterialDefinition>> IEnumerable<KeyValuePair<string, MaterialDefinition>>.GetEnumerator()
    {
        return ((IReadOnlyDictionary<string, MaterialDefinition>)stringLookup).GetEnumerator();
    }

    public MaterialDefinition this[MatPairStruct mat]
    {
        get
        {
            return itemLookup[mat];
        }
    }
}
