// Primary Author : Erik Pilström - erpi3245

using Framework.StateMachine;

namespace Entity.Player.States.Physical
{
    public abstract class PlayerBaseState : State
    {
        private PlayerController player;

        protected PlayerController Player => player = player != null ? player : (PlayerController) owner;

        public abstract override void Enter();

        public abstract override void Exit();

        public abstract override void Run();
    }
}