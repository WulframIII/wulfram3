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
