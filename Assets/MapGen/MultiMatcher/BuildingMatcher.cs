using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class BuildingMatcher<T> {
    Dictionary<BuildingStruct, T> buildingList;
    public T this[string token]
    {
        set
        {
            if (!BuildingTokenList.BuildingLookup.ContainsKey(token))
            {
                Debug.Log("Invalid building: " + token);
                return;
            }
            if (buildingList == null)
                buildingList = new Dictionary<BuildingStruct, T>();
            buildingList[BuildingTokenList.BuildingLookup[token].building_type] = value;
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
            bld.building_custom = -1; //Try once more with a more generic value.
            if (buildingList.TryGetValue(bld, out output))
            {
                value = output;
                return true;
            }
            bld.building_subtype = -1; //and even more generic
            if (buildingList.TryGetValue(bld, out output))
            {
                value = output;
                return true;
            }
            bld.building_type = -1; //and finally check if there's a universal default set.
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
