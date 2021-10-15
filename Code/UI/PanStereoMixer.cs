// Primary Author : Maximiliam Rosén - maka4519

using System;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PanStereoMixer : MonoBehaviour
    {
        [SerializeField] private ScriptObjVar<float> stereoPan;
        [SerializeField] private ScriptObjVar<bool> mono;
        [SerializeField] private Toggle monoToggle;
        public static Action onUpdate;
        private Slider _stereoPanSlider;

        private void Start()
        {
            _stereoPanSlider = GetComponent<Slider>();
            _stereoPanSlider.value = stereoPan.value;
            monoToggle.isOn = mono.value;
        }

        public void UpdateStereoPan(float pan)
        {
            stereoPan.value = pan;
            onUpdate?.Invoke();
        }

        public void UpdateMono(bool state)
        {
            mono.value = state;
            onUpdate?.Invoke();
        }
    }
}