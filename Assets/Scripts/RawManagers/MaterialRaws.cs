﻿using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class MaterialRaws : IReadOnlyDictionary<MatPairStruct, MaterialDefinition>
{
    [SerializeField]
    List<MaterialDefinition> _materialList = new List<MaterialDefinition>();

    Dictionary<MatPairStruct, MaterialDefinition> materialLookup = new Dictionary<MatPairStruct, MaterialDefinition>();

    private void PopulateLookupTable()
    {
        MaterialTokenList.MaterialTokens = _materialList;
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
                _instance = new MaterialRaws();
                _instance.PopulateLookupTable();
            }
            return _instance;
        }
    }

    public IEnumerable<MatPairStruct> Keys => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).Keys;

    public IEnumerable<MaterialDefinition> Values => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).Values;

    public int Count => ((IReadOnlyDictionary<MatPairStruct, MaterialDefinition>)materialLookup).Count;


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
