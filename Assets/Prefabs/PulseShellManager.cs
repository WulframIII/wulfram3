using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class PulseShellManager : Photon.PunBehaviour {
        public float velocity = 30f;
        public int directHitpointsDamage = 200;
        public float splashRadius = 8f;

        public Transform redPulse;
        public Transform bluePulse;

        private GameManager gameManager;
        private float lifetime = 100f;
        private float lifetimer = 0f;
        private PunTeams.Team team;

        // Use this for initialization
        void Start() {
            if (photonView.owner != null && photonView.owner.IsLocal) {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity = transform.forward * velocity;
                gameManager = FindObjectOfType<GameManager>();
            }
            team = (PunTeams.Team) transform.GetComponent<PhotonView>().instantiationData[0];
            Unit unit_Component = transform.GetComponent<Unit>();
            if (unit_Component != null)
            {
                unit_Component.unitTeam = team;
            }
            if (team == PunTeams.Team.blue)
            {
                redPulse.gameObject.SetActive(false);
                bluePulse.gameObject.SetActive(true);
            } else
            {
                redPulse.gameObject.SetActive(true);
                bluePulse.gameObject.SetActive(false);
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
            if (unit != null && hpm != null && !unit.IsUnitFriendly())
            {
                hpm.TellServerTakeDamage(amount);
            }
        }

        void SplashDamage(Collider[] hitObjects, Vector3 hitPos, Transform originalHit)
        {
            foreach (Collider c in hitObjects)
            {
                if (c.transform != originalHit)
                {
                    float pcnt = (splashRadius - Vector3.Distance(hitPos, c.transform.position)) / splashRadius;
                    DoDamage(c.transform, (int)Mathf.Ceil(directHitpointsDamage * pcnt));
                }
            }
        }

        void OnCollisionEnter(Collision col) {
            if (PhotonNetwork.isMasterClient) {
                Vector3 hitPosition = col.contacts[0].point;
                Collider[] splashedObjects = Physics.OverlapSphere(hitPosition, splashRadius);
                DoEffects(hitPosition);
                DoDamage(col.transform, directHitpointsDamage);
                SplashDamage(splashedObjects, hitPosition, col.transform);

            }
        }
    }
}
