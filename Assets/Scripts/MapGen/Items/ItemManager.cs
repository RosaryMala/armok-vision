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
            itemCache.Remove(index);
        }
        loadedAnyItems = false;
    }

    Dictionary<int, Item> itemCache = new Dictionary<int, Item>();

    internal void LoadBlock(MapBlock block)
    {
        if (block.items.Count > 0)
            loadedAnyItems = true;
        foreach (var item in block.items)
        {
            removedItems.Remove(item.id);

            itemCache[item.id] = item;
        }
    }

    void UpdateItem(Item item)
    {
        ItemModel placedItem;
        var itemPos = GameMap.DFtoUnityCoord(item.pos.x + item.subpos_x, item.pos.y + item.subpos_y, item.pos.z + item.subpos_z);

        //Disable the item if it's too far from the camera.
        if((itemPos - Camera.main.transform.position).sqrMagnitude > (GameSettings.Instance.rendering.itemDrawDistance * GameSettings.Instance.rendering.itemDrawDistance))
        {
            if (sceneItems.ContainsKey(item.id))
                sceneItems[item.id].gameObject.SetActive(false);
            return;
        }

        //Also disable if it's outside of the viewing AABB
        if(!((item.pos.z < (GameMap.Instance.firstPerson ? GameMap.Instance.PosZ + GameSettings.Instance.rendering.drawRangeUp : GameMap.Instance.PosZ))
                    && (item.pos.z >= (GameMap.Instance.PosZ - GameSettings.Instance.rendering.drawRangeDown))
                    && (item.pos.x / GameMap.blockSize > (GameMap.Instance.PosXBlock - GameSettings.Instance.rendering.drawRangeSide))
                    && (item.pos.x / GameMap.blockSize < (GameMap.Instance.PosXBlock + GameSettings.Instance.rendering.drawRangeSide))
                    && (item.pos.y / GameMap.blockSize > (GameMap.Instance.PosYBlock - GameSettings.Instance.rendering.drawRangeSide))
                    && (item.pos.y / GameMap.blockSize < (GameMap.Instance.PosYBlock + GameSettings.Instance.rendering.drawRangeSide))))
        {
            if (sceneItems.ContainsKey(item.id))
                sceneItems[item.id].gameObject.SetActive(false);
            return;
        }

        //Limit the number of items per tile.
        if (itemCount.ContainsKey(item.pos) && itemCount[item.pos] >= GameSettings.Instance.rendering.maxItemsPerTile)
        {
            if (sceneItems.ContainsKey(item.id))
                sceneItems[item.id].gameObject.SetActive(false);
            return;
        }

        //Instantiate it if it's not already there.
        if (!sceneItems.ContainsKey(item.id))
        {
            placedItem = InstantiateItem(item, transform);
            sceneItems[item.id] = placedItem;
        }
        else
        {
            placedItem = sceneItems[item.id];
            placedItem.gameObject.SetActive(true);

            //If there's no significant differences between the new item and the existing one, there's no need to update it.
            if (AreItemsEqual(placedItem.originalItem, item))
                return;
            else
                placedItem.originalItem = item;
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
        placedItem.transform.position = itemPos;
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
            if (item.velocity_x > 0 || item.velocity_y > 0 || item.velocity_z > 0)
            {
                placedItem.transform.rotation = Quaternion.LookRotation(GameMap.DFtoUnityDirection(item.velocity_x, item.velocity_y, item.velocity_z), Vector3.up);
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
        GameMap.BeginSample("Update Item Visibility", this);
        foreach (var item in itemCache)
        {
            UpdateItem(item.Value);
        }
        GameMap.EndSample();
    }

    //Roughly checks if any two DF items are equal.
    bool AreItemsEqual(Item a, Item b)
    {
        if (a.id != b.id)
            return false;
        if (((DfCoord)a.pos) != b.pos)
            return false;
        if (a.projectile != b.projectile)
            return false;
        if (a.subpos_x != b.subpos_x)
            return false;
        if (a.subpos_y != b.subpos_y)
            return false;
        if (a.subpos_z != b.subpos_z)
            return false;
        if (a.image != null)
        {
            if (b.image == null)
                return false;
            if (a.image.id != b.image.id)
                return false;
        }
        else if (b.image != null)
            return false;
        if (a.improvements != null)
        {
            if (b.improvements == null)
                return false;
            if (a.improvements.Count != b.improvements.Count)
                return false;
        }
        else if (b.improvements != null)
            return false;

        return true;
    }
}
