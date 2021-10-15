// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace VFX
{
    public class LineGhostController : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer = default;
        [SerializeField]
        private float distance = default;
        [SerializeField]
        private bool pierceWalls = default;
        [SerializeField]
        private LayerMask layerMask = default;
        
        private Transform _owner;

        private void Update()
        {
            if (_owner == null)
            {
                return;
            }

            lineRenderer.SetPosition(1,
                Vector3.forward *
                (!pierceWalls && Physics.Raycast(transform.position, transform.forward, out var hit, distance, layerMask)
                    ? Vector3.Distance(transform.position, hit.point)
                    : distance));
        }

        public void Init(Transform owner)
        {
            _owner = owner;
        }
    }
}