using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class MaterialRaws : ScriptableObject, IReadOnlyDictionary<MatPairStruct, MaterialDefinition>
{
    [SerializeField]
    List<MaterialDefinition> _materialList;

    Dictionary<MatPairStruct, MaterialDefinition> materialLookup;

    private void PopulateLookupTable()
    {
        MaterialTokenList.MaterialTokens = _materialList;
        if (materialLookup == null)
            materialLookup = new Dictionary<MatPairStruct, MaterialDefinition>();
        materialLookup.Clear();
        foreach (var item in _materialList)
        {
            materialLookup[item.mat_pair] = item;
        }
    }

    static MaterialRaws _instance = null;

    public List<MaterialDefinition> MaterialList
    {
        set
        {
            _materialList = value;
            PopulateLookupTable();
        }
        get
        {
            return _materialList;
        }
    }


    public static MaterialRaws Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<MaterialRaws>("MaterialRaws");
                if (_instance == null)
                    _instance = CreateInstance<MaterialRaws>();
                _instance.PopulateLookupTable();
            }
            return _instance;
        }
    }

    public IEnumerable<MatPairStruct> Keys => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).Keys;

    public IEnumerable<MaterialDefinition> Values => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).Values;

    public int Count => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).Count;

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
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).ContainsKey(key);
    }

    public bool TryGetValue(MatPairStruct key, out MaterialDefinition value)
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<MatPairStruct, MaterialDefinition>> GetEnumerator()
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).GetEnumerator();
    }

    public MaterialDefinition this[MatPairStruct mat]
    {
        get
        {
            return materialLookup[mat];
        }
    }
}
