// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Viktor Dahlberg - vida6631

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity
{
    public class AnimationHandler : MonoBehaviour
    {
        private const bool Continue = true;
        private const bool Failed = false;
        private Animator _animator;
        private TaskCompletionSource<bool> _continueSequence;
        public Action<string, string[]> AttackStartEvent;
        public Action OnAnimationFootStep;
        public Action OnAttackAnimationEnded;
        public Action OnAttackClimaxed;
        public Action OnAttackCompleted;
        public Action OnAttackContinued;
        public Action OnAttackInterrupted;
        public Action OnAttackStarted;
        public Action OnComboWindowEnded;
        public Action OnMiscEvent1;
        public Action OnMiscEvent2;
        public Action OnStaggerCompleted;

        public bool IsAttacking { get; private set; }

        private void OnEnable()
        {
            IsAttacking = false;
            _animator = GetComponent<Animator>();
            AttackStartEvent += (title, animationIDs) => BeginSequence(animationIDs.Length > 0 ? animationIDs : new[] {title});
            OnAttackContinued += AttackContinued;
            _animator.speed = 1f;
        }

        private void OnDisable()
        {
            AttackStartEvent = null;
            OnAttackContinued -= AttackContinued;
        }

        private async void BeginSequence(params string[] animationIDs)
        {
            OnAttackStarted?.Invoke();
            StartAnimation(animationIDs[0]);
            for (var i = 1; i < animationIDs.Length; i++)
            {
                _continueSequence = new TaskCompletionSource<bool>();
                var result = await _continueSequence.Task;
                _continueSequence = null;

                if (result == Continue)
                {
                    StartAnimation(animationIDs[i]);
                }
            }
        }

        private void AttackContinued()
        {
            if (_continueSequence != null)
            {
                _continueSequence.SetResult(Continue);
            }
        }

        private void StartAnimation(string attackTitle, bool isAttack = true)
        {
            _animator.speed = 1f;
            if (isAttack)
            {
                IsAttacking = true;
                _animator.SetBool("Attacking", true);
            }
            
            _animator.CrossFadeInFixedTime(attackTitle, 0.1f, 0, 0f);
        }

        public void SetAnimationSpeed(float speed)
        {
            _animator.speed = speed;
        }

        public void ScaleAnimationDuration(float animationLength, float targetDuration)
        {
            _animator.speed /= targetDuration / animationLength;
        }

        public void AttackInterrupted()
        {
            IsAttacking = false;
            _animator.SetBool("Attacking", false);
            if (_continueSequence != null)
            {
                _continueSequence.SetResult(Failed);
            }

            _animator.speed = 1f;
            OnAttackInterrupted?.Invoke();
        }

        public void AttackClimaxed()
        {
            OnAttackClimaxed?.Invoke();
        }

        public void AttackCompleted()
        {
            IsAttacking = false;
            _animator.SetBool("Attacking", false);
            OnAttackCompleted?.Invoke();
        }

        public void ComboWindowEnded()
        {
            OnComboWindowEnded?.Invoke();
        }

        public void AttackAnimationEnded()
        {
            if (!IsAttacking)
            {
                OnAttackAnimationEnded?.Invoke();
            }
        }

        /// <summary>
        ///     Animation event slots for things unrelated to attacks
        /// </summary>
        public void MiscEvent1()
        {
            OnMiscEvent1?.Invoke();
        }

        public void MiscEvent2()
        {
            OnMiscEvent2?.Invoke();
        }

        public void OnFootStep()
        {
            OnAnimationFootStep?.Invoke();
        }

        public void CompletedStagger()
        {
            OnStaggerCompleted?.Invoke();
        }

        public void ResetAnimation(string parameterAnimationName)
        {
            _animator.Play(parameterAnimationName, -1, 0);
        }

        public void SetTrigger(string parameterAnimationName)
        {
            _animator.SetTrigger(parameterAnimationName);
        }

        public void SetBool(string parameterAnimationName, bool value)
        {
            _animator.SetBool(parameterAnimationName, value);
        }
        
        public void SetFloat(string floatName, float value)
        {
            _animator.SetFloat(floatName, value);
        }
    }
}