// Primary Author : Viktor Dahlberg - vida6631

using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Entity.AI.States;
using UnityEngine;

namespace Entity.AI.Bosses.Khonsu.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/Khonsu/KhonsuAwakening")]
    public class KhonsuAwakening : AIBaseState
    {
        [SerializeField] 
        private InvincibilityCondition invincibilityCondition = default;
        public override void Enter()
        {
            ConditionManager.AddCondition(invincibilityCondition, AI, AI);
            AI.AnimationHandler.SetFloat("SpeedModifier", 1f);
            AI.AnimationHandler.SetBool("Falling", false);
            AI.AnimationHandler.SetBool("Rising", true);
            AI.AnimationHandler.OnComboWindowEnded += BeginPhasing;
        }

        private void BeginPhasing()
        {
            AI.AnimationHandler.SetBool("Rising", false);
            StateMachine.TransitionTo<KhonsuPhaseOne>();
            AI.AnimationHandler.OnComboWindowEnded -= BeginPhasing;
        }
    }
}