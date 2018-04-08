using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton, gets gyroscope info from the phone (or PS4 controller in debug mode),
/// and has a delegate for gyro input update 
/// </summary>
public class InputManager : SingletonBehaviour<InputManager>
{
    protected override void setInstance()
    {
        instance = this;
    }

    protected override void Awake()
    {
        base.Awake(); // set or replace instance


    }
}
