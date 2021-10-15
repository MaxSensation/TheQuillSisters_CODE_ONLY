// Primary Author :  Maximiliam Rosén - maka4519

using System;
using System.Threading;
using System.Threading.Tasks;
using Entity.HealthSystem;
using UnityEngine;

namespace Combat.ConditionSystem.Condition
{
    /// <summary>
    ///     Is a Test Condition to test the conditionSystem
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Condition/Bleed")]
    public class TickDamageCondition : ConditionBase
    {
        [SerializeField]
        private int tickDamage = default;
        [SerializeField]
        private float tickTime = default;

        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        public override void Modify(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            var killable = affectedEntity.GetComponent<Health>();
            if (killable != null)
            {
                StartDamageTick(affectedEntity);
            }
        }

        private async void StartDamageTick(EntityBase entity)
        {
            var entityHealth = entity.GetComponent<Health>();
            while (!_cancel.Token.IsCancellationRequested)
            {
                entityHealth.TakeDamage(tickDamage);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(tickTime), _cancel.Token);
                }
                catch (TaskCanceledException e)
                {
                }
            }
        }

        public override void UnModify(EntityBase entity)
        {
            _cancel.Cancel();
        }
    }
}