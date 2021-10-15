// Primary Author : Andreas Berzelius - anbe4918

using Entity.AI.States.Behavior;
using Entity.AI.States.Combat;
using Environment;
using UnityEngine;

namespace Entity.AI.Mummies
{
    public abstract class Mummy : Enemy, ISpawnable
    {
        [SerializeField] 
        private float alertDistance = default;

        private new void Start()
        {
            base.Start();
            DoAlert += Alert;
        }

        private void OnDisable()
        {
            DoAlert -= Alert;
        }

        public abstract Vector3 GetSpawnScale();

        private void Alert(GameObject alertingEnemy)
        {
            if (BehaviorStateMachine.CurrentState is Hunting || BehaviorStateMachine.CurrentState is Attacking ||
                BehaviorStateMachine.CurrentState is Staggering)
            {
                return;
            }

            if (!gameObject.Equals(alertingEnemy))
            {
                var distanceToAlerting = (alertingEnemy.transform.position - transform.position).magnitude;
                if (distanceToAlerting <= alertDistance)
                {
                    AnimationHandler.OnAttackAnimationEnded = null;
                    BehaviorStateMachine.TransitionTo<Alerted>();
                }
            }
        }
    }
}