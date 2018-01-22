/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using Photon;
using UnityEngine;

namespace PhotonChatUI
{
    public class ChatUIAutoLogin : PunBehaviour
    {
        bool ischatConnected = false;
        private ChatUI _chatUI;

        public ChatUI chatUI
        {
            get { return _chatUI ?? (_chatUI = GetComponent<ChatUI>()); }
        }

        void Update()
        {
            //Debug.Log(Chat.Instance.State);
            if (Chat.Instance.State == ExitGames.Client.Photon.Chat.ChatState.Disconnected)
            {
                ischatConnected = false;
            }

            if (ischatConnected == false)
            {
                base.OnJoinedRoom();
                Debug.Log("Joined Room CHAT");
                chatUI.Connect(PhotonNetwork.playerName);
                    Debug.Log("Chat Player Connected");
                ischatConnected = true;
                var team = PhotonNetwork.player.GetTeam();
                if(team == PunTeams.Team.Blue)
                {
                    chatUI.CreatePublicChannel("Azure Alliance");
                }
                else
                {
                    chatUI.CreatePublicChannel("Crimson Federation");
                }     
            }
        }
    }
}
