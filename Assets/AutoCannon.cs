using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class AutoCannon : Photon.PunBehaviour {
        public AudioClip shootSound;
        public AudioClip hitSound;
        public AudioClip missSound;
		public AudioSource audio;
        public int bulletDamageinHitpoints = 1;
        public float bulletsPerSecond = 10;
        public float range = 80;
        public float deviationConeRadius = 1;
        public int fuelPerBullet = 1;
		private GameManager gameManager;
		//Start of the laser
		public Transform gunEnd;
        bool shooting = false;
		//camera for firing
        public float simDelayInSeconds = 0.1f;

        private float lastSimTime = 0;


		//wait for second on laser
		private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
		//line render for gun shots
		public LineRenderer laserLine;
		//next fire of laser
		private float nextFire;

        public bool debug = true;

        private float timeBetweenShots;
        private float lastFireTime;

        private int screenW;
        private int screenH;
        private Transform targetPosition;


        // Use this for initialization
        void Start() {
			laserLine = GetComponent<LineRenderer> ();
            timeBetweenShots = 1f / bulletsPerSecond;
        }

		private GameManager GetGameManager() {
			if (gameManager == null) {
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

        [PunRPC]
        public void SetShooting(bool newShootingValue) {
            if (!photonView.isMine) {
                shooting = newShootingValue;
            }   
        }

        // Update is called once per frame
        void Update() {

            if (photonView.isMine) {
                CheckAndFire();
            }
            ShowFeedback();
        }

        private void SetAndSyncShooting(bool newValue) {
            if (shooting != newValue) {
                shooting = newValue;
                SyncShooting();
            }
        }


        private void CheckAndFire() {
            if (Input.GetMouseButton(0)) {

                float currentTime = Time.time;
                if (lastFireTime + timeBetweenShots > currentTime ) {
                    return;
                }
                if (Cursor.visible || GetComponent<PlayerMovementManager>().isDead) {
                    SetAndSyncShooting(false);
                    return;
                }
                if (!GetComponent<FuelManager>().TakeFuel(fuelPerBullet)) {
                    SetAndSyncShooting(false);
                    return;
                }
                SetAndSyncShooting(true);

                lastFireTime = currentTime;
                Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                Vector3 pos = gunEnd.position; //transform.position + (transform.forward * 1.0f + transform.up * 0.2f);
                Quaternion rotation = gunEnd.rotation; // transform.rotation;


                Vector3 targetPoint = rotation * GetRandomPointInCircle();
                targetPoint += pos + transform.forward * range;
                if (debug) {
                    Debug.DrawLine(pos, targetPoint, Color.white, 1, false);
                }

                RaycastHit objectHit;
                Vector3 targetDirection = (targetPoint - pos).normalized;
                bool targetFound = Physics.Raycast(rayOrigin, targetDirection, out objectHit, range);// && objectHit.transform.GetComponent<Unit>() != null;
                //check if user is on same team
                //CHANGED HERE
                if (targetFound && ValidTarget(objectHit.transform))
                {
                    objectHit.transform.GetComponent<HitPointsManager>().TellServerTakeDamage(bulletDamageinHitpoints);
                }
                AudioSource.PlayClipAtPoint(shootSound, gunEnd.position, 0.1f);
            }
            else {
                SetAndSyncShooting(false);
            }
        }

        private bool ValidTarget(Transform t)
        {
            HitPointsManager hpm = t.GetComponent<HitPointsManager>();
            if (hpm != null)
            {
                Unit u = t.GetComponent<Unit>();
                if (u != null && u.unitTeam != this.gameObject.GetComponent<Unit>().unitTeam)
                {
                    return true;
                }
            }
            return false;
        }

        private void ShowFeedback() {
            if (shooting && (lastSimTime + simDelayInSeconds) < Time.time) {
                lastSimTime = Time.time;

                //Laser Effect
                StartCoroutine(ShotEffect());
                Vector3 rayOrigin;
                RaycastHit hit;
                if (photonView.isMine)
                {
                    rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                }
                else
                {
                    rayOrigin = gunEnd.position;
                }

                laserLine.SetPosition(0, gunEnd.position);

                Vector3 pos = gunEnd.position; // transform.position + (transform.forward * 1.0f + transform.up * 0.2f);
                Quaternion rotation = gunEnd.rotation; // transform.rotation;
                Vector3 bulletHitPoint;
                Vector3 targetPoint = rotation * GetRandomPointInCircle();
                targetPoint += pos + transform.forward * range;
                RaycastHit objectHit;
                Vector3 targetDirection = (targetPoint - pos).normalized;
                if (Physics.Raycast(rayOrigin, targetDirection, out hit, range)) {
                    bulletHitPoint = hit.point;
                    if (hit.transform && ValidTarget(hit.transform))
                    {
                        AudioSource.PlayClipAtPoint(hitSound, hit.point);
                    } else
                    {
                        AudioSource.PlayClipAtPoint(missSound, targetPoint);
                    }
                }
                else {
                    bulletHitPoint = targetPoint;
                    AudioSource.PlayClipAtPoint(missSound, targetPoint);
                }
                laserLine.SetPosition(1, bulletHitPoint);

                if (!photonView.isMine)
                {
                    //play sound

                    audio.PlayOneShot(shootSound, 1);
                }
            }    
        }

        private void SyncShooting() {
            photonView.RPC("SetShooting", PhotonTargets.All, shooting);
        }

        private Vector3 GetRandomPointInCircle() {
            Vector2 randomPoint = Random.insideUnitCircle * deviationConeRadius;
            return new Vector3(randomPoint.x, randomPoint.y, 0);
        }


    }

}
