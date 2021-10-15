// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Andreas Berzelius - anbe4918

using System;
using System.Threading.Tasks;
using Entity.AI.Mummies.Ranged;
using Entity.AI.States.Behavior;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity.AI.States.Combat
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Attacking")]
    public class Attacking : AIBaseState
    {
        [SerializeField] [Tooltip("speed for turning the enemy against the player")]
        private float turnSpeed = 5f;
        [SerializeField]
        private float attackCoolDownTimer = 1f;
        [SerializeField] [Tooltip("Time until transition back to Hunting state")]
        private float transitionCoolDownTimer = 1.25f;
        [SerializeField]
        private ScriptObjRef<float> attackRange = default;
        [SerializeField]
        private LayerMask layerMask = default;
        [SerializeField]
        private float fleeTriggerDistance = 5f;

        private bool _canAttack;
        private bool _canTurn;
        private bool _initialized;
        private float _timer;

        public override void Enter()
        {
            _timer = 0f;
            AI.StopWalkAnimation();
            if (!_initialized)
            {
                AI.AnimationHandler.OnAttackCompleted += AttackCooldown;
                Enemy.OnAttack?.Invoke(AI.gameObject);
                _initialized = true;
            }

            _canAttack = true;
            _canTurn = true;
            if (AI.AnimationHandler.enabled == false)
            {
                AI.AnimationHandler.enabled = true;
            }
        }

        public override void Run()
        {
            _timer += Time.deltaTime;
            var distanceMagnitude = (AI.playerPosition.Value - AI.transform.position).magnitude;
            if (distanceMagnitude < fleeTriggerDistance && StateMachine.HasState<Fleeing>())
            {
                StateMachine.TransitionTo<Fleeing>();
            }
            else
            {
                if (_canTurn)
                {
                    LookAtPlayer();
                }

                if (_canAttack)
                {
                    if (AI is MummyRanged ranged && !ranged.CanSeePlayer())
                    {
                        return;
                    }

                    (AI as MummyRanged)?.handLight.gameObject.SetActive(true);
                    AI.localAttacks[Random.Range(0, AI.localAttacks.Count)].Trigger();
                    _canAttack = false;
                    _canTurn = false;
                }
            }
        }

        private async void TransitionToHunting()
        {
            AI.AnimationHandler.enabled = false;
            await Task.Delay(TimeSpan.FromSeconds(transitionCoolDownTimer));
            _timer = 0f;
            StateMachine.TransitionTo<Hunting>();
        }

        private async void AttackCooldown()
        {
            (AI as MummyRanged)?.handLight.FadeOut();
            if (_timer >= 0.3f)
            {
                PlayerOutOfRange();
            }

            _canTurn = true;
            await Task.Delay(TimeSpan.FromSeconds(attackCoolDownTimer));
            _canAttack = true;
        }

        private void LookAtPlayer()
        {
            var newRotation = Quaternion.LookRotation(AI.playerPosition.Value - AI.transform.position, Vector3.up);
            newRotation.x = 0.0f;
            newRotation.z = 0.0f;
            AI.transform.rotation = Quaternion.Slerp(AI.transform.rotation, newRotation, Time.deltaTime * turnSpeed);
        }

        private void PlayerOutOfRange()
        {
            var closestPoint = AI.playerCollider.Value.ClosestPointOnBounds(AI.eyePosition.position);
            var playerPos = new Vector3(AI.playerPosition.Value.x, closestPoint.y, AI.playerPosition.Value.z);
            Physics.Linecast(AI.eyePosition.position, playerPos, out var hit, layerMask);
            if (!hit.collider || hit.distance >= attackRange.Value)
            {
                TransitionToHunting();
            }

            _timer = 0f;
        }
    }
}