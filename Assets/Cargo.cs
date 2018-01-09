
using Assets.Wulfram3.Scripts.InternalApis.Classes;
using UnityEngine;
using System.Collections;


namespace Com.Wulfram3
{
    public class Cargo : Photon.PunBehaviour
    { 

        public PunTeams.Team team;
        public UnitType content;

        void Start()
        {
            if (photonView.instantiationData.Length > 0)
            {
                content = (UnitType) photonView.instantiationData[0];
                team = (PunTeams.Team)photonView.instantiationData[1];
            } else { 
                Unit u = GetComponent<Unit>();
                if (u != null)
                {
                    team = u.unitTeam;
                    content = UnitType.PowerCell;
                } else
                {
                    Debug.Log("Cargo.cs Attached, but missing Unit component.");
                }
            }
        }

        [PunRPC]
        public void SetContent(UnitType content)
        {
            this.content = content;
        }
    }
}
