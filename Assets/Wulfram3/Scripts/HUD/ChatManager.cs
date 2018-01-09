using PhotonChatUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Wulfram3.Scripts.HUD
{
    public class ChatManager : MonoBehaviour
    {
        public static bool isChatOpen;

        public InputField messageBox;
        public GameObject chatPanel;
        private ChatPanelUI chatPanelUI;


        void Start()
        {
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //GetChatPanelUI().Toggle();
                isChatOpen = !isChatOpen;

                Debug.Log("Return Hit:" + isChatOpen);
                if(isChatOpen)
                {
                    messageBox.ActivateInputField();
                }
                else
                {
                    messageBox.DeactivateInputField();
                }
            }
            else if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                chatPanel.GetComponent<ChatUI>().MainDock.ActivatePrevious();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                chatPanel.GetComponent<ChatUI>().MainDock.ActivateNext();
            }
        }
    }
}
