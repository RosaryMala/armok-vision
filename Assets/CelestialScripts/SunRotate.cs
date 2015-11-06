using UnityEngine;

public class SunRotate : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    TimeHolder timeHolder;

    void Awake()
    {
        timeHolder = FindObjectOfType<TimeHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotate = Quaternion.AngleAxis(timeHolder.displayedTime.SunAngle, Vector3.back);
        transform.localRotation = rotate;
    }
}
