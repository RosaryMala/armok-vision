using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    Dictionary<MatPairStruct, ItemModel> itemPrefabs = new Dictionary<MatPairStruct, ItemModel>();

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
                Debug.LogWarning("Cannot find model for " + item.id);
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
}
