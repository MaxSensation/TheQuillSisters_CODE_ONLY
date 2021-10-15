// Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SensitivityControl : MonoBehaviour
    {
        [SerializeField] 
        private ScriptObjVar<float> sensitivity = default;

        private Slider _slider;

        private void Start()
        {
            _slider = GetComponent<Slider>();
            _slider.value = sensitivity.value;
        }

        public void UpdateSensitivity(float newValue)
        {
            sensitivity.SetValueNotify(newValue);
        }
    }
}