using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class PulseShellManager : Photon.PunBehaviour {
        public float velocity = 30;
        public int directHitpointsDamage = 200;

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
            lifetimer += Time.deltaTime;
            if (lifetimer >= lifetime)
            {
                DoEffects(transform.position);
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
            if (unit != null && unit.unitTeam != team)
            {
                HitPointsManager hpm = target.GetComponent<HitPointsManager>();
                if (hpm != null)
                {
                    hpm.TellServerTakeDamage(amount);
                }
            }
        }


        void OnCollisionEnter(Collision col) {
            if (PhotonNetwork.isMasterClient) {
                Vector3 hitPosition = col.contacts[0].point;
                Collider[] splashedObjects = Physics.OverlapSphere(hitPosition, 6f);
                Unit unit = col.gameObject.GetComponent<Unit>();
                DoEffects(hitPosition);
                DoDamage(col.transform, directHitpointsDamage);
                foreach (Collider c in splashedObjects)
                {
                    if (c.transform != col.gameObject.transform)
                    {
                        DoDamage(c.transform, (int)Mathf.Ceil(directHitpointsDamage / Vector3.Distance(hitPosition, c.transform.position)));
                    }
                }
            }
        }
    }
}
