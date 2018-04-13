using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace Com.PodSquad.GDPPNF
{ 
    public class MasterScoreViewer : PunBehaviour
    {
        private int _currentCanScore = 0;

        public UnityEngine.UI.Text logInfoText;

        // setup our OnEvent as callback:
        void OnEnable()
        {
            PhotonNetwork.OnEventCall += this.onEvent;
        }
        void OnDisable()
        {
            PhotonNetwork.OnEventCall -= this.onEvent;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("<color=red>[MasterScoreViewer]</color>  OnJoinedRoom() called by PUN. Now this client is in a room. \nRoom name is: " + PhotonNetwork.room.Name);
        }

        // handle custom events:
        private void onEvent(byte eventcode, object content, int senderid)
        {
            Debug.Log("[MasterScoreViewer] We have recieved a riaseEvent!");

            if (eventcode == 0)
            {
                Debug.Log("[MasterScoreViewer] We have recieved a riaseEvent!");
                ++_currentCanScore;

                if(logInfoText != null)
                {
                    logInfoText.text = "Current Score: " + _currentCanScore.ToString();
                }

                PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
                
                byte[] selected = content as byte[];
                for (int i = 0; i < selected.Length; i++)
                {
                    byte unitId = selected[i];
                    // do something
                }
            }
        }

        private void OnGUI()
        {
            string playerNote = (PhotonNetwork.isMasterClient) ? "You are the Master Client" : "Nope";
            GUI.Label(new Rect(10, 100, 150, 20), playerNote);

        }
    }
}