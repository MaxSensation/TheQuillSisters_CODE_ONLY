// Primary Author : Andreas Berzelius - anbe5918
// Secondary Author : Viktor Dahlberg - vida6631

using Entity.AI.Mummies;
using Entity.AI.States.Combat;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.AI.States.Behavior
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Hunting")]
    public class Hunting : AIBaseState
    {
        [SerializeField] [Tooltip("speed for turning the enemy against the player")]
        private float turnSpeed = 5f;
        [SerializeField]
        private ScriptObjRef<float> attackRange = default;
        [SerializeField]
        private LayerMask layerMask = default;
        [SerializeField]
        private float beginRotatingOffset = default;
        [SerializeField] 
        private float fleeTriggerDistance = 5f;

        private bool _initialized;
        private float _timer;

        public override void Enter()
        {
            if (AI == null)
            {
                return;
            }

            AI.agent.avoidancePriority = AI is MummyGiant ? 50 : 51;
            _timer = 0f;
            if (!_initialized)
            {
                _initialized = true;
                Enemy.DoAlert?.Invoke(AI.gameObject);
            }
        }

        public override void Run()
        {
            _timer += Time.deltaTime;
            base.Run();
            if ((AI.playerPosition.Value - AI.agent.pathEndPosition).magnitude >= 0.1f && AI.agent.enabled)
            {
                AI.agent.isStopped = false;
                AI.agent.SetDestination(AI.playerPosition.Value);
            }

            var distanceMagnitude = (AI.playerPosition.Value - AI.transform.position).magnitude;
            if (distanceMagnitude < fleeTriggerDistance && StateMachine.HasState<Fleeing>())
            {
                StateMachine.TransitionTo<Fleeing>();
            }
            else
            {
                if (distanceMagnitude < attackRange.Value - beginRotatingOffset)
                {
                    LookAtPlayer();
                }

                if (distanceMagnitude < attackRange.Value && _timer >= 0.3f)
                {
                    PlayerInRange();
                }
            }
        }

        private void LookAtPlayer()
        {
            var newRotation = Quaternion.LookRotation(AI.playerPosition.Value - AI.transform.position, Vector3.up);
            newRotation.x = 0.0f;
            newRotation.z = 0.0f;
            AI.transform.rotation = Quaternion.Slerp(AI.transform.rotation, newRotation, Time.deltaTime * turnSpeed);
        }

        private void PlayerInRange()
        {
            var closestPoint = AI.playerCollider.Value.ClosestPointOnBounds(AI.eyePosition.position);
            var playerPos = new Vector3(AI.playerPosition.Value.x, closestPoint.y, AI.playerPosition.Value.z);
            var smallOffset = (playerPos - AI.eyePosition.position).normalized * 0.001f;
            Physics.Linecast(AI.eyePosition.position, playerPos + smallOffset, out var hit, layerMask);
            if (!hit.collider)
            {
                if (hit.distance <= attackRange.Value && AI.agent.enabled)
                {
                    AI.agent.isStopped = true;
                    StateMachine.TransitionTo<Attacking>();
                }
            }

            _timer = 0f;
        }

        public override void Exit()
        {
            _timer = 0f;
            AI.agent.avoidancePriority = 49;
        }
    }
}