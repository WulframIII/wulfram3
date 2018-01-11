using Assets.Wulfram3.Scripts.InternalApis.Classes;
using System;
using UnityEngine;

namespace Com.Wulfram3
{
    public class Unit : MonoBehaviour {


        public PunTeams.Team unitTeam;

        public UnitType unitType;

        // Use this for initialization
        void Start() {
            if (unitTeam == null)
            {
                Debug.Log("Unit.cs is missing a team assignment.");
            }
            if (unitType == null)
            {
                Debug.Log("Unit.cs is missing a type assignment.");
            }
        }

        // Update is called once per frame
        void Update() {
            //Debug.Log(this.ToString());
        }

        public string GetTypeString()
        {
            switch(unitType)
            {
                case UnitType.Cargo:
                    return "Cargo Box";
                case UnitType.Darklight:
                    return "Darklight";
                case UnitType.FlakTurret:
                    return "Flak Turret";
                case UnitType.GunTurret:
                    return "Gun Turret";
                case UnitType.MissleLauncher:
                    return "Missile Launcher";
                case UnitType.PowerCell:
                    return "Powercell";
                case UnitType.RefuelPad:
                    return "Refuel Pad";
                case UnitType.RepairPad:
                    return "Repair Pad";
                case UnitType.Scout:
                    return "Scout";
                case UnitType.Skypump:
                    return "Skypump";
                case UnitType.Tank:
                    return "Tank";
                case UnitType.Unlink:
                    return "Uplink";
                case UnitType.None:
                    return "UnitType.None";
                default: return "UnitType.ERROR";
            }
        }

        public string GetTeamString()
        {
            if (unitTeam == null)
                return "unitTeam.ERROR";
            if (unitTeam == PunTeams.Team.blue)
                return "Blue";
            if (unitTeam == PunTeams.Team.red)
                return "Red";
            return "Grey";
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
            return Enum.GetName(typeof(UnitType), unitType) + " " + Enum.GetName(typeof(PunTeams.Team), unitTeam) + "| |" + this.unitType.ToString() + " " + this.unitTeam.ToString();
        }
    }
}
