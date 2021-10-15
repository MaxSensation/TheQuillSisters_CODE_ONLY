// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Entity.Player.States.Physical
{
    [CreateAssetMenu(menuName = "States/PlayerStates/PhysicalStates/Falling")]
    public class PlayerFallingState : PlayerBaseState
    {
        public override void Enter()
        {
            Player.Anim.SetBool("Falling", true);
        }

        public override void Exit()
        {
            Player.Anim.SetBool("Falling", false);
        }

        public override void Run()
        {
            if (!Player.Movement.PlayerIsFalling)
            {
                StateMachine.TransitionTo<PlayerIdlingState>();
            }

            Player.Movement.AddMovement();
        }
    }
}