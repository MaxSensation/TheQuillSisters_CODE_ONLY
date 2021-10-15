// Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UI;
using UnityEngine;

namespace Framemwork.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class SteroPanReader : MonoBehaviour
    {
        [SerializeField] private ScriptObjVar<bool> mono;
        [SerializeField] private ScriptObjVar<float> pan;

        private bool _isMonoDefault;
        private AudioSource _audioSource;

        private void Start()
        {
            PanStereoMixer.onUpdate += UpdateValues;
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource.spatialBlend == 0)
            {
                _isMonoDefault = true;
            }

            UpdateValues();
        }

        private void OnDestroy()
        {
            PanStereoMixer.onUpdate -= UpdateValues;
        }

        private void UpdateValues()
        {
            _audioSource.panStereo = pan.value;
            if (!_isMonoDefault)
            {
                _audioSource.spatialBlend = mono.value ? 0f : 1f;
            }
        }
    }
}