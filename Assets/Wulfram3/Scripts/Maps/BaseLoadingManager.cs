using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Wulfram3.Scripts.Maps
{
    public class BaseLoadingManager : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        [PunRPC]
        void MapSetup(string mapName)
        {
            if (PhotonNetwork.isMasterClient)
            {
              
            }
        }

        private void CreateBase(Base singleBase)
        {
            foreach (var unit in singleBase.units)
            {
                PhotonNetwork.Instantiate(unit.GetPrefabNameFromUnitType(singleBase.team), new Vector3(unit.posX, unit.posY, unit.posZ), Quaternion.identity, 0);
            }
        }
    }
}