using UnityEngine;
using System.Collections;
using System;
using UnityEngine.VR;

public class CameraScale : MonoBehaviour
{
    public float zoomLevel = 2.0f;
    public float zoomSpeed = 0.1f;
    public float firstPersonThreshold = 1.0f;
    public float maxZoom = 3.5f;

    GameMap gameMap;
    public Transform viewCamera;

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
    }

    void Update()
    {
        HandleMouseRotation();
    }

    private void HandleMouseRotation()
    {
        //horizontal rotation
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            if (zoomLevel > maxZoom)
                zoomLevel = maxZoom;
            if (zoomLevel <= firstPersonThreshold)
            {
                zoomLevel = firstPersonThreshold;
                if (gameMap != null)
                    gameMap.firstPerson = true;
                viewCamera.transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one * Mathf.Pow(10, zoomLevel);
            }
            else
            {
                viewCamera.transform.localPosition = new Vector3(0, 0, -1.5f);
                transform.localScale = Vector3.one * Mathf.Pow(10, zoomLevel);
                if (gameMap != null)
                    gameMap.firstPerson = false;
            }
        }

        if (VRSettings.enabled)
        {
            Vector3 angles = transform.rotation.eulerAngles;
            angles = new Vector3(0, angles.y, 0);
            viewCamera.transform.rotation = Quaternion.Euler(angles);
        }

    }


}
