using UnityEngine;
using System.Collections;

public class CameraRotateVertical : MonoBehaviour
{

    private float mouseY;

    public bool verticalRotationEnabled = true;

    public float verticalRotationMin = 0.0f;
    public float verticalRotationMax = 80.0f;

    public float mouseSpeed = 10.0f;

    public float rotationX = 45.0f;

    void Update()
    {
        HandleMouseRotation();

        mouseY = Input.mousePosition.y;
    }

    public void HandleMouseRotation()
    {
        if (Input.GetMouseButton(1))
        {
            //horizontal rotation
            if (verticalRotationEnabled && Input.mousePosition.y != mouseY)
            {
                float cameraRotationX = (mouseY - Input.mousePosition.y) * mouseSpeed * Time.deltaTime;
                rotationX = rotationX + cameraRotationX;
                if (rotationX < verticalRotationMin)
                    rotationX = verticalRotationMin;
                if (rotationX > verticalRotationMax)
                    rotationX = verticalRotationMax;
                this.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
            }
        }
    }
}
