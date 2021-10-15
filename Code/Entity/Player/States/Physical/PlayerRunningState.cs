// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Entity.Player.States.Physical
{
	[CreateAssetMenu(menuName = "States/PlayerStates/PhysicalStates/Running")]
	public class PlayerRunningState : PlayerBaseState
	{
		public override void Enter()
		{
			Player.Anim.SetBool("Running", true);
		}

		public override void Exit()
		{
			Player.Anim.SetBool("Running", false);
		}

		public override void Run()
		{
			var vel = Player.Movement.PlayerVelocity;
			vel.y = 0f;
			if(vel.magnitude < 0.3f && Player.Movement.PlayerGrounded)
			{
				StateMachine.TransitionTo<PlayerIdlingState>();
			}
			else if (Player.Movement.PlayerIsFalling)
			{
				StateMachine.TransitionTo<PlayerFallingState>();
			}
			Player.Movement.AddMovement();
		}
	}
}
