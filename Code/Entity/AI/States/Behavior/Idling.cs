// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Andreas Berzelius - anbe5918

using UnityEngine;

namespace Entity.AI.States.Behavior
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Idling")]
    public class Idling : AIBaseState
    {
        [Header("Random Wandering values")] 
        
        [SerializeField]
        private float minWanderTimer = default;
        [SerializeField] 
        private float maxWanderTimer = default;
        [SerializeField] 
        private float minWanderRadius = 1f;
        [SerializeField] 
        private float maxWanderRadius = 4f;

        private float _timer;

        public override void Enter()
        {
            _timer = 0f;
        }
        
        public override void Run()
        {
            base.Run();
            _timer += Time.deltaTime;

            if (AI.CanSeePlayer())
            {
                StateMachine.TransitionTo<Hunting>();
            }
            else if (_timer >= Random.Range(minWanderTimer, maxWanderTimer))
            {
                AI.agent.SetDestination(GetRandomPosition());
                _timer = 0;
            }
        }

        private Vector3 GetRandomPosition()
        {
            var randomXZ = Random.insideUnitSphere.normalized * Random.Range(minWanderRadius, maxWanderRadius);
            return new Vector3(randomXZ.x, 0f, randomXZ.z) + AI.transform.position;
        }
    }
}