// Primary Author : Andreas Berzelius - anbe5918

using Combat;
using UnityEngine;

namespace Entity.AI.Attacks
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Enemy/Melee/MeleeAttack")]
    public class MeleeAttack : ScriptObjAttackBase
    {
        protected override void Attack()
        {
            foreach (var entity in AttackColliderCollisionDetection.GetDamageables())
            {
                entity.TakeDamage(damage);
            }
        }
    }
}