// Primary Author : Andreas Berzelius - anbe5918

using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    ///     A script for setting the audio sliders to the actual volume
    /// </summary>
    public class AudioSettings : MonoBehaviour
    {
        [Header("Audio sliders")] 
        
        [SerializeField]
        private Slider masterVolume;
        [SerializeField]
        private ScriptObjVar<float> masterSetting = default;
        [SerializeField]
        private Slider musicVolume = default;
        [SerializeField]
        private ScriptObjVar<float> musicSetting = default;
        [SerializeField]
        private Slider ambientVolume = default;
        [SerializeField]
        private ScriptObjVar<float> ambientSetting = default;
        [SerializeField]
        private Slider soundEffectsVolume = default;
        [SerializeField]
        private ScriptObjVar<float> soundFXSetting = default;
        [SerializeField]
        private Slider dialogVolume = default;
        [SerializeField]
        private ScriptObjVar<float> dialogSetting = default;
        [SerializeField] [Space] 
        private AudioMixer mixer = default;

        private void OnEnable()
        {
            masterVolume.value = masterSetting.value;
            mixer.SetFloat("Master", Mathf.Log10(masterSetting.value) * 20);
            musicVolume.value = musicSetting.value;
            mixer.SetFloat("Music", Mathf.Log10(musicSetting.value) * 20);
            ambientVolume.value = ambientSetting.value;
            mixer.SetFloat("Ambient", Mathf.Log10(ambientSetting.value) * 20);
            soundEffectsVolume.value = soundFXSetting.value;
            mixer.SetFloat("FX", Mathf.Log10(soundFXSetting.value) * 20);
            dialogVolume.value = dialogSetting.value;
            mixer.SetFloat("Voice", Mathf.Log10(dialogSetting.value) * 20);
        }
    }
}