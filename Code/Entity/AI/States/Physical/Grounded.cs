// Primary Author : Andreas Berzelius - anbe4918

using UnityEngine;

namespace Entity.AI.States.Physical
{
    [CreateAssetMenu(menuName = "States/EnemyStates/PhysicalStates/Grounded")]
    public class Grounded : AIBaseState
    {
        public override void Enter()
        {
            AI.agent.enabled = true;
        }

        public override void Exit()
        {
            AI.StopWalkAnimation();
            AI.agent.enabled = false;
        }

        public override void Run()
        {
            AI.BehaviorStateMachine.Run();
        }
    }
}