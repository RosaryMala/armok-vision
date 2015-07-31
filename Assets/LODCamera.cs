using UnityEngine;

public class LODCamera : MonoBehaviour
{

    public Camera parentCamera;
    public float scaleFactor;
    public bool scaleClipPlanes;
    Camera thisCamera;

    void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = parentCamera.transform.position * scaleFactor;
        transform.rotation = parentCamera.transform.rotation;
        thisCamera.fieldOfView = parentCamera.fieldOfView;
        if (scaleClipPlanes)
        {
            thisCamera.nearClipPlane = parentCamera.nearClipPlane * scaleFactor;
            thisCamera.farClipPlane = parentCamera.farClipPlane * scaleFactor;
        }
    }
}
