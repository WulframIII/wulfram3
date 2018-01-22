using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class SplashProjectileController : Photon.PunBehaviour {
        /* This script facilitates control of our direct fire splash damage projectiles
         * 
         * Static variables are used in Start() to initialize each projectile with the proper
         * information and mesh     */

        public static int PulseVelocity = 30;
        public static int FlakVelocity = 60; // To intercept, flak must be faster than pulse

        public static int PulseDirectDamage = 200;
        public static int FlakDirectDamage = 70;

        public static int PulseSplashRadius = 12;
        public static int FlakSplashRadius = 7;

        public float lifetime = 12f; // This is used by pulse shells to control self detonation. Flak turret pass a "fuse" time when creating shells

        // Each projectile mesh should be a child transform of the main projectile and linked to these vars in the inspector.
        public Transform redPulse;
        public Transform bluePulse;
        public Transform flakShell;

        // These vars are used internally by the script, overwritten in Start() as needed.
        private int velocity = 30;
        private int directDamage = 100;
        private int splashRadius = 6;
        private GameManager gameManager;
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
                lifetime = (float) instanceData[2]; // This value is determined by the turret
                // These lines turn the meshes on and off
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
                if (team == PunTeams.Team.Blue)
                {
                    redPulse.gameObject.SetActive(false);
                    bluePulse.gameObject.SetActive(true);
                }
                else if (team == PunTeams.Team.Red)
                {
                    redPulse.gameObject.SetActive(true);
                    bluePulse.gameObject.SetActive(false);
                }
            }
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * velocity;
            if (PhotonNetwork.isMasterClient)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
        }

        // Update is called once per frame
        void Update() {
            if (PhotonNetwork.isMasterClient) // Force masterclient control of time based detonation
            {
                lifetimer += Time.deltaTime;
                if (lifetimer >= lifetime)
                {
                    DoEffects(transform.position);
                }
            }
        }

        private void FixedUpdate()
        {
            if (PhotonNetwork.isMasterClient) // Force masterclient control of velocity base detonation
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb.velocity != transform.forward * velocity)
                {
                    SplashDetonation();
                }
            }
        }

        void DoEffects(Vector3 pos)
        {
            // We should not get here unless we are masterclient (See Update(), OnCollisionEnter())
            gameManager.SpawnExplosion(pos);
            PhotonNetwork.Destroy(gameObject);
        }

        void DoDamage(Transform target, int amount)
        {
            // We should not get here unless we are masterclient (See OnCollisionEnter())
            Unit unit = target.GetComponent<Unit>();
            HitPointsManager hpm = target.GetComponent<HitPointsManager>();
            if (unit != null && hpm != null && unit.unitTeam != team)
            {
                hpm.TellServerTakeDamage(amount);
            }
        }

        void SplashDetonation()
        {
            /*
             * Desired Effect When Hit Here
             * 
             */
            DoEffects(transform.position);
        }

        void SplashDamage(Collider[] hitObjects, Vector3 hitPos, Transform originalHit)
        {
            // We should not get here unless we are masterclient (See OnCollisionEnter())
            foreach (Collider c in hitObjects)
            {
                if (!c.transform.Equals(originalHit)) // Ignore the object hit directly, it has already been damaged
                {
                    float pcnt = (splashRadius - Vector3.Distance(hitPos, c.transform.position)) / splashRadius;
                    DoDamage(c.transform, (int)Mathf.Ceil(directDamage * pcnt));
                }
            }
        }

        void OnCollisionEnter(Collision col) {
            if (PhotonNetwork.isMasterClient) { // Force masterclient handling of damage and effects
                Vector3 hitPosition = col.contacts[0].point;
                Collider[] splashedObjects = Physics.OverlapSphere(hitPosition, splashRadius);
                DoEffects(hitPosition);
                DoDamage(col.transform, directDamage);
                SplashDamage(splashedObjects, hitPosition, col.transform);
            }
        }
    }
}
