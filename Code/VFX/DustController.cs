// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;
using UnityEngine.VFX;

namespace VFX
{
    public class DustController : MonoBehaviour
    {
        [SerializeField]
        private Transform target = default;
        [SerializeField]
        private VisualEffect vfx = default;

        private Vector3 _lastPosition;

        private void Start()
        {
            _lastPosition = target.position;
            vfx.Play();
        }

        private void LateUpdate()
        {
            var targetPosition = target.position;
            transform.position = targetPosition;
            var positionDelta = _lastPosition - targetPosition;
            vfx.playRate = 1 + positionDelta.magnitude;
            vfx.SetVector3("Delta", positionDelta);
            _lastPosition = targetPosition;
        }
    }
}