// Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Framework
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField]
        private ScriptObjRef<bool> isGrounded = default;
        [SerializeField]
        private float groundDistance = default;
        [SerializeField]
        private LayerMask groundLayer = default;
        [SerializeField]
        private float slopeLimit = default;
        [SerializeField]
        private GameEvent becameGrounded = default;

        public Vector3 GroundNormal { get; private set; }
        public GameObject GroundGameObject { get; private set; }
        public bool IsGrounded => isGrounded.Value;
        public bool HasGround { get; private set; }

        private void Update()
        {
            CheckForGround();
            GetGroundNormal();
            CheckIfGrounded();
        }

        private void CheckForGround()
        {
            HasGround = Physics.CheckSphere(transform.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
        }

        private void GetGroundNormal()
        {
            Physics.Raycast(transform.position, Vector3.down, out var hit, 4f, groundLayer, QueryTriggerInteraction.Ignore);
            if (HasGround && hit.collider)
            {
                GroundNormal = hit.normal;
                GroundGameObject = hit.collider.gameObject;
            }
        }

        private void CheckIfGrounded()
        {
            var result = HasGround && Vector3.Angle(Vector3.up, GroundNormal) <= slopeLimit;
            if (result && result != isGrounded.Value) becameGrounded?.Raise();
            if (isGrounded.useConstant)
            {
                isGrounded.constantValue = result;
            }
            else
            {
                isGrounded.Value = result;
            }
        }
    }
}