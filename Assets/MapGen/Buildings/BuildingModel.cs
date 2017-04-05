using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingModel : MonoBehaviour
    {
        public RotationType rotationType;

        [SerializeField]
        RemoteFortressReader.BuildingInstance originalBuilding;

        BuildingRoom filler;

        private void Awake()
        {
            filler = GetComponent<BuildingRoom>();
        }

        public void Initialize(RemoteFortressReader.BuildingInstance buildingInput)
        {
            originalBuilding = buildingInput;

            if (filler != null)
            {
                filler.Initialize(buildingInput);
            }

            var parts = GetComponentsInChildren<BuildingPart>();
            foreach (var part in parts)
            {
                part.SetMaterial(buildingInput);
            }

            DFHack.DFCoord pos = new DFHack.DFCoord(
                (buildingInput.pos_x_min + buildingInput.pos_x_max) / 2,
                (buildingInput.pos_y_min + buildingInput.pos_y_max) / 2,
                buildingInput.pos_z_max);

            transform.localRotation = MeshContent.TranslateRotation(rotationType, MapDataStore.Main[pos]);

        }
    }
}
