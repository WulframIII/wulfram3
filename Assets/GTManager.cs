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
        public float scanRadius = 10;
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

        void Start()
        {

            timeBetweenShots = 1f / bulletsPerSecond;
            laserLine = GetComponent<LineRenderer>();
            if (PhotonNetwork.isMasterClient)
            {
                gameManager = GetGameManager();
            }
        }

        private GameManager GetGameManager()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
            return gameManager;
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
                if (targetOnSight && objectHit.transform.GetComponent<Unit>().team != this.gameObject.GetComponent<Unit>().team)
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
                if (t.GetComponent<Unit>().team != GetComponent<Unit>().team)
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
            var distance = Vector3.Distance(objectHit.transform.position, transform.position);
            if (distance >= scanRadius)
            {
                targetOnSight = false;
                currentTarget = null;
                return;
            }
            if (!targetOnSight)
            {
                SetAndSyncShooting(false);
            }
            {
                SetAndSyncShooting(true);
            }
        }

        private void TurnTowardsCurrentTarget()
        {
            if (currentTarget != null)
            {
                Vector3 lookPos = currentTarget.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            }
        }

        private void FindTarget()
        {

            timeSinceLastScan += Time.deltaTime;
            if (timeSinceLastScan >= scanInterval)
            {
                Transform closestTarget = null;
                float minDistance = scanRadius + 1f;
                Collider[] cols = Physics.OverlapSphere(transform.position, scanRadius);
                List<Rigidbody> rigidbodies = new List<Rigidbody>();
                if (cols.Length > 0)
                {
                    foreach (var col in cols)
                    {
                        if (col.GetComponent<Unit>() && col.GetComponent<Unit>().team != this.transform.GetComponent<Unit>().team && col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                        {
                            rigidbodies.Add(col.attachedRigidbody);
                        }
                    }
                } else
                {
                    currentTarget = null;
                    return;
                }

                foreach (Rigidbody rb in rigidbodies)
                {
                    Transform target = rb.transform;
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = target;
                    }
                }
                currentTarget = closestTarget;
                timeSinceLastScan = 0;
            }
        }
    }
}

