// Primary Author : Maximiliam Rosén - maka4519

using Entity;
using UnityEngine;

namespace VFX
{
    public class WeaponTrailRenderer : MonoBehaviour
    {
        [SerializeField]
        private float activationVelocity = default;
        [SerializeField]
        private AnimationHandler animationHandler = default;
        
        private Vector3 _handVelocity;
        private Vector3 _prevFramePosition;
        private TrailRenderer _trailRenderer;

        private void Start()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            _prevFramePosition = transform.position;
        }

        private void Update()
        {
            _handVelocity = _prevFramePosition - transform.position;
            if (_handVelocity.magnitude > activationVelocity && animationHandler.IsAttacking)
            {
                EnableTrailRenderer();
            }
            else
            {
                DisableTrailRenderer();
            }

            _prevFramePosition = transform.position;
        }

        private void EnableTrailRenderer()
        {
            _trailRenderer.emitting = true;
        }

        private void DisableTrailRenderer()
        {
            _trailRenderer.emitting = false;
        }
    }
}