// Primary Author : Andreas Berzelius - anbe5918

using UnityEngine;
using UnityEngine.AI;

namespace Entity.AI.Bosses.Zoe.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/Zoe/Retreat")]
    public class Retreat : ZoeBaseState
    {
        [SerializeField] 
        private float waitTillAttackTimer = 0.5f;
        [SerializeField] 
        [Range(0f, 1f)] private float areaOffset = default;
        [SerializeField] 
        private int retreatAcceleration = 50;
        [SerializeField] 
        private int retreatSpeed = 15;
        [SerializeField] 
        private float maxRetreatSampleDistance = 1.5f;
        [SerializeField]
        private int originalAcceleration = 20;
        [SerializeField]
        private int originalSpeed = 5;
        
        private bool _retreated;
        private bool _usedAttackIndex;
        private int _attackIndex;
        private float _timer;
        private Vector3 _currentRetreatPos;
        
        public override void Enter()
        {
            _timer = 0;
            _retreated = false;
            if (!_usedAttackIndex)
            {
                _attackIndex = 3;
                _usedAttackIndex = true;
            }
            else
            {
                _attackIndex = 2;
                _usedAttackIndex = false;
            }

            SetRetreatPosition();
        }
        
        private void SetRetreatPosition()
        {
            var roomGasPosition = Boss.roomGasTransform.position;
            var roomGasRadius = Boss.roomGasTransform.localScale.x * Boss.roomGasSphereCollider.radius;
            var playerToCenter = roomGasPosition - Boss.playerPosition.Value;
            var edgeWithOffset = playerToCenter.normalized * (roomGasRadius - areaOffset * roomGasRadius);
            edgeWithOffset += roomGasPosition;
            var retreatPos = new Vector3(edgeWithOffset.x, Boss.transform.position.y, edgeWithOffset.z);
            Boss.agent.acceleration = retreatAcceleration;
            Boss.agent.speed = retreatSpeed;
            NavMesh.SamplePosition(retreatPos, out var hit, maxRetreatSampleDistance, NavMesh.AllAreas);
            Boss.agent.SetDestination(hit.position);
            _currentRetreatPos = hit.position;
        }

        public override void Run()
        {
            base.Run();
            Boss.agent.SetDestination(_currentRetreatPos);
            if ((Boss.agent.remainingDistance <= 1f || Boss.agent.velocity.magnitude <= 0.01f) && !_retreated)
            {
                _timer += Time.deltaTime;
                LookAtPlayer();
                if (_timer >= waitTillAttackTimer)
                {
                    Boss.localAttacks[_attackIndex].Trigger();
                    _retreated = true;
                    _timer = 0;
                }
            }

            // Vänta på att magin är klar sen gå tillbaka
            if (_retreated && !Boss.localAttacks[_attackIndex].IsAttacking)
            {
                Boss.BehaviorStateMachine.TransitionTo<PhaseTwo>();
            }
        }

        public override void Exit()
        {
            Boss.agent.acceleration = originalAcceleration;
            Boss.agent.speed = originalSpeed;
        }
    }
}