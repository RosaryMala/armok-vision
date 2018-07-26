using UnityEngine;

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
}
