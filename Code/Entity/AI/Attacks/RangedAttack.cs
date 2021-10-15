// Primary Author : Andreas Berzelius - anbe5918

using Combat;
using Entity.AI.Mummies.Ranged;
using UnityEngine;

namespace Entity.AI.Attacks
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Enemy/Ranged/RangedAttack")]
    public class RangedAttack : ScriptObjAttackBase
    {
        [SerializeField] 
        private GameObject projectilePrefab = default;

        private MummyRanged _mummyRanged;

        protected override void Attack()
        {
            if (_mummyRanged == null)
            {
                _mummyRanged = owner.GetComponent<MummyRanged>();
            }

            Instantiate(
                projectilePrefab, 
                owner.transform.TransformPoint(_mummyRanged.handPosition),
                _mummyRanged.GetHandRotation()
                ).GetComponent<HomingProjectile>().Init(damage);
        }
    }
}