using RemoteFortressReader;
using UnityEngine;

namespace Building
{
    public class ItemPart : MonoBehaviour, IBuildingPart
    {
        public int itemIndex = -1;

        ItemModel item;
        MatPairStruct itemType = new MatPairStruct(-1, -1);

        public void UpdatePart(BuildingInstance buildingInstance)
        {
            if (itemIndex < 0)
                return;
            if (itemIndex >= buildingInstance.items.Count)
                return; // There isn't enough items for this part.

            var placedItem = buildingInstance.items[itemIndex];
            if (placedItem.item.type == itemType)
                return; // hasn't changed

            if (item != null)
                Destroy(item.gameObject);

            item = ItemManager.InstantiateItem(placedItem.item, transform, false);
        }
    }
}