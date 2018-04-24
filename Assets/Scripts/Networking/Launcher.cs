
using System;
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{
    /// <summary>
    /// Behvaior for the launcher scene that either auto creates a server, 
    /// or provide the functionality to the client to join a room and 
    /// set their player name
    /// </summary>
    public class Launcher : Photon.PunBehaviour
    {
        #region Fields

        public bool ShowDebug = true;

        private static string roomNamePrefKey = "RoomName";

        [Header("Network UI")]
        [Tooltip("The UI panel to lket the user enter their name, connect and play")]
        public GameObject[] ControlPanels;
        [Tooltip("The UI Label to inform the user htat the connection is in progress")]
        public GameObject[] ProgressLabels;
        [Tooltip("A text element that we want to use to tell the client if something has gone wrong")]
        public UnityEngine.UI.Text ClientErrorText;

        [Space(10)]
        public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        public byte MaxPlayersPerRoom = 4;

        /// <summary>
        /// This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).
        /// </summary>
        private static string _gameVersion = "1";
        private string _roomName = null;

        public static string GameVersion { get { return _gameVersion;  } }


        #endregion

        #region Private Methods

        private void Awake()
        {
            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;

            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;

            // Force the log level to this
            PhotonNetwork.logLevel = Loglevel;
        }

        private void Start()
        {
            foreach(GameObject g in ProgressLabels)
            {
                g.SetActive(false);
            }
            foreach(GameObject g in ControlPanels)
            {
                g.SetActive(true);
            }

            // Automatically connect to the master server on start
            if (!PhotonNetwork.connected)
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        /// <summary>
        /// If we are running this on the desktop, then automatically start a server
        /// with a random set of letters
        /// </summary>
        private void _autoStartServer()
        {
            SetRoomName(_generateRandomRoomName());

            if(ShowDebug)
                Debug.Log("<color=yellow>[Launcher]</color> Auto-create a server with name " + _roomName);

            CreateSpecificRoom();
        }

        /// <summary>
        /// Generated a random assortment of letters
        /// </summary>
        /// <returns>A string of length 5 to be used as a random room name</returns>
        private string _generateRandomRoomName()
        {
            // Generate the name
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[5];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        /// <summary>
        /// try and create a room with the given room name
        ///  - Create the room with a specific name
        ///  - Show the "Connecting" UI
        /// </summary>
        private void CreateSpecificRoom()
        {
            // Setup the UI as necessary
            foreach (GameObject g in ProgressLabels)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in ControlPanels)
            {
                g.SetActive(false);
            }

            // Create e room for this
            if (PhotonNetwork.connected)
            {
                RoomOptions options = new RoomOptions()
                {
                    MaxPlayers = MaxPlayersPerRoom,
                    IsVisible = true
                    // TODO: Make a hashtable for this data??
                };

                PhotonNetwork.CreateRoom(_roomName, options, null);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempt to join a room with the given name.
        /// - If we are not connected, then we need to 
        /// </summary>
        public void JoinSpecificRoom()
        {
            // Setup the UI as necessary
            foreach (GameObject g in ProgressLabels)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in ControlPanels)
            {
                g.SetActive(false);
            }

            // Create e room for this
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRoom(_roomName);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        /// <summary>
        /// Used by the input field to set the room name
        /// </summary>
        /// <param name="value"></param>
        public void SetRoomName(string value)
        {
            _roomName = value;

            PlayerPrefs.SetString(roomNamePrefKey, value);
        }

        #endregion

        #region Photon.PunBehaviour CallBacks

        /// <summary>
        /// When we successfully join a room, then load the proper level.
        /// </summary>
        public override void OnJoinedRoom()
        {
            if (ShowDebug)
                Debug.Log("<color=yellow>[Launcher]</color>  OnJoinedRoom() called by PUN. Now this client is in a room. \nRoom name is: "
                + PhotonNetwork.room.Name + " with " + PhotonNetwork.countOfPlayers.ToString() + " players");

            PhotonNetwork.LoadLevel(Constants.SceneNames.LobbyRoom);
        }

        /// <summary>
        /// We are connected to the master server, which just means the Photon cloud services
        /// that are used to match make the rooms.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            // If we are on a PC, then create a room! 
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_STANDALONE_WIN
            _autoStartServer();
#endif
            if (ShowDebug)
            {
                Debug.Log("<color=yellow>[Launcher]</color>  OnConnectedToMaster() was called by PUN");
            }            
        }

        /// <summary>
        /// When we disconnect from photon master server, allow us to connect to it again if we want to
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            foreach (GameObject g in ProgressLabels)
            {
                g.SetActive(false);
            }
            foreach (GameObject g in ControlPanels)
            {
                g.SetActive(true);
            }

            //ProgressLabel.SetActive(false);
            //ControlPanel.SetActive(true);
            if (ShowDebug)
                Debug.LogWarning("<color=yellow>[Launcher]</color>  OnDisconnectedFromPhoton() was called by PUN");
        }

        /// <summary>
        /// Show the debug info if we fail to join a specific room.
        /// </summary>
        /// <param name="codeAndMsg">Debug info from PUN</param>
        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            if (ShowDebug)
                Debug.LogWarningFormat(
                "<color=yellow>[Launcher]</color>  OnPhotonJoinRoomFailed() was called by PUN\nError Code {0} {1}",
                 codeAndMsg[0], codeAndMsg[1]);

            if(ClientErrorText != null)
            {
                // Display this in our room name
                ClientErrorText.text = "Could not connect to room " + _roomName + "\nMake sure you typed it correctly!";
            }
        }

        /// <summary>
        /// Show the debug info if we fail to create a room. 
        /// </summary>
        /// <param name="codeAndMsg">Debug info from PUN</param>
        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            if (ShowDebug)
            {
                Debug.LogWarningFormat(
                    "<color=yellow>[Launcher]</color>  OnPhotonCreateRoomFailed() was called by PUN\nError Code {0} {1}",
                    codeAndMsg[0], codeAndMsg[1]);
            }
        }

        #endregion
    }
}