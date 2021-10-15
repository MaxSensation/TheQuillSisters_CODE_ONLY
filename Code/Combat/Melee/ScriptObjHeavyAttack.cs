// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Viktor Dahlberg - vida6631

using System;
using System.Threading;
using System.Threading.Tasks;
using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Combat.Melee
{
    /// <summary>
    ///     A HeavyAttack class used by the combat system
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Melee/Heavy")]
    public class ScriptObjHeavyAttack : ScriptObjMeleeBase
    {
        [Header("Charge Info")] [Header("Heavy Attack Specifics")] [SerializeField]
        private float maximumChargeTime = default;

        [Range(0f, 2f)] [SerializeField] 
        private double multiplierPerSecond = default;

        [SerializeField] 
        private double maximumChargeBonusMultiplier = default;

        [SerializeField] 
        private float chargeAnimationDuration = default;

        [Header("Can Use Conditions")] [SerializeField]
        private ScriptObjVar<bool> isGrounded = default;

        [SerializeField] [Tooltip("Condition used to stagger enemies")]
        protected StaggerCondition staggerCondition = default;

        [SerializeField] [Tooltip("Condition used to knockback enemies")]
        protected KnockbackCondition knockbackCondition = default;

        private bool _alreadyCancelled;
        private float _chargeStartTime;

        private TaskCompletionSource<bool> _finishedCharging;
        private float _latestChargeTime;
        private int _multipliedDamage;

        protected override bool CanUse()
        {
            return isGrounded == null || isGrounded.value;
        }

        protected override void PostOwnerSet()
        {
            animationHandler.OnAttackInterrupted += OnChargeInterrupted;
        }

        public override void TriggerWithInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                _alreadyCancelled = false;
                OnAttackPressed();
            }
            else if (obj.canceled)
            {
                OnAttackUnleashed();
            }
        }

        public override void DisableAttackTrigger()
        {
            attackCanBeTriggered = false;
        }

        private void OnAttackUnleashed()
        {
            if (_finishedCharging != null)
            {
                _finishedCharging.SetResult(AttackCompleted);
                animationHandler.OnAttackContinued.Invoke();
            }
            else
            {
                _alreadyCancelled = true;
            }
        }

        private void OnChargeInterrupted()
        {
            if (_finishedCharging != null)
            {
                _finishedCharging.SetResult(AttackInterrupted);
            }
            _alreadyCancelled = false;
        }

        /// <summary>
        ///     This is where we start charging the attack.
        /// </summary>
        /// <returns>Whether or not the attack was unleashed or interrupted.</returns>
        protected override async Task<bool> Delay()
        {
            if (_alreadyCancelled)
            {
                _alreadyCancelled = false;
                animationHandler.OnAttackContinued.Invoke();
                return true;
            }

            _multipliedDamage = damage;
            _chargeStartTime = Time.time;

            animationHandler.ScaleAnimationDuration(chargeAnimationDuration, maximumChargeTime);

            _finishedCharging = new TaskCompletionSource<bool>();
            var maxChargeTimerCancel = new CancellationTokenSource();
            PrepareMaxCharge(maxChargeTimerCancel.Token);
            var result = await _finishedCharging.Task;
            _finishedCharging = null;
            maxChargeTimerCancel.Cancel();
            maxChargeTimerCancel.Dispose();

            if (result == AttackCompleted)
            {
                var totalChargeTime = Mathf.Clamp(Time.time - _chargeStartTime, 0f, maximumChargeTime);
                var chargeTimeMultiplayer = totalChargeTime * multiplierPerSecond >= 1
                    ? totalChargeTime * multiplierPerSecond
                    : 1f;
                var maxChargeMultiPlayer = totalChargeTime >= maximumChargeTime ? maximumChargeBonusMultiplier : 1f;
                _multipliedDamage = Mathf.CeilToInt((float) (damage * chargeTimeMultiplayer * maxChargeMultiPlayer));
            }

            return result;
        }

        private async void PrepareMaxCharge(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(maximumChargeTime), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                /* if this is thrown, it just means the delay actually was cancelled */
            }

            if (_finishedCharging != null)
            {
                OnSpecial?.Invoke();
            }
        }

        protected override void Attack()
        {
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables())
            {
                damageable.TakeDamage(_multipliedDamage);
                var entity = damageable.GetEntity();
                if (entity != null)
                {
                    ConditionManager.AddCondition(staggerCondition, entityBase, entity);
                    ConditionManager.AddCondition(knockbackCondition, entityBase, entity);
                }
            }

            _alreadyCancelled = false;
        }

        public void SetDamageMultiplier(double damageMultiplier)
        {
            multiplierPerSecond = damageMultiplier;
        }

        public void SetDamageFullyChargedMultiplier(double fullyChargedMultiplier)
        {
            maximumChargeBonusMultiplier = fullyChargedMultiplier;
        }
    }
}