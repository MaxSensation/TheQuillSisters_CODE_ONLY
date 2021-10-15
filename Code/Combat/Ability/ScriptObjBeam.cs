//Primary Author : Maximiliam Rosén - maka4519

using Entity.Player;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using VFX;

namespace Combat.Ability
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Ability/Beam")]
    public class ScriptObjBeam : ScriptObjAbilityBase
    {
        [Header("Can Use Conditions")] [SerializeField]
        private ScriptObjVar<bool> isGrounded = default;

        [Header("Telegraph")] [SerializeField]
        private GameObject ghost = default;

        private GameObject _ghostInstance;
        private RotateWithCamera _rotateWithCamera;

        protected override bool CanUse()
        {
            return (isGrounded == null || isGrounded.value) && base.CanUse();
        }

        protected override void OnAttack()
        {
            if (ghost == null)
            {
                return;
            }
            AttackColliderCollisionDetection.transform.parent.GetComponent<ControlWithCamera>().enabled = true;
            var transform = AttackColliderCollisionDetection.transform;
            _ghostInstance = Instantiate(ghost, transform.position, Quaternion.identity, transform.parent);
            _ghostInstance.GetComponentInChildren<LineGhostController>().Init(owner.transform);
            _ghostInstance.transform.rotation = new Quaternion();
        }

        protected override void PostOwnerSet()
        {
            _rotateWithCamera = owner.GetComponent<RotateWithCamera>();
        }

        protected override void Attack()
        {
            if (_ghostInstance != null)
            {
                Destroy(_ghostInstance);
            }
            base.Attack();
            _rotateWithCamera.enabled = false;
            AttackColliderCollisionDetection.transform.parent.GetComponent<ControlWithCamera>().enabled = false;
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables())
            {
                damageable.TakeDamage(damage);
            }
        }

        protected override void OnAttackCompleted()
        {
            AttackColliderCollisionDetection.transform.parent.GetComponent<ControlWithCamera>().enabled = false;
            _rotateWithCamera.enabled = true;
            base.OnAttackCompleted();
        }

        protected override void OnAttackInterrupted()
        {
            if (_ghostInstance != null)
            {
                Destroy(_ghostInstance);
            }
            AttackColliderCollisionDetection.transform.parent.GetComponent<ControlWithCamera>().enabled = false;
            _rotateWithCamera.enabled = true;
            base.OnAttackInterrupted();
        }
    }
}