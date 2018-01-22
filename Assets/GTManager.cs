using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3
{
    public class GTManager : Photon.PunBehaviour
    {

        public Transform gunEnd;
        public int bulletDamageinHitpoints = 1;
        public float scanInterval = 2;
        public float scanRadius = 8;
        public float testTargetOnSightInterval = 0.5f;
        public float turnSpeed = 10;
        public float bulletsPerSecond = 4;

        private GameManager gameManager;
        private LineRenderer laserLine;
        private Transform currentTarget = null;
        private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
        private bool targetOnSight = false;
        private bool shooting = false;
        private float timeSinceLastScan = 0;
        private float timeSinceLastDamage = 0;
        private float timeSinceLastEffect = 0;
        private float deviationConeRadius = 1;
        private float timeBetweenShots;

        private Unit myUnit;

        void Start()
        {

            timeBetweenShots = 1f / bulletsPerSecond;
            laserLine = GetComponent<LineRenderer>();
            //if (PhotonNetwork.isMasterClient)
            //{
                gameManager = FindObjectOfType<GameManager>();
                myUnit = GetComponent<Unit>();
            //}
        }

        private IEnumerator ShotEffect()
        {
            laserLine.enabled = true;
            yield return shotDuration;
            laserLine.enabled = false;
        }

        private void SetAndSyncShooting(bool newValue)
        {
            if (shooting != newValue)
            {
                shooting = newValue;
                SyncShooting();
            }
        }

        [PunRPC]
        public void SetShooting(bool newShootingValue)
        {
            if (!photonView.isMine)
            {
                shooting = newShootingValue;
            }
        }


        private void SyncShooting()
        {
            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("SetShooting", PhotonTargets.All, shooting);
            }
        }

        private void ShowFeedback()
        {
            StartCoroutine(ShotEffect());
            laserLine.SetPosition(0, gunEnd.position);
            laserLine.SetPosition(1, currentTarget.transform.position);
        }

        private Vector3 GetRandomPointInCircle()
        {
            Vector2 randomPoint = Random.insideUnitCircle * deviationConeRadius;
            return new Vector3(randomPoint.x, randomPoint.y, 0);
        }

        // Update is called once per frame
        void Update()
        {
            if (myUnit.hasPower)
            {
                FindTarget();
                TurnTowardsCurrentTarget();
                if (shooting)
                {
                    timeSinceLastEffect += Time.deltaTime;
                    if (timeSinceLastEffect >= timeBetweenShots)
                    {
                        timeSinceLastEffect = 0f;
                        ShowFeedback();
                    }
                }
                if (PhotonNetwork.isMasterClient)
                {
                    CheckTargetOnSight();
                    FireAtTarget();
                }
            }
        }

        private void FireAtTarget()
        {
            if (currentTarget == null || targetOnSight == false)
            {
                targetOnSight = false;
                return;
            }
            timeSinceLastDamage += Time.deltaTime;
            if (timeSinceLastDamage >= 1f)
            {
                Vector3 pos = transform.position + (transform.forward * 3.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;

                RaycastHit objectHit;
                targetOnSight = Physics.Raycast(pos, transform.forward, out objectHit, scanRadius) && ValidTarget(objectHit.collider.transform);
                if (targetOnSight && objectHit.transform.GetComponent<Unit>().unitTeam != this.gameObject.GetComponent<Unit>().unitTeam)
                {
                    HitPointsManager hitPointsManager = objectHit.transform.GetComponent<HitPointsManager>();
                    if (hitPointsManager != null)
                    {
                        hitPointsManager.TellServerTakeDamage((int) Mathf.Ceil(bulletDamageinHitpoints * bulletsPerSecond));

                    }
                }
                timeSinceLastDamage = 0;
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

        private void CheckTargetOnSight()
        {
            if (currentTarget == null)
            {
                SetAndSyncShooting(false);
                targetOnSight = false;
                return;
            }

            RaycastHit objectHit;
            targetOnSight = Physics.Raycast(gunEnd.position, gunEnd.forward, out objectHit, scanRadius) && ValidTarget(objectHit.collider.transform);
            if (!targetOnSight)
            {
                SetAndSyncShooting(false);
            } else 
            {
                SetAndSyncShooting(true);
            }
        }

        private void TurnTowardsCurrentTarget()
        {
            if (currentTarget != null)
            {
                var distance = Vector3.Distance(currentTarget.transform.position, transform.position);
                if (distance >= scanRadius)
                {
                    targetOnSight = false;
                    currentTarget = null;
                    SetAndSyncShooting(false);
                    return;
                }
                else
                {
                    Vector3 lookPos = currentTarget.transform.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
                }
            }
        }

        private void FindTarget()
        {

            timeSinceLastScan += Time.deltaTime;
            if (timeSinceLastScan >= scanInterval)
            {
                currentTarget = null;
                Transform closestTarget = null;
                float minDistance = scanRadius + 1f;
                Collider[] cols = Physics.OverlapSphere(transform.position, scanRadius);
                foreach (var col in cols)
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
    }
}

