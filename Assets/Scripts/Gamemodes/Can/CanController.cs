using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanController : MonoBehaviour
{
    public float Health = 100f;

    public float WiggleDampening = 1f;
    public GameObject Renderer;

    private void Start()
    {
        // add shake event listener
        CanGameManager.Instance.OnCanShakeEvent += _onShakeEvent;
    }

    private void _onShakeEvent(float intensity)
    {
        if (Health <= 0) return; // don't do anything if already broken

        // "hurt" can
        Health -= intensity + 1f;
        if(Health <= 0)
        {
            _onCanBroken();
        }
    }

    private void Update()
    {
        // wiggle
        Renderer.transform.localPosition = InputManager.Instance.AccelData.Delta * WiggleDampening;
    }

    private void _onCanBroken()
    {
        Debug.Log("[CanController] Can broken!!");
    }
}
