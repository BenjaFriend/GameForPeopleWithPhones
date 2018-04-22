using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CanStates
{
    public Sprite Sprite;
    public float HealthPercentage;
}

public class CanController : SingletonBehaviour<CanController>
{
    /* Editor exposed attributes */
    [Header("Game variables")]
    public float Health = 100f;
    private float startHealth;

    [Space()]
    [Header("Visual")]
    public GameObject RendererObject;
    //public Sprite ExplodedSprite;
    [SerializeField]
    public CanStates[] VisualStates;
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

    private int currentVisualState;

    private SpriteRenderer canRenderer;

    private bool _isReady = false;

    protected override void setInstance()
    {
        instance = this;
        // intentionally excluding DontDestroyOnLoad for CanController to let scene change destroy it
    }

    protected override void Awake()
    {
        base.Awake();

        // disable foam particle system (if active)
        FoamParticleSystem.gameObject.SetActive(false);

        // get can renderer
        canRenderer = RendererObject.GetComponentInChildren<SpriteRenderer>();

        rando = new System.Random();
        _updateCanSprite();
        startHealth = Health;
    }

    private void Start()
    {
        // add shake event listener
        CanGameManager.Instance.OnCanShakeEvent += _onShakeEvent;
        InputManager.Instance.OnAccelDataChanged += _onAccelDataChanged;
    }

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += this._onEvent;

    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this._onEvent;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // remove event listeners
        if (CanGameManager.Instance != null)
            CanGameManager.Instance.OnCanShakeEvent -= _onShakeEvent;

        InputManager.Instance.OnAccelDataChanged -= _onAccelDataChanged;
    }

    private void _onEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == (byte)Constants.EVENT_ID.COUNTDOWN_FINISHED)
        {
            _isReady = true;
            Debug.Log("<color=green>[CanController]</color> Is Ready!");
        }
    }

    private void _onShakeEvent(float intensity)
    {
        if (Health <= 0 || !_isReady) return; // don't do anything if already broken

        _playRandomShake(); // play a random shake sound effect, hopefully the pool size is gucci enough at 10 for max shakes/second :)

        // shake vibration
#if UNITY_ANDROID || UNITY_IOS
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
#endif

        // "hurt" can
        Health -= intensity + 1f;
        if (Health <= 0)
        {
            AudioManager.Instance.PlayOneShot(OpenTab, AudioPoolType.SFX, Constants.Mixer.Mixers.Master.SFX.Name); // get that crunch tab open effect
            AudioManager.Instance.PlayOneShot(Fizz, AudioPoolType.SFX, Constants.Mixer.Mixers.Master.SFX.Name); // immediately layering over some fizz

            _onCanBroken();
        }

        _updateCanSprite();
    }

    private void _updateCanSprite()
    {
        if (VisualStates.Length > currentVisualState + 1
            && VisualStates[currentVisualState + 1].HealthPercentage >= Health / startHealth * 100f)
        {
            // change sprite
            currentVisualState++;
            canRenderer.sprite = VisualStates[currentVisualState].Sprite;
        }
    }

    private void _onAccelDataChanged(AccelData data)
    {
        // wiggle
        RendererObject.transform.localPosition = data.Delta * WiggleDampening;
    }

    private void _onCanBroken()
    {
        //canRenderer.sprite = ExplodedSprite;
        FoamParticleSystem.gameObject.SetActive(true);
#if UNITY_ANDROID || UNITY_IOS
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
#endif
        _dispatchOnCanBroken();
    }

    private void _dispatchOnCanBroken()
    {
        // Send this event over the network to the master client (the web view)
        byte evCode = (byte)Constants.EVENT_ID.CAN_BROKE;
        object[] content = { PhotonNetwork.player.NickName, PhotonNetwork.player.ID };
        bool reliable = true;
        RaiseEventOptions options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.MasterClient  // Onl send this event to the master client
        };
        PhotonNetwork.RaiseEvent(
            evCode,
            content,
            reliable,
            options
       );

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
