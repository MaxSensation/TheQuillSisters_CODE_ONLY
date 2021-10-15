// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using Combat;
using Combat.ConditionSystem;
using Combat.Interfaces;
using Entity.HealthSystem;
using Entity.Player.States.Physical;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using Framework.StateMachine;
using Player.States.Health;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity.Player
{
    public class PlayerController : EntityBase, IMovable, IHoverable
    {
        
        [Header("Debug")] 
        
        [SerializeField]
        private bool enableMovementImmediately;
        [SerializeField] [Space]
        private CharacterController characterController;
        [SerializeField]
        private GameObject playerModel;
        
        [Header("States")]
        
        [SerializeField] 
        private State[] physicalStates;
        [SerializeField] 
        private State[] healthStates;
        
        [Header("Attacks")]
        
        [SerializeField] 
        private List<ScriptObjAttackBase> attacks;
        
        
        [Header("Events")] 
        
        [SerializeField]
        private GameEvent saveManagerDone;
        [SerializeField]
        private GameEvent scenePreloaderLoaded;
        
        
        [Header("Reposition")]
        
        [SerializeField]
        private ScriptObjVar<Vector3> playerRespawnPos;
        [SerializeField] 
        private ScriptObjVar<Quaternion> playerRespawnRot;

        private Tuple<float, float> _cachedDimensions;

        private readonly HashSet<ConditionBase> _moveConditions = new HashSet<ConditionBase>();
        public static Action OnPlayerDied;
        private bool _isHoverCondition;
        private bool _isMoveCondition;
        private bool _onDiedSubscribed;
        private PlayerHealth _health;
        private RotateWithCamera _rotateWithCamera;
        private GameObject PlayerMesh => playerModel;
        public CharacterController CharController => characterController;
        public PlayerMovement Movement { get; private set; }
        public Animator Anim { get; private set; }
        public StateMachine PhysicalStateMachine { get; private set; }
        public StateMachine HealthStateMachine { get; private set; }
        public AnimationHandler AnimationHandler { get; private set; }

        private void Awake()
        {
            _rotateWithCamera = GetComponent<RotateWithCamera>();
            AnimationHandler = GetComponentInChildren<AnimationHandler>();
            Movement = GetComponent<PlayerMovement>();
            Anim = GetComponentInChildren<Animator>();
            PhysicalStateMachine = new StateMachine(physicalStates, this);
            HealthStateMachine = new StateMachine(healthStates, this);
            UpdateAttacks();
        }

        private void Start()
        {
            LoadingScreen.GameLoadRequested += FreezePlayer;
            Movement.PlayerVelocity = Vector3.zero;
            Movement.enabled = enableMovementImmediately;
            Health.EntityDied += PlayerDied;
            _onDiedSubscribed = true;
            AnimationHandler.OnAttackStarted += () => PhysicalStateMachine.TransitionTo<PlayerAttackingState>();
            _health = GetComponent<PlayerHealth>();
            if (!FindObjectOfType<MenuManager>())
            {
                characterController.enabled = true;
                Movement.enabled = true;
            }
        }

        private void Update()
        {
            PhysicalStateMachine.Run();
            HealthStateMachine.Run();
        }

        public void EnableHover()
        {
            _isHoverCondition = true;
            Movement.PlayerVelocity = Vector3.zero;
            Movement.enabled = false;
        }

        public void DisableHover()
        {
            _isHoverCondition = false;
            if (!_isMoveCondition && _onDiedSubscribed)
            {
                Movement.enabled = true;
            }
            Movement.PlayerVelocity = Vector3.down;
            Movement.PlayerIsFalling = true;
            PhysicalStateMachine.TransitionTo<PlayerFallingState>();
        }

        public void MoveToPosition(Vector3 movePosition)
        {
            characterController.Move(movePosition);
        }

        public void OnMoveCompleted(ConditionBase condition)
        {
            _moveConditions.Remove(condition);
            _isMoveCondition = false;
            if (!_isHoverCondition && _moveConditions.Count == 0 && _onDiedSubscribed)
            {
                Movement.enabled = true;
            }
            if (_moveConditions.Count == 0)
            {
                _rotateWithCamera.enabled = true;
            }
            Movement.PlayerVelocity = new Vector3(Movement.PlayerVelocity.x, 0, Movement.PlayerVelocity.z) * 1.5f;
            Anim.SetBool("Dashing", false);
        }

        public void OnMoveStarted(ConditionBase condition)
        {
            _moveConditions.Add(condition);
            _isMoveCondition = true;
            Movement.PlayerVelocity = Vector3.zero;
            Movement.enabled = false;
        }

        private void PlayerDied(GameObject obj)
        {
            //In the future the player should not die and instantly disappear
            if (obj.Equals(gameObject))
            {
                Health.EntityDied -= PlayerDied;
                _onDiedSubscribed = false;
                ConditionManager.CancelAllConditionOnEntity(this);
                HealthStateMachine.TransitionTo<Dead>();
                OnPlayerDied?.Invoke();
                AnimationHandler.AttackInterrupted();
            }
        }

        private void UpdateAttacks()
        {
            foreach (var attack in attacks)
            {
                attack.SetOwner(gameObject);
                if (!attack.isFollowup)
                {
                    attack.EnableAttackTrigger();
                }
                else
                {
                    attack.DisableAttackTrigger();
                }
            }
        }

        public override Tuple<float, float> GetDimensions()
        {
            return _cachedDimensions ?? (_cachedDimensions = new Tuple<float, float>(characterController.radius, characterController.height));
        }

        public override void TakeDamage(float damage)
        {
            _health.TakeDamage(damage);
        }

        #region Spawning & Respawning

        /// <summary>
        ///     Disables player movement and mesh.
        /// </summary>
        /// <param name="isDefault"></param>
        private void FreezePlayer(bool isDefault)
        {
            saveManagerDone.OnEvent += TeleportPlayer;
            Movement.enabled = false;
            PlayerMesh.SetActive(false);
        }

        /// <summary>
        ///     Teleports player.
        /// </summary>
        private void TeleportPlayer()
        {
            saveManagerDone.OnEvent -= TeleportPlayer;
            scenePreloaderLoaded.OnEvent += UnFreezePlayer;
            Movement.PlayerVelocity = Vector3.zero;
            characterController.transform.position = playerRespawnPos;
            gameObject.transform.rotation = playerRespawnRot;
        }

        /// <summary>
        ///     Enables player movement and mesh.
        /// </summary>
        private void UnFreezePlayer()
        {
            scenePreloaderLoaded.OnEvent -= UnFreezePlayer;
            Movement.enabled = true;
            PlayerMesh.SetActive(true);
            if (!_onDiedSubscribed)
            {
                Health.EntityDied += PlayerDied;
                _onDiedSubscribed = true;
            }
        }

        #endregion
    }
}