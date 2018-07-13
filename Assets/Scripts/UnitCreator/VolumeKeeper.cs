using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VolumeKeeper : MonoBehaviour
{
    public float volume = 0;

#if UNITY_EDITOR
    private void Update()
    {
        if (transform.hasChanged)
        {
            FixVolume();
            transform.hasChanged = false;
        }
    }

    private void OnValidate()
    {
        FixVolume();
    }
#endif

    public void FixVolume()
    {
        if (volume == 0)
            volume = transform.localScale.x * transform.localScale.y * transform.localScale.z * 100 * 100 * 100;
        float factor = transform.localScale.x * transform.localScale.y * transform.localScale.z;
        if (factor == 0)
            factor = 0.000001f;
        factor = volume / factor / 1000000;
        transform.localScale *= Mathf.Pow(factor, 1 / 3.0f);
    }
}
