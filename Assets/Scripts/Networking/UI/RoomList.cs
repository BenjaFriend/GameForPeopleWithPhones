using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{
    public class RoomList : Photon.PunBehaviour
    {
        public bool ShowDebug = true;

        [Tooltip("The prefab that you want the text to be set for showing the list of rooms")]
        public GameObject RoomInfoPrefab;

        private RoomInfo[] _roomList;

        /// <summary>
        /// 
        /// </summary>
        public void RefreshRoomList()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Grab the freshest room list
            _roomList = PhotonNetwork.GetRoomList();
            if (ShowDebug)
                Debug.Log("<color=yellow>[Launcher]</color> Room List length: " + _roomList.Length.ToString());

            // Create a new room list!
            if (RoomInfoPrefab == null)
            {
                Debug.LogError("<color=yellow>[Launcher]</color>Cannot refresh host list, the roomInfoPrefab is null!");
                return;
            }
            foreach (RoomInfo game in _roomList)
            {
                // Display this info to the plyare
                RoomInfoPanel panel = Instantiate(RoomInfoPrefab, transform).GetComponent<RoomInfoPanel>();
                panel.RoomNameText.text = game.Name;
                panel.NumPlayersText.text = game.PlayerCount.ToString() + "/" + game.MaxPlayers.ToString();
                panel.JoinRoombutton.onClick.AddListener(delegate { JoinRoom(game.Name, game.MaxPlayers); });
            }
            if (ShowDebug)
                Debug.Log("<color=yellow>[Launcher]</color> Room list refreshed!");
        }

        public void JoinRoom(string roomName = null, byte maxPlayers = 4)
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = maxPlayers, IsVisible = true }, null);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(Launcher.GameVersion);
            }
        }
    }
}