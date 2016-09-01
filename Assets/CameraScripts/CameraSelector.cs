using UnityEngine;
using System.Collections;

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


    CameraOption currentCamera = CameraOption.GodView;
    public void ChangeCamera(CameraOption option)
    {
        currentCamera = option;
        switch (option)
        {
            case CameraOption.GodView:
                ChangeParent(GodViewCamera);
                break;
            case CameraOption.FirstPerson:
                ChangeParent(FirstPersonCamera);
                break;
            case CameraOption.GodViewVR:
                break;
            case CameraOption.FirstPersonVR:
                break;
            default:
                break;
        }
    }

    void CycleCamera()
    {
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

        oldRoot.gameObject.SetActive(false);
        newRoot.gameObject.SetActive(true);

        newRoot.transform.position = oldRoot.transform.position;

        ActualCamera.transform.parent = newSibling.parent;
        ActualCamera.transform.localPosition = newSibling.localPosition;
        ActualCamera.transform.localRotation = newSibling.localRotation;
        ActualCamera.transform.localScale = newSibling.localScale;
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
}
