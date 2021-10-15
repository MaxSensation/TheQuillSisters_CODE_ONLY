// Primary Author : Viktor Dahlberg - vida6631

using Combat.ConditionSystem.Condition;
using Entity.AI.Bosses.Khonsu.Attacks;
using Entity.AI.States;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.AI.Bosses.Khonsu.States
{
    public abstract class KhonsuPhase : AIBaseState
    {
        [SerializeField]
        protected ScriptObjVar<bool> enraged = default;
        [SerializeField]
        protected InvincibilityCondition invincibilityCondition = default;
        [SerializeField]
        private ScriptObjVar<float> enragedNovaDamageMultiplier = default;
        [SerializeField]
        private ScriptObjVar<float> enragedNovaCastSpeed = default;
        [SerializeField]
        protected float overlayFadeSpeed = default;

        private readonly Vector3 _offset = new Vector3(0, 0.9f, 0);
        private TelegraphedNova _nova;
        private bool _tracking;

        protected void BeginNova()
        {
            if ((_nova = AI.localAttacks[0] as TelegraphedNova) == null)
            {
                return;
            }
            
            var position = AI.transform.position;
            var offset = new Vector3(0, 0.9f, 0); //player height offset
            _nova.FinalDamageMultiplier = enraged ? enragedNovaDamageMultiplier : 1f;
            _nova.attackOffset = AI.transform.InverseTransformPoint(AI.playerPosition.Value + offset);
            _nova.visualEffectOffset = -position + AI.playerPosition.Value + offset;
            AI.AnimationHandler.OnComboWindowEnded += StopTracking;
            AI.AnimationHandler.SetAnimationSpeed(enraged ? enragedNovaCastSpeed : 1f);
            _nova.TriggerWithCancel();
            _tracking = true;
        }

        public override void Run()
        {
            if (_tracking)
            {
                Transform transform;
                _nova.attackOffset = (transform = AI.transform).InverseTransformPoint(AI.playerPosition.Value + _offset);
                _nova.visualEffectOffset = -transform.position + AI.playerPosition.Value + _offset;
                _nova.Refresh();
            }

            base.Run();
        }

        private void StopTracking()
        {
            _tracking = false;
            AI.AnimationHandler.OnComboWindowEnded -= StopTracking;
        }
    }
}