using System;
using RemoteFortressReader;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Building
{
    public class BuildingModel : MonoBehaviour
    {
        public RotationType rotationType;

        public RemoteFortressReader.BuildingInstance originalBuilding;

        IBuildingPart[] parts;
        private Matrix4x4[] tilePositions;
        private List<Collider> selectionColliders = new List<Collider>();

        private void Awake()
        {
            parts = gameObject.GetInterfacesInChildren<IBuildingPart>();
        }

        public void Initialize(RemoteFortressReader.BuildingInstance buildingInput)
        {
            if (originalBuilding != null
                && originalBuilding.active == buildingInput.active
                && originalBuilding.items.Count == buildingInput.items.Count
                && originalBuilding.pos_x_min == buildingInput.pos_x_min
                && originalBuilding.pos_y_min == buildingInput.pos_y_min)
                return; //There's nothing changed.

            originalBuilding = buildingInput;

            foreach (var part in parts)
            {
                part.UpdatePart(buildingInput);
            }

            DFHack.DFCoord pos = new DFHack.DFCoord(
                (buildingInput.pos_x_min + buildingInput.pos_x_max) / 2,
                (buildingInput.pos_y_min + buildingInput.pos_y_max) / 2,
                buildingInput.pos_z_max);

            if (MapDataStore.Main[pos] != null && rotationType != RotationType.BuildingDirection)
                transform.localRotation = MeshContent.TranslateRotation(rotationType, MapDataStore.Main[pos]);

            var group = GetComponent<LODGroup>();
            if (group == null)
            {
                group = gameObject.AddComponent<LODGroup>();
                var lods = new LOD[1];
                lods[0] = new LOD(0.05f, GetComponentsInChildren<MeshRenderer>());
                group.SetLODs(lods);
                group.RecalculateBounds();
            }

            UpdateTilePositions(buildingInput);

            foreach (var item in GetComponentsInChildren<Collider>())
            {
                if (item.GetComponent<BuildingSelect>() == null)
                {
                    item.gameObject.AddComponent<BuildingSelect>().root = this;
                }
            }
        }

        private void UpdateTilePositions(BuildingInstance buildingInput)
        {
            ClearSelectionColliders();
            List<Vector3> transformList = new List<Vector3>();

            var room = buildingInput.room;
            if (room == null || room.extents.Count == 0 || buildingInput.is_room)
            {
                for (int x = buildingInput.pos_x_min; x <= buildingInput.pos_x_max; x++)
                    for (int y = buildingInput.pos_y_min; y <= buildingInput.pos_y_max; y++)
                    {
                        transformList.Add(GameMap.DFtoUnityCoord(x,y,buildingInput.pos_z_max));
                    }
            }
            else
            {
                for (int y = 0; y < room.height; y++)
                    for (int x = 0; x < room.width; x++)
                    {
                        var set = room.extents[x + y * room.width];
                        if (set == 0)
                            continue;
                        transformList.Add(GameMap.DFtoUnityCoord(room.pos_x + x, room.pos_y + y, buildingInput.pos_z_max));
                    }
            }
            tilePositions = new Matrix4x4[transformList.Count];
            for (int i = 0; i < transformList.Count; i++)
            {
                Instantiate(BuildingManager.Instance.selectionFloor, transformList[i], Quaternion.identity, transform);
                tilePositions[i] = Matrix4x4.Translate(transformList[i]);
            }
        }

        internal void DrawSelection()
        {
            if (tilePositions != null)
            {
                if(tilePositions.Length > 1023)
                    Graphics.DrawMeshInstanced(BuildingManager.Instance.selectionMesh, 0, BuildingManager.Instance.selectionMaterial, tilePositions.Take(1023).ToArray());
                else
                    Graphics.DrawMeshInstanced(BuildingManager.Instance.selectionMesh, 0, BuildingManager.Instance.selectionMaterial, tilePositions);
            }
        }

        internal void PrintInfo()
        {
            Debug.Log(GameMap.buildings[originalBuilding.building_type].id);
        }

        private void ClearSelectionColliders()
        {
            foreach (var item in selectionColliders)
            {
                Destroy(item);
            }
            selectionColliders.Clear();
        }
    }
}
