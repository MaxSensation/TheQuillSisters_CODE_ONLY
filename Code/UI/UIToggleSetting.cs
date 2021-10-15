// Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIToggleSetting : MonoBehaviour
    {
        [SerializeField] 
        private ScriptObjVar<bool> setting = default;
        
        private void Start()
        {
            GetComponent<Toggle>().isOn = setting.value;
        }

        public void OnChange(bool state)
        {
            setting.value = state;
        }
    }
}