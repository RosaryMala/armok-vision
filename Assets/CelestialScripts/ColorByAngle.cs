using UnityEngine;

[RequireComponent(typeof(Light))]
public class ColorByAngle : MonoBehaviour {

    public float noonTemp = 5500;
    public float sunriseTemp = 2000;
    public float currentTemp;

    Light lightComponent;

    Quaternion prevAngle = Quaternion.identity;
    public float angleThreshold = 5;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update () 
    {
        if (Quaternion.Angle(prevAngle, transform.rotation) > angleThreshold)
        {
            prevAngle = transform.rotation;
            float angle = transform.rotation.eulerAngles.x;
            angle = Mathf.Sin(angle * Mathf.PI / 180);
            currentTemp = Mathf.Lerp(sunriseTemp, noonTemp, angle);
            lightComponent.color = ColorTemperature.Color(currentTemp);
            DynamicGI.UpdateEnvironment();
        }
    }
}
