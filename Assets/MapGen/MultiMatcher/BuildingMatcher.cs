using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class BuildingMatcher<T> {
    Dictionary<BuildingStruct, T> buildingList;
    public T this[string token]
    {
        set
        {
            BuildingStruct bld = new BuildingStruct();
            if (token == "*")
            {
                bld = new BuildingStruct(-1, -1, -1);
            }
            else
            {
                if (!BuildingTokenList.BuildingLookup.ContainsKey(token))
                {
                    Debug.Log("Invalid building: " + token);
                    return;
                }
                bld = BuildingTokenList.BuildingLookup[token].BuildingType;
            }
            if (buildingList == null)
                buildingList = new Dictionary<BuildingStruct, T>();
            buildingList[bld] = value;
        }
    }
    public T this[BuildingStruct bld]
    {
        set
        {
            if (buildingList == null)
                buildingList = new Dictionary<BuildingStruct, T>();
            buildingList[bld] = value;
        }
    }
    public bool Get(BuildingStruct bld, out T value)
    {
        if (buildingList != null)
        {
            T output;
            if (buildingList.TryGetValue(bld, out output))
            {
                value = output;
                return true;
            }
            bld = new BuildingStruct(bld.building_type, bld.building_subtype, -1);//Try once more with a more generic value.
            if (buildingList.TryGetValue(bld, out output))
            {
                value = output;
                return true;
            }
            bld = new BuildingStruct(bld.building_type, -1, -1); //and even more generic
            if (buildingList.TryGetValue(bld, out output))
            {
                value = output;
                return true;
            }
            bld = new BuildingStruct(-1, -1, -1); //and finally check if there's a universal default set.
            if (buildingList.TryGetValue(bld, out output))
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
        buildingList.Clear();
    }

}
