using DFHack;
using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Building
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        public BuildingModel defaultBuilding;

        public Mesh selectionMesh;
        public Material selectionMaterial;
        public Collider selectionFloor;

        Dictionary<BuildingStruct, BuildingModel> buildingPrefabs = new Dictionary<BuildingStruct, BuildingModel>();

        Dictionary<DFCoord, BuildingInstance> buildingInfoMap = new Dictionary<DFCoord, BuildingInstance>();

        public static BuildingInstance GetBuildingInfo(DFCoord pos)
        {
            if (Instance == null)
                return null;
            if (!Instance.buildingInfoMap.ContainsKey(pos))
                return null;
            return Instance.buildingInfoMap[pos];
        }

        IEnumerator LoadBuildings()
        {
            if (DFConnection.Instance.NetBuildingList == null)
                yield break;
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            var buildingList = DFConnection.Instance.NetBuildingList.building_list;

            foreach (var building in buildingList)
            {
                string path = "Buildings/" + building.id;
                GameMap.BeginSample(path);
                var loadedBuilding = Resources.Load<BuildingModel>(path);
                if (loadedBuilding == null)
                {
                    Debug.LogWarning("Cannot find model for " + building.id);
                    GameMap.EndSample();
                    if (stopWatch.ElapsedMilliseconds > 100)
                    {
                        yield return null;
                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                    continue;
                }

                buildingPrefabs[building.building_type] = loadedBuilding;
                GameMap.EndSample();
                if (stopWatch.ElapsedMilliseconds > 100)
                {
                    yield return null;
                    stopWatch.Reset();
                    stopWatch.Start();
                }
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ContentLoader.RegisterLoadCallback(LoadBuildings);
        }

        private void LateUpdate()
        {
            UpdateVisibility();
        }

        Dictionary<int, BuildingModel> sceneBuildings = new Dictionary<int, BuildingModel>();
        HashSet<int> removedBuildings = new HashSet<int>();
        bool loadedAnyBuildngs = false;
        internal void BeginExistenceCheck()
        {
            removedBuildings.Clear();
            removedBuildings.UnionWith(sceneBuildings.Keys);
        }

        internal void EndExistenceCheck()
        {
            if (!loadedAnyBuildngs)
                return;
            foreach (var index in removedBuildings)
            {
                Destroy(sceneBuildings[index].gameObject);
                sceneBuildings.Remove(index);
            }
            loadedAnyBuildngs = false;
        }

        internal void LoadBlock(MapBlock block)
        {
            if (block.buildings.Count > 0)
                loadedAnyBuildngs = true;
            foreach (var building in block.buildings)
            {
                removedBuildings.Remove(building.index);
                if (building.pos_x_max == default(int)
                    && building.pos_x_min == default(int)
                    && building.pos_y_max == default(int)
                    && building.pos_y_min == default(int)
                    && building.pos_z_max == default(int)
                    && building.pos_z_min == default(int))
                    continue;
                if (building.building_type.building_type == 30)//Civzone
                    continue;
                BuildingModel builtBuilding;
                if (!sceneBuildings.ContainsKey(building.index))
                {
                    Profiler.BeginSample("Init Building " + building.index);
                    BuildingStruct type = building.building_type;

                    DFCoord origin = new DFCoord(
                        (building.pos_x_min + building.pos_x_max) / 2,
                        (building.pos_y_min + building.pos_y_max) / 2,
                        building.pos_z_max);

                    if (type.building_type == 19) //Bridge
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

                    if (!buildingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(type.building_type, type.building_subtype, -1);
                    if (!buildingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(type.building_type, -1, -1);
                    if (!buildingPrefabs.ContainsKey(type))
                        type = new BuildingStruct(-1, -1, -1);
                    if (buildingPrefabs.ContainsKey(type))
                        builtBuilding = Instantiate(buildingPrefabs[type], GameMap.DFtoUnityCoord(origin), TranslateDirection(building.direction), transform);
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

                StoreBuildingInfo(building);
            }
        }

        void UpdateVisibility()
        {
            foreach (var item in sceneBuildings)
            {
                var building = item.Value.originalBuilding;
                item.Value.gameObject.SetActive(
                    (building.pos_z_min < (GameMap.Instance.firstPerson ? GameMap.Instance.PosZ + GameSettings.Instance.rendering.drawRangeUp : GameMap.Instance.PosZ))
                    && (building.pos_z_max >= (GameMap.Instance.PosZ - GameSettings.Instance.rendering.drawRangeDown))
                    && (building.pos_x_max / GameMap.blockSize > (GameMap.Instance.PosXBlock - GameSettings.Instance.rendering.drawRangeSide))
                    && (building.pos_x_min / GameMap.blockSize < (GameMap.Instance.PosXBlock + GameSettings.Instance.rendering.drawRangeSide))
                    && (building.pos_y_max / GameMap.blockSize > (GameMap.Instance.PosYBlock - GameSettings.Instance.rendering.drawRangeSide))
                    && (building.pos_y_min / GameMap.blockSize < (GameMap.Instance.PosYBlock + GameSettings.Instance.rendering.drawRangeSide))
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

        void StoreBuildingInfo(BuildingInstance building)
        {
            if (building.building_type.building_type == 30)
                return; // We won't do civzones right now.
            for (int zz = building.pos_z_min; zz <= building.pos_z_max; zz++)
                for (int yy = building.pos_y_min; yy <= building.pos_y_max; yy++)
                    for (int xx = building.pos_x_min; xx <= building.pos_x_max; xx++)
                    {

                        if ((building.building_type.building_type == 29)
                        && building.room != null && building.room.extents.Count > 0)
                        {
                            int buildingLocalX = xx - building.room.pos_x;
                            int buildingLocalY = yy - building.room.pos_y;

                            if (building.room.extents[buildingLocalY * building.room.width + buildingLocalX] == 0)
                                continue;
                        }
                        buildingInfoMap[new DFCoord(xx, yy, zz)] = building;
                    }

        }

        public string GetBuildingInfoText(DFCoord pos)
        {
            if (!buildingInfoMap.ContainsKey(pos))
                return "";
            if (buildingInfoMap[pos] == null)
                return "";

            var building = buildingInfoMap[pos];

            StringBuilder statusText = new StringBuilder();

            statusText.Append("Building: ");
            if (GameMap.buildings.ContainsKey(building.building_type))
                statusText.Append(GameMap.buildings[building.building_type].id).AppendLine();
            else
                statusText.Append(building.building_type).AppendLine();

            if (GameMap.materials.ContainsKey(building.material))
            {
                statusText.Append("Building Material: ");
                statusText.Append(GameMap.materials[building.material].id).AppendLine();
            }
            else
                statusText.Append("Unknown Building Material\n");

            if (building.items != null && building.items.Count > 0)
            {
                statusText.Append("Building items:").AppendLine();
                for(int i = 0; i < building.items.Count && i < 10; i++)
                {
                    var item = building.items[i];
                    if (GameMap.materials.ContainsKey(item.item.material))
                        statusText.Append(GameMap.materials[item.item.material].id).Append(" ");
                    if (GameMap.items.ContainsKey(item.item.type))
                        statusText.Append(GameMap.items[item.item.type].id);
                    else
                        statusText.Append(item.item.type);
                    statusText.Append(" [").Append(item.mode).Append("]").AppendLine();
                }
                if (building.items.Count > 10)
                    statusText.Append("+ ").Append(building.items.Count - 10).Append(" more...");
                statusText.AppendLine();
            }
            statusText.AppendLine();
            return statusText.ToString();
        }
    }
}
