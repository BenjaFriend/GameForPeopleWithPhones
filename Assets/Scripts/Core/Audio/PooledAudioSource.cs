using UnityEngine;
using System.Collections;
using System;

public class PooledAudioSource : MonoBehaviour
{
    public const bool DEFAULT_LOOP = false;
    public const float DEFAULT_VOLUME = 1f;
    public const float DEFAULT_PITCH = 1f;
    public const float DEFAULT_DELAY = 0;

    public AudioSource Source;

    public bool Paused
    {
        get { return _isPaused; }
    }
    private bool _isPaused = false;

    public bool IsStopped
    {
        get { return !Source.isPlaying && !_isPaused; }
    }

    private AudioPoolType _group;
    public AudioPoolType Group
    {
        get { return _group; }
    }

    public Action OnReturnedToPool;

    public static PooledAudioSource CreateInstance(Transform parent, AudioPoolType g)
    {
        GameObject obj = new GameObject("[" + Enum.GetName(typeof(AudioPoolType), g) + "] PooledAudioSource");
        PooledAudioSource inst = obj.AddComponent<PooledAudioSource>();
        inst._group = g;
        inst.Source = inst.gameObject.AddComponent<AudioSource>();
        inst.transform.SetParent(parent);
        inst.ResetValues(); // init values
        return inst;
    }

    // Initialize values when removed from pool
    public void Init(AudioClip clip, string mixerGroup, float volume = DEFAULT_VOLUME, float pitch = DEFAULT_PITCH, bool loop = DEFAULT_LOOP, Action onReturnedToPool = null)
    {
        Source.clip = clip;
        Source.volume = volume;
        Source.pitch = pitch;
        Source.loop = loop;
        Source.outputAudioMixerGroup = AudioManager.Instance.GetMixerGroup(mixerGroup);

        if (onReturnedToPool != null)
            OnReturnedToPool += onReturnedToPool;

        gameObject.SetActive(true);
    }

    // Reset source values before returning to pool
    public void ResetValues()
    {
        if (Source.isPlaying)
            Source.Stop();

        Source.clip = null;
        Source.playOnAwake = false;
        _isPaused = false;
        OnReturnedToPool = null;
        gameObject.SetActive(false);
    }

    public void Play(float delay = DEFAULT_DELAY)
    {
        if (delay != 0)
        {
            Debug.Log("Play delayed: " + delay);
            Source.PlayDelayed(delay);
        }
        else
        {
            Source.Play();
        }

        _isPaused = false;
    }

    public void Pause()
    {
        Source.Pause();
        _isPaused = true;
    }

    public void Stop()
    {
        Source.Stop();
        _isPaused = false;
    }

    public void OnReturned()
    {
        if (OnReturnedToPool != null)
        {
            OnReturnedToPool();
        }
        ResetValues();
    }

}