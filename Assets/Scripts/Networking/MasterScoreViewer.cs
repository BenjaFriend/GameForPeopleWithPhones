using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{ 
    public class MasterScoreViewer : Photon.PunBehaviour
    {
        // setup our OnEvent as callback:
        void OnEnable()
        {
            PhotonNetwork.OnEventCall += this.OnEvent;
        }
        void OnDisable()
        {
            PhotonNetwork.OnEventCall -= this.OnEvent;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("<color=red>[MasterScoreViewer]</color>  OnJoinedRoom() called by PUN. Now this client is in a room. \nRoom name is: " + PhotonNetwork.room.Name);
        }

        // handle custom events:
        void OnEvent(byte eventcode, object content, int senderid)
        {
            Debug.Log("[MasterScoreViewer] We have recieved a riaseEvent!");

            if (eventcode == 0)
            {
                Debug.Log("[MasterScoreViewer] We have recieved a riaseEvent!");

                PhotonPlayer sender = PhotonPlayer.Find(senderid);  // who sent this?
                byte[] selected = content as byte[];
                for (int i = 0; i < selected.Length; i++)
                {
                    byte unitId = selected[i];
                    // do something
                }
            }
        }
    }
}