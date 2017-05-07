using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

namespace Building
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        public BuildingModel defaultBuilding;

        Dictionary<BuildingStruct, BuildingModel> buidlingPrefabs = new Dictionary<BuildingStruct, BuildingModel>();
        void LoadBuildings()
        {
            if (DFConnection.Instance.NetBuildingList == null)
                return;
            var buildingList = DFConnection.Instance.NetBuildingList.building_list;

            foreach (var building in buildingList)
            {
                string path = "Buildings/" + building.id;
                var loadedBuilding = Resources.Load<BuildingModel>(path);
                if (loadedBuilding == null)
                    continue;

                buidlingPrefabs[building.building_type] = loadedBuilding;

                Debug.Log("Loaded building: " + path);

            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            DFConnection.RegisterConnectionCallback(LoadBuildings);
        }

        private void Update()
        {
            UpdateVisibility();
        }

        Dictionary<int, BuildingModel> sceneBuildings = new Dictionary<int, BuildingModel>();

        internal void LoadBlock(RemoteFortressReader.MapBlock block)
        {
            foreach (var building in block.buildings)
            {
                BuildingModel builtBuilding;
                if (!sceneBuildings.ContainsKey(building.index))
                {
                    BuildingStruct type = building.building_type;
                    if (!buidlingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(type.building_type, type.building_subtype, -1);
                    if (!buidlingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(type.building_type, -1, -1);
                    if (!buidlingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(-1, -1, -1);
                    if (buidlingPrefabs.ContainsKey(type))
                        builtBuilding = Instantiate(buidlingPrefabs[type],
                            GameMap.DFtoUnityCoord(
                                (building.pos_x_min + building.pos_x_max) / 2,
                                (building.pos_y_min + building.pos_y_max) / 2,
                                building.pos_z_max),
                            Quaternion.identity, transform);
                    else
                        builtBuilding = Instantiate(defaultBuilding,
                            GameMap.DFtoUnityCoord(
                                (building.pos_x_min + building.pos_x_max) / 2,
                                (building.pos_y_min + building.pos_y_max) / 2,
                                building.pos_z_max),
                            Quaternion.identity, transform);

                    sceneBuildings[building.index] = builtBuilding;
                    builtBuilding.Initialize(building);
                }
                else
                    builtBuilding = sceneBuildings[building.index];
            }
        }

        void UpdateVisibility()
        {
            foreach (var item in sceneBuildings)
            {
                var building = item.Value.originalBuilding;
                item.Value.gameObject.SetActive(
                    (building.pos_z_min < GameMap.Instance.PosZ)
                    && (building.pos_z_max >= (GameMap.Instance.PosZ - GameSettings.Instance.rendering.drawRangeDown))
                    );
            }
        }
    }
}
