// Primary Author : Andreas Berzelius - anbe5918

using Entity.AI.States.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.AI.Scarabs.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Scarab/FlockIdling")]
    public class FlockIdling : ScarabBaseState
    {
        [Header("Random Wandering values")] 
        
        [SerializeField]
        private float wanderTimer = default;
        [SerializeField] 
        private float minWanderRadius = 1f;
        [SerializeField] 
        private float maxWanderRadius = default;

        private float _timer;

        public override void Enter()
        {
            _timer = 0f;
        }

        public override void Run()
        {
            base.Run();

            _timer += Time.deltaTime;

            if (S.CanSeePlayer() && (!S.isLeader || (AI.playerPosition.Value - AI.transform.position).magnitude < 3f))
            {
                StateMachine.TransitionTo<Hunting>();
            }

            if (_timer >= wanderTimer)
            {
                S.targetPosition = GetNewPosition();
                S.agent.SetDestination(S.targetPosition);
                _timer = 0;
            }
        }

        private Vector3 GetNewPosition()
        {
            if (S.isLeader)
            {
                var randomXZ = Random.insideUnitSphere.normalized * Random.Range(minWanderRadius, maxWanderRadius);
                var randomDir = new Vector3(randomXZ.x, 0f, randomXZ.z);
                randomDir += AI.transform.position;
                return CreateNewPosition(randomDir);
            }

            return S.leaderObj.targetPosition;
        }

        private Vector3 CreateNewPosition(Vector3 randDirection)
        {
            NavMesh.SamplePosition(randDirection, out var navHit, 1f, NavMesh.AllAreas);
            return navHit.position;
        }

        public override void Exit()
        {
            if (S != null)
            {
                S.agent.stoppingDistance = 0f;
            }
        }
    }
}