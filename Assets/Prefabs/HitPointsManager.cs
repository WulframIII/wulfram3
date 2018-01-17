using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class HitPointsManager : Photon.PunBehaviour {

        public int initialHealth = 100;
        public int maxHealth = 100;


        [HideInInspector]
        public int health;
        private GameManager gameManager;

        //private int lastUpdateHealth = 0;

        // Use this for initialization
        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            if (PhotonNetwork.isMasterClient)
            {
                SetHealth(initialHealth);
            }
        }

        // Update is called once per frame
        void Update()
        {
            /*
             * TODO: Implement onDemand Update
            if (PhotonNetwork.isMasterClient && health != lastUpdateHealth)
            {
                lastUpdateHealth = health;
                object[] o = new object[1];
                o[0] = health;
                photonView.RPC("UpdateHealth", PhotonTargets.All, o);
            }
            */
        }

        [PunRPC]
        public void TakeDamage(int amount) {
            if (PhotonNetwork.isMasterClient) {
                int newHealth = Mathf.Clamp(health - amount, 0, maxHealth);
                SetHealth(newHealth);
            }
        }

        [PunRPC]
        public void UpdateHealth(int amount) {
            int newHealth = Mathf.Clamp(amount, 0, maxHealth);
            health = newHealth;
            gameManager.UnitsHealthUpdated(this);
        }

        public void SetHealth(int newHealth) {
            if (PhotonNetwork.isMasterClient) {
                photonView.RPC("UpdateHealth", PhotonTargets.AllBuffered, newHealth);
            }
        }

        [PunRPC]
        public void SetHealthFromClient(int newHealth)
        {
            // Can do validation here later
            SetHealth(newHealth);
        }   

        public void TellServerHealth(int newHealth)
        {
            photonView.RPC("SetHealthFromClient", PhotonTargets.AllBuffered, newHealth);
        }

        public void TellServerTakeDamage(int amount) {
            photonView.RPC("TakeDamage", PhotonTargets.MasterClient, amount);
        }

    }
}
