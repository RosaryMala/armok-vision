using UnityEngine;
using UnityEngine.VR;
using UnityStandardAssets.Characters.FirstPerson;

public class CameraScale : MonoBehaviour
{
    public float zoomLevel = 2.0f;
    public float zoomSpeed = 0.1f;
    public float firstPersonThreshold = 1.0f;
    public float maxZoom = 3.5f;

    GameMap gameMap;
    public Transform viewCamera;
    new Camera camera;
    CharacterController charController;
    FirstPersonController fpsController;
    CameraRotate cameraRotate;
    CameraMovement cameraMovement;
    CameraRotateVertical cameraRotateVertical;
    public Camera steamVrCamera;

    void SetFirstPerson(bool value)
    {
        charController.enabled = value;
        fpsController.enabled = value;
        cameraRotate.enabled = !value;
        cameraMovement.enabled = !value;
        cameraRotateVertical.enabled = !value;
        if (value)
            cameraRotateVertical.transform.localRotation = Quaternion.identity;
        else
        {
            camera.transform.localRotation = Quaternion.identity;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
        charController = GetComponent<CharacterController>();
        fpsController = GetComponent<FirstPersonController>();
        cameraRotate = GetComponent<CameraRotate>();
        cameraMovement = GetComponent<CameraMovement>();
        cameraRotateVertical = GetComponentInChildren<CameraRotateVertical>();
        camera = Camera.main;
    }

    void Update()
    {
        HandleMouseRotation();
        if (gameMap != null && gameMap.firstPerson)
            gameMap.UpdateCenter(transform.position);

    }

    public void Start()
    {
        SetFirstPerson(false);
        if (SteamVR.active)
        {
            camera.gameObject.SetActive(false);
            steamVrCamera.gameObject.SetActive(true);
        }
        else
        {
            camera.gameObject.SetActive(true);
            steamVrCamera.gameObject.SetActive(false);
        }
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
                transform.localScale = Vector3.one;
                SetFirstPerson(true);
            }
            else
            {
                viewCamera.transform.localPosition = new Vector3(0, 0, -1.5f);
                transform.localScale = Vector3.one * Mathf.Pow(10, zoomLevel);
                if (gameMap != null)
                    gameMap.firstPerson = false;
                SetFirstPerson(false);
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
