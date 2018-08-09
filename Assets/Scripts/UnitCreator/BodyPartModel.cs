using UnityEngine;
using System.Collections.Generic;

public class BodyPartModel : MonoBehaviour
{
    public float canonVolume = 1;
    public float volume;

    internal void FixVolume()
    {
        if (volume == 0)
            volume = transform.localScale.x * transform.localScale.y * transform.localScale.z * canonVolume;
        float factor = transform.localScale.x * transform.localScale.y * transform.localScale.z;
        if (factor == 0)
            factor = 0.000001f;
        factor = volume / factor / canonVolume;
        transform.localScale *= Mathf.Pow(factor, 1 / 3.0f);
    }

    public Dictionary<MatPairStruct, ItemModel> Equipment { get; private set; }

    internal void CollectEquipment()
    {
        Equipment = new Dictionary<MatPairStruct, ItemModel>();
        foreach (var item in GetComponentsInChildren<ItemModel>())
        {
            Equipment[ItemRaws.Instance.FromToken(item.name).mat_pair] = item;
            item.gameObject.SetActive(false);
        }
    }
}
