// Primary Author : Maximiliam Rosén - maka4519

using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Framework;
using Scripts.Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace Entity.Player.MovementAbilities
{
    public class PlayerDash : MovementAbility
    {
        [Header("Conditions Info")] 
        
        [SerializeField]
        private InvincibilityCondition invincibilityCondition = default;
        [SerializeField]
        private MoveCondition moveCondition = default;
        [SerializeField]
        private VisualEffect dashVFX = default;
        [SerializeField]
        private FOVController fovController = default;
        [SerializeField] [Tooltip("x = intensity, y = duration, z = transition speed")]
        private Vector3 fovChange = default;

        private Animator _animator;
        private bool _canUse = true;
        private CharacterController _controller;
        private GroundChecker _groundChecker;
        private RotateWithCamera _rotateWithCamera;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        protected override void Start()
        {
            base.Start();
            _groundChecker = GetComponentInChildren<GroundChecker>();
            _controller = GetComponent<CharacterController>();
            _rotateWithCamera = GetComponent<RotateWithCamera>();
        }

        private void Update()
        {
            if (Movement.isActiveAndEnabled && Movement.PlayerGrounded && notInCoolDown)
            {
                _canUse = true;
            }
        }

        public void Dash(InputAction.CallbackContext obj)
        {
            if (_canUse && obj.performed)
            {
                _animator.SetBool("Dashing", true);
                _canUse = false;
                dashVFX.Play();
                if (fovController != null)
                {
                    fovController.FOVFlash(fovChange.x, fovChange.y, fovChange.z);
                }

                StartCooldown();
                var forward = transform.forward;
                moveCondition.MoveDirection = _groundChecker.IsGrounded
                    ? Movement.GetMoveInput().magnitude != 0 ? forward : -forward
                    : forward;
                Movement.PlayerVelocity = Movement.PlayerVelocity.magnitude * moveCondition.MoveDirection;
                moveCondition.Height = _controller.height;
                moveCondition.Radius = _controller.radius;
                PlayerController.AnimationHandler.AttackInterrupted();
                _rotateWithCamera.enabled = false;
                ConditionManager.AddCondition(moveCondition, PlayerController, PlayerController);
                ConditionManager.AddCondition(invincibilityCondition, PlayerController, PlayerController);
            }
        }
    }
}