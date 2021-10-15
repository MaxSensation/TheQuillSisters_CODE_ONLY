// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace VFX
{
    public class PuffController : MonoBehaviour
    {
        [SerializeField] 
        private float lifetimeMultiplier = default;

        private Transform _reParentTarget;

        private void OnEnable()
        {
            Invoke(nameof(Disable), lifetimeMultiplier * 2f);
        }

        public void SetParentTarget(Transform parent)
        {
            transform.parent = null;
            _reParentTarget = parent;
        }

        private void Disable()
        {
            transform.parent = _reParentTarget;
            gameObject.SetActive(false);
        }
    }
}