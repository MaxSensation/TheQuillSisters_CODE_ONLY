// Primary Author: Viktor Dahlberg - vida6631

using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Entity.Player.MovementAbilities;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Combat.Melee
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Melee/AirLight")]
    public class ScriptObjAirLightAttack : ScriptObjAttackBase
    {
        [Header("Air Light Attack Specifics")] [SerializeField]
        private ScriptObjVar<bool> isGrounded = default;

        [SerializeField] 
        private ScriptObjVar<bool> playerJumped = default;

        [SerializeField]
        private ConditionBase hover = default;

        [SerializeField] 
        private GameEvent becameGrounded = default;

        [SerializeField] 
        private ScriptObjVar<int> airAttackCount = default;

        [SerializeField]
        private ScriptObjVar<int> maxAirAttackCount = default;

        [SerializeField]
        private StaggerCondition staggerCondition = default;

        [SerializeField] 
        private KnockbackCondition knockbackCondition = default;

        private bool _inWindow;

        protected override bool CanUse()
        {
            return isGrounded != null && !isGrounded && airAttackCount < maxAirAttackCount && _inWindow;
        }

        protected override void PostOwnerSet()
        {
            _inWindow = true;
            airAttackCount.value = 0;
            becameGrounded.OnEvent += () => airAttackCount.value = 0;
            becameGrounded.OnEvent += ResetInputs;
            PlayerJump.PlayerJumped += CloseWindow;
            animationHandler.OnComboWindowEnded += OpenWindow;
        }

        private void CloseWindow()
        {
            _inWindow = false;
        }

        private void OpenWindow()
        {
            _inWindow = true;
        }

        protected override void OnAttack()
        {
            airAttackCount.value++;
            TriggerMovement();
        }

        protected override void Attack()
        {
            var didDamage = false;
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables(true))
            {
                damageable.TakeDamage(damage);
                var entity = damageable.GetEntity();
                if (!entity) continue;
                if (playerJumped.value)
                {
                    ConditionManager.AddCondition(hover, entityBase, entity);
                    if (staggerCondition != null)
                    {
                        ConditionManager.AddCondition(staggerCondition, entityBase, entity);
                    }
                    if (knockbackCondition != null)
                    {
                        ConditionManager.AddCondition(knockbackCondition, entityBase, entity);
                    }
                }

                didDamage = true;
            }

            if (didDamage && playerJumped.value)
            {
                ConditionManager.AddCondition(hover, entityBase, entityBase);
            }
        }
    }
}