using Assets.InternalApis.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Wulfram3 {
    public class Unit : MonoBehaviour {

        public string team;

        public string name;

        public PunTeams.Team unitTeam;

        public UnitType unitType;

        

        // Use this for initialization
        void Start() {
            Debug.Log(this.ToString());
            switch (team.ToLower())
            {
                case "blue":
                    unitTeam = PunTeams.Team.blue;
                    break;
                case "red":
                    unitTeam = PunTeams.Team.red;
                    break;
                //default:
                //    unitTeam = PunTeams.Team.none;
                //    break;
            }

            switch (name.ToLower())
            {
                case "tank":
                    unitType = UnitType.Tank;
                    break;
                case "scout":
                    unitType = UnitType.Scout;
                    break;
                case "repair pad":
                    unitType = UnitType.RepairPad;
                    break;
                //default:
                //    unitType = UnitType.None;
                //    break;
            }
        }

        // Update is called once per frame
        void Update() {
            Debug.Log(this.ToString());
        }

        public bool IsUnitFriendly()
        {
            if (PlayerMovementManager.LocalPlayerInstance.GetComponent<Unit>().unitTeam == this.unitTeam)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public PunTeams.Team GetHostileTeam()
        {
            switch (unitTeam)
            {
                case PunTeams.Team.red:
                    return PunTeams.Team.blue;

                case PunTeams.Team.blue:
                    return PunTeams.Team.red;
                default:
                    return PunTeams.Team.none;
            }
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(UnitType), unitType) + " " + Enum.GetName(typeof(PunTeams.Team), unitTeam) + "| |" + this.name + " " + this.team;
        }
    }
}
