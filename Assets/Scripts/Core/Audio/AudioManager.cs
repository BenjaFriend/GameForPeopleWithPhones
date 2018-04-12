using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public enum AudioPoolType
{
    Music,
    SFX
}

public class AudioManager : MonoBehaviour
{
    private const int DEF_POOL_SIZE = 1; // default pool size
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _createInstance();
            }

            return _instance;
        }
    }

    private AudioMixer _mainMixer;
    public AudioMixer Mixer
    {
        get { return _mainMixer; }
    }

    private Dictionary<AudioPoolType, uint> _poolSizes;
    private Dictionary<AudioPoolType, Stack<PooledAudioSource>> _inactiveSources;
    private Dictionary<AudioPoolType, List<PooledAudioSource>> _activeSources;
    private AudioPoolType[] _allGroups;

    private static void _createInstance()
    {
        GameObject obj = new GameObject("AudioManager");
        obj.AddComponent<AudioManager>();
        // _setInstance and _initialize will be called in Awake when AudioManager is added above
    }

    private static void _setInstance(AudioManager inst)
    {
        _instance = inst;
        DontDestroyOnLoad(inst);
    }

    public AudioMixerGroup GetMixerGroup(string name)
    {
        return _mainMixer.FindMatchingGroups(name)[0];
    }

    /// <summary>
    /// Resizes a pool to the specified size. 
    /// If the size is smaller than the current number of active sources, 
    /// they will not be removed until after they are finished playing.
    /// </summary>
    /// <param name="group">The pool being resized</param>
    /// <param name="size">The new size</param>
    public void SetPoolSize(AudioPoolType group, uint size)
    {
        if (size > _poolSizes[group])
        {
            // add more sources (ez)
            _poolSizes[group] = size;
            _populatePool(group);
        }
        else if (size < _poolSizes[group])
        {
            // remove sources (lil harder)
            // attempt to remove as many inactive sources as we can
            float delta = _poolSizes[group] - size;
            while (_inactiveSources[group].Count > 0 && delta > 0)
            {
                //TODO: move to "None" pool group instead of destroying?
                Destroy(_inactiveSources[group].Pop().gameObject);
            }

            _poolSizes[group] = size;
        }
    }

    /// <summary>
    /// Plays a clip and does not return the source
    /// </summary>
    public void PlayOneShot(AudioClip clip, AudioPoolType group, string mixerGroup, float volume = PooledAudioSource.DEFAULT_VOLUME, float pitch = PooledAudioSource.DEFAULT_PITCH, float delay = PooledAudioSource.DEFAULT_DELAY)
    {
        Play(clip, group, mixerGroup, volume, pitch, false, delay, null);
    }

    /// <summary>
    /// Plays a clip and returns the active audio source (for later reference). Returns null if no sources available in the pool.
    /// </summary>
    public PooledAudioSource Play(AudioClip clip, AudioPoolType group, string mixerGroup, float volume, float pitch, bool loop, float delay, Action onReturnedToPool)
    {
        PooledAudioSource source = _getFromPool(group);
        if (source == null) return null;

        source.Init(clip, mixerGroup, volume, pitch, loop, onReturnedToPool);
        source.Play(delay);
        return source;
    }

    public PooledAudioSource Play(AudioClip clip, AudioPoolType group, string mixerGroup, Action onReturnedToPool)
    {
        return Play(
            clip, 
            group, 
            mixerGroup, 
            PooledAudioSource.DEFAULT_VOLUME, 
            PooledAudioSource.DEFAULT_PITCH, 
            PooledAudioSource.DEFAULT_LOOP,
            PooledAudioSource.DEFAULT_DELAY,
            onReturnedToPool);
    }


    public PooledAudioSource Play(AudioClip clip, AudioPoolType group, string mixerGroup, bool loop, Action onReturnedToPool)
    {
        return Play(
            clip, 
            group, 
            mixerGroup, 
            PooledAudioSource.DEFAULT_VOLUME, 
            PooledAudioSource.DEFAULT_PITCH, 
            loop,
            PooledAudioSource.DEFAULT_DELAY,
            onReturnedToPool);
    }

    public PooledAudioSource Play(AudioClip clip, AudioPoolType group, string mixerGroup, float volume, Action onReturnedToPool)
    {
        return Play(
            clip, 
            group, 
            mixerGroup, 
            volume, 
            PooledAudioSource.DEFAULT_PITCH, 
            PooledAudioSource.DEFAULT_LOOP,
            PooledAudioSource.DEFAULT_DELAY,
            onReturnedToPool);
    }

    public PooledAudioSource Play(AudioClip clip, AudioPoolType group, string mixerGroup, float volume, float pitch, Action onReturnedToPool)
    {
        return Play(
            clip, 
            group, 
            mixerGroup, 
            volume, 
            pitch, 
            PooledAudioSource.DEFAULT_LOOP,
            PooledAudioSource.DEFAULT_DELAY,
            onReturnedToPool);
    }

    public PooledAudioSource PlayDelayed(AudioClip clip, AudioPoolType group, string mixerGroup, float delay, Action onReturnedToPool)
    {
        return Play(
            clip,
            group,
            mixerGroup,
            PooledAudioSource.DEFAULT_VOLUME,
            PooledAudioSource.DEFAULT_PITCH,
            PooledAudioSource.DEFAULT_LOOP,
            delay,
            onReturnedToPool);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("[AudioManager] Another instance of the audio manager is already in the scene, destroying self...");
            Destroy(this.gameObject);
        }
        else if (_instance == null)
        {
            _setInstance(this);
            _initialize();
        }
    }

    private void Update()
    {
        if (Application.isFocused)
            _updateSources();
    }

    private void _updateSources()
    {
        // check for sources that need to be returned to the pool
        for (int i = 0; i < _allGroups.Length; i++)
        {
            AudioPoolType g = _allGroups[i];
            for (int j = _activeSources[g].Count - 1; j >= 0; j--)
            {
                if (_activeSources[g][j].IsStopped)
                    _returnToPool(_activeSources[g][j]);
            }
        }
    }

    private void _initialize()
    {
        _mainMixer = Resources.Load<AudioMixer>(Constants.Mixer.Path);

        _inactiveSources = new Dictionary<AudioPoolType, Stack<PooledAudioSource>>();
        _activeSources = new Dictionary<AudioPoolType, List<PooledAudioSource>>();

        _poolSizes = new Dictionary<AudioPoolType, uint>();
        _allGroups = (AudioPoolType[])Enum.GetValues(typeof(AudioPoolType));
        for (int i = 0; i < _allGroups.Length; i++)
        {
            AudioPoolType g = _allGroups[i];
            _poolSizes.Add(g, DEF_POOL_SIZE);
            _activeSources.Add(g, new List<PooledAudioSource>());
            _inactiveSources.Add(g, new Stack<PooledAudioSource>());
            _populatePool(g);
        }
    }

    private void _populatePool(AudioPoolType g)
    {
        while (_inactiveSources[g].Count < _poolSizes[g])
        {
            _inactiveSources[g].Push(PooledAudioSource.CreateInstance(this.transform, g));
        }
    }

    private PooledAudioSource _getFromPool(AudioPoolType g)
    {
        if (_inactiveSources[g].Count > 0)
        {
            PooledAudioSource source = _inactiveSources[g].Pop();
            _activeSources[g].Add(source);
            return source;
        }
        else
        {
            Debug.LogWarning("[AudioManager] Audio source pool is empty! Consider increasing the size of the pool to play all these dang sounds.");
            return null;
        }
    }

    private void _returnToPool(PooledAudioSource source)
    {
        source.OnReturned();
        _activeSources[source.Group].Remove(source);

        float currentPoolSize = _inactiveSources[source.Group].Count + _activeSources[source.Group].Count;
        if (currentPoolSize >= _poolSizes[source.Group]) // destroy the source if there's too many in pool
        {
            Destroy(source.gameObject); //TODO: move to "none" pool instead of destroying?
        }
        else
        {
            _inactiveSources[source.Group].Push(source);
        }
    }
}
