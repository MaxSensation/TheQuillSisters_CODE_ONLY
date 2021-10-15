// Primary Author : Maximiliam Rosén - maka4519

using Environment.Pickup;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(EyeOfHorus))]
public class EyeOfHorusAudioSource : MonoBehaviour
{
    [SerializeField]
    private AudioClip audioClip = default;
    
    private AudioSource _audioSource;
    private EyeOfHorus _eyeOfHorus;

    private void Start()
    {
        _eyeOfHorus = GetComponent<EyeOfHorus>();
        _audioSource = GetComponent<AudioSource>();
        _eyeOfHorus.OnActivated += OnActivated;
    }

    private void OnActivated()
    {
        _audioSource.PlayOneShot(audioClip);
    }
}