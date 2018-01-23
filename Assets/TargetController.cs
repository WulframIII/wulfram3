using Assets.Wulfram3.Scripts.HUD;
using Assets.Wulfram3.Scripts.InternalApis.Classes;
using Greyman;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Wulfram3
{
    public class TargetController : Photon.PunBehaviour {
        //Old Wulfram key bindings
        //bind "targeting" "ctrl-F5" "store_target 0"
        //bind "targeting" "ctrl-F6" "store_target 1"
        //bind "targeting" "ctrl-F7" "store_target 2"
        //bind "targeting" "ctrl-F8" "store_target 3"

        //bind "targeting" "F5" "retrieve_target 0"
        //bind "targeting" "F6" "retrieve_target 1"
        //bind "targeting" "F7" "retrieve_target 2"
        //bind "targeting" "F8" "retrieve_target 3"

        //bind "targeting" "`" "clear_target"
        //bind "targeting" "tab" "target_cycle nearest frontward major_object"
        //bind "targeting" "u" "target_cycle nearest under_reticle any_object"
        //bind "targeting" "y" "target_cycle"

        //bind "targeting" "g" "target_cycle nearest friendly energy"
        //bind "targeting" "alt-g" "target_cycle nearest hostile energy"

        //bind "targeting" "r" "target_cycle nearest friendly repair"
        //bind "targeting" "alt-r" "target_cycle nearest hostile repair"

        //bind "targeting" "f" "target_cycle nearest friendly fuel"
        //bind "targeting" "alt-f" "target_cycle nearest hostile fuel"

        //bind "targeting" "c" "target_cycle nearest friendly cargo"
        //bind "targeting" "alt-c" "target_cycle nearest friendly cargo"

        //bind "targeting" "t" "target_only nearest hostile player"
        //bind "targeting" "alt-t" "target_only nearest friendly player"

        //bind "targeting" "i" "target_only nearest hostile hunter_missile"
        //bind "targeting" "l" "target_cycle nearest friendly uplink"

        //bind "targeting" "h" "target_cycle nearest most_damaged friendly major_object"
        //bind "targeting" "shift-h" "target_cycle most_damaged friendly major_object"
        //bind "targeting" "alt-h" "target_cycle nearest most_damaged hostile major_object"
        public GameObject currentPlayerTarget;

        private GameManager gameManager;
        public GameObject[] targets;
        private int currentTarget;
        private int totalTargets;

        public Transform target;
        public Texture2D image;

        Vector3 point;

        private GameObject storedTarget0;
        private GameObject storedTarget1;
        private GameObject storedTarget2;
        private GameObject storedTarget3;

        


        // Use this for initialization
        void Start() {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update() {
            if (!photonView.isMine)
                return;


            if (ChatManager.isChatOpen)
            {
                return;
            }
            // Original code
            ////if (Input.GetKeyDown(KeyCode.T)) {
            ////    Vector3 pos = transform.position + (transform.forward * 2.0f + transform.up * 0.2f);
            ////    Quaternion rotation = transform.rotation;

            ////    RaycastHit objectHit;
            ////    bool targetFound = Physics.Raycast(pos, transform.forward, out objectHit, 300) && objectHit.transform.GetComponent<Unit>() != null;
            ////    if (targetFound) {
            ////        this.SetThisAsPlayerCurrentTarget(objectHit.transform.gameObject);
            ////    }
            ////}

            bool doNotExecuteRetrives = false;

            // Storing Targets
            //bind "targeting" "ctrl-F5" "store_target 0"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F5))
            {
                doNotExecuteRetrives = true;
                StoreTarget(this.currentPlayerTarget, 0);
                Debug.Log("store_target 0");
            }

            //bind "targeting" "ctrl-F6" "store_target 1"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F6))
            {
                doNotExecuteRetrives = true;
                StoreTarget(this.currentPlayerTarget, 1);
                Debug.Log("store_target 1");
            }

            //bind "targeting" "ctrl-F7" "store_target 2"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F7))
            {
                doNotExecuteRetrives = true;
                StoreTarget(this.currentPlayerTarget, 2);
                Debug.Log("store_target 2");
            }

            //bind "targeting" "ctrl-F8" "store_target 3"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F8))
            {
                doNotExecuteRetrives = true;
                StoreTarget(this.currentPlayerTarget, 3);
                Debug.Log("store_target 3");
            }

            //bind "targeting" "F5" "retrieve_target 0"
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (doNotExecuteRetrives) return;
                RetrieveStoredTarget(0);
                Debug.Log("retrieve_target 0");
            }

            //bind "targeting" "F6" "retrieve_target 1"
            if (Input.GetKeyDown(KeyCode.F6))
            {
                if (doNotExecuteRetrives) return;
                RetrieveStoredTarget(1);
                Debug.Log("retrieve_target 1");
            }

            //bind "targeting" "F7" "retrieve_target 2"
            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (doNotExecuteRetrives) return;
                RetrieveStoredTarget(2);
                Debug.Log("retrieve_target 2");
            }

            //bind "targeting" "F8" "retrieve_target 3"
            if ( Input.GetKeyDown(KeyCode.F8))
            {
                if (doNotExecuteRetrives) return;
                RetrieveStoredTarget(3);
                Debug.Log("retrieve_target 3");
            }



            //Main Targeting
            //bind "targeting" "`" "clear_target"
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Debug.Log("clear_target");
                gameManager.SetCurrentTarget(null);
            }

            //bind "targeting" "y" "target_cycle"
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log("target_cycle");
            }

            //bind "targeting" "tab" "target_cycle nearest frontward major_object"
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));//transform.position + (transform.forward * 2.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;

                RaycastHit objectHit;
                bool targetFound = Physics.Raycast(pos, transform.forward, out objectHit, 300) && objectHit.transform.GetComponent<Unit>() != null;
                if (targetFound)
                {
                    this.SetThisAsPlayerCurrentTarget(objectHit.transform.gameObject);
                }
                Debug.Log("target_cycle nearest frontward major_object");
            }

            //bind "targeting" "u" "target_cycle nearest under_reticle any_object"
            if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("target_cycle nearest under_reticle any_object");
            }

            //bind "targeting" "h" "target_cycle nearest most_damaged friendly major_object"
            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest most_damaged friendly major_object");
            }

            //bind "targeting" "shift-h" "target_cycle most_damaged friendly major_object"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle most_damaged friendly major_object");
            }

            //bind "targeting" "alt-h" "target_cycle nearest most_damaged hostile major_object"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest most_damaged hostile major_object");
            }



            //Unit Targeting
            //bind "targeting" "g" "target_cycle nearest friendly energy"
            if (Input.GetKeyDown(KeyCode.G))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.PowerCell, true));
                Debug.Log("target_cycle nearest friendly energy");
            }

            //bind "targeting" "alt-g" "target_cycle nearest hostile energy"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.PowerCell, false));
                Debug.Log("target_cycle nearest hostile energy");
            }

            //bind "targeting" "r" "target_cycle nearest friendly repair"
            if (Input.GetKeyDown(KeyCode.R))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.RepairPad, true));
                Debug.Log("target_cycle nearest friendly repair");
            }

            //bind "targeting" "alt-r" "target_cycle nearest hostile repair"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.R))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.RepairPad, false));
                Debug.Log("target_cycle nearest hostile repair");
            }

            //bind "targeting" "f" "target_cycle nearest friendly fuel"
            if (Input.GetKeyDown(KeyCode.F))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.RefuelPad, true));
                Debug.Log("target_cycle nearest friendly fuel");
            }

            //bind "targeting" "alt-f" "target_cycle nearest hostile fuel"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.RefuelPad, false));
                Debug.Log("target_cycle nearest hostile fuel");
            }

            //bind "targeting" "c" "target_cycle nearest friendly cargo"
            if (Input.GetKeyDown(KeyCode.C))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.Cargo, true));
                Debug.Log("target_cycle nearest friendly cargo");
            }

            //bind "targeting" "alt-c" "target_cycle nearest hostile cargo"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.C))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.Cargo, false));
                Debug.Log("target_cycle nearest hostile cargo");
            }

            //bind "targeting" "t" "target_only nearest hostile player"
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("target_only nearest hostile player");
            }

            //bind "targeting" "alt-t" "target_only nearest friendly player"
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("target_only nearest friendly player");
            }

            //bind "targeting" "i" "target_only nearest hostile hunter_missile"
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("target_only nearest hostile hunter_missile");
            }

            //bind "targeting" "l" "target_cycle nearest friendly uplink"
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.SetThisAsPlayerCurrentTarget(this.GetTarget(UnitType.Uplink, true));
                Debug.Log("target_cycle nearest friendly uplink");
            }
        }

        private GameObject GetTarget(UnitType type, bool isTargetFriendly)
        {        
            var tempTargets = new List<UnitTarget>();
            var units = (Unit[])GameObject.FindObjectsOfType(typeof(Unit));
            if (units.Length == 0)
            {
                return null;
            }
            else 
            {
                Debug.Log("units count:" + units.Length + "Find Friendly?:" + isTargetFriendly);
                foreach (var item in units)
                {
                    PunTeams.Team findTeam;
                    if(isTargetFriendly)
                    {
                        // Find units for same team
                        findTeam = PlayerMovementManager.LocalPlayerInstance.GetComponent<Unit>().unitTeam;
                    }
                    else
                    {
                        // Find units for hostile team
                        findTeam = PlayerMovementManager.LocalPlayerInstance.GetComponent<Unit>().GetHostileTeam(); ;
                    }

                    PlayerMovementManager player = PlayerMovementManager.LocalPlayerInstance.GetComponent<PlayerMovementManager>();
                    if (item.unitTeam == findTeam)
                    {
                        if (type == UnitType.None)
                        {
                            // Select any Units
                            tempTargets.Add(new UnitTarget
                            {
                                Object = item.gameObject,
                                ObjectTeam = item.unitTeam,
                                ObjectType = item.unitType,
                                LastDistance = Math.Round(Vector3.Distance(item.gameObject.transform.position, player.transform.position), 0)
                            });
                        }
                        else
                        {
                            if (item.unitType == type)
                            {
                                // Select only Units of a type
                                tempTargets.Add(new UnitTarget
                                {
                                    Object = item.gameObject,
                                    ObjectTeam = item.unitTeam,
                                    ObjectType = item.unitType,
                                    LastDistance = Math.Round(Vector3.Distance(item.gameObject.transform.position, player.transform.position), 0)
                                });
                            }
                        }
                    }
                    
                }

                Debug.Log("tempTargets count:" + tempTargets.Count);
                if(tempTargets.Count > 0)
                {
                    var min = tempTargets.Where(c => c.LastDistance > 0).Min(c => c.LastDistance);

                    return tempTargets.Where(entry => entry.LastDistance == min).FirstOrDefault().Object;
                }
                else
                {
                    return null;
                }

            }
        }



        private void StoreTarget(GameObject gameObject, int position)
        {
            switch (position)
            {
                case 0:
                    storedTarget0 = gameObject;
                    break;
                case 1:
                    storedTarget1 = gameObject;
                    break;
                case 2:
                    storedTarget2 = gameObject;
                    break;
                case 3:
                    storedTarget3 = gameObject;
                    break;
                default:
                    break;
            }
        }

        private void RetrieveStoredTarget(int position)
        {
            switch (position)
            {
                case 0:
                    this.SetThisAsPlayerCurrentTarget(storedTarget0);
                    break;
                case 1:
                    this.SetThisAsPlayerCurrentTarget(storedTarget1);
                    break;
                case 2:
                    this.SetThisAsPlayerCurrentTarget(storedTarget2);
                    break;
                case 3:
                    this.SetThisAsPlayerCurrentTarget(storedTarget3);
                    break;
                default:
                    break;
            }
        }

        private void SetThisAsPlayerCurrentTarget(GameObject gameObject)
        {
            this.currentPlayerTarget = gameObject;
            gameManager.SetCurrentTarget(gameObject);
        }

    }


}
