using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class UnitAnchor : Photon.PunBehaviour {

        private Vector3 anchorPosition;
        private float anchorStrength;
        private float distanceFromAnchor;
        private float unitHeight; // used for gun and flak turrets
        private Rigidbody myRigidbody;
        private Unit myUnit;
        private bool isAnchored = false;
        private bool heightAttained = false;
        private float diffX;
        private float diffZ;
        private float xVelocitySmoothing = 0.0f;
        private float zVelocitySmoothing = 0.0f;
        private float rotationSmoothing = 0.0f;
        private Quaternion referenceRotation;
        // Use this for initialization
        void Start() {
            myRigidbody = GetComponent<Rigidbody>();
            if (!PhotonNetwork.isMasterClient)
            {
                myRigidbody.isKinematic = true;
                return;
            }
            myUnit = GetComponent<Unit>();
            if (myUnit != null)
            {
                unitHeight = 0.05f;
                anchorStrength = 0.05f;
                if (myUnit.unitType == UnitType.FlakTurret) {
                    unitHeight = 6f;
                    anchorStrength = .1f;
                }
                if (myUnit.unitType == UnitType.GunTurret) {
                    unitHeight = 4f;
                    anchorStrength = .1f; // 2f;
                }
            }

        }

        // Update is called once per frame
        void Update() {
        }

        private Vector3 CenteredLowestPoint()
        {
            return new Vector3(transform.position.x, GetComponent<Collider>().bounds.min.y, transform.position.z);
        }

        private void SetToNormal()
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit))
            {
                Vector3 fwd = transform.forward;
                Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
                transform.rotation = Quaternion.LookRotation(proj, hit.normal);
                if (hit.distance < 0.05f)
                {
                    //transform.Translate(hit.point - CenteredLowestPoint());
                    isAnchored = true;
                    myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }

        private void FixedUpdate()
        {
            if (unitHeight > 0.05f)
            {
                if (!heightAttained)
                {
                    Ray ray = new Ray(CenteredLowestPoint(), -Vector3.up);
                    Ray checkRay = new Ray(transform.position, -Vector3.up); // This double check helps make sure the unit doesn't get stuck in the ground
                    RaycastHit hit;
                    RaycastHit groundCheck;
                    if (Physics.Raycast(ray, out hit, unitHeight) || (Physics.Raycast(checkRay, out groundCheck, unitHeight) && groundCheck.distance != 0 && hit.distance == 0))
                    {
                        float d = hit.distance;
                        if (d < unitHeight)
                        {
                            float ftcGravity = Physics.gravity.y * myRigidbody.mass;
                            float ftcVelocity = myRigidbody.velocity.y * myRigidbody.mass;
                            float multi = (unitHeight - d) / unitHeight;
                            float force = (ftcGravity + ftcVelocity) * (multi * myRigidbody.mass);
                            myRigidbody.AddForce(new Vector3(0f, -force, 0f));
                        }
                        if (Mathf.Abs(unitHeight - d) < 0.25f)
                        {
                            myRigidbody.constraints = RigidbodyConstraints.FreezePosition;
                            heightAttained = true;
                        }
                    }
                }
                /*
                float distanceFromAnchorX = transform.position.x - anchorPosition.x;
                float distanceFromAnchorZ = transform.position.z - anchorPosition.z;
                if (distanceFromAnchorX + distanceFromAnchorZ != 0f)
                {
                    float newX = Mathf.SmoothDamp(transform.position.x, anchorPosition.x, ref xVelocitySmoothing, anchorStrength);
                    float newZ = Mathf.SmoothDamp(transform.position.z, anchorPosition.z, ref zVelocitySmoothing, anchorStrength);
                    transform.position = new Vector3(newX, transform.position.y, newZ);
                }
                */
            }
            else if (unitHeight <= 0.05f && !isAnchored)
            {
                SetToNormal();
            }
        }
    }
}
