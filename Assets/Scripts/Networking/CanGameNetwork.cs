using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{
    /// <summary>
    /// Controller for the can game network that will generate the UI we need
    /// </summary>
    public class CanGameNetwork : Photon.PunBehaviour
    {
        [Space(10)]
        public GameObject ConnectedPlayerUIPrefab;
        public Transform ConnectedPlayersParentTransform;
        [Space(10)]
        public Sprite poppedSprite;

        /// <summary>
        /// A list of the can info panels, so that we can eaily pop them.
        /// </summary>
        private List<CanPlayerInfoPanel> _canPlayerPanels = new List<CanPlayerInfoPanel>();

        private void OnEnable()
        {
            PhotonNetwork.OnEventCall += _onEvent;
        }

        private void OnDisable()
        {
            PhotonNetwork.OnEventCall -= _onEvent;
        }

        private void _onEvent(byte eventcode, object content, int senderid)
        {
            if (eventcode == (byte)Constants.EVENT_ID.CAN_BROKE && PhotonNetwork.player.IsMasterClient)
            {
                // Pop that can
                try
                {
                    _canPlayerPanels[senderid].PopCan();
                }
                catch(System.Exception e)
                {
                    DebugString("Could not find that player ID!\n" + e.Message);
                }
            }
        }
        /// <summary>
        /// Update the player list whenever someone new joins
        /// </summary>
        public override void OnJoinedRoom()
        {
            DebugString("Someone has joined a room!");
            UpdateConnectedPlayers();

            base.OnJoinedRoom();
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            DebugString("Someone has joined a room!");
            UpdateConnectedPlayers();
            base.OnPhotonPlayerConnected(newPlayer);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            DebugString("Someone has left a room!");
            UpdateConnectedPlayers();
            base.OnPhotonPlayerDisconnected(otherPlayer);
        }

        /// <summary>
        /// Update the UI element that tells us what players are connected
        /// </summary>
        private void UpdateConnectedPlayers()
        {
            if (ConnectedPlayerUIPrefab == null)
            {
                DebugString("Cannot Updoated connected players UI because the ConnectedPlayerUIPrefab is null!");
                return;
            }
            if (ConnectedPlayersParentTransform == null)
            {
                DebugString("Cannot Updoated connected players UI because the ConnectedPlayersParentTransform is null!");
                return;
            }

            // Delete the current stuff
            _canPlayerPanels.Clear();

            foreach (Transform child in ConnectedPlayersParentTransform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (PhotonPlayer p in PhotonNetwork.playerList)
            {
                // Add this to the connected player UI
                CanPlayerInfoPanel info = Instantiate(ConnectedPlayerUIPrefab, ConnectedPlayersParentTransform).GetComponent<CanPlayerInfoPanel>();
                info.PlayerNameText.text = p.NickName;
                info.ConnIdText.text = p.ID.ToString();

                _canPlayerPanels.Add(info);
            }
        }



        /// <summary>
        /// Simple formatter for a debug log, so that i have unified debug logs and stuff
        /// </summary>
        /// <param name="info"></param>
        private void DebugString(string info)
        {
            Debug.Log("<color=green>[LobbyRoom]</color> " + info);
        }
    }
}