// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Entity.Player.States.Physical
{
    [CreateAssetMenu(menuName = "States/PlayerStates/PhysicalStates/Dodging")]
    public class PlayerDodgingState : PlayerBaseState
    {
        public override void Enter()
        {
            Player.AnimationHandler.AttackInterrupted();
        }

        public override void Exit()
        {
        }

        public override void Run()
        {
            StateMachine.TransitionTo<PlayerIdlingState>();
        }
    }
}