// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Threading.Tasks;
using Combat;
using Entity.AI.States;
using Entity.Player;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity.AI.Bosses.TwinofRa.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/TwinOfRa/PhaseOne")]
    public class TwinOfRaAttacking : AIBaseState
    {
        [SerializeField]
        private LayerMask hitMask = default;
        [SerializeField]
        private int maxTries = default;
        [SerializeField]
        private float phaseOneAttackFrequency = default;
        [SerializeField]
        private float phaseTwoAttackFrequency = default;
        [SerializeField]
        private float phaseOneAttackDelay = default;
        [SerializeField]
        private float phaseTwoAttackDelay = default;
        [SerializeField]
        private float phaseOneTeleportDelay = default;
        [SerializeField]
        private float phaseTwoTeleportDelay = default;
        [SerializeField]
        private float attackOffset = default;
        [SerializeField]
        private GameObject twinsOfRaLifeLineGO = default;
        [SerializeField]
        private AudioClip beginTeleportAudioClip = default;
        [SerializeField]
        private ScriptObjVar<Component> audioSource = default;

        private ScriptObjAttackBase _currentAttack;
        private TwinOfRa _twinsOfRa;
        private float _currentAttackDelay;
        private float _currentAttackFrequency;
        private float _currentPhaseAttackFrequency;
        private float _currentTeleportDelay;
        private float _currentTime;
        private bool _hasPlannedAttack;
        private bool _isTeleporting;
        private bool _usedAttackSinceTeleport;

        public override void Enter()
        {
            base.Enter();
            UpdateCurrentFrequency();
            _twinsOfRa = AI.GetComponent<TwinOfRa>();
            TwinOfRa.onPhaseTwo += OnPhaseTwo;
            _twinsOfRa.OnTakeDamage += OnTakeDamage;
            _currentPhaseAttackFrequency = phaseOneAttackFrequency;
            _currentAttackDelay = phaseOneAttackDelay;
            _currentTeleportDelay = phaseOneTeleportDelay;
        }

        private void OnTakeDamage()
        {
            Teleport(false);
        }

        private void OnPhaseTwo()
        {
            attackOffset = 0;
            _currentPhaseAttackFrequency = phaseTwoAttackFrequency;
            _currentAttackDelay = phaseTwoAttackDelay;
            _currentTeleportDelay = phaseTwoTeleportDelay;
            _currentAttack = AI.localAttacks[1];
        }

        private void UpdateCurrentFrequency()
        {
            _currentAttackFrequency = _currentPhaseAttackFrequency + Random.Range(0, attackOffset);
        }

        public override void Run()
        {
            if (_isTeleporting)
            {
                return;
            }

            if (_currentAttack == null)
            {
                _currentAttack = AI.localAttacks[0];
                Teleport(false);
            }

            if (!_currentAttack.IsAttacking)
            {
                if (_usedAttackSinceTeleport)
                {
                    Teleport();
                }
                else
                {
                    if (_currentTime >= _currentAttackFrequency)
                    {
                        if (SeesPlayer())
                        {
                            if (!_hasPlannedAttack)
                            {
                                StartAttack();
                            }
                        }
                        else
                        {
                            Teleport(false);
                        }

                        _currentTime = 0f;
                    }

                    RotateStatue();
                    _currentTime += Time.deltaTime;
                }
            }
        }

        private async void StartAttack()
        {
            _hasPlannedAttack = true;
            await Task.Delay(TimeSpan.FromSeconds(_currentAttackDelay));
            if (Application.isPlaying && AI != null)
            {
                _currentAttack.ForceAttack();
                _usedAttackSinceTeleport = true;
            }

            _hasPlannedAttack = false;
        }

        private bool SeesPlayer()
        {
            Physics.Linecast(AI.eyePosition.position, AI.playerPosition.Value + AI.playerCollider.Value.height / 2 * Vector3.up, out var hit, hitMask);
            return hit.collider != null && hit.collider.GetComponent<PlayerController>();
        }

        private async void Teleport(bool dropBall = true)
        {
            _isTeleporting = true;
            for (var currentTry = 0; currentTry < maxTries; currentTry++)
            {
                var position = _twinsOfRa.TeleportHandler.GetRandomSpawnPoint(_twinsOfRa.CapsuleCollider);
                if (position != Vector3.zero)
                {
                    if (dropBall)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_currentTeleportDelay));
                        if (!Application.isPlaying)
                        {
                            break;
                        }

                        DropLifeLine();
                    }

                    AI.transform.position = position;
                    break;
                }
            }

            _usedAttackSinceTeleport = false;
            UpdateCurrentFrequency();
            _isTeleporting = false;
            _currentTime = 0;
            (audioSource.value as AudioSource)?.PlayOneShot(beginTeleportAudioClip);
        }

        private void RotateStatue()
        {
            var prevRotation = _twinsOfRa.StatueModel.rotation;
            _twinsOfRa.StatueModel.LookAt(AI.playerPosition.Value, Vector3.up);
            _twinsOfRa.StatueModel.rotation = 
                new Quaternion(prevRotation.x, 
                    _twinsOfRa.StatueModel.rotation.y, 
                    prevRotation.z, 
                    _twinsOfRa.StatueModel.rotation.w);
        }

        private void DropLifeLine()
        {
            var lifeline =
                Instantiate(
                    twinsOfRaLifeLineGO, 
                    AI.transform.position + Vector3.up * _twinsOfRa.CapsuleCollider.height / 2, 
                    Quaternion.identity).GetComponent<TwinOfRaLifeLine>(); 
            lifeline.SetLink(_twinsOfRa);
        }
    }
}