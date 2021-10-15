// Primary Author : Viktor Dahlberg - vida6631

using System.Collections.Generic;
using Framework;
using Framework.ScriptableObjectVariables.Primitives;
using Framework.ScriptableObjectVariables.Sets;
using UnityEngine;

namespace Environment
{
    public class BoolReset : MonoBehaviour
    {
        [SerializeField] 
        private List<ScriptObjScriptObjBoolSet> boolSets = default;
        [SerializeField] 
        private List<SerializableTuple<ScriptObjBool, bool>> bools = default;

        private void Awake()
        {
            boolSets.ForEach(b => b.items.ForEach(e => e.SetValue(false)));
            bools.ForEach(e => e.item1.value = e.item2);
        }
    }
}