
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
            if (photonView.instantiationData != null)
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

        private void OnCollisionEnter(Collision collision)
        {
            if (PhotonNetwork.isMasterClient)
            {
                CargoManager cm = collision.transform.GetComponent<CargoManager>();
                if (cm != null)
                {
                    PhotonView pv = collision.transform.GetComponent<PhotonView>();
                    if (pv != null)
                    {
                        pv.RPC("GetCargo", PhotonTargets.All, team, content, this.photonView.viewID);
                    }
                }
            }
        }

        [PunRPC]
        public void PickedUp(int senderID, PhotonMessageInfo info)
        {
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log(string.Format("Cargo.cs -- PickedUp() ==> RPC Sender: {0}", PhotonView.Find(senderID).name));
                PhotonNetwork.Destroy(this.gameObject);
            }
        }

        [PunRPC]
        public void SetContent(UnitType content)
        {
            this.content = content;
        }
    }
}
