using UnityEngine;
using System.Collections;

public class SunRotate : MonoBehaviour {
    public float rotationSpeed = 1.0f;

    Light currentLight;

    void Awake()
    {
        currentLight = GetComponent<Light>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 rotation = new Vector3((Time.time * rotationSpeed) % 360, 0, 0);
        float brightness = rotation.x + 1;
        brightness /= 2.0f;
        if (brightness < 0)
            currentLight.intensity = 0;
        else if (brightness > 1)
            currentLight.intensity = 1;
        else
            currentLight.intensity = brightness;
        Quaternion rotate = Quaternion.Euler(rotation);
        transform.rotation = rotate;
	}
}
