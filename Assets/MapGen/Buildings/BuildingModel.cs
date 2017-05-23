using UnityEngine;

namespace Building
{
    public class BuildingModel : MonoBehaviour
    {
        public RotationType rotationType;

        public RemoteFortressReader.BuildingInstance originalBuilding;

        IBuildingPart[] parts;
        int items = -1;

        private void Awake()
        {
            parts = gameObject.GetInterfacesInChildren<IBuildingPart>();
        }

        public void Initialize(RemoteFortressReader.BuildingInstance buildingInput)
        {
            originalBuilding = buildingInput;


            if (buildingInput.items.Count != items)
            {
                foreach (var part in parts)
                {
                    part.UpdatePart(buildingInput);
                }
                items = buildingInput.items.Count;
            }

            DFHack.DFCoord pos = new DFHack.DFCoord(
                (buildingInput.pos_x_min + buildingInput.pos_x_max) / 2,
                (buildingInput.pos_y_min + buildingInput.pos_y_max) / 2,
                buildingInput.pos_z_max);

            if(MapDataStore.Main[pos] != null)
                transform.localRotation = MeshContent.TranslateRotation(rotationType, MapDataStore.Main[pos]);

        }
    }
}
