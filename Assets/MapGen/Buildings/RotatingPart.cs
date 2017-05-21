using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class RotatingPart : MonoBehaviour
    {
        public bool on;
        public float acceleration = 5;
        public float speed = 0;
        public float topSpeed = 30;
        public Vector3 axis = Vector3.left;

        // Update is called once per frame
        void Update()
        {
            if(on)
            {
                if (speed < topSpeed)
                    speed += acceleration * Time.deltaTime;
                speed = Mathf.Min(speed, topSpeed);
            }
            else
            {
                if (speed > 0)
                    speed -= acceleration * Time.deltaTime;
                speed = Mathf.Max(speed, 0);
            }
            if(speed > 0)
            {
                transform.Rotate(axis, speed * 6 * Time.deltaTime);
            }
        }

        internal void SetState(BuildingInstance buildingInput)
        {

        }
    }
}
