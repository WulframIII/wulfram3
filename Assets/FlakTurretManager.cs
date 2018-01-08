using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class FlakTurretManager : Photon.PunBehaviour {

        public float reloadTime = 5;
        public float turnSpeed = 10;
        public float scanInterval = 3;
        public float scanRadius = 300;
        public float testTargetOnSightInterval = 0.5f;
        public Transform gunEnd;

        private GameManager gameManager;
        private float timeSinceLastScan = 0;
        private Transform currentTarget = null;
        private bool targetOnSight = false;
        private float timeSinceLastFire = 0;
        private PunTeams.Team team;

        // Use this for initialization
        void Start() {
            if (PhotonNetwork.isMasterClient) {
                gameManager = GetGameManager();
            }
            team = transform.GetComponent<Unit>().unitTeam;
        }

        private GameManager GetGameManager() {
            if (gameManager == null) {
                gameManager = FindObjectOfType<GameManager>();
            }
            return gameManager;
        }

        // Update is called once per frame
        void Update() {
            if (PhotonNetwork.isMasterClient) {
                FindTarget();
                TurnTowardsCurrentTarget();
                CheckTargetOnSight();
                FireAtTarget();
            }
        }

        private void FireAtTarget() {
            timeSinceLastFire += Time.deltaTime;
            if (timeSinceLastFire >= reloadTime && targetOnSight) {
                //Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
                //Quaternion rotation = transform.rotation;
                GetGameManager().SpawnFlakShell(gunEnd.position, gunEnd.rotation, team);
                timeSinceLastFire = 0;
            }
        }

        private void CheckTargetOnSight() {
            if (currentTarget == null) {
                targetOnSight = false;
                return;
            }

            RaycastHit objectHit;
            //Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
            targetOnSight = Physics.Raycast(gunEnd.position, transform.forward, out objectHit, scanRadius) && ValidTarget(objectHit.transform);
        }

        private void TurnTowardsCurrentTarget() {
            if (currentTarget != null) {
                Vector3 lookPos = currentTarget.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            }
        }

        private void FindTarget() {
            timeSinceLastScan += Time.deltaTime;
            if (timeSinceLastScan >= scanInterval) {
                Transform closestTarget = null;
                float minDistance = scanRadius + 10f;
                Collider[] cols = Physics.OverlapSphere(transform.position, scanRadius);
                foreach (Collider col in cols)
                {
                    if (ValidTarget(col.transform))
                    {
                        float distance = Vector3.Distance(transform.position, col.transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestTarget = col.transform;
                        }
                    }
                }
                currentTarget = closestTarget;
                timeSinceLastScan = 0;
            }
        }

        private bool ValidTarget(Transform t)
        {
            if (t.GetComponent<HitPointsManager>())
            {
                if (t.GetComponent<Unit>().unitTeam != GetComponent<Unit>().unitTeam)
                    return true;
            }
            return false;
        }
    }
}
