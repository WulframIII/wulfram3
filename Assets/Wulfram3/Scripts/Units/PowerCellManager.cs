using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class PowerCellManager : Photon.PunBehaviour {

        private int maxEnergy = 2;
        private int currentEnergy = 0;

        private List<Transform> powerableObjects = new List<Transform>();
        private List<Transform> poweredObjects = new List<Transform>();
        private float checkStamp = 0;
        private float checkDelay = 1.25f;

        private Unit myUnit;

        void Awake()
        {
            if (PhotonNetwork.isMasterClient)
            {
                myUnit = GetComponent<Unit>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (Time.time > checkStamp)
                {
                    checkStamp = Time.time + checkDelay;
                    for (int i = 0; i < poweredObjects.Count; i++)
                    {
                        if (poweredObjects[i] == null)
                            poweredObjects.RemoveAt(i);
                    }

                    for (int i = 0; i < powerableObjects.Count; i++)
                    {
                        if (powerableObjects[i] == null)
                        {
                            powerableObjects.RemoveAt(i);
                        }
                        else
                        {
                            Unit u = powerableObjects[i].transform.GetComponent<Unit>();
                            if (u.needsPower && !u.hasPower && poweredObjects.Count < maxEnergy)
                            {
                                PhotonView pv = u.transform.GetComponent<PhotonView>();
                                //Debug.Log(transform.name + " powering " + u.transform.name + ". Units Powered: " + (poweredObjects.Count + 1));
                                //u.hasPower = true; // RPC should cover this line
                                poweredObjects.Add(powerableObjects[i]);
                                object[] o = new object[1];
                                o[0] = myUnit.unitTeam;
                                pv.RPC("RecievePower", PhotonTargets.All, o);
                            }
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            for (int i=0; i<poweredObjects.Count; i++)
            {
                if (poweredObjects[i] != null)
                {
                    PhotonView pv = poweredObjects[i].GetComponent<PhotonView>();
                    pv.RPC("LosePower", PhotonTargets.All);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PhotonNetwork.isMasterClient)
            {
                Unit u = other.transform.GetComponent<Unit>();
                if (u != null && u.needsPower && u.unitTeam == myUnit.unitTeam && !powerableObjects.Contains(other.transform))
                {
                    powerableObjects.Add(other.transform);
                }
            }
        }

        /*
        private void OnTriggerStay(Collider other)
        {
            if (PhotonNetwork.isMasterClient)
            {
                Unit u = other.transform.GetComponent<Unit>();
                if (u != null && u.needsPower && u.unitTeam == myUnit.unitTeam && !powerableObjects.Contains(other.transform))
                {
                    powerableObjects.Add(other.transform);
                }
            }
        }*/
    }
}