// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Erik Pilström - erpi3245

using Framework;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private ScriptObjVar<CharacterController> playerCollider;
        
        [Header("Mobility")]
        
        [SerializeField]
        private ScriptObjVar<Vector3> playerVelocity = default;
        [SerializeField]
        private ScriptObjVar<Vector3> playerPosition = default;
        [SerializeField]
        private ScriptObjVar<bool> playerJumped = default;
        [SerializeField]
        private float acceleration = default;
        [SerializeField] [Range(0f, 1f)] 
        private float airControl = default;
        [SerializeField]
        private float maxSpeed = default;
        [SerializeField]
        private float unStuckForce = default;
        
        [Header("Rotation Info")]
        
        [SerializeField] [Range(0.01f, 1f)]
        private float targetDirectionAlignment = 0.9f;
        
        [Header("World Info")]
        
        [SerializeField]
        private ScriptObjVar<float> gravity = default;
        [SerializeField]
        private ScriptObjVar<float> groundFriction = default;
        [SerializeField]
        private ScriptObjVar<float> airFriction = default;
        [SerializeField]
        private ScriptObjVar<float> slopeFriction = default;
        
        private CharacterController _controller;
        private GroundChecker _groundChecker;
        private Vector3 _moveInput;
        private RotateWithCamera _rotateWithCamera;

        public bool PlayerGrounded => _groundChecker.IsGrounded;

        public Vector3 PlayerVelocity
        {
            get => playerVelocity.value;
            set => playerVelocity.value = value;
        }

        public bool PlayerIsFalling { get; set; }

        private void Start()
        {
            _groundChecker = GetComponentInChildren<GroundChecker>();
            _rotateWithCamera = GetComponent<RotateWithCamera>();
            _controller = GetComponent<CharacterController>();
            playerCollider.value = _controller;
            playerVelocity.value = Vector3.zero;
            UpdatePlayerPosition();
        }

        private void Update()
        {
            UpdatePlayerPosition();
            AddGravity();
            AddFriction();
            AddSlopeForce();
            CheckIfFalling();
            CapVelocity();
            ApplyVelocity();
            CancelGravity();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Fix Stuck problem
            var dotProduct = Vector3.Dot(hit.moveDirection, Vector3.down);
            if (!_groundChecker.IsGrounded && dotProduct > 0.99f)
            {
                playerVelocity.value += (transform.position - hit.point).normalized * (unStuckForce * Time.deltaTime);
            }

            // Fix Head problem
            if (dotProduct < -0.99f && playerVelocity.value.y > 0f)
            {
                playerVelocity.value.y = 0;
            }
        }

        private void CapVelocity()
        {
            playerVelocity.value = Vector3.ClampMagnitude(playerVelocity.value, maxSpeed);
        }

        private void UpdatePlayerPosition()
        {
            playerPosition.value = transform.position;
        }

        private void CheckIfFalling()
        {
            if (!PlayerIsFalling && !_groundChecker.IsGrounded && playerVelocity.value.y < 0f)
            {
                PlayerIsFalling = true;
            }
            else if (_groundChecker.IsGrounded)
            {
                PlayerIsFalling = false;
            }
        }

        private void CancelGravity()
        {
            if (_groundChecker.IsGrounded && playerVelocity.value.y < 0f)
            {
                playerVelocity.value.y = gravity / 4;
                playerJumped.value = false;
            }
        }

        private void AddSlopeForce()
        {
            if (!_groundChecker.IsGrounded && _groundChecker.HasGround)
            {
                playerVelocity.value.x += (1f - _groundChecker.GroundNormal.y) * _groundChecker.GroundNormal.x *
                                          (1f - slopeFriction) * Time.deltaTime;
                playerVelocity.value.z += (1f - _groundChecker.GroundNormal.y) * _groundChecker.GroundNormal.z *
                                          (1f - slopeFriction) * Time.deltaTime;
            }
        }

        private void AddFriction()
        {
            if (_groundChecker.IsGrounded)
            {
                playerVelocity.value.x /= 1 + groundFriction * Time.deltaTime;
                playerVelocity.value.z /= 1 + groundFriction * Time.deltaTime;
            }
            else
            {
                playerVelocity.value.x /= 1 + airFriction * Time.deltaTime;
                playerVelocity.value.z /= 1 + airFriction * Time.deltaTime;
            }
        }

        private void ApplyVelocity()
        {
            _controller.Move(playerVelocity.value * Time.deltaTime);
        }

        private void AddGravity()
        {
            playerVelocity.value.y += gravity * Time.deltaTime;
        }

        public void AddMovement()
        {
            if (_rotateWithCamera.WantedPercentageDirection > targetDirectionAlignment)
            {
                if (_groundChecker.IsGrounded)
                {
                    playerVelocity.value +=
                        transform.forward * (_moveInput.magnitude * (Time.deltaTime * acceleration));
                }
                else
                {
                    playerVelocity.value += transform.forward *
                                            (_moveInput.magnitude * (Time.deltaTime * acceleration * airControl));
                }
            }
        }

        public void UpdateMove(InputAction.CallbackContext obj)
        {
            var moveVector = obj.ReadValue<Vector2>();
            _moveInput = new Vector3(moveVector.x, 0, moveVector.y).normalized;
        }

        public Vector3 GetMoveInput()
        {
            return _moveInput;
        }
    }
}