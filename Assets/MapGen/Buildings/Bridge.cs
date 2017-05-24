using System;
using RemoteFortressReader;
using UnityEngine;

namespace Building
{
    public class Bridge : MonoBehaviour, IBuildingPart
    {
        public bool shouldBeRaised;

        public bool raising;
        public bool lowering;

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
                lowering = true;
                if (raising && hingeJoint.angle >= hingeJoint.limits.max)
                {
                    rigidbody.isKinematic = true;
                    raising = false;
                }
                if (raising)
                {
                    hingeJoint.useMotor = true;
                    rigidbody.isKinematic = false;
                }
            }
            else
            {
                raising = true;
                if (lowering && Mathf.Approximately(hingeJoint.angle, 0) && hingeJoint.velocity < 0.001)
                {
                    rigidbody.isKinematic = true;
                    lowering = false;
                }
                if (lowering)
                {
                    hingeJoint.useMotor = false;
                    rigidbody.isKinematic = false;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(shouldBeRaised 
                && raising 
                && (collision.rigidbody == null || collision.rigidbody.isKinematic)
                && ((collision.contacts[0].point - transform.position).y > 1)
                )
            {
                rigidbody.isKinematic = true;
                raising = false;
            }
        }

        public void UpdatePart(BuildingInstance buildingInstance)
        {
            if (buildingInstance.direction == BuildingDirection.NONE)
                gameObject.SetActive(buildingInstance.active == 0);
            else
                shouldBeRaised = buildingInstance.active == 1;
        }
    }
}
