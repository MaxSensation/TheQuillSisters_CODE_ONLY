// Primary Author : Andreas Berzelius - anbe4918

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.AI.States.Physical
{
    [CreateAssetMenu(menuName = "States/EnemyStates/PhysicalStates/Airborne")]
    public class Airborne : AIBaseState
    {
        [SerializeField] 
        private float groundDetectionWait = default;
        [SerializeField] 
        private float initialWait = default;
        [SerializeField] 
        private float reWait = default;

        private bool _ready;
        private float _startTime;
        private float _waitTime;

        private void Reset(GameObject gameObject)
        {
            if (AI == null)
            {
                HealthSystem.Health.TookDamage -= (go, damage) => Reset(go);
            }
            else if (gameObject.Equals(AI.gameObject))
            {
                DisableRigidbody();
                _startTime = Time.time;
            }
        }

        public override void Enter()
        {
            _waitTime = initialWait;
            HealthSystem.Health.TookDamage += (go, damage) => Reset(go);
            _startTime = Time.time;
            AI.GroundChecker.enabled = true;
            _ready = false;
            WaitALittle();
        }

        public override void Exit()
        {
            HealthSystem.Health.TookDamage -= (go, damge) => Reset(go);
            AI.GroundChecker.enabled = false;
        }

        public override void Run()
        {
            if (Time.time - _startTime > _waitTime)
            {
                _waitTime = reWait;
                EnableRigidbody();
            }

            if (AI.GroundChecker.IsGrounded && _ready)
            {
                DisableRigidbody();
                StateMachine.TransitionTo<Grounded>();
            }
        }

        private async void WaitALittle()
        {
            await Task.Delay(TimeSpan.FromSeconds(groundDetectionWait));
            _ready = true;
        }

        private void EnableRigidbody()
        {
            AI.RigidBody.useGravity = true;
            AI.RigidBody.isKinematic = false;
        }

        private void DisableRigidbody()
        {
            AI.RigidBody.useGravity = false;
            AI.RigidBody.isKinematic = true;
        }
    }
}