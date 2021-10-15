// Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    public class RotateWithCamera : MonoBehaviour
    {
        [SerializeField] [Range(0.01f, 1f)] 
        private float turnAroundSpeed = 0.2f;
        
        [Header("Camera Info")] 
        
        [SerializeField]
        private ScriptObjRef<Quaternion> cameraRotation = default;

        private Vector3 _moveInput;
        public float WantedPercentageDirection { get; private set; }

        private void Update()
        {
            RotatePlayerWithCamera();
        }

        public void UpdateMove(InputAction.CallbackContext obj)
        {
            var moveVector = obj.ReadValue<Vector2>();
            _moveInput = new Vector3(moveVector.x, 0, moveVector.y);
        }

        private void RotatePlayerWithCamera()
        {
            if (_moveInput != Vector3.zero)
            {
                var rotationVector = new Vector3(0, cameraRotation.Value.eulerAngles.y, 0);
                var adjustedRot = Quaternion.Euler(rotationVector);
                var wantedDirection = (adjustedRot * _moveInput).normalized;
                var forward = transform.forward;
                WantedPercentageDirection = Mathf.Clamp01(Vector3.Dot(wantedDirection, forward));
                forward = Vector3.Slerp(forward, adjustedRot * _moveInput * Time.deltaTime, turnAroundSpeed);
                transform.forward = forward;
            }
        }
    }
}