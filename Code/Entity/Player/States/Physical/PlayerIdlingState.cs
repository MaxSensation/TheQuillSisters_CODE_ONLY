// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Entity.Player.States.Physical
{
    [CreateAssetMenu(menuName = "States/PlayerStates/PhysicalStates/Idling")]
    public class PlayerIdlingState : PlayerBaseState
    {
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Run()
        {
            if (Player.Movement.GetMoveInput().magnitude > 0.3f && Player.Movement.PlayerGrounded)
            {
                StateMachine.TransitionTo<PlayerRunningState>();
            }
        }
    }
}