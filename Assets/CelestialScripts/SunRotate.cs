using UnityEngine;

public class SunRotate : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        Quaternion rotate = Quaternion.AngleAxis(TimeHolder.DisplayedTime.SunAngle, Vector3.back);
        transform.localRotation = rotate;
    }
}
