using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSelector : MonoBehaviour
{

    public enum CameraOption
    {
        GodView,
        FirstPerson,
        VR,
    }

    public Camera ActualCamera;
    public Transform GodViewCamera;
    public Transform FirstPersonCamera;
    public Transform GodViewVRCamera;

    CameraOption currentCamera = CameraOption.GodView;
    public void ChangeCamera(CameraOption option)
    {
        Debug.Log("Changing to " + option);
        currentCamera = option;
        switch (option)
        {
            case CameraOption.GodView:
#if SteamVR
                if(ActualCamera.GetComponent<SteamVR_Camera>().enabled)
                {
                    ActualCamera.GetComponent<SteamVR_Camera>().enabled = false;
                    ActualCamera.GetComponent<SteamVR_Camera>().Collapse();
                }
#endif
                ChangeParent(GodViewCamera);
                break;
            case CameraOption.FirstPerson:
#if SteamVR
                if (ActualCamera.GetComponent<SteamVR_Camera>().enabled)
                {
                    ActualCamera.GetComponent<SteamVR_Camera>().enabled = false;
                    ActualCamera.GetComponent<SteamVR_Camera>().Collapse();
                }
#endif
                ChangeParent(FirstPersonCamera);
                break;
#if SteamVR
            case CameraOption.VR:
                ChangeParent(GodViewVRCamera);
                ActualCamera.GetComponent<SteamVR_Camera>().enabled = true;
                break;
#endif
            default:
                break;
        }
    }

    void CycleCamera()
    {
#if SteamVR
        if (SteamVR.active)
            return;
#endif
        switch (currentCamera)
        {
            case CameraOption.GodView:
                ChangeCamera(CameraOption.FirstPerson);
                break;
            case CameraOption.FirstPerson:
                ChangeCamera(CameraOption.GodView);
                break;
            default:
                ChangeCamera(CameraOption.GodView);
                break;
        }
    }

    void ChangeParent(Transform newSibling)
    {
        Transform oldRoot = FindParentWithTag(ActualCamera.transform, "CameraRig");
        Transform newRoot = FindParentWithTag(newSibling.transform, "CameraRig");
        if (oldRoot == null)
        {
            Debug.Log("Cannot find source CameraRig! Exiting.");
            return;
        }
        if (newRoot == null)
        {
            Debug.Log("Cannot find destination CameraRig! Exiting.");
            return;
        }

        newRoot.transform.position = oldRoot.transform.position;

        ActualCamera.transform.parent = newSibling.parent;
        ActualCamera.transform.localPosition = newSibling.localPosition;
        ActualCamera.transform.localRotation = newSibling.localRotation;
        ActualCamera.transform.localScale = newSibling.localScale;

        oldRoot.gameObject.SetActive(false);
        newRoot.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && EventSystem.current.currentSelectedGameObject == null)
            CycleCamera();
    }

    Transform FindParentWithTag(Transform trans, string tag)
    {
        for (; trans != null; trans = trans.parent)
        {
            if (trans.tag == tag)
                return trans;
        }
        return null;
    }

    public void Start()
    {
#if !UNITY_STANDALONE_LINUX
        if (UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR")
        {
            ChangeCamera(CameraOption.VR);
            Debug.Log("Started VR Mode");
        }
        else
#endif
        {
            ChangeCamera(CameraOption.GodView);
            Debug.Log("Started Monitor Mode");
        }
    }
}
