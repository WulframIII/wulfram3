
using Assets.Wulfram3.Scripts.InternalApis.Classes;
using UnityEngine;
using System.Collections;


namespace Com.Wulfram3
{
    public class Cargo : Photon.PunBehaviour
    { 
        public PunTeams.Team team;
        public UnitType content;
        private float pickupDelay = 1f;
        private float lifeStamp;


        void Start()
        {
            if (photonView.instantiationData != null) // photonView.instantiation data contains an object passed along with the Instantiate() call
            {
                content = (UnitType) photonView.instantiationData[0];
                team = (PunTeams.Team)photonView.instantiationData[1];
            } else {
                Unit u = GetComponent<Unit>();
                if (u != null && u.unitTeam == PunTeams.Team.none) // Checking for team == none allows cargo to be pre-deployed and maintain scene settings
                {
                    team = u.unitTeam;
                    content = UnitType.PowerCell;
                }
            }
            lifeStamp = Time.time;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (PhotonNetwork.isMasterClient && Time.time > lifeStamp)
            {
                CargoManager cm = collision.transform.GetComponent<CargoManager>();
                PhotonView   pv = collision.transform.GetComponent<PhotonView>();
                if (pv != null && cm != null && !cm.hasCargo)
                {
                    // This RPC maintains the team of the box
                    //pv.RPC("GetCargo", PhotonTargets.All, team, content, this.photonView.viewID);

                    // This RPC "unlocks" enemy cargo on pickup.
                    pv.RPC("GetCargo", PhotonTargets.All, cm.myTeam, content, this.photonView.viewID);
                }
            }
        }
    }
}
