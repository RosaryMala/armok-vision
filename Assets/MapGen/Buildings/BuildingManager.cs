using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;
using UnityEngine.Profiling;
using DFHack;

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
                    Profiler.BeginSample("Init Building " + building.index);
                    BuildingStruct type = building.building_type;

                    DFCoord origin = new DFCoord(
                        (building.pos_x_min + building.pos_x_max) / 2,
                        (building.pos_y_min + building.pos_y_max) / 2,
                        building.pos_z_max);

                    if(type.building_type == 19) //Bridge
                    {
                        switch (building.direction)
                        {
                            case BuildingDirection.NORTH:
                                origin = new DFCoord(
                                    (building.pos_x_min + building.pos_x_max) / 2,
                                    building.pos_y_min,
                                    building.pos_z_max);
                                break;
                            case BuildingDirection.EAST:
                                origin = new DFCoord(
                                    building.pos_x_max,
                                    (building.pos_y_min + building.pos_y_max) / 2,
                                    building.pos_z_max);
                                break;
                            case BuildingDirection.SOUTH:
                                origin = new DFCoord(
                                    (building.pos_x_min + building.pos_x_max) / 2,
                                    building.pos_y_max,
                                    building.pos_z_max);
                                break;
                            case BuildingDirection.WEST:
                                origin = new DFCoord(
                                    building.pos_x_min,
                                    (building.pos_y_min + building.pos_y_max) / 2,
                                    building.pos_z_max);
                                break;
                            case BuildingDirection.NONE:
                                break;
                            default:
                                break;
                        }
                    }

                    if (!buidlingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(type.building_type, type.building_subtype, -1);
                    if (!buidlingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(type.building_type, -1, -1);
                    if (!buidlingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(-1, -1, -1);
                    if (buidlingPrefabs.ContainsKey(type))
                        builtBuilding = Instantiate(buidlingPrefabs[type], GameMap.DFtoUnityCoord(origin), TranslateDirection(building.direction), transform);
                    else
                        builtBuilding = Instantiate(defaultBuilding, GameMap.DFtoUnityCoord(origin), TranslateDirection(building.direction), transform);

                    sceneBuildings[building.index] = builtBuilding;
                    Profiler.EndSample();
                }
                else
                    builtBuilding = sceneBuildings[building.index];
                Profiler.BeginSample("BuildingModel.Initialize");
                builtBuilding.Initialize(building);
                Profiler.EndSample();
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

        public static Quaternion TranslateDirection(BuildingDirection direction)
        {
            switch (direction)
            {
                case BuildingDirection.NORTH:
                    return Quaternion.Euler(0, 0, 0);
                case BuildingDirection.EAST:
                    return Quaternion.Euler(0, 90, 0);
                case BuildingDirection.SOUTH:
                    return Quaternion.Euler(0, 180, 0);
                case BuildingDirection.WEST:
                    return Quaternion.Euler(0, -90, 0);
                default:
                    return Quaternion.identity;
            }
        }
    }
}
