// Primary Author : Andreas Berzelius - anbe4918

using Framework.StateMachine;

namespace Entity.AI.States
{
    public abstract class AIBaseState : State
    {
        private Enemy _ai;
        protected Enemy AI => _ai = _ai != null ? _ai : (Enemy) owner;

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Run()
        {
            if (AI.agent == null)
            {
                return;
            }

            if (AI.agent.velocity.magnitude > 0.1f && !AI.IsWalking())
            {
                AI.StartWalkAnimation();
            }
            else if (AI.agent.velocity.magnitude < 0.001f && AI.IsWalking())
            {
                AI.StopWalkAnimation();
            }
        }
    }
}