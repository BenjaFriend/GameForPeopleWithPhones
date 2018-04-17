using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip Song;

    private PooledAudioSource _source;

    // Use this for initialization
    void Start()
    {
        if (_source == null)
            _source = AudioManager.Instance.Play(Song, AudioPoolType.Music, Constants.Mixer.Mixers.Master.Music.Name, true, () => { _source = null; });
    }

    private void OnDestroy()
    {
        if (_source != null)
            _source.Stop();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
