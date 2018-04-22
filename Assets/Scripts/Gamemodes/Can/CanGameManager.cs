using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanGameManager : SingletonBehaviour<CanGameManager>
{
    public float ShakeThreshold;
    [Space(10)]
    public bool ShowDebug = false;

    public GameObject CanPrefab;

    public int CountdownLength = 5;
    
    public Action<float> OnCanShakeEvent;

    private bool _isGameOver;
    private List<string> _winnersList = new List<string>();
    private int _currentlyReadyPlayers = 0;

    protected override void setInstance()
    {
        instance = this;
        // intentionally excluding DontDestroyOnLoad for CanGameManager to let scene change destroy it
    }

    protected override void Awake()
    {
        base.Awake();

        // If I am not the master server (the web view)
        if (!PhotonNetwork.player.IsMasterClient)
        {
            if(CanPrefab != null)
            {
                // Create the can LOCALLY. 
                Instantiate(CanPrefab);
                DebugString("Create a can locally!");
            }
            GameOverlay.Instance.SetNormalClientUI();
        }
        // We are the master client, so do NOT make a can for us
        else
        {
            DebugString(" Set the master UI!");
            GameOverlay.Instance.SetMasterClientUI();
        }

        _isGameOver = false;
    }

    private void Start()
    {
        InputManager.Instance.OnAccelDataChanged += _onAccelDataChanged;
        PhotonNetwork.OnEventCall += _onEvent;

        _initAudio();

        if(PhotonNetwork.player.IsMasterClient)
        {
            _startCountdown();
        }
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this._onEvent;
    }

    /// <summary>
    /// Setup the audio loops on the client and the server
    /// </summary>
    private void _initAudio()
    {
        if(PhotonNetwork.player.IsMasterClient)
        {
            // TODO: Play main game loop that will play on the server
        }
        else
        {
            // TODO: This audio will play on the client
        }
    }

    private void _startCountdown()
    {
        StartCoroutine(_countdownRoutine(CountdownLength));
    }

    private IEnumerator _countdownRoutine(int length)
    {
        if (length <= 0)
        {
            yield break;
        }

        for(int i = length; i >= 0; i--)
        {
            DebugString("Countdown: " + i.ToString());
            // Tell the Game overlay script about this
            GameOverlay.Instance.SetCountdwonText(i);
            yield return new WaitForSeconds(1f);
        }

        _countDownFinished();
    }

    /// <summary>
    /// Send the countdown finished event to all the others
    /// </summary>
    private void _countDownFinished()
    {
        if (!PhotonNetwork.player.IsMasterClient) return;

        // Send this event over the network to the master client (the web view)
        byte evCode = (byte)Constants.EVENT_ID.START_COUNTDOWN_FINISHED;
        bool reliable = true;
        RaiseEventOptions options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others  // Onl send this event to all the others
        };
        PhotonNetwork.RaiseEvent(
            evCode,
            null,
            reliable,
            options
       );

        DebugString("Countdown Finished! Event Sent!");
    }

    private void _onAccelDataChanged(AccelData data)
    {
        float intensity = data.Delta.magnitude - ShakeThreshold;
        if (intensity > 0)
            _dispatchShakeEvent(intensity);
    }

    private void _dispatchShakeEvent(float intensity)
    {
        if(ShowDebug)
            Debug.LogFormat("[CanGameManager] Shake!! Intensity: {0}", intensity);

        if (OnCanShakeEvent != null)
            OnCanShakeEvent(intensity);
    }

    private void _onEvent(byte eventcode, object content, int senderid)
    {
        if(eventcode == (byte)Constants.EVENT_ID.CAN_BROKE)
        {
            // If we are the master client, do this stuff
            if (PhotonNetwork.player.IsMasterClient)
            {
                DebugString("Recieved a network call that the can has broken ON MASTER CLIENT. Sender ID" + senderid.ToString());

                object[] selected = content as object[];
                string name = selected[0] as string;

                // Do not allow any other player to say that they broke!
                if (!_isGameOver)
                {
                    GameOverlay.Instance.SetMasterGameOver(name);
                    _isGameOver = true;
                }
                _winnersList.Add(name);
                // TODO: Update the list of winners on the server UI
            }
            else
            {
                DebugString("Recieved a network call that the can has broken on normal client.");
                GameOverlay.Instance.SetText("You popped your can!");
                GameOverlay.Instance.FadeIn(0.6f);
            }
        }
    }

    private void _onCanBroken()
    {
        GameOverlay.Instance.SetText("You win!");
        GameOverlay.Instance.FadeIn(0.6f);
    }

    /// <summary>
    /// Check if all the players in the game are connected to the room. 
    /// - If so, start a countdown.
    /// - Otherwise, continue waiting.
    /// </summary>
    private void _readyCheck()
    {
        if(_currentlyReadyPlayers >= PhotonNetwork.playerList.Length)
        {
            // Play the count down
        }
        DebugString(_currentlyReadyPlayers.ToString() +  "/" + PhotonNetwork.playerList.Length
            + " players ready");
    }

    private void DebugString(string content)
    {
        if(ShowDebug)
        {
            Debug.Log("<color=pink>[CanGameManager]</color> " + content);
        }
    }
}
