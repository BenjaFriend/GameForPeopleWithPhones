
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Fields

        public bool ShowDebug = true;

        private static string roomNamePrefKey = "RoomName";

        [Header("Network UI")]
        [Tooltip("The UI panel to lket the user enter theirr name, connect and play")]
        public GameObject ControlPanel;
        [Tooltip("The UI Label to inform the user htat the connection is in progress")]
        public GameObject ProgressLabel;

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

        void Awake()
        {
            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;

            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;

            // Force the log level to this
            PhotonNetwork.logLevel = Loglevel;
        }

        void Start()
        {
            ProgressLabel.SetActive(false);
            ControlPanel.SetActive(true);

            // Automatically connect to the master server on start
            if(!PhotonNetwork.connected)
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #region Public Methods
        

        /// <summary>
        /// Try and join a room with teh given room name from the user
        ///  - If this room does not exist, then create it.
        ///  - Show the "Connecting" UI
        /// </summary>
        public void JoinOrCreateSpecificRoom()
        {
            ProgressLabel.SetActive(true);
            ControlPanel.SetActive(false);

            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom, IsVisible = true }, null);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        /// <summary>
        /// Creates a random room, used as a failsafe for if we cannot make a new room.
        /// </summary>
        public void CreateRandomRoom()
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinOrCreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
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

            PhotonNetwork.LoadLevel("Lobby Room");
        }

        /// <summary>
        /// If we fail to join a random room, then make a new random room. 
        /// </summary>
        /// <param name="codeAndMsg"></param>
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            if(ShowDebug)
                Debug.Log("<color=yellow>[Launcher]</color> OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            CreateRandomRoom();
        }

        /// <summary>
        /// We are connected to the master server, which just means the Photon cloud services
        /// that are used to match make the rooms.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            if (ShowDebug)
                Debug.Log("<color=yellow>[Launcher]</color>  OnConnectedToMaster() was called by PUN");       
        }

        /// <summary>
        /// When we disconnect from photon master server, allow us to connect to it again if we want to
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            ProgressLabel.SetActive(false);
            ControlPanel.SetActive(true);
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
        }

        /// <summary>
        /// Show the debug info if we fail to create a room. 
        /// </summary>
        /// <param name="codeAndMsg">Debug info from PUN</param>
        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            if (ShowDebug)
                Debug.LogWarningFormat(
                "<color=yellow>[Launcher]</color>  OnPhotonCreateRoomFailed() was called by PUN\nError Code {0} {1}",
                 codeAndMsg[0], codeAndMsg[1]);
        }

        #endregion
    }
}