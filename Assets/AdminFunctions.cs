using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Wulfram3;
using Photon;

public class AdminFunctions : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
		
	}


    [PunRPC]
    private void KickPlayer()
    {
        PhotonNetwork.LeaveRoom(); // load lobby scene, returns to master server
    }

    public void SendPlayerKick(string name) {
        Debug.Log("Player List: ");
        foreach (PhotonPlayer player in PhotonNetwork.playerList) {
            string s = player.NickName;
             if (s != null && s == name) {
                Debug.Log("Kicking player :" + s);
                //PhotonNetwork.CloseConnection(player);
                photonView.RPC("KickPlayer", player);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SendKickPlayer(int playerID)
    {
        PhotonPlayer player = PhotonPlayer.Find(playerID);

        photonView.RPC("KickPlayer", player);
    }
}
