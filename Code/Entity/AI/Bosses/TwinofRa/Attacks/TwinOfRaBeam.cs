// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Threading.Tasks;
using Combat.Ability;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.AI.Bosses.TwinofRa.Attacks
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Bosses/TwinofRa/Beam")]
    public class TwinOfRaBeam : ScriptObjAbilityBase
    {
        [SerializeField]
        private float preFireTime = default;
        [SerializeField]
        private ScriptObjVar<Vector3> playerPos = default;
        [SerializeField]
        private GameObject prefireVFXGO = default;
        [SerializeField]
        private float vfxDestroyDelay = 0.5f;

        private async void WarmUpBeam()
        {
            UpdateRotation();
            PreFire();
            await Task.Delay(TimeSpan.FromSeconds(preFireTime));
            if (Application.isPlaying)
            {
                Fire();
            }
        }

        private void UpdateRotation()
        {
            attackColliderRotator.rotation = Quaternion.LookRotation((playerPos - owner.transform.position).normalized, Vector3.up);
        }

        private void PreFire()
        {
            var prefireVFXGOInstance = Instantiate(prefireVFXGO, owner.transform.position + visualEffectOffset, attackColliderRotator.transform.rotation);
            prefireVFXGOInstance.SetActive(true);
            Destroy(prefireVFXGOInstance, vfxDestroyDelay);
        }

        private void Fire()
        {
            SpawnVFX();
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables())
            {
                damageable.TakeDamage(damage);
            }

            IsAttacking = false;
        }

        protected override void Attack()
        {
            WarmUpBeam();
        }
    }
}