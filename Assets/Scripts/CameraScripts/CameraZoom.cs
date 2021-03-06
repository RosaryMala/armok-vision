﻿using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class CameraZoom : MonoBehaviour
{

    public float zoomLevel = 2.0f;
    public float zoomSpeed = 0.1f;
    public float firstPersonThreshold = 1.0f;

    GameMap gameMap;

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
    }

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
            if (zoomLevel <= firstPersonThreshold)
            {
                zoomLevel = firstPersonThreshold;
                if (gameMap != null)
                    gameMap.firstPerson = true;
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, -Mathf.Pow(10, zoomLevel));
                if (gameMap != null)
                    gameMap.firstPerson = false;
            }
        }
#if !UNITY_STANDALONE_LINUX
        if(UnityEngine.XR.XRSettings.enabled)
        {
            Vector3 angles = transform.rotation.eulerAngles;
            angles = new Vector3(0, angles.y, 0);
            transform.rotation = Quaternion.Euler(angles);
        }
#endif
    }
}
