// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Threading.Tasks;
using Combat.ConditionSystem;
using UnityEngine;

namespace Entity.AI.Bosses.Khonsu.States
{
    [CreateAssetMenu(menuName = "States/EnemyStates/Bosses/Khonsu/KhonsuPhaseTwo")]
    public class KhonsuPhaseTwo : KhonsuPhase
    {
        [SerializeField]
        private float vulnerabilityDuration = default;
        [SerializeField]
        private float enragedVulnerabilityDuration = default;
        [SerializeField]
        private bool performSecondNova = default;

        public override async void Enter()
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(enraged ? enragedVulnerabilityDuration : vulnerabilityDuration),
                    ((Khonsu) AI).Cancel.Token);
                ConditionManager.AddCondition(invincibilityCondition, AI, AI);
                ((Khonsu) AI).FadeIn(overlayFadeSpeed);
                if (performSecondNova)
                {
                    BeginNova();
                    AI.AnimationHandler.OnAttackAnimationEnded += () => StateMachine.TransitionTo<KhonsuPhaseOne>();
                }
                else
                {
                    StateMachine.TransitionTo<KhonsuPhaseOne>();
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        public override void Exit()
        {
            AI.AnimationHandler.OnAttackAnimationEnded = null;
        }
    }
}