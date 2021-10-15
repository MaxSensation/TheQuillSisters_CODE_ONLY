// Primary Author : Andreas Berzelius - anbe4918

using UnityEngine;

namespace Entity.AI.States.Behavior
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Patrolling")]
    public class Patrolling : AIBaseState
    {
        public override void Run()
        {
            base.Run();
            if (AI.CanSeePlayer())
            {
                StateMachine.TransitionTo<Hunting>();
            }
        }
    }
}