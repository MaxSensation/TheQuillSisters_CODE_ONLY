// Primary Author : Viktor Dahlberg - vida6631

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Framework
{
    public class ComponentWriter : MonoBehaviour
    {
        [SerializeField]
        private Component component = default;
        [SerializeField]
        private ScriptObjVar<Component> targetScriptObjVar = default;

        private void Awake()
        {
            targetScriptObjVar.value = component;
        }
    }
}