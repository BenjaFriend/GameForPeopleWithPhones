using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanGameManager : SingletonBehaviour<CanGameManager>
{
    public float ShakeThreshold;
    [Space(10)]
    public bool ShowDebug = false;

    public Action<float> OnCanShakeEvent;

    protected override void setInstance()
    {
        instance = this;
        // intentionally excluding DontDestroyOnLoad for CanGameManager to let scene change destroy it
    }

    private void Start()
    {
        InputManager.Instance.OnAccelDataChanged += _onAccelDataChanged;
        CanController.Instance.OnCanBrokenEvent += _onCanBroken;
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

    private void _onCanBroken()
    {
        GameOverlay.Instance.SetText("You win!");
        GameOverlay.Instance.FadeIn(0.6f);
    }
}
