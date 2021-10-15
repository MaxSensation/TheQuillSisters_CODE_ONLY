// Primary Author : Maximiliam Rosén - maka4519

using Entity;
using UnityEngine;

namespace Framework.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class FootStepAudioManager : MonoBehaviour
    {
        [SerializeField] 
        private SoundBundle defaultSoundsBundle = default;
        
        [Header("Dependencies")] 
        
        [SerializeField]
        private GroundChecker groundChecker = default;
        [SerializeField] 
        private AnimationHandler animationHandler = default;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            animationHandler.OnAnimationFootStep += OnFootStep;
        }

        private void OnFootStep()
        {
            if (groundChecker.HasGround)
            {
                var groundSurface = groundChecker.GroundGameObject.GetComponent<WalkingSurface>();
                _audioSource.PlayOneShot(groundSurface
                    ? groundSurface.soundBundle.GetRandom()
                    : defaultSoundsBundle.GetRandom());
            }
        }
    }
}