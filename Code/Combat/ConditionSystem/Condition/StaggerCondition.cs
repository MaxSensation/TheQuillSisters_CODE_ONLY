// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Andreas Berzelius - anbe5918

using System.Linq;
using Combat.Interfaces;
using UnityEngine;

namespace Combat.ConditionSystem.Condition
{
    /// <summary>
    ///     Makes the entity stagger
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Condition/Stagger")]
    public class StaggerCondition : ConditionBase
    {
        [SerializeField]
        private int maxStaggerCombo = default;
        [SerializeField]
        private float staggerTime = default;

        public override void Modify(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            var staggeredEntity = affectedEntity.GetComponent<IStaggerable>();
            if (staggeredEntity != null && IsStaggarable(affectedEntity))
            {
                staggeredEntity.OnStagger();
                RotateToApplier(applyingEntity, affectedEntity);
            }
            ConditionManager.RemoveCondition(this, affectedEntity);
        }

        private bool IsStaggarable(EntityBase affectedEntity)
        {
            var totalAmountStaggerd = ConditionManager.ConditionHistoryEntries
                .Where(entry =>
                    entry.Type == ConditionEntryType.Modified &&
                    entry.Condition == this &&
                    entry.AffectedEntity == affectedEntity &&
                    Time.time - entry.Timestamp <= staggerTime
                )
                .ToList().Count;
            return totalAmountStaggerd <= maxStaggerCombo;
        }

        private static void RotateToApplier(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            if (affectedEntity.gameObject.activeSelf)
            {
                var transform = affectedEntity.transform;
                var vectorToPlayer = (applyingEntity.transform.position - transform.position).normalized;
                transform.forward = vectorToPlayer;
            }
        }
    }
}