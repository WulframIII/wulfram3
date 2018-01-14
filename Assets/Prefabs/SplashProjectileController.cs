using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class SplashProjectileController : Photon.PunBehaviour {
        private int velocity     = 30;
        private int directDamage = 100;
        private int splashRadius = 6;

        public static int PulseVelocity = 30;
        public static int FlakVelocity = 80;

        public static int PulseDirectDamage = 200;
        public static int FlakDirectDamage = 60;

        public static int PulseSplashRadius = 12;
        public static int FlakSplashRadius = 7;

        public Transform redPulse;
        public Transform bluePulse;
        public Transform flakShell;

        private GameManager gameManager;
        public float lifetime = 12f;
        private float lifetimer = 0f;
        [HideInInspector]
        public PunTeams.Team team;

        // Use this for initialization
        void Start() {
            object[] instanceData = transform.GetComponent<PhotonView>().instantiationData;
            team = (PunTeams.Team) instanceData[0];
            UnitType firedBy = (UnitType) instanceData[1];
            if (firedBy == UnitType.FlakTurret)
            {
                velocity = FlakVelocity;
                directDamage = FlakDirectDamage;
                splashRadius = FlakSplashRadius;
                lifetime = (float) instanceData[2];
                redPulse.gameObject.SetActive(false);
                bluePulse.gameObject.SetActive(false);
                flakShell.gameObject.SetActive(true);

            }
            else
            {
                velocity = PulseVelocity;
                directDamage = PulseDirectDamage;
                splashRadius = PulseSplashRadius;
                flakShell.gameObject.SetActive(false);
                if (team == PunTeams.Team.blue)
                {
                    redPulse.gameObject.SetActive(false);
                    bluePulse.gameObject.SetActive(true);
                }
                else
                {
                    redPulse.gameObject.SetActive(true);
                    bluePulse.gameObject.SetActive(false);
                }
            }
            if (photonView.owner != null && photonView.owner.IsLocal)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = transform.forward * velocity;
                gameManager = FindObjectOfType<GameManager>();
            }
        }

        // Update is called once per frame
        void Update() {
            if (PhotonNetwork.isMasterClient)
            {
                lifetimer += Time.deltaTime;
                if (lifetimer >= lifetime)
                {
                    DoEffects(transform.position);
                }
            }
        }

        void DoEffects(Vector3 pos)
        {
            gameManager.SpawnExplosion(pos);
            PhotonNetwork.Destroy(gameObject);
        }

        void DoDamage(Transform target, int amount)
        {
            Unit unit = target.GetComponent<Unit>();
            HitPointsManager hpm = target.GetComponent<HitPointsManager>();
            if (unit != null && hpm != null && unit.unitTeam != team)
            {
                hpm.TellServerTakeDamage(amount);
            }
        }

        void SplashDamage(Collider[] hitObjects, Vector3 hitPos, Transform originalHit)
        {
            foreach (Collider c in hitObjects)
            {
                if (!c.transform.Equals(originalHit))
                {
                    float pcnt = (splashRadius - Vector3.Distance(hitPos, c.transform.position)) / splashRadius;
                    DoDamage(c.transform, (int)Mathf.Ceil(directDamage * pcnt));
                }
            }
        }

        void OnCollisionEnter(Collision col) {
            if (PhotonNetwork.isMasterClient) {
                Vector3 hitPosition = col.contacts[0].point;
                Collider[] splashedObjects = Physics.OverlapSphere(hitPosition, splashRadius);
                DoEffects(hitPosition);
                DoDamage(col.transform, directDamage);
                SplashDamage(splashedObjects, hitPosition, col.transform);
            }
        }
    }
}
