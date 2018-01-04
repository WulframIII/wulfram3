using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
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

        private GameManager gameManager;
        public GameObject[] targets;

        private int currentTarget;
        private int totalTargets;

        public Transform target;
        public Texture2D image;

        Vector3 point;

        // Use this for initialization
        void Start() {
            gameManager = FindObjectOfType<GameManager>();
            targets = GameObject.FindGameObjectsWithTag("Unit");
            var units = (Unit[])GameObject.FindObjectsOfType(typeof(Unit));
        }

        // Update is called once per frame
        void Update() {
            if (!photonView.isMine)
                return;

            if (Input.GetKeyDown(KeyCode.T)) {
                Vector3 pos = transform.position + (transform.forward * 2.0f + transform.up * 0.2f);
                Quaternion rotation = transform.rotation;

                RaycastHit objectHit;
                bool targetFound = Physics.Raycast(pos, transform.forward, out objectHit, 300) && objectHit.transform.GetComponent<Unit>() != null;
                if (targetFound) {
                    gameManager.SetCurrentTarget(objectHit.transform.gameObject);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentTarget = currentTarget + 1 % targets.Length;
                //target = targets[currentTarget];
                gameManager.SetCurrentTarget(targets[currentTarget]);           
            }


            // Storing Targets
            //bind "targeting" "ctrl-F5" "store_target 0"
            if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("store_target 0");
            }

            //bind "targeting" "ctrl-F6" "store_target 1"
            if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F6))
            {
                Debug.Log("store_target 1");
            }

            //bind "targeting" "ctrl-F7" "store_target 2"
            if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F7))
            {
                Debug.Log("store_target 2");
            }

            //bind "targeting" "ctrl-F8" "store_target 3"
            if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F8))
            {
                Debug.Log("store_target 3");
            }

            //bind "targeting" "F5" "retrieve_target 0"
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("retrieve_target 0");
            }

            //bind "targeting" "F6" "retrieve_target 1"
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Debug.Log("retrieve_target 1");
            }

            //bind "targeting" "F7" "retrieve_target 2"
            if (Input.GetKeyDown(KeyCode.F7))
            {
                Debug.Log("retrieve_target 2");
            }

            //bind "targeting" "F8" "retrieve_target 3"
            if ( Input.GetKeyDown(KeyCode.F8))
            {
                Debug.Log("retrieve_target 3");
            }



            //Main Targeting
            //bind "targeting" "`" "clear_target"
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                Debug.Log("clear_target3");
            }

            //bind "targeting" "y" "target_cycle"
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log("target_cycle");
            }

            //bind "targeting" "tab" "target_cycle nearest frontward major_object"
            if (Input.GetKeyDown(KeyCode.Tab))
            {
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
            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle most_damaged friendly major_object");
            }

            //bind "targeting" "alt-h" "target_cycle nearest most_damaged hostile major_object"
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest most_damaged hostile major_object");
            }



            //Unit Targeting
            //bind "targeting" "g" "target_cycle nearest friendly energy"
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("target_cycle nearest friendly energy");
            }

            //bind "targeting" "alt-g" "target_cycle nearest hostile energy"
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest hostile energy");
            }

            //bind "targeting" "r" "target_cycle nearest friendly repair"
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("target_cycle nearest friendly repair");
            }

            //bind "targeting" "alt-r" "target_cycle nearest hostile repair"
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest hostile repair");
            }

            //bind "targeting" "f" "target_cycle nearest friendly fuel"
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("target_cycle nearest friendly fuel");
            }

            //bind "targeting" "alt-f" "target_cycle nearest hostile fuel"
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest hostile fuel");
            }

            //bind "targeting" "c" "target_cycle nearest friendly cargo"
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("target_cycle nearest friendly cargo");
            }

            //bind "targeting" "alt-c" "target_cycle nearest hostile cargo"
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log("target_cycle nearest hostile cargo");
            }

            //bind "targeting" "t" "target_only nearest hostile player"
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("target_only nearest hostile player");
            }

            //bind "targeting" "alt-t" "target_only nearest friendly player"
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
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
                Debug.Log("target_cycle nearest friendly uplink");
            }
        }
    }
}
