//Primary Author : Viktor Dahlberg - vida6631

using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Entity.Player.MovementAbilities;
using Framework.ScriptableObjectEvent;
using UnityEngine;

namespace Combat.Melee
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Melee/Uppercut")]
    public class ScriptObjUppercut : ScriptObjMeleeBase
    {
        [Header("Uppercut Specifics")] [SerializeField]
        private MoveCondition updraftCondition = default;

        [SerializeField] private HoverCondition preFreezeCondition = default;

        [SerializeField] private GameEvent becameGrounded = default;

        [SerializeField] private GameEvent tutorialEvent = default;

        private bool _inWindow;
        private bool _triggeredEvent;

        protected override void PostOwnerSet()
        {
            PlayerJump.PlayerJumped += () => BeginWindow();
            becameGrounded.OnEvent += () => ResetInputs();
            animationHandler.OnComboWindowEnded += () => EndWindow();
        }

        private void BeginWindow()
        {
            _inWindow = true;
        }

        protected override bool CanUse()
        {
            return _inWindow;
        }

        private void EndWindow()
        {
            _inWindow = false;
        }

        protected override void OnAttack()
        {
            ConditionManager.AddCondition(preFreezeCondition, entityBase, entityBase);
        }

        protected override void Attack()
        {
            if (!_triggeredEvent)
            {
                _triggeredEvent = true;
                tutorialEvent.Raise();
            }

            updraftCondition.MoveDirection = Vector3.up;
            var dimensions = entityBase.GetDimensions();
            updraftCondition.Radius = dimensions.Item1;
            updraftCondition.Height = dimensions.Item2;
            ConditionManager.AddCondition(updraftCondition, entityBase, entityBase);
            foreach (var damageable in AttackColliderCollisionDetection.GetDamageables())
            {
                damageable.TakeDamage(damage);
                var entity = damageable.GetEntity();
                if (entity != null)
                {
                    dimensions = entity.GetDimensions();
                    updraftCondition.Radius = dimensions.Item1;
                    updraftCondition.Height = dimensions.Item2;
                    ConditionManager.AddCondition(updraftCondition, entityBase, entity);
                }
            }
        }
    }
}