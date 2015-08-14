using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class ColorByAngle : MonoBehaviour {

    public float noonTemp = 5500;
    public float sunriseTemp = 2000;
    public float currentTemp;

    Light lightComponent;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update () 
    {
        float angle = transform.rotation.eulerAngles.x;
        angle = Mathf.Sin(angle * Mathf.PI / 180);
        currentTemp = Mathf.Lerp(sunriseTemp, noonTemp, angle);
        lightComponent.color = ColorTemperature.Color(currentTemp);
    }
}
