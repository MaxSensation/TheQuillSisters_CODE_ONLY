// Primary Author : Andreas Berzelius - anbe4918

using UnityEngine;

namespace Entity.AI.States.Behavior
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Staggering")]
    public class Staggering : AIBaseState
    {
        public override void Enter()
        {
            AI.AnimationHandler.AttackInterrupted();
            AI.AnimationHandler.ResetAnimation("MummyTakeHit");
            if (AI.agent.isOnNavMesh)
            {
                AI.agent.velocity = Vector3.zero;
                AI.agent.isStopped = true;
            }
            AI.StopWalkAnimation();
        }

        public override void Exit()
        {
            if (AI.agent.isOnNavMesh)
            {
                AI.agent.isStopped = false;
            }

            AI.StartWalkAnimation();
        }
    }
}