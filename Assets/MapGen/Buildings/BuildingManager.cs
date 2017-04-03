using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingManager : MonoBehaviour
    {
        public BuildingModel defaultBuilding;

        Dictionary<BuildingStruct, BuildingModel> buildings = new Dictionary<BuildingStruct, BuildingModel>();
        void LoadBuildings()
        {
            if (DFConnection.Instance.NetBuildingList == null)
                return;
            var buildingList = DFConnection.Instance.NetBuildingList.building_list;

            foreach (var building in buildingList)
            {
                var loadedBuilding = Resources.Load<BuildingModel>("Buildings/" + building.id);
                if (loadedBuilding != null)
                    Debug.Log("Loaded building: " + building.id);
            }
        }

        private void Start()
        {
            DFConnection.RegisterConnectionCallback(LoadBuildings);
        }
    }
}
