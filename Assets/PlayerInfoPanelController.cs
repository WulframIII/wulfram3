﻿using Assets.Wulfram3.Scripts.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Wulfram3 {
    public class PlayerInfoPanelController : Photon.PunBehaviour {
        private GameObject target;
        private Vector3 pos;
        private GameManager gameManager;

        public Text playerNameText;


        // Use this for initialization
        void Start() {
            Canvas canvas = FindObjectOfType<Canvas>();
            transform.SetParent(canvas.transform);
            gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void LateUpdate() {
            var isMeshVisable = target.GetComponentInChildren<MeshRenderer>().isVisible;
            var isMapIconVisable = target.GetComponent<KGFMapIcon>().GetIsVisible();



            if (target != null && isMeshVisable && isMapIconVisable) {
                playerNameText.gameObject.SetActive(false);
                pos = Camera.main.WorldToScreenPoint(target.transform.position);
                pos.z = 0;
                RectTransform rectTransform = GetComponent<RectTransform>();
                pos.y += 50;

                string playerName = target.GetComponent<PhotonView>().owner.NickName;
                string hitpoints = target.GetComponent<HitPointsManager>().health + "/" + target.GetComponent<HitPointsManager>().maxHealth;

                var name = gameManager.GetColoredPlayerName(playerName, target.GetComponent<PhotonView>().owner.IsMasterClient, true, target.GetComponent<Unit>().unitTeam);
                playerNameText.text = name;

                rectTransform.SetPositionAndRotation(pos, rectTransform.rotation);
            } else {
                playerNameText.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update() {
 
        }

        public void SetTarget(GameObject target) {
            this.target = target;
        }

    }
}