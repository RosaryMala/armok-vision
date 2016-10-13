using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class TurnOffBelowHorizon : MonoBehaviour {

    public float angleRange = 1;

    float originalIntensity;
    Light lightComponent;

    public bool setFog = false;

    public Color dayFog;

    public Color nightFog;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
    }

    // Use this for initialization
    void Start () 
    {
        originalIntensity = lightComponent.intensity;
    }

    float prevIntensity = float.NaN;

    // Update is called once per frame
    void Update () {
        
        float x = -(90 - Vector3.Angle(transform.forward, Vector3.up));

        if (x > 180)
            x -= 360;

        x += (angleRange / 2);
        x /= angleRange;
        x = Mathf.Clamp(x, 0, 1);

        if (x != prevIntensity)
        {
            //Debug.Log(this.name + " light=" + x + ", xrot=" + transform.rotation.eulerAngles.x);
            lightComponent.intensity = x * originalIntensity;
            if(setFog)
            {
                RenderSettings.fogColor = Color.Lerp(nightFog, dayFog, x);
            }
            prevIntensity = x;
        }
    }
}
