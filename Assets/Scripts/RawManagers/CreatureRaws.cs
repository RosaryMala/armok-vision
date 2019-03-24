using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class CreatureRaws : ScriptableObject, IReadOnlyList<CreatureRaw>
{
    [SerializeField]
    List<CreatureRaw> _creatureList = new List<CreatureRaw>();


    static CreatureRaws _instance = null;

    public List<CreatureRaw> CreatureList
    {
        set
        {
            _creatureList = value;
        }
        get
        {
            return _creatureList;
        }
    }


    public static CreatureRaws Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<CreatureRaws>("CreatureRaws");
                if (_instance == null)
                    _instance = CreateInstance<CreatureRaws>();
            }
            return _instance;
        }
        set
        {
            if (_instance != null && _instance != value)
                DestroyImmediate(_instance);
            _instance = value;
        }
    }

    public int Count => ((IReadOnlyList<CreatureRaw>)_creatureList).Count;

    public CreatureRaw this[int index] => ((IReadOnlyList<CreatureRaw>)_creatureList)[index];


    public IEnumerator<CreatureRaw> GetEnumerator()
    {
        return ((IReadOnlyList<CreatureRaw>)_creatureList).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IReadOnlyList<CreatureRaw>)_creatureList).GetEnumerator();
    }
}
