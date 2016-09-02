using UnityEngine;
using UnityEngine.VR;
public class CameraSelector : MonoBehaviour
{

    public enum CameraOption
    {
        GodView,
        FirstPerson,
        GodViewVR,
        FirstPersonVR
    }

    public Camera ActualCamera;
    public Transform GodViewCamera;
    public Transform FirstPersonCamera;
    public Transform GodViewVRCamera;

    CameraOption currentCamera = CameraOption.GodView;
    public void ChangeCamera(CameraOption option)
    {
        currentCamera = option;
        switch (option)
        {
            case CameraOption.GodView:
                if(ActualCamera.GetComponent<SteamVR_Camera>().enabled)
                {
                    ActualCamera.GetComponent<SteamVR_Camera>().enabled = false;
                    ActualCamera.GetComponent<SteamVR_Camera>().Collapse();
                }
                ChangeParent(GodViewCamera);
                break;
            case CameraOption.FirstPerson:
                if (ActualCamera.GetComponent<SteamVR_Camera>().enabled)
                {
                    ActualCamera.GetComponent<SteamVR_Camera>().enabled = false;
                    ActualCamera.GetComponent<SteamVR_Camera>().Collapse();
                }
                ChangeParent(FirstPersonCamera);
                break;
            case CameraOption.GodViewVR:
                ChangeParent(GodViewVRCamera);
                ActualCamera.GetComponent<SteamVR_Camera>().enabled = true;
                break;
            case CameraOption.FirstPersonVR:
                break;
            default:
                break;
        }
    }

    void CycleCamera()
    {
        if (SteamVR.active)
            return;
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
        if (oldRoot == null || newRoot == null)
            return;

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
        if (Input.GetKeyDown(KeyCode.C))
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
        if (VRSettings.loadedDeviceName == "OpenVR")
        {
            ChangeCamera(CameraOption.GodViewVR);
            Debug.Log("Started VR Mode");
        }
        else
            ChangeCamera(CameraOption.GodView);
    }
}
