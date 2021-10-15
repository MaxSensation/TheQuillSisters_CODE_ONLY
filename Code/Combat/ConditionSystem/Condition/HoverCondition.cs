// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Viktor Dahlberg - vida6631 

using System;
using System.Threading;
using System.Threading.Tasks;
using Combat.Interfaces;
using UnityEngine;

namespace Combat.ConditionSystem.Condition
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Condition/Hover")]
    public class HoverCondition : ConditionBase
    {
        [SerializeField] 
        private float duration = default;

        private CancellationTokenSource _cancel;
        private bool _waitAgain;

        public override void Modify(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            var hoverable = affectedEntity.GetComponent<IHoverable>();
            if (hoverable != null)
            {
                hoverable.EnableHover();
                DelayedUnmodify(affectedEntity);
            }
        }

        public override void UnModify(EntityBase entity)
        {
            var hoverable = entity.GetComponent<IHoverable>();
            if (hoverable != null)
            {
                hoverable.DisableHover();
            }
        }

        public override void UpdateCondition(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            _waitAgain = true;
            _cancel?.Cancel();
        }

        private async void DelayedUnmodify(EntityBase entity)
        {
            do
            {
                _waitAgain = false;
                _cancel = new CancellationTokenSource();
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(duration), _cancel.Token);
                }
                catch (TaskCanceledException)
                {
                }
            } while (_waitAgain);

            ConditionManager.RemoveCondition(this, entity);
        }

        public override void CancelCondition(EntityBase entity)
        {
            _waitAgain = false;
            _cancel?.Cancel();
        }
    }
}