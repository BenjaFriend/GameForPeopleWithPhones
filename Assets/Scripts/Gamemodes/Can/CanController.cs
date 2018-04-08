using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanController : MonoBehaviour
{
    public float Health = 100f;

    private void Start()
    {
        // add shake event listener
        CanGameManager.Instance.OnCanShakeEvent += _onShakeEvent;
    }

    private void _onShakeEvent(float intensity)
    {
        if (Health <= 0) return; // don't do anything if already broken

        // "hurt" can
        Health -= intensity;
        if(Health <= 0)
        {
            _onCanBroken();
        }
    }

    private void _onCanBroken()
    {
        Debug.Log("[CanController] Can broken!!");
    }
}
