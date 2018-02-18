using UnityEngine;

public class LODCamera : MonoBehaviour
{

    public Transform parentTransform;
    public float scaleFactor;
    public bool scaleClipPlanes;
    public bool ignoreRotation = false;
    Camera thisCamera;
    Camera parentCamera;

    void Awake()
    {
        thisCamera = GetComponent<Camera>();
        parentCamera = parentTransform.GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = parentTransform.transform.position * scaleFactor;
        if(!ignoreRotation)
            transform.rotation = parentTransform.transform.rotation;

        if (parentCamera != null && thisCamera != null)
        {
            thisCamera.fieldOfView = parentCamera.fieldOfView;
            if (scaleClipPlanes)
            {
                thisCamera.nearClipPlane = parentCamera.nearClipPlane * scaleFactor;
                thisCamera.farClipPlane = parentCamera.farClipPlane * scaleFactor;
            }
        }
    }
}
