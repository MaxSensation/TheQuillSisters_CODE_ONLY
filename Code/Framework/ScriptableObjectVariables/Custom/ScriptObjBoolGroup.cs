// Primary Author : Viktor Dahlberg - vida6631

using System.Linq;
using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
    [CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/Bool Group")]
    public class ScriptObjBoolGroup : ScriptObjVar
    {
        [SerializeField] private ScriptObjVar<bool>[] bools;
#if UNITY_EDITOR
        [ReadOnly]
#endif
        [SerializeField]
        private bool allTrue;

        public bool IsAllTrue()
        {
            var result = BoolTotal();
            allTrue = result;
            return result;
        }

        private bool BoolTotal()
        {
            return bools.All(b => b);
        }
    }
}