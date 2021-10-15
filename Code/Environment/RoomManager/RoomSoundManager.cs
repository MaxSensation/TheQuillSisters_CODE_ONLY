// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Environment.RoomManager
{
    [RequireComponent(typeof(Room), typeof(AudioSource))]
    public class RoomSoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioClip onRoomStarted = default;
        [SerializeField]
        private AudioClip onRoundCompleted = default;
        [SerializeField]
        private AudioClip onRoomCompleted = default;

        private AudioSource _audioSource;
        private Room _room;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _room = GetComponent<Room>();
            _room.OnRoomStarted += OnRoomStarted;
            _room.OnRoundCompleted += OnRoundCompleted;
            _room.OnRoomCompleted += OnRoomCompleted;
        }

        private void OnRoomCompleted()
        {
            if (onRoomStarted == null)
            {
                return;
            }

            _audioSource.PlayOneShot(onRoomStarted);
        }

        private void OnRoundCompleted()
        {
            if (onRoundCompleted == null)
            {
                return;
            }

            _audioSource.PlayOneShot(onRoundCompleted);
        }

        private void OnRoomStarted()
        {
            if (onRoomCompleted == null)
            {
                return;
            }

            _audioSource.PlayOneShot(onRoomCompleted);
        }
    }
}