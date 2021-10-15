// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Threading;
using System.Threading.Tasks;
using Combat.Interfaces;
using UnityEngine;

namespace Combat.ConditionSystem.Condition
{
	/// <summary>
	///     Makes the entity invincible for a fixed time
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Attacks/Condition/Invincibility")]
    public class InvincibilityCondition : ConditionBase
    {
        [SerializeField] 
        private float iframeTimeMilliSeconds = default;

        private CancellationTokenSource _cancel;

        public override void Modify(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            var invincibilityHealthSystem = affectedEntity.gameObject.GetComponent<IInvincible>();
            if (invincibilityHealthSystem != null)
            {
                invincibilityHealthSystem.SetInvincibility(true);
                StartInvincibility(affectedEntity);
            }
        }

        public override void CancelCondition(EntityBase entity)
        {
            _cancel?.Cancel();
        }

        private async void StartInvincibility(EntityBase entity)
        {
            _cancel = new CancellationTokenSource();
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(iframeTimeMilliSeconds), _cancel.Token);
            }
            catch (TaskCanceledException)
            {
            }

            ConditionManager.RemoveCondition(this, entity);
        }

        public override void UnModify(EntityBase entity)
        {
            var invincibilityEntity = entity.gameObject.GetComponent<IInvincible>();
            invincibilityEntity.SetInvincibility(false);
        }

        public void SetIframeTime(float iframeTimeMilliSeconds)
        {
            this.iframeTimeMilliSeconds = iframeTimeMilliSeconds;
        }
    }
}