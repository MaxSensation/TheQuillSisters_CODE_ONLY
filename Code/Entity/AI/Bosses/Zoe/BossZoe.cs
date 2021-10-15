// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Andreas Berzelius - anbe5918

using System;
using Entity.AI.Bosses.Zoe.Attacks;
using Entity.HealthSystem;
using Environment.RoomManager;
using Framework.ScriptableObjectEvent;
using UI.Bar;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.States
{
    public class BossZoe : Boss
    {
        [HideInInspector]
        public Transform roomGasTransform = default;
        [HideInInspector]
        public SphereCollider roomGasSphereCollider = default;
        [HideInInspector]
        public AudioSource zoeAudioSource = default;
        [SerializeField]
        private AudioClip music = default;
        [SerializeField]
        private GameEvent endEvent = default;
        [SerializeField]
        private GameObject roomGasObj = default;
        [SerializeField]
        private Room roomManager = default;
        [SerializeField]
        private GameObject forceField = default;
        [SerializeField]
        [Tooltip("At which percentage of lost health from maximum health \nshall room be filled with more poison")]
        private int damageThresholdFraction = default;
        
        public static Action IncreaseGas;
        private float _damageThreshHold;
        private bool _retreat;
        private RoomGas _roomGas;

        private void OnEnable()
        {
            MusicManager.OnMusicStart?.Invoke(music);
            BossHealthBarManager.Instance.Add(displayName, GetComponent<EnemyHealth>(), 0);
            roomGasTransform = roomGasObj.transform;
            roomGasSphereCollider = roomGasObj.GetComponent<SphereCollider>();
            zoeAudioSource = GetComponent<AudioSource>();
            _roomGas = roomGasObj.GetComponent<RoomGas>();
            _roomGas.gameObject.SetActive(true);
            _roomGas.enabled = true;
            _roomGas.Filled += InitializeRetreat;
            roomManager.OnRoomCompleted += InitializePhaseTwo;
            HealthSystem.Health.TookDamage += CheckToFill;
            forceField.SetActive(true);
            HealthSystem.Health.EntityDied += Died;
        }

        private void OnDisable()
        {
            BossHealthBarManager.Instance.Remove(0);
            _roomGas.Filled -= InitializeRetreat;
            roomManager.OnRoomCompleted -= InitializePhaseTwo;
            HealthSystem.Health.TookDamage -= CheckToFill;
            HealthSystem.Health.EntityDied -= Died;
        }

        private void Died(GameObject obj)
        {
            if (gameObject == obj)
            {
                endEvent.Raise();
                _roomGas.enabled = false;
                _roomGas.gameObject.SetActive(false);
                MusicManager.OnMusicStop?.Invoke();
            }
        }

        private void InitializeRetreat()
        {
            if (_retreat)
            {
                BehaviorStateMachine.TransitionTo<Retreat>();
                _retreat = false;
            }
        }

        private void CheckToFill(GameObject obj, float damage)
        {
            if (!_retreat)
            {
                if (obj != gameObject) return;
                _damageThreshHold += damage;
                if (_damageThreshHold >= Health.GetMaxHealth() / damageThresholdFraction)
                {
                    IncreaseGas?.Invoke();
                    _retreat = true;
                    _damageThreshHold = 0;
                }
            }
        }

        private void InitializePhaseTwo()
        {
            BehaviorStateMachine.TransitionTo<PhaseTwo>();
            forceField.SetActive(false);
            IncreaseGas?.Invoke();
        }
    }
}