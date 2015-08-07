using UnityEngine;
using System.Collections;

public class SunRotate : MonoBehaviour {
    public float rotationSpeed = 1.0f;
	
	// Update is called once per frame
	void Update () {
        Vector3 rotation = new Vector3((Time.time * rotationSpeed) % 360, 0, 0);
        Quaternion rotate = Quaternion.Euler(rotation);
        transform.rotation = rotate;
	}
}
