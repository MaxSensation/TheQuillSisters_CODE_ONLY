// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using Combat.ConditionSystem;
using Entity.Player.States.Physical;
using Framework;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player.MovementAbilities
{
    public class PlayerJump : MovementAbility
    {
        [SerializeField] 
        private float jumpHeight = default;
        [SerializeField] 
        private ScriptObjVar<bool> playerJumped = default;
        
        [Header("World Info")] 
        
        [SerializeField]
        private ScriptObjVar<float> gravity = default;
        [SerializeField] [Tooltip("Place conditions that would interfere with jump here")]
        private List<ConditionBase> conditions = default;
        [SerializeField] 
        private LayerMask enemyLayer = default;

        public static Action PlayerJumped;
        private GroundChecker _groundChecker;
        private ActionGroup<EntityBase> _group;
        private PlayerController _playerController;

        protected override void Start()
        {
            base.Start();
            _playerController = GetComponentInChildren<PlayerController>();
            _groundChecker = GetComponentInChildren<GroundChecker>();
        }

        public void JumpRequested(InputAction.CallbackContext obj)
        {
            if (NoEnemyOnHead() && _groundChecker.IsGrounded && obj.performed)
                if (conditions != null)
                {
                    if (ConditionManager.HasAny(conditions, PlayerController))
                    {
                        _group = ConditionManager.CancelMultiple(conditions, PlayerController);
                        _group.whenAll += () => Jump(obj);
                    }
                    else
                    {
                        Jump(obj);
                    }
                }
        }

        private bool NoEnemyOnHead()
        {
            return !Physics.CheckBox(
                transform.position, 
                Vector3.up * _playerController.CharController.height, 
                Quaternion.identity, enemyLayer);
        }

        private void Jump(InputAction.CallbackContext obj)
        {
            _group?.Dispose();
            _group = null;
            playerJumped.value = true;
            Movement.PlayerVelocity = new Vector3(Movement.PlayerVelocity.x, 0, Movement.PlayerVelocity.z);
            Movement.PlayerVelocity += Vector3.up * Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlayerJumped?.Invoke();
            _playerController.PhysicalStateMachine.TransitionTo<PlayerJumpingState>();
        }
    }
}