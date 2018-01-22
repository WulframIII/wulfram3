using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3
{

    public class CargoShooter : Photon.PunBehaviour
    {
        private float fireStamp;
        private float turnStamp;
        private float turnDelay = 2f;
        private float fireDelay = 10f;
        public Transform[] cargoTypes;
        public Transform Turret;
        public Transform gunEnd;
        private bool rotating = false;
        private Quaternion desiredRotation;
        private float turnSpeed;
        private List<Transform> cargoList = new List<Transform>();

        // Use this for initialization
        void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                turnStamp = turnDelay;
                fireStamp = fireDelay;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!PhotonNetwork.isMasterClient)
                return;
            if (Time.time > turnStamp)
            {
                if (rotating)
                {
                    Turret.rotation = Quaternion.Slerp(Turret.rotation, desiredRotation, turnSpeed * Time.deltaTime);
                    if (Quaternion.Angle(Turret.rotation, desiredRotation) <= 5f)
                    {
                        rotating = false;
                        turnStamp = Time.time + turnDelay;
                    }

                }
                else
                {
                    CleanCargoList();
                    rotating = true;
                    Quaternion tr = Quaternion.identity;
                    Vector3 tv = tr.eulerAngles;
                    tv.y = Random.Range(0, 359);
                    tr.eulerAngles = tv;
                    desiredRotation = tr;
                    turnSpeed = Random.Range(0.5f, 3f);
                }
            }
            if (Time.time > fireStamp && cargoList.Count < 8)
            {
                fireStamp = Time.time + fireDelay;
                FireCargo();
            }
        }

        private void CleanCargoList()
        {
            for (int i=0; i<cargoList.Count; i++)
            {
                if(cargoList[i] == null)
                {
                    cargoList.RemoveAt(i);
                }
            }
        }

        void FireCargo()
        {
            float rTn = Random.Range(0, 2);
            PunTeams.Team randomTeam = PunTeams.Team.none;
            if (rTn == 0)
            {
                randomTeam = PunTeams.Team.Blue;
            } else if (rTn == 1)
            {
                randomTeam = PunTeams.Team.Red;
            }
            string prefabName = Unit.GetPrefabName(UnitType.Cargo, randomTeam);
            UnitType randomUnit;
            float rUn = Random.Range(0, 10000);
            if (rUn < 100)
            {
                randomUnit = UnitType.RepairPad;
            }
            else if (rUn >= 100 && rUn < 500)
            {
                randomUnit = UnitType.MissleLauncher;
            } else if (rUn >= 500 && rUn < 1000)
            {
                randomUnit = UnitType.Darklight;
            } else if (rUn >= 1000 && rUn < 5000)
            {
                randomUnit = UnitType.PowerCell;
            } else if (rUn >= 5000 && rUn < 7500)
            {
                randomUnit = UnitType.FlakTurret;
            } else
            {
                randomUnit = UnitType.GunTurret;
            }

            object[] o = new object[2];
            o[0] = randomUnit;
            o[1] = randomTeam;
            /* Given that this is sort of a gimmick unit, and that we shouldn't get here without 
             * being masterClient, it should be okay to instantiate without gameManager */
            float rV = Random.Range(12, 24);
            GameObject g = PhotonNetwork.InstantiateSceneObject(prefabName, gunEnd.position, gunEnd.rotation, 0, o);
            g.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * rV, ForceMode.Impulse);
            cargoList.Add(g.transform);
        }
    }
}
