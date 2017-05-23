using UnityEngine;

namespace Building
{
    public class Bridge : MonoBehaviour
    {
        public bool shouldBeRaised;

        public bool isRaised;
        public bool isLowered;

        new Rigidbody rigidbody;
        new HingeJoint hingeJoint;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            hingeJoint = GetComponent<HingeJoint>();
        }

        private void Update()
        {
            if (shouldBeRaised)
            {
                isLowered = false;
                if (!isRaised && hingeJoint.angle >= hingeJoint.limits.max)
                {
                    rigidbody.isKinematic = true;
                    isRaised = true;
                }
                if (!isRaised)
                {
                    hingeJoint.useMotor = true;
                    rigidbody.isKinematic = false;
                }
            }
            else
            {
                isRaised = false;
                if (!isLowered && Mathf.Approximately(hingeJoint.angle, 0) && hingeJoint.velocity < 0.001)
                {
                    rigidbody.isKinematic = true;
                    isLowered = true;
                }
                if (!isLowered)
                {
                    hingeJoint.useMotor = false;
                    rigidbody.isKinematic = false;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(shouldBeRaised && !isRaised && collision.relativeVelocity.y < 0)
            {
                rigidbody.isKinematic = true;
                isRaised = true;
            }
        }
    }
}
