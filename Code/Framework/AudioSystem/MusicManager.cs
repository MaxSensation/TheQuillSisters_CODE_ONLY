// Primary Author : Maximiliam Rosén - maka4519

using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static Action<AudioClip> OnMusicStart;
    public static Action OnMusicStop;
    public static Action<bool> OnMusicSetLoop;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        OnMusicStart += Play;
        OnMusicStop += Stop;
        OnMusicSetLoop += SetLoop;
    }

    private void Play(AudioClip music)
    {
        _audioSource.clip = music;
        _audioSource.Play();
    }

    private void Stop()
    {
        _audioSource.Stop();
    }

    public void SetLoop(bool state)
    {
        _audioSource.loop = state;
    }
}