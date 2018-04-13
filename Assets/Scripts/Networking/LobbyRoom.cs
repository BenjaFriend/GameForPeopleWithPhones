﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{
    public class LobbyRoom : Photon.PunBehaviour
    {
        [Header("Player Connection Colors")]
        public Color IsMasterClientColor = Color.blue;
        public Color IsNotMasterColor = Color.gray;
        [Space(10)]
        public GameObject ConnectedPlayerUIPrefab;
        public Transform ConnectedPlayersParentTransform;

        [Space(10)]
        [Header("UI")]
        public GameObject MasterClientOptions;
        public UnityEngine.UI.Text RoomNameText;

        private void Start()
        {
            MasterClientOptions.SetActive(false);

            UpdateConnectedPlayers();
            ShowMasterClientOptions();

            if(RoomNameText != null)
            {
                RoomNameText.text = PhotonNetwork.room.Name;
            }
            else
            {
                DebugString("Cannot set the RoomNameText because it is null!");
            }
        }

        private void ShowMasterClientOptions()
        {
            // Only show the Gud Gud to the master client (hopefully the web view)
            if(!PhotonNetwork.player.IsMasterClient) { return; }

            if(MasterClientOptions == null)
            {
                DebugString("Cannot show contiue because MasterClientOptions is null!");
                return;
            }

            DebugString("Show the level selection options to the master client!");

            MasterClientOptions.SetActive(true);
        }

        /// <summary>
        /// Update the player list whenever someone new joins
        /// </summary>
        public override void OnJoinedRoom()
        {
            DebugString("Someone has joined a room!");
            UpdateConnectedPlayers();
        }

        /// <summary>
        /// Load a level with photon network
        /// </summary>
        /// <param name="levelName">The name of the level</param>
        public void LoadLevel(string levelName)
        {
            PhotonNetwork.LoadLevel(levelName);
        }

        /// <summary>
        /// Load a level with teh photon network
        /// </summary>
        /// <param name="leveindex">Level build ID</param>
        public void LoadLevel(int leveindex)
        {
            PhotonNetwork.LoadLevel(leveindex);
        }

        /// <summary>
        /// Update the UI element that tells us what players are connected
        /// </summary>
        private void UpdateConnectedPlayers()
        {
            if(ConnectedPlayerUIPrefab == null)
            {
                DebugString("Cannot Updoated connected players UI because the ConnectedPlayerUIPrefab is null!");
                return;
            }
            if(ConnectedPlayersParentTransform == null)
            {
                DebugString("Cannot Updoated connected players UI because the ConnectedPlayersParentTransform is null!");
                return;
            }

            // Delete the current stuff
            foreach (Transform child in ConnectedPlayersParentTransform)
            {
                Destroy(child.gameObject);
            }

            foreach (PhotonPlayer p in  PhotonNetwork.playerList)
            {
                // Add this to the connected player UI
                PlayerInfoPanel info = Instantiate(ConnectedPlayerUIPrefab, ConnectedPlayersParentTransform).GetComponent<PlayerInfoPanel>();
                info.PlayerNameText.text = p.NickName;
                info.ConnIdText.text = p.ID.ToString();
                info.MasterClientStatusImage.color = (p.IsMasterClient) ? IsMasterClientColor : IsNotMasterColor;
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