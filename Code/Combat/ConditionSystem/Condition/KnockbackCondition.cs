// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Threading.Tasks;
using Combat.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Combat.ConditionSystem.Condition
{
    /// <summary>
    ///     A knockback Test Condition that will be refactored for rigidBody
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Condition/Knockback")]
    public class KnockbackCondition : ConditionBase
    {
        [SerializeField] 
        private float strength = default;

        /// <summary>
        ///     Applies the knockback effect on the affected entity
        /// </summary>
        /// <param name="applyingEntity">The entity that applied this condition to the affected entity</param>
        /// <param name="affectedEntity">The entity that will be affected by the condition</param>
        /// <returns></returns>
        public override void Modify(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            if (affectedEntity.GetComponent<IKnockable>() != null)
            {
                StartKnockback(applyingEntity, affectedEntity);
            }
        }

        public override void UpdateCondition(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            var direction = affectedEntity.transform.position - applyingEntity.transform.position;
            var projectedDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
            var rigidBody = affectedEntity.GetComponent<Rigidbody>();
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(projectedDirection * strength, ForceMode.Impulse);
        }

        private async void StartKnockback(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            var direction = affectedEntity.transform.position - applyingEntity.transform.position;
            var projectedDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
            var rigidBody = affectedEntity.GetComponent<Rigidbody>();
            var agent = affectedEntity.GetComponent<NavMeshAgent>();
            agent.enabled = false;
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;
            rigidBody.AddForce(projectedDirection * strength, ForceMode.Impulse);
            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime * 10));
            
            while (rigidBody.velocity.magnitude > 0.1f)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50f));
            }

            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            agent.enabled = true;
            ConditionManager.RemoveCondition(this, affectedEntity);
        }
    }
}