// Primary Author : Erik Pilström - erpi3245

using UnityEngine;

namespace Entity.Player.States.Physical
{
    [CreateAssetMenu(menuName = "States/PlayerStates/PhysicalStates/Attacking")]
    public class PlayerAttackingState : PlayerBaseState
    {
        public override void Enter()
        {
        }

        private void CheckIfAttacking()
        {
            if (!Player.AnimationHandler.IsAttacking)
            {
                StateMachine.TransitionTo<PlayerIdlingState>();
            }
        }

        public override void Run()
        {
            CheckIfAttacking();
        }

        public override void Exit()
        {
        }
    }
}