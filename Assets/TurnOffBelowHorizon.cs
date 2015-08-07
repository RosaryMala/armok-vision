using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class TurnOffBelowHorizon : MonoBehaviour {

    public float angleRange = 1;

    float originalIntensity;
    Light lightComponent;

    void Awake()
    {
        lightComponent = GetComponent<Light>();
    }

    // Use this for initialization
    void Start () 
    {
        originalIntensity = lightComponent.intensity;
    }
    
    // Update is called once per frame
    void Update () {
        float x = transform.rotation.eulerAngles.x;

        if (x > 180)
            x -= 360;

        x += (angleRange / 2);
        x /= angleRange;
        x = Mathf.Clamp(x, 0, 1);
        //Debug.Log(this.name + " light=" + x + ", xrot=" + transform.rotation.eulerAngles.x);
        lightComponent.intensity = x * originalIntensity;
    }
}
