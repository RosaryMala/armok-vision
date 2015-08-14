using UnityEngine;
using System.Collections;

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
        Vector3 rotation = new Vector3(timeHolder.displayedTime.SunAngle, 0, 0);
        Quaternion rotate = Quaternion.Euler(rotation);
        transform.rotation = rotate;
    }
}
