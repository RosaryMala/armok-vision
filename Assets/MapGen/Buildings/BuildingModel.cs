using UnityEngine;

namespace Building
{
    public class BuildingModel : MonoBehaviour
    {
        public RotationType rotationType;

        public RemoteFortressReader.BuildingInstance originalBuilding;

        IBuildingPart[] parts;

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
        }
    }
}
