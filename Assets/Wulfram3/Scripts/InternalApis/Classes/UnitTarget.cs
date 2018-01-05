using UnityEngine;

namespace Assets.Wulfram3.Scripts.InternalApis.Classes
{
    public class UnitTarget
    {
        public GameObject Object { get; set; }

        public double LastDistance { get; set; }

        public UnitType ObjectType { get; set; }

        public PunTeams.Team ObjectTeam { get; set; }
    }
}