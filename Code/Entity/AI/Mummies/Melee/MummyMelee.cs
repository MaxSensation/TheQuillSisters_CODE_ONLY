// Primary Author :  Andreas Berzelius - anbe5918

using Combat.ConditionSystem;
using Combat.Interfaces;
using Entity.AI.States.Behavior;
using Entity.AI.States.Physical;
using UnityEngine;

namespace Entity.AI.Mummies
{
    public class MummyMelee : Mummy, IStaggerable, IKnockable, IMovable
    {
        public new void Start()
        {
            AnimationHandler.OnStaggerCompleted += OnStaggerComplete;
            base.Start();
        }

        public void MoveToPosition(Vector3 movePosition)
        {
            transform.position += movePosition;
        }

        public void OnMoveStarted(ConditionBase condition)
        {
            EnemyCollider.enabled = false;
            PhysicalStateMachine.TransitionTo<Airborne>();
        }

        public void OnMoveCompleted(ConditionBase condition)
        {
            EnemyCollider.enabled = true;
        }

        public void OnStagger()
        {
            BehaviorStateMachine.TransitionTo<Staggering>();
        }

        public void OnStaggerComplete()
        {
            BehaviorStateMachine.TransitionTo<Hunting>();
        }

        public override Vector3 GetSpawnScale()
        {
            return Vector3.one;
        }
    }
}