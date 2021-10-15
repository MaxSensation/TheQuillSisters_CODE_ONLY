//Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Combat.Ability
{
    /// <summary>
    ///     Nova Ability that creates a AE around the player
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Ability/Nova")]
    public class ScriptObjNova : ScriptObjAbilityBase
    {
        protected override void Attack()
        {
            base.Attack();
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables())
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}