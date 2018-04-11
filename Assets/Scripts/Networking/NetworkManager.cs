using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main network manager for this game
/// 
/// Used this guide: http://www.paladinstudios.com/2013/07/10/how-to-create-an-online-multiplayer-game-with-unity/
/// </summary>
public class NetworkManager : MonoBehaviour
{
    #region Fields

    public GameObject PlayerPrefab;
    public Transform PlayerSpawn;

    private const int _maxPlayers = 4;
    private const int _port = 25000;

    private const string _typeName = "GFPP_F";
    private const string _gameName = "RoomName";

    private HostData[] _hostList;

    #endregion

    private void StartServer()
    {
        // Init the server
        Network.InitializeServer(_maxPlayers, _port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(_typeName, _gameName);
    }

    private void OnServerInitialized()
    {
        Debug.Log("[NetworkManager] Server Initializied");
        // Spawn the player
    }

    private void refreshHostList()
    {
        MasterServer.RequestHostList(_typeName);
    }

    private void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            _hostList = MasterServer.PollHostList();
        }
    }

    private void joinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

    private void OnConnectedToServer()
    {
        Debug.Log("[NetworkManager] OnConnectedToServer :: Server Joined");
        // Spawn player
        spawnPlayer();
    }

    private void spawnPlayer()
    {
        Network.Instantiate(PlayerPrefab, PlayerSpawn.position, PlayerSpawn.rotation, 0);
    }

    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                StartServer();

            if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
                refreshHostList();

            if (_hostList != null)
            {
                for (int i = 0; i < _hostList.Length; i++)
                {
                    if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), _hostList[i].gameName))
                        joinServer(_hostList[i]);
                }
            }
        }
    }

}
