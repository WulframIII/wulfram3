using Assets.Wulfram3.Scripts.InternalApis.Classes;
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
