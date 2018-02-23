using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

namespace Building
{
    public class ImagePart : MonoBehaviour, IBuildingPart
    {
        public int itemIndex = 0;

        MatPairStruct itemType = new MatPairStruct(-1, -1);

        MeshFilter meshFilter;

        public void UpdatePart(BuildingInstance buildingInstance)
        {
            if (itemIndex < 0)
            {
                gameObject.SetActive(false);
                return;
            }
            if (itemIndex >= buildingInstance.items.Count)
            {
                gameObject.SetActive(false);
                return;
            }
            // There isn't enough items for this part.
            if (buildingInstance.items[itemIndex].item.image == null)
            {
                gameObject.SetActive(false);
                return;
            }

            var placedItem = buildingInstance.items[itemIndex];
            if (placedItem.item.type == itemType)
                return; // hasn't changed

            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

            meshFilter.mesh = ImageManager.Instance.CreateMesh(buildingInstance.items[itemIndex].item.image, ImageManager.Direction.Front);
        }
    }
}
