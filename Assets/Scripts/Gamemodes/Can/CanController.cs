using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanController : SingletonBehaviour<CanController>
{
    /* Editor exposed attributes */
    [Header("Game variables")]
    public float Health = 100f;

    [Space()]
    [Header("Visual")]
    public GameObject RendererObject;
    public Sprite ExplodedSprite;
    public ParticleSystem FoamParticleSystem;
    public float WiggleDampening = 1f;

    [Space()]
    [Header("SFX")]
    public AudioClip[] Shakes;
    public AudioClip Fizz;
    public AudioClip OpenTab;

    private System.Random rando;

    /* Non-serialized public attributes */
    public Action OnCanBrokenEvent;


    private SpriteRenderer canRenderer;

    protected override void setInstance()
    {
        instance = this;
    }

    protected override void Awake()
    {
        base.Awake();

        // disable foam particle system (if active)
        FoamParticleSystem.gameObject.SetActive(false);

        // get can renderer
        canRenderer = RendererObject.GetComponentInChildren<SpriteRenderer>();

        rando = new System.Random();
    }

    private void Start()
    {
        // add shake event listener
        CanGameManager.Instance.OnCanShakeEvent += _onShakeEvent;
        InputManager.Instance.OnAccelDataChanged += _onAccelDataChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // remove event listeners
        CanGameManager.Instance.OnCanShakeEvent -= _onShakeEvent;
        InputManager.Instance.OnAccelDataChanged -= _onAccelDataChanged;
    }

    private void _onShakeEvent(float intensity)
    {
        if (Health <= 0) return; // don't do anything if already broken
        
        _playRandomShake(); // play a random shake sound effect, hopefully the pool size is gucci enough at 10 for max shakes/second :)
        
        // "hurt" can
        Health -= intensity + 1f;
        if(Health <= 0)
        {
            AudioManager.Instance.PlayOneShot(OpenTab, AudioPoolType.SFX, Constants.Mixer.Mixers.Master.SFX.Name); // get that crunch tab open effect
            AudioManager.Instance.PlayOneShot(Fizz, AudioPoolType.SFX, Constants.Mixer.Mixers.Master.SFX.Name); // immediately layering over some fizz

            _onCanBroken();
        }
    }

    private void _onAccelDataChanged(AccelData data)
    {
        // wiggle
        RendererObject.transform.localPosition = data.Delta * WiggleDampening;
    }

    private void _onCanBroken()
    {
        canRenderer.sprite = ExplodedSprite;
        FoamParticleSystem.gameObject.SetActive(true);
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
        _dispatchOnCanBroken();
    }

    private void _dispatchOnCanBroken()
    {
        if (OnCanBrokenEvent != null)
            OnCanBrokenEvent();
    }

    /// <summary>
    /// Play a random shake from within the Shakes AudioClip array
    /// </summary>
    private void _playRandomShake()
    {
        int random = rando.Next(2);
        Console.WriteLine(random);

        AudioManager.Instance.PlayOneShot(Shakes[random], AudioPoolType.SFX, Constants.Mixer.Mixers.Master.SFX.Name);
    }
}
