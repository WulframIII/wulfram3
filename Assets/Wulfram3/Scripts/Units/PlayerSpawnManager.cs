using Assets.Wulfram3.Scripts.InternalApis.Classes;
using Com.Wulfram3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Wulfram3.Scripts.Units
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        public static SpawnStatus status;

        public static float currentSpawnTime = 30.0f; // in seconds

        public Text countdownText;


        public void StartSpawn()
        {
            status = SpawnStatus.IsSpawning;
        }

        public static void SpawnPlayer(Vector3 spawnPoint)
        {
            PlayerMovementManager player = PlayerMovementManager.LocalPlayerInstance.GetComponent<PlayerMovementManager>();
            //if (player.isDead)
            //{
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                player.photonView.RPC("SetPosAndRotation", PhotonTargets.All, spawnPoint + new Vector3(0, 50, 0), Quaternion.identity);

                HitPointsManager hitpointsManager = player.GetComponent<HitPointsManager>();
                hitpointsManager.TellServerHealth(hitpointsManager.maxHealth);

                player.GetComponent<FuelManager>().ResetFuel();
                PlayerSpawnManager.status = SpawnStatus.IsAlive;
            //}
        }


        void Update()
        {
            if(status == SpawnStatus.IsSpawning)
            {
                currentSpawnTime -= Time.deltaTime;
                if (currentSpawnTime < 0)
                {
                    status = SpawnStatus.IsReady;
                    currentSpawnTime = 30.0f; // in seconds
                    countdownText.text = "READY FOR DEPLOYMENT!";
                }
                else
                {
                    countdownText.text = Math.Round(currentSpawnTime, 0).ToString();
                }
            }
            
        }
    }


}
