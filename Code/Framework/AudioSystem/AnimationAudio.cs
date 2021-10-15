// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Framework.AudioSystem
{
    public class AnimationAudio : StateMachineBehaviour
    {
        [SerializeField]
        private AudioClip enterAudioClip = default;
        [SerializeField] [Range(0, 1)]
        private float enterVolume = default;
        [SerializeField]
        private AudioClip runAudioClip = default;
        [SerializeField] [Range(0, 1)]
        private float runVolume = default;
        [SerializeField]
        private AudioClip exitAudioClip = default;
        [SerializeField] [Range(0, 1)]
        private float exitVolume = default;

        private bool _isPlaying;
        private double _time;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _isPlaying = false;
            if (enterAudioClip != null)
            {
                var audioSource = animator.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(enterAudioClip, enterVolume);
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (exitAudioClip != null)
            {
                var audioSource = animator.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(exitAudioClip, exitVolume);
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (runAudioClip == null)
            {
                return;
            }

            if (_time >= runAudioClip.length)
            {
                _isPlaying = false;
            }

            if (!_isPlaying)
            {
                _isPlaying = true;
                _time = 0;
                var audioSource = animator.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(runAudioClip, runVolume);
                }
            }

            _time += Time.deltaTime;
        }
    }
}