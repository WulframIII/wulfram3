using Assets.Wulfram3.Scripts.InternalApis.Classes;
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
        private float range = 60;
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

        private Vector3 screenCenter;
        private Transform targetPosition;

        public List<Material> teamColorMaterials;

        // Use this for initialization
        void Start() {
            laserLine = GetComponent<LineRenderer>();
            timeBetweenShots = 1f / bulletsPerSecond;
            if (photonView.isMine)
            {
                screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
            }
            Unit u = GetComponent<Unit>();
            if (u.unitTeam == PunTeams.Team.Red)
            {
                laserLine.material = teamColorMaterials[0];
            } else if (u.unitTeam == PunTeams.Team.Blue)
            {
                laserLine.material = teamColorMaterials[1];
            } else
            {
                laserLine.material = teamColorMaterials[2];
            }
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
            if ((Input.GetMouseButton(0) || Input.GetAxisRaw("Fire1") != 0) && (GetComponent<Unit>().unitType == UnitType.Scout || !GetComponent<CargoManager>().isDeploying)) {
                deviationConeRadius = 1f;
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

                /*
                Vector3 targetPoint = rotation * GetRandomPointInCircle();
                targetPoint += pos + transform.forward * range;
                if (debug) {
                    Debug.DrawLine(pos, targetPoint, Color.white, 1, false);
                }
                Vector3 targetDirection = (targetPoint - pos).normalized;
                */
                RaycastHit objectHit;
                bool targetFound = Physics.Raycast(rayOrigin, transform.forward, out objectHit, range);// && objectHit.transform.GetComponent<Unit>() != null;
                //check if user is on same team
                //CHANGED HERE
                if (targetFound && ValidTarget(objectHit.transform))
                {
                    Vector3 tPos = Camera.main.WorldToScreenPoint(objectHit.transform.GetComponent<Collider>().bounds.center);
                    Vector3 cPos = new Vector3(tPos.x, tPos.y, 0.0f);
                    float distanceFromCenter = Vector3.Distance(screenCenter, cPos);
                    deviationConeRadius = Mathf.Clamp(distanceFromCenter / (Object2dRect(objectHit.transform.gameObject).size.x / 2), 0, 1);
                    float damageMultiplier = 1.5f - deviationConeRadius;

                    objectHit.transform.GetComponent<HitPointsManager>().TellServerTakeDamage((int) Mathf.Ceil(bulletDamageinHitpoints * damageMultiplier));
                } 
                AudioSource.PlayClipAtPoint(shootSound, gunEnd.position, 0.1f);
            }
            else {
                SetAndSyncShooting(false);
            }
        }

        public static Rect Object2dRect(GameObject go)
        {
            Bounds b = go.GetComponent<Collider>().bounds;
            return new Rect(b.min.x, b.min.y, b.max.x - b.min.x, b.max.y - b.min.y);
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
