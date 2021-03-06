﻿using DFHack;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    GameMap gameMap;
    public Transform cameraPos;
    public bool following = true;
    public float followSpeed = 0.5f;
    public float followSnapDistance = 10;
    public float speed = 10;
    public float minDistance = 5;
    public float fasterMultiplier = 10;
    public float upDownSpeed = 5;

    public CameraScale cameraScale;

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("FollowDF") && !GameSettings.Instance.game.showDFScreen)
        {
            following = true;
        }
        if ((following || GameSettings.Instance.game.showDFScreen) && gameMap != null)
        {
            Vector3 goal = GameMap.DFtoUnityTileCenter(GameMap.Instance.FollowPos);
            Vector3 diff = goal - transform.position;
            if (diff.sqrMagnitude > followSnapDistance * followSnapDistance)
            {
                transform.position = goal;
            }
            else
            {
                float interp = Time.unscaledDeltaTime * diff.magnitude * followSpeed;
                if (interp > 1)
                {
                    interp = 1;
                }
                transform.Translate(diff * interp, Space.World);
            }
        }
        if (GameSettings.Instance.game.showDFScreen || EventSystem.current.currentSelectedGameObject != null || DFConnection.Instance.WorldMode == dfproto.GetWorldInfoOut.Mode.MODE_ADVENTURE)
            return;

        float moveZ = Input.GetAxisRaw("CamUpDown");
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");


        if (moveZ != 0.0f || moveY != 0.0f || moveX != 0.0f)
        {
            following = false;
            float cameraDistance = Mathf.Pow(10, cameraScale.zoomLevel);
            if (cameraDistance < minDistance)
                cameraDistance = minDistance;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                cameraDistance *= fasterMultiplier;
            Vector3 movement = new Vector3(moveX * cameraDistance, moveZ * upDownSpeed, moveY * cameraDistance);
            transform.Translate(movement * Time.unscaledDeltaTime * speed, Space.Self);
        }
    }

    public void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
