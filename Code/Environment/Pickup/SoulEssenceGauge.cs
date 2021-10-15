// Primary Author : Viktor Dahlberg - vida6631

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Pickup
{
    public class SoulEssenceGauge : MonoBehaviour
    {
        [SerializeField] 
        private ScriptObjVar<float> max = default;

        [SerializeField] 
        private ScriptObjVar<float> currentAmount = default;
        public float Max => max;
    }
}