// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    public class LiveAudioChange : MonoBehaviour, IDeselectHandler, IBeginDragHandler
    {
        private AudioSource _audioSource;
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_audioSource.clip != null)
            {
                _audioSource.Play();   
            }
        }
        
        public void OnDeselect(BaseEventData eventData)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();   
            }
        }
    }
}