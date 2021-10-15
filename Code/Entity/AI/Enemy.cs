// Primary Author : Andreas Berzelius - anbe5918
// Secondary Author : Maximiliam Rosén - maka4519
// Third Author : Erik Pilström - erpi3245 

using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Entity.HealthSystem;
using Framework;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using Framework.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.AI
{
    public abstract class Enemy : EntityBase
    {
        [SerializeField]
        private GameEvent onGameLoadReady = default;
        
        [Header("States")] 
        
        [SerializeField]
        protected State[] physicalStates = default;
        [SerializeField]
        protected State[] healthStates = default;
        [SerializeField]
        protected State[] behaviorStates = default;
        
        [Header("Attacks")] 
        
        [SerializeField]
        public List<ScriptObjAttackBase> attacks = default;
        [HideInInspector]
        public List<ScriptObjAttackBase> localAttacks = default;
        
        [Header("Sight")] 
        
        [SerializeField]
        public ScriptObjRef<Vector3> playerPosition = default;
        [SerializeField]
        public ScriptObjRef<CharacterController> playerCollider = default;
        [SerializeField]
        private LayerMask blockingLayers = default;
        [SerializeField]
        private ScriptObjVar<float> enemySightRange = default;
        [SerializeField]
        private string walkParamAnimName = default;
        [SerializeField]
        public Transform eyePosition = default;
        [HideInInspector]
        public NavMeshAgent agent = default;
        
        public static Action<GameObject> OnSpawned;
        public static Action<GameObject> OnDied;
        public static Action<GameObject> DoAlert;
        public static Action<GameObject> OnAttack;

        private Animator _animator;
        protected CapsuleCollider EnemyCollider;

        protected EnemyHealth Health { get; private set; }
        protected StateMachine PhysicalStateMachine { get; private set; }
        private StateMachine HealthStateMachine { get; set; }
        public StateMachine BehaviorStateMachine { get; private set; }
        public AnimationHandler AnimationHandler { get; private set; }
        public GroundChecker GroundChecker { get; private set; }
        public Rigidbody RigidBody { get; private set; }

        private void Awake()
        {
            RigidBody = GetComponent<Rigidbody>();
            EnemyCollider = GetComponent<CapsuleCollider>();
            _animator = GetComponentInChildren<Animator>();
            GroundChecker = GetComponentInChildren<GroundChecker>();
            AnimationHandler = GetComponentInChildren<AnimationHandler>();
            agent = GetComponent<NavMeshAgent>();
            Health = GetComponent<EnemyHealth>();
            PhysicalStateMachine = new StateMachine(physicalStates, this);
            HealthStateMachine = new StateMachine(healthStates, this);
            BehaviorStateMachine = new StateMachine(behaviorStates, this);
        }

        protected void Start()
        {
            HealthSystem.Health.EntityDied += EnemyDied;
            onGameLoadReady.OnEvent += DestroyMe;
            RegisterAttacks();
            OnSpawned?.Invoke(gameObject);
        }

        protected void Update()
        {
            PhysicalStateMachine.Run();
            HealthStateMachine.Run();
        }

        private void OnDestroy()
        {
            DoAlert = null;
            HealthSystem.Health.EntityDied -= EnemyDied;
            onGameLoadReady.OnEvent -= DestroyMe;
        }

        private void DestroyMe()
        {
            Destroy(gameObject);
        }

        private void RegisterAttacks()
        {
            foreach (var localAttack in attacks.Select(Instantiate))
            {
                localAttack.SetOwner(gameObject);
                localAttack.EnableAttackTrigger();
                localAttacks.Add(localAttack);
            }
        }

        public bool CanSeePlayer()
        {
            var distanceToPlayer = (playerPosition.Value - eyePosition.position).magnitude;
            Physics.Linecast(eyePosition.position, playerPosition.Value, out var hit, blockingLayers);
            if (!hit.collider || hit.collider && hit.collider.gameObject == gameObject &&
                distanceToPlayer <= enemySightRange.value)
            {
                return true;
            }

            return false;
        }

        protected virtual void EnemyDied(GameObject entity)
        {
            if (entity.Equals(gameObject))
            {
                OnDied?.Invoke(gameObject);
                gameObject.SetActive(false);
            }
        }

        public void StartWalkAnimation()
        {
            _animator.SetBool(walkParamAnimName, true);
        }

        public void StopWalkAnimation()
        {
            _animator.SetBool(walkParamAnimName, false);
        }

        public bool IsWalking()
        {
            return _animator.GetBool(walkParamAnimName);
        }

        public override Tuple<float, float> GetDimensions()
        {
            return new Tuple<float, float>(EnemyCollider.radius, EnemyCollider.height);
        }

        public override void TakeDamage(float damage)
        {
            Health.TakeDamage(damage);
        }
    }
}