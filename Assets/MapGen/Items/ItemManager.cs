using DFHack;
using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    static Dictionary<MatPairStruct, ItemModel> itemPrefabs = new Dictionary<MatPairStruct, ItemModel>();

    public ItemModel defaultItem;

    IEnumerator LoadItems()
    {
        if (DFConnection.Instance.NetItemList == null)
            yield break;
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        var itemList = DFConnection.Instance.NetItemList.material_list;

        foreach (var item in itemList)
        {
            string path = "Items/" + item.id;
            var loadedItem = Resources.Load<ItemModel>(path);
            if(loadedItem == null)
            {
                //Debug.LogWarning("Cannot find model for " + item.id);
                if (stopWatch.ElapsedMilliseconds > 100)
                {
                    yield return null;
                    stopWatch.Reset();
                    stopWatch.Start();
                }
                continue;
            }

            itemPrefabs[item.mat_pair] = loadedItem;
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
        ContentLoader.RegisterLoadCallback(LoadItems);
    }

    private void LateUpdate()
    {
        UpdateVisibility();
    }

    Dictionary<int, ItemModel> sceneItems = new Dictionary<int, ItemModel>();
    Dictionary<DFCoord, int> itemCount = new Dictionary<DFCoord, int>();
    HashSet<int> removedItems = new HashSet<int>();
    bool loadedAnyItems = false;
    internal void BeginExistenceCheck()
    {
        removedItems.Clear();
        removedItems.UnionWith(sceneItems.Keys);
        itemCount.Clear();
    }

    internal void EndExitenceCheck()
    {
        if (!loadedAnyItems)
            return;
        foreach (var index in removedItems)
        {
            Destroy(sceneItems[index].gameObject);
            sceneItems.Remove(index);
        }
        loadedAnyItems = false;
    }

    internal void LoadBlock(MapBlock block)
    {
        if (block.items.Count > 0)
            loadedAnyItems = true;
        foreach (var item in block.items)
        {
            removedItems.Remove(item.id);
            ItemModel placedItem;
            if (!sceneItems.ContainsKey(item.id))
            {
                if (itemCount.ContainsKey(item.pos) && itemCount[item.pos] >= GameSettings.Instance.rendering.maxItemsPerTile)
                    continue;

                placedItem = InstantiateItem(item, transform);
                sceneItems[item.id] = placedItem;
            }
            else
                placedItem = sceneItems[item.id];

            if (itemCount.ContainsKey(item.pos) && itemCount[item.pos] >= GameSettings.Instance.rendering.maxItemsPerTile)
            {
                placedItem.gameObject.SetActive(false);
                continue;
            }
            int currentTileCount = 0;
            if (itemCount.ContainsKey(item.pos))
                currentTileCount = itemCount[item.pos];
            itemCount[item.pos] = currentTileCount + 1;
            //RaycastHit hitInfo;
            //Vector3 position = GameMap.DFtoUnityCoord(item.pos) + Stacker.SpiralHemisphere(currentTileCount);
            //if (Physics.Raycast(position + new Vector3(0, 2.9f, 0), Vector3.down, out hitInfo, 3, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            //    placedItem.transform.position = hitInfo.point;
            //else
            placedItem.transform.position = GameMap.DFtoUnityCoord(item.pos.x + item.subpos_x, item.pos.y + item.subpos_y, item.pos.z + item.subpos_z);
            if (item.projectile)
            {
                placedItem.transform.position += new Vector3(0, GameMap.tileHeight / 2, 0);
                if (item.velocity_x > 0 || item.velocity_y > 0 || item.velocity_z > 0)
                {
                    placedItem.transform.rotation = Quaternion.LookRotation(GameMap.DFtoUnityDirection(item.velocity_x, item.velocity_y, item.velocity_z), Vector3.up);
                }
            }
            else
            {
                placedItem.transform.position += (Stacker.SpiralHemisphere(currentTileCount) + new Vector3(0, GameMap.floorHeight, 0));
                if(item.velocity_x > 0 || item.velocity_y > 0 || item.velocity_z > 0)
                {
                    placedItem.transform.rotation = Quaternion.LookRotation(GameMap.DFtoUnityDirection(item.velocity_x, item.velocity_y, item.velocity_z), Vector3.up);
                }
            }
        }
    }

    public static ItemModel InstantiateItem(Item item, Transform parent, bool worldPositionStays = true)
    {
        MatPairStruct type = item.type;
        ItemModel placedItem;

        var prefab = Instance.defaultItem;

        if (!itemPrefabs.ContainsKey(type))
            type = new MatPairStruct(type.mat_type, -1);
        if (!itemPrefabs.ContainsKey(type))
            type = new MatPairStruct(-1, -1);
        if (itemPrefabs.ContainsKey(type))
            prefab = itemPrefabs[type];

        if(worldPositionStays)
            placedItem = Instantiate(prefab, GameMap.DFtoUnityCoord(item.pos), Quaternion.identity);
        else
            placedItem = Instantiate(prefab);

        if (GameMap.items.ContainsKey(item.type))
            placedItem.name = GameMap.items[item.type].id + "_" + item.id;

        placedItem.transform.SetParent(parent, worldPositionStays);
        placedItem.transform.parent = parent;
        placedItem.UpdateMaterial(item);
        return placedItem;
    }

    private void UpdateVisibility()
    {
        foreach (var item in sceneItems)
        {
            var itemInstance = item.Value.originalItem;
            item.Value.gameObject.SetActive(
                    (itemInstance.pos.z < (GameMap.Instance.firstPerson ? GameMap.Instance.PosZ + GameSettings.Instance.rendering.drawRangeUp : GameMap.Instance.PosZ))
                    && (itemInstance.pos.z >= (GameMap.Instance.PosZ - GameSettings.Instance.rendering.drawRangeDown))
                    && (itemInstance.pos.x / GameMap.blockSize > (GameMap.Instance.PosXBlock - GameSettings.Instance.rendering.drawRangeSide))
                    && (itemInstance.pos.x / GameMap.blockSize < (GameMap.Instance.PosXBlock + GameSettings.Instance.rendering.drawRangeSide))
                    && (itemInstance.pos.y / GameMap.blockSize > (GameMap.Instance.PosYBlock - GameSettings.Instance.rendering.drawRangeSide))
                    && (itemInstance.pos.y / GameMap.blockSize < (GameMap.Instance.PosYBlock + GameSettings.Instance.rendering.drawRangeSide))
                );
        }
    }
}
