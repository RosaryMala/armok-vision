using System;
using System.Collections.Generic;
using RemoteFortressReader;
using TokenLists;

public class CreatureRaceMatcher<T>
{

    Dictionary<MatPairStruct, T> creatureRaceList = new Dictionary<MatPairStruct, T>();

    public T this[MatPairStruct caste]
    {
        set
        {
            if (creatureRaceList == null)
                creatureRaceList = new Dictionary<MatPairStruct, T>();
            creatureRaceList[caste] = value;
        }
        get
        {
            return creatureRaceList[caste];
        }
    }
    public bool TryGetValue(MatPairStruct caste, out T value)
    {
        if (creatureRaceList != null)
        {
            if (creatureRaceList.TryGetValue(caste, out value))
            {
                return true;
            }
            caste = new MatPairStruct(caste.mat_type, -1);
            if (creatureRaceList.TryGetValue(caste, out value))
            {
                return true;
            }
        }
        value = default(T);
        return false;
    }
    public bool ContainsKey(MatPairStruct val)
    {
        return creatureRaceList.ContainsKey(val);
    }
    public void Clear()
    {
        creatureRaceList.Clear();
    }
}
