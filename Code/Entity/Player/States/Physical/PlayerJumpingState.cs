// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Entity.Player.States.Physical
{
    [CreateAssetMenu(menuName = "States/PlayerStates/PhysicalStates/Jumping")]
    public class PlayerJumpingState : PlayerBaseState
    {
        private float _time;

        public override void Enter()
        {
            _time = 0;
            Player.Anim.SetBool("Jumping", true);
            Player.AnimationHandler.AttackInterrupted();
        }

        public override void Exit()
        {
            Player.Anim.SetBool("Jumping", false);
        }

        public override void Run()
        {
            if (Player.Movement.PlayerIsFalling || Player.Movement.PlayerGrounded && _time > 0.2f)
            {
                StateMachine.TransitionTo<PlayerFallingState>();
            }

            _time += Time.deltaTime;
            Player.Movement.AddMovement();
        }
    }
}