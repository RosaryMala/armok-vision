using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraRotate : MonoBehaviour {

    private float mouseX;

    public float rotateSpeed = 10.0f;

    public float moveSpeed = 1.0f;

    
	void Update()
    {
        HandleMouseRotation();

        mouseX = Input.mousePosition.x;
    }

    public void HandleMouseRotation()
    {
        if (Input.GetMouseButton(1) && EventSystem.current.currentSelectedGameObject == null)
        {
            //horizontal rotation
            if(Input.mousePosition.x != mouseX)
            {
                float cameraRotationY = (Input.mousePosition.x - mouseX) * rotateSpeed * Time.unscaledDeltaTime;
                this.transform.Rotate(0, cameraRotationY, 0);
            }
        }
    }
}
