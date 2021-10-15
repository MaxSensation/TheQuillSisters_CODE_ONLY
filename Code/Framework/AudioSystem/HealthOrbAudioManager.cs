// Primary Author : Maximiliam Rosén - maka4519

using Environment.Pickup;
using UnityEngine;

namespace Framework.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class HealthOrbAudioManager : MonoBehaviour
    {
        [SerializeField] 
        private AudioClip pickupSound = default;

        private AudioSource _audioSource;
        private HealthOrb _healthOrb;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _healthOrb = GetComponentInChildren<HealthOrb>();
            _healthOrb.OnPickup += PickedUp;
        }

        private void PickedUp()
        {
            _healthOrb.gameObject.SetActive(false);
            _audioSource.PlayOneShot(pickupSound);
            Destroy(gameObject, pickupSound.length);
        }
    }
}