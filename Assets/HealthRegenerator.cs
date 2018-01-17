using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class HealthRegenerator : Photon.PunBehaviour {

        public float healthPerSecond = 0.7f;
        public float playerLandedBoost = 2.65f;
        public float repairPadBoost = 75f;
        private float boost = 1f;
        private float healthCollected = 0;
        private PlayerMovementManager playerManager;

        // Use this for initialization
        void Start() {
            if (photonView.isMine)
                playerManager = GetComponent<PlayerMovementManager>();
        }

        // Update is called once per frame
        void Update() {
            if (photonView.isMine) {
                boost = 1f;
                if (playerManager != null)
                {
                    if (playerManager.GetIsGrounded() && playerManager.onRepairPad == null)
                    {
                        boost = playerLandedBoost;
                    } else if (playerManager.GetIsGrounded() && playerManager.onRepairPad != null)
                    {
                        boost = repairPadBoost;
                    }
                }
                float health = healthPerSecond * boost * Time.deltaTime;
                healthCollected += health;
                if (healthCollected >= 1f) {
                    healthCollected--;
                    if (playerManager == null || !playerManager.isDead) {
                        GetComponent<HitPointsManager>().TellServerTakeDamage(-1);
                    }                
                }
            }  
        }
    }
}
