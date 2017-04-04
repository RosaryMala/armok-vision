using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingModel : MonoBehaviour
    {
        public void Initialize(RemoteFortressReader.BuildingInstance buildingInput)
        {
            var parts = GetComponentsInChildren<BuildingPart>();
            foreach (var part in parts)
            {
                part.SetMaterial(buildingInput);
            }
        }
    }
}
