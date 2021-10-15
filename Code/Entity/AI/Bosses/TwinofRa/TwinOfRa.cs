// Primary Author : Maximiliam Rosén - maka4519

using System;
using Entity.HealthSystem;
using Environment.RoomManager;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UI.Bar;
using UnityEngine;

namespace Entity.AI.Bosses.TwinofRa
{
    public class TwinOfRa : Boss
    {
        [SerializeField]
        private int bossHealthBarIndex = default;
        [SerializeField]
        private Color color = default;
        [SerializeField]
        private Spawner teleportHandler = default;
        [SerializeField]
        private GameEvent startEvent = default;
        [SerializeField]
        private CapsuleCollider capsuleCollider = default;
        [SerializeField]
        private ScriptObjVar<float> phaseTwoHealth = default;
        [SerializeField]
        private Transform statueModel = default;

        public static Action onPhaseTwo;
        private bool _isEnabled;
        private EnemyHealth _enemyHealth;
        public Action OnTakeDamage;

        public Spawner TeleportHandler => teleportHandler;
        public CapsuleCollider CapsuleCollider => capsuleCollider;
        public Color Color => color;
        public Transform StatueModel => statueModel;

        public new void Start()
        {
            startEvent.OnEvent += EnableBoss;
            _enemyHealth = GetComponent<EnemyHealth>();
            base.Start();
        }

        private new void Update()
        {
            if (_isEnabled)
            {
                BehaviorStateMachine.Run();
                base.Update();
            }
        }

        private void OnDisable()
        {
            startEvent.OnEvent -= EnableBoss;
            BossHealthBarManager.Instance.Remove(bossHealthBarIndex);
        }

        private void EnableBoss()
        {
            BossHealthBarManager.Instance.Add(displayName, GetComponent<Health>(), bossHealthBarIndex);
            _isEnabled = true;
        }

        protected override void EnemyDied(GameObject entity)
        {
            if (entity == gameObject)
            {
                OnBossDied?.Invoke();
                Destroy(gameObject);
            }
            else if (entity.GetComponent<TwinOfRa>())
            {
                _enemyHealth.SetCurrentHealth(phaseTwoHealth);
                BossHealthBarManager.Instance.Refresh(bossHealthBarIndex, "The Perpetually Solitary, Abandoned, and Forgotten Son of Ra");
                onPhaseTwo?.Invoke();
            }
        }

        public void LifeLineDestroyed(float damage)
        {
            Health.TakeDamage(damage);
        }

        public override void TakeDamage(float damage)
        {
            if (_isEnabled)
            {
                OnTakeDamage?.Invoke();
            }
        }
    }
}