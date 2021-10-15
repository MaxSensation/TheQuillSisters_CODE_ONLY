// Primary Author : Andreas Berzelius - anbe5918

using System;
using Combat.ConditionSystem;
using Combat.Interfaces;
using Entity.AI.States.Behavior;
using Entity.AI.States.Physical;
using VFX;
using UnityEngine;

namespace Entity.AI.Mummies.Ranged
{
    public class MummyRanged : Mummy, IStaggerable, IKnockable, IMovable
    {
        [SerializeField]
        private Transform handTransform = default;
        [SerializeField]
        public LightController handLight = default;
        
        public Vector3 handPosition => handTransform.localPosition;

        public override Tuple<float, float> GetDimensions()
        {
	        return new Tuple<float, float>(EnemyCollider.radius, EnemyCollider.height);
        }

        public Quaternion GetHandRotation()
        {
            handTransform.LookAt(playerPosition.Value + Vector3.up * 1.8f);
            return handTransform.rotation;
        }

        public new void Start()
        {
            AnimationHandler.OnStaggerCompleted += OnStaggerComplete;
            base.Start();
        }

        public override Vector3 GetSpawnScale()
        {
	        return Vector3.one;
        }

        public void OnStagger()
        {
            BehaviorStateMachine.TransitionTo<Staggering>();
        }

        public void OnStaggerComplete()
        {
            BehaviorStateMachine.TransitionTo<Idling>();
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
        
    }
}