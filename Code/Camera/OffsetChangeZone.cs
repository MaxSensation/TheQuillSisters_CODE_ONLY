// Primary Author : Viktor Dahlberg - vida6631

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Scripts.Camera
{
    public class OffsetChangeZone : MonoBehaviour
    {
        [SerializeField]
        private ScriptObjVar<Offset> offsetToModify = default;
        [SerializeField]
        private ScriptObjRef<Offset> offset = default;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                offsetToModify.value = offset.Value;
            }
        }
    }
}