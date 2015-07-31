using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

    public float zoomLevel = 2.0f;

    public float zoomSpeed = 0.1f;

    
    void Update()
    {
        HandleMouseRotation();
    }

    public void HandleMouseRotation()
    {

        //horizontal rotation
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            transform.localPosition = new Vector3(0, 0, -Mathf.Pow(10,zoomLevel));
        }
    }
}
