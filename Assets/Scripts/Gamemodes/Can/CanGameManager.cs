using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanGameManager : SingletonBehaviour<CanGameManager>
{
    public float ShakeThreshold;

    public Action<float> OnCanShakeEvent;

    protected override void setInstance()
    {
        instance = this;
        // intentionally excluding DontDestroyOnLoad for CanGameManager to let scene change destroy it
    }

    private void Start()
    {
        InputManager.Instance.OnAccelDataChanged += _onAccelDataChanged;
    }

    private void _onAccelDataChanged(AccelData data)
    {
        float intensity = data.Delta.magnitude - ShakeThreshold;
        if (intensity > 0)
            _dispatchShakeEvent(intensity);
    }

    private void _dispatchShakeEvent(float intensity)
    {
        Debug.LogFormat("[CanGameManager] Shake!! Intensity: {0}", intensity);
        if (OnCanShakeEvent != null)
            OnCanShakeEvent(intensity);
    }

}
