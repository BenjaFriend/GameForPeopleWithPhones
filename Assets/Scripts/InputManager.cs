using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AccelData
{
    public Vector3 Current;
    public Vector3 Last;
    public Vector3 Delta
    {
        get { return Current - Last; }
    }
}

/// <summary>
/// Singleton, gets gyroscope info from the phone (or PS4 controller in debug mode),
/// and exposes the gyro info publicly as a vector
/// </summary>
public class InputManager : SingletonBehaviour<InputManager>
{
    public float DebugAccelMagnitude = 10f;

    private AccelData _accelData;

    public AccelData AccelData
    {
        get { return _accelData; }
    }

    public Action<AccelData> OnAccelDataChanged;

    protected override void setInstance()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    protected override void Awake()
    {
        base.Awake(); // set or replace instance
    }

    private void OnDestroy() { }

    private void Update()
    {
        // TODO: Only do this if it is a local PhotonView

        _accelData.Last = AccelData.Current;
        _accelData.Current = Input.acceleration;

        // debug input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _accelData.Current = Vector3.up * DebugAccelMagnitude;
        }

        if (_accelData.Last != _accelData.Current)
            dispatchAccelDataChangedEvent();
    }

    private void dispatchAccelDataChangedEvent()
    {
        if (OnAccelDataChanged != null)
            OnAccelDataChanged(AccelData);
    }
}
