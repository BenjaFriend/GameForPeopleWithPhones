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
    
    public Action<float> OnCanShakeEvent;

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
    }

    private void Start()
    {
        InputManager.Instance.OnAccelDataChanged += _onAccelDataChanged;
        //CanController.Instance.OnCanBrokenEvent += _onCanBroken;
        PhotonNetwork.OnEventCall += _onEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this._onEvent;
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
                GameOverlay.Instance.SetMasterGameOver(name);
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

    private void DebugString(string content)
    {
        Debug.Log("<color=pink>[CanGameManager]</color> " + content);
    }
}
