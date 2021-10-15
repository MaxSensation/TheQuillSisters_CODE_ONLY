// Primary Author : Andreas Berzelius - anbe5918

using Combat;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Bosses/Zoe/ThrowPoison")]
    public class ThrowPoison : ScriptObjAttackBase
    {
        [SerializeField] 
        private GameObject poisonParticlesObj = default;

        private ParticleSystem _poisonParticles;

        protected override void Attack()
        {
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables())
            {
                damageable.TakeDamage(damage);
            }
            Instantiate(poisonParticlesObj, owner.transform);
        }
    }
}