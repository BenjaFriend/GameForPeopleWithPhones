
using UnityEngine;

namespace Com.PodSquad.GDPPNF
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Fields

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
        private string _gameVersion = "1";
        private string _roomName = null;
        #endregion

        #region Monobehavior Functions

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

        #endregion

        #region Public Methods


        /// <summary>
        /// Joins a random room if we are connected
        /// </summary>
        public void ConnectRandomRoom()
        {
            ProgressLabel.SetActive(true);
            ControlPanel.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.connected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                // TODO: Join a game with a room code
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        public void JoinOrCreateSpecificRoom()
        {
            ProgressLabel.SetActive(true);
            ControlPanel.SetActive(false);

            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
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

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("<color=red>[Launcher]</color> OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            CreateRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("<color=red>[Launcher]</color>  OnJoinedRoom() called by PUN. Now this client is in a room. \nRoom name is: " + PhotonNetwork.room.Name);

            // When you connect to the room, then you should load in the level for the games
            PhotonNetwork.LoadLevel("Room Test 1");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("<color=red>[Launcher]</color>  OnConnectedToMaster() was called by PUN");
        }

        public override void OnDisconnectedFromPhoton()
        {
            ProgressLabel.SetActive(false);
            ControlPanel.SetActive(true);
            Debug.LogWarning("<color=red>[Launcher]</color>  OnDisconnectedFromPhoton() was called by PUN");
        }

        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            Debug.LogWarningFormat(
                "<color=red>[Launcher]</color>  OnPhotonJoinRoomFailed() was called by PUN\nError Code {0} {1}",
                 codeAndMsg[0], codeAndMsg[1]);
        }

        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            Debug.LogWarningFormat(
                "<color=red>[Launcher]</color>  OnPhotonCreateRoomFailed() was called by PUN\nError Code {0} {1}",
                 codeAndMsg[0], codeAndMsg[1]);
        }

        #endregion
    }
}