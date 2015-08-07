using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class ColorByAngle : MonoBehaviour {

    public float noonTemp = 5500;
    public float sunriseTemp = 2000;

    Light lightComponent;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update () 
    {
        float angle = transform.rotation.eulerAngles.x;
        angle /= 90;
        angle = Mathf.Clamp(angle, 0, 1);
        lightComponent.color = ColorTemperature.Color(Mathf.Lerp(sunriseTemp, noonTemp, angle));
    }
}
