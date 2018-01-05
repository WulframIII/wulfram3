﻿using Assets.Wulfram3.Scripts.InternalApis.Classes;
using UnityEngine;

namespace Com.Wulfram3
{
    public class CargoManager : Photon.PunBehaviour {
        public AudioClip pickup;
        public AudioClip dropdown;
        public string pickedUpCargo = "";
        public UnitType cargoType;
        public PunTeams.Team cargoTeam;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (photonView.isMine) {     
                if (Input.GetKeyDown("q")) {
                    GetGameManager().PickUpCargo(this);
                    Debug.Log("Picked up cargo: " + pickedUpCargo);
                }

                if (Input.GetKeyDown("z")) {
                    GetGameManager().DropCargo(this);
                }

                if (pickedUpCargo != "") {
                    //TODO: show animation of cargo in player HUD
                }
            }

        }

        [PunRPC]
        public void SetPickedUpCargo(string cargo) {
            pickedUpCargo = cargo;
            if (photonView.isMine) {
                if (cargo != "") {
                    GetComponent<AudioSource>().PlayOneShot(pickup, 1.0f);
                } else {
                    GetComponent<AudioSource>().PlayOneShot(dropdown, 1.0f);
                }
            }
        }

        public GameManager GetGameManager() {
            return FindObjectOfType<GameManager>();
        }
    }
}
