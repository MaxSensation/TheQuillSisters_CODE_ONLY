// Primary Author : Keziah Ferreira Dos Santos kefe3770
// Secondary Author : Maximiliam Rosén - maka4519
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.Audio;

namespace Framework.AudioSystem
{
    public class AudioMixerSetting : MonoBehaviour
    {
        [SerializeField]
        private AudioMixer audioMixer = default;
        [SerializeField]
        private ScriptObjVar<float> masterSetting = default;
        [SerializeField]
        private ScriptObjVar<float> musicSetting = default;
        [SerializeField]
        private ScriptObjVar<float> ambientSetting = default;
        [SerializeField]
        private ScriptObjVar<float> soundFXSetting = default;
        [SerializeField]
        private ScriptObjVar<float> dialogSetting = default;

        private void Start()
        {
            SetMasterVolume(masterSetting);
            SetMusicVolume(musicSetting);
            SetAmbientVolume(ambientSetting);
            SetFxVolume(soundFXSetting);
            SetDialogVolume(dialogSetting);
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
            masterSetting.value = volume;
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
            musicSetting.value = volume;
        }

        public void SetAmbientVolume(float volume)
        {
            audioMixer.SetFloat("Ambient", Mathf.Log10(volume) * 20);
            ambientSetting.value = volume;
        }

        public void SetFxVolume(float volume)
        {
            audioMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
            soundFXSetting.value = volume;
        }

        public void SetDialogVolume(float volume)
        {
            audioMixer.SetFloat("Voice", Mathf.Log10(volume) * 20);
            dialogSetting.value = volume;
        }
    }
}