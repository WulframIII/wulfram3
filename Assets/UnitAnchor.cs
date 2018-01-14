using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class UnitAnchor : Photon.PunBehaviour {

        private Vector3 anchorPosition;
        private float anchorStrength;
        private float distanceFromAnchor;
        private float turretHeight;
        private Rigidbody myRigidbody;
        private Unit myUnit;
        private bool isAnchored = false;
        private float diffX;
        private float diffZ;
        private float xVelocitySmoothing = 0.0f;
        private float zVelocitySmoothing = 0.0f;

        // Use this for initialization
        void Start() {
            myRigidbody = GetComponent<Rigidbody>();
            if (!PhotonNetwork.isMasterClient)
            {
                myRigidbody.isKinematic = true;
                return;
            }
            Ray groundFinder = new Ray(transform.position, -transform.up);
            RaycastHit groundInfo;
            Physics.Raycast(groundFinder, out groundInfo);
            anchorPosition = groundInfo.point;
            myUnit = GetComponent<Unit>();
            if (myUnit != null)
            {
                if (myUnit.unitType == UnitType.FlakTurret) {
                    turretHeight = 6f;
                    anchorStrength = 3f;
                }
                if (myUnit.unitType == UnitType.GunTurret) {
                    turretHeight = 4f;
                    anchorStrength = 0.1f;
                }
                if (myUnit.unitType == UnitType.MissleLauncher) {
                    turretHeight = 0.05f;
                    anchorStrength = 3f;
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
            /*
            Vector3 fwd = transform.forward;
            Vector3 proj = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
            transform.rotation = Quaternion.LookRotation(proj, hit.normal);
            transform.Translate(hit.point - CenteredLowestPoint());
            */
        }




        private void FixedUpdate()
        {
            Ray ray = new Ray(CenteredLowestPoint(), -Vector3.up);
            Ray checkRay = new Ray(transform.position, -Vector3.up); // This double check helps make sure the tank doesn't get stuck in the ground
            RaycastHit hit;
            RaycastHit groundCheck;
            if (Physics.Raycast(ray, out hit, turretHeight) || (Physics.Raycast(checkRay, out groundCheck, turretHeight) && groundCheck.distance != 0 && hit.distance == 0))
            {
                if (hit.distance < turretHeight)
                {
                    float ftcGravity = Physics.gravity.y * myRigidbody.mass;
                    float ftcVelocity = myRigidbody.velocity.y * myRigidbody.mass;
                    float multi = (turretHeight - hit.distance) / turretHeight;
                    float force = (ftcGravity + ftcVelocity) * (multi * myRigidbody.mass);
                    myRigidbody.AddForce(new Vector3(0f, -force, 0f));
                }
            }
            float newX = Mathf.SmoothDamp(transform.position.x, anchorPosition.x, ref xVelocitySmoothing, anchorStrength);
            float newZ = Mathf.SmoothDamp(transform.position.z, anchorPosition.z, ref zVelocitySmoothing, anchorStrength);
            transform.position = new Vector3(newX, transform.position.y, newZ);

        }


    }
}
