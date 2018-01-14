using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class FlakTurretManager : Photon.PunBehaviour {

        private List<GameObject> targetList = new List<GameObject>();

        private int shellCount = 3;
        private int shellCountCurrent = 0;
        private int shellVelocity = 0;
        private float rangeMax = 75f;
        private float rangeMin = 6f;
        private float fireDelay = 3f;
        private float shellDelay = 0.4f;
        private float fireStamp;

        private float turnSpeed = 90;
        public Transform gunEnd;

        private GameManager gameManager;
        private Transform currentTarget = null;
        private Vector3 currentIntercept = new Vector3(99999f, 99999f, 99999f);
        private float interceptTime;
        private bool targetOnSight = false;
        private PunTeams.Team team;

        // Use this for initialization
        void Start() {
            if (PhotonNetwork.isMasterClient) {
                gameManager = FindObjectOfType<GameManager>();
            }
            team = transform.GetComponent<Unit>().unitTeam;
            fireStamp = Time.time;
            shellVelocity = SplashProjectileController.FlakVelocity;
        }

        // Update is called once per frame
        void Update() {
            if (currentTarget == null)
                FindTarget();
            // Needs to be separate in case target is found above
            if (currentTarget != null)
            {
                TurnTowardsCurrentTarget();
                if (PhotonNetwork.isMasterClient)
                {
                    CheckTargetOnSight();
                    FireAtTarget();
                }
            }
        }

        private void UpdateInterceptPoint()
        {
            currentIntercept = getInterceptPoint(gunEnd.position, GetComponent<Rigidbody>().velocity, shellVelocity, currentTarget.position, currentTarget.GetComponent<Rigidbody>().velocity);
        }

        private void ResetTarget()
        {
            currentIntercept = new Vector3(99999f, 99999f, 99999f);
            interceptTime = -1f;
            currentTarget = null;
            targetOnSight = false;

        }

        private void FindTarget() {
            if (currentTarget != null)
            {
                float distanceToTarget = Vector3.Distance(gunEnd.position, currentIntercept);
                if (distanceToTarget > rangeMax || distanceToTarget < rangeMin)
                {
                    ResetTarget();
                } else
                {
                    return;
                }
            }
            Transform closestTarget = null;
            float minDistance       = rangeMax;
            for (int i = targetList.Count-1; i>=0; i--) {
                if (targetList[i] == null)
                {
                    targetList.RemoveAt(i);
                }
                else
                {
                    float distance = Vector3.Distance(transform.position, targetList[i].transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = targetList[i].transform;
                    }
                }
            }
            currentTarget = closestTarget;
        }

        private void TurnTowardsCurrentTarget()
        {
            UpdateInterceptPoint();
            Vector3 lookPos = currentIntercept - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
        }

        private void CheckTargetOnSight()
        {
            RaycastHit hit;
            Physics.Raycast(gunEnd.position, gunEnd.transform.forward, out hit);
            if (hit.transform == null)
            {
                targetOnSight = true;
                return;
            }
            float diff = Vector3.Distance(hit.transform.position, currentIntercept);
            if (diff < (SplashProjectileController.FlakSplashRadius-2) && diff > -(SplashProjectileController.FlakSplashRadius-2))
            {
                targetOnSight = true;
                return;
            }
            if (hit.transform && ValidTarget(hit.transform))
            {
                targetOnSight = true;
                return;
            }
            ResetTarget();
        }

        private void FireAtTarget()
        {
            if ((currentTarget != null && targetOnSight) || shellCountCurrent > 0)
            {
                if (Time.time > fireStamp)
                {
                    if (shellCountCurrent < shellCount)
                    {
                        gameManager.SpawnFlakShell(gunEnd.position, gunEnd.rotation, team, interceptTime);
                        shellCountCurrent += 1;
                        fireStamp = Time.time + shellDelay;
                    }
                    else if (shellCountCurrent == shellCount)
                    {
                        fireStamp = Time.time + fireDelay;
                        shellCountCurrent = 0;
                    }
                }
            }
        }


        private bool ValidTarget(Transform t)
        {
            Unit u = t.GetComponent<Unit>();
            if (u != null && u.unitTeam != team)
            {
                return true;
            }
            SplashProjectileController p = t.GetComponent<SplashProjectileController>();
            if (p != null && p.team != team)
            {
                return true;
            }
            return false;
        }

        Vector3 getInterceptPoint(Vector3 turretPos, Vector3 turretVel, float pSpeed, Vector3 targetPos, Vector3 targetVel)
        {
            Vector3 targetRPos = targetPos - turretPos;
            Vector3 targetRVel = targetVel - turretVel;
            interceptTime = getInterceptTime(pSpeed, targetRPos, targetRVel);
            return targetPos + targetRVel * interceptTime;
        }

        float getInterceptTime(float pSpeed, Vector3 targetRPos, Vector3 targetRVel)
        {
            float targetSVel = targetRVel.sqrMagnitude;
            float targetSPos = targetRPos.sqrMagnitude;
            if (targetSVel < 0.001f)
                return 0f;
            float a = targetSVel - pSpeed * pSpeed;
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetSPos / (2f * Vector3.Dot(targetRVel, targetRPos));
                return Mathf.Max(t, 0f);
            }
            float b = 2f * Vector3.Dot(targetRVel, targetRPos);
            float d = b * b - 4f * a * targetSPos;
            if (d > 0f)
            {
                float t1 = (-b + Mathf.Sqrt(d)) / (2f * a), t2 = (-b - Mathf.Sqrt(d)) / (2f * a);
                if (t1 > 0f)
                {
                    if (t2 > 0f)
                    {
                        return Mathf.Min(t1, t2);
                    }
                    else
                    {
                        return t1;
                    }
                }
                else
                {
                    return Mathf.Max(t2, 0f);
                }
            }
            else if (d < 0f)
            {
                return 0f;
            }
            else
            {
                return Mathf.Max(-b / (2f * a), 0f);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (PhotonNetwork.isMasterClient && !targetList.Contains(other.gameObject) && ValidTarget(other.gameObject.transform))
            {
                targetList.Add(other.gameObject);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (PhotonNetwork.isMasterClient && !targetList.Contains(other.gameObject) && ValidTarget(other.gameObject.transform))
            {
                targetList.Add(other.gameObject);
            }
        }

        void OnTriggerLeave(Collider other)
        {
            if (PhotonNetwork.isMasterClient && targetList.Contains(other.gameObject))
            {
                targetList.Remove(other.gameObject);
            }
        }



    }
}
