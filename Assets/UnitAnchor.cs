﻿using Assets.Wulfram3.Scripts.InternalApis.Classes;
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
            Ray groundFinder = new Ray(transform.position, -transform.up);
            RaycastHit groundInfo;
            Physics.Raycast(groundFinder, out groundInfo);
            anchorPosition = groundInfo.point;
            //Debug.Log(transform.name + " anchoring to " + anchorPosition);
            myUnit = GetComponent<Unit>();
            if (myUnit != null)
            {
                if (myUnit.unitType == UnitType.FlakTurret) {
                    unitHeight = 6f;
                    anchorStrength = .1f;
                }
                if (myUnit.unitType == UnitType.GunTurret) {
                    unitHeight = 4f;
                    anchorStrength = 1f;
                }
                if (myUnit.unitType == UnitType.MissleLauncher) {
                    unitHeight = 0.05f;
                    anchorStrength = .05f;
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
            if (unitHeight > 0.05f)
            {
                Ray ray = new Ray(CenteredLowestPoint(), -Vector3.up);
                Ray checkRay = new Ray(transform.position, -Vector3.up); // This double check helps make sure the unit doesn't get stuck in the ground
                RaycastHit hit;
                RaycastHit groundCheck;
                if (Physics.Raycast(ray, out hit, unitHeight) || (Physics.Raycast(checkRay, out groundCheck, unitHeight) && groundCheck.distance != 0 && hit.distance == 0))
                {
                    if (hit.distance < unitHeight)
                    {
                        float ftcGravity = Physics.gravity.y * myRigidbody.mass;
                        float ftcVelocity = myRigidbody.velocity.y * myRigidbody.mass;
                        float multi = (unitHeight - hit.distance) / unitHeight;
                        float force = (ftcGravity + ftcVelocity) * (multi * myRigidbody.mass);
                        myRigidbody.AddForce(new Vector3(0f, -force, 0f));
                    }
                }
            }
            distanceFromAnchor = Vector3.Distance(transform.position, anchorPosition);
            if (myUnit.unitType == UnitType.GunTurret && myUnit.unitTeam == PunTeams.Team.blue)
            {
                //Debug.Log("Current Position: " + transform.position);
                //Debug.Log("Anchor  Position: " + anchorPosition);
                //Debug.Log("Distance From Anchor: " + distanceFromAnchor);

            }
            float newX = Mathf.SmoothDamp(transform.position.x, anchorPosition.x, ref xVelocitySmoothing, distanceFromAnchor / anchorStrength);
            float newZ = Mathf.SmoothDamp(transform.position.z, anchorPosition.z, ref zVelocitySmoothing, distanceFromAnchor / anchorStrength);
            transform.position = new Vector3(newX, transform.position.y, newZ);

        }


    }
}
