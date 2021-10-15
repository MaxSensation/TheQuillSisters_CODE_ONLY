// Primary Author : Maximiliam Rosén - maka4519

using System;
using UnityEngine;

namespace Combat.ConditionSystem
{
	/// <summary>
	///     A ConditionBase for each condition
	/// </summary>
	public abstract class ConditionBase : ScriptableObject
    {
        public int priority;
        public Action<EntityBase, EntityBase> Modified;
        public Action<EntityBase> UnModified;
        public Action<EntityBase> Updated;

        /// <summary>
        ///     Apply the effect on the affected entity.
        /// </summary>
        /// <param name="applyingEntity">The entity that added this condition to the affected Entity</param>
        /// <param name="affectedEntity">The entity that will be affected by the condition</param>
        /// <returns>Whether or not the condition was applied.</returns>
        public abstract void Modify(EntityBase applyingEntity, EntityBase affectedEntity);

        /// <summary>
        ///     Remove the effect from the entity.
        /// </summary>
        /// <param name="entity"> The affected entity. </param>
        public virtual void UnModify(EntityBase entity)
        {
        }

        /// <summary>
        ///     Update the condition if the condition is applied multiply times
        /// </summary>
        /// <param name="entity"> The affected entity. </param>
        public virtual void UpdateCondition(EntityBase applyingEntity, EntityBase affectedEntity)
        {
        }

        public virtual void CancelCondition(EntityBase entity)
        {
        }
    }
}