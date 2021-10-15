// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Viktor Dahlberg - vida6631

using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Combat.Melee
{
    /// <summary>
    ///     A LightAttack class used by the combat system
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Melee/GroundLight")]
    public class ScriptObjGroundLightAttack : ScriptObjMeleeBase
    {
        [Header("Ground Light Attack Specifics")] [SerializeField]
        private ScriptObjVar<bool> isGrounded = default;

        [SerializeField] 
        private GameEvent becameGrounded = default;

        [SerializeField] 
        private KnockbackCondition knockbackCondition = default;

        protected override bool CanUse()
        {
            return isGrounded != null && isGrounded;
        }

        protected override void PostOwnerSet()
        {
            becameGrounded.OnEvent += () => ResetInputs();
        }

        protected override void OnAttack()
        {
            TriggerMovement();
        }

        protected override void Attack()
        {
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables(true))
            {
                damageable.TakeDamage(damage);
                var entity = damageable.GetEntity();
                if (entity)
                {
                    ConditionManager.AddCondition(knockbackCondition, entityBase, entity);
                }
            }
        }
    }
}