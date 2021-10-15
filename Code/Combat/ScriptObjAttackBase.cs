// Primary Author : Viktor Dahlberg - vida6631
// Secondary Author: Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat.AttackCollider;
using Combat.ConditionSystem;
using Combat.ConditionSystem.Condition;
using Entity;
using Framework.ScriptableObjectVariables.Primitives;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Combat
{
	/// <summary>
	///     Base class for all attacks that want to utilize the combat system,
	///     including the input buffering and comboing features.
	/// </summary>
	public abstract class ScriptObjAttackBase : ScriptableObject
    {
        #region Fields

        [Header("Attack Info")]
        
        [SerializeField] [Tooltip("Used to determine animation to be played. Ignored if animationID's is used.")]
        protected string title = default;

        [SerializeField] [Tooltip("Used to determine animations to be played. If used, Title will be ignored.")]
        protected string[] animationIDs = default;

        [SerializeField] [Tooltip("Base damage dealt to enemies on attack climax.")]
        protected int damage = default;

        [SerializeField] [Tooltip("Hard delay before attack can be reused.")]
        protected int reuseAttackDelayMillis = default;

        [SerializeField] [Tooltip("Condition used to dash with every attack")]
        protected MoveCondition moveCondition = default;

        [Header("Attack Collider Info")]
        
        [SerializeField]
        protected AttackColliderType attackCollider = default;
        [SerializeField]
        protected Transform attackColliderRotator = default;
        [SerializeField]
        protected ScriptObjFloat range = default;
        [SerializeField]
        public Vector3 attackOffset = default;
        [SerializeField]
        protected Vector2 attackSize = Vector2.one;
        
        [Header("Combo Info")] 
        
        public bool isFollowup = default;
        [SerializeField] [Tooltip("Attacks whose inputs will be enabled once the this one is used.")]
        protected List<ScriptObjAttackBase> followups = default;
        [SerializeField] [Tooltip("Time before a followup input can be buffered.")]
        private int comboWindowDelayMillis = default;
        [SerializeField] [Tooltip("Time before reuse if combo was dropped.")]
        private int comboDroppedPunishmentMillis = default;
        
        [Header("Input Info")] 
        
        [SerializeField]
        private bool ignoreInputSystem = default;

        public Action OnImmediateBegin;
        public Action OnBeginAttack;
        public Action OnSpecial;

        //combo link bookkeeping
        private ScriptObjAttackBase _origin;
        private ScriptObjAttackBase _previous;

        //combo determination
        private TaskCompletionSource<bool> _currentAttackFinished;
        private TaskCompletionSource<bool> _pressedAttackInComboWindow;
        private TaskCompletionSource<bool> _cancelledBeforeComboWindow;
        private bool _readyToClimax;
        protected bool attackCanBeTriggered;

        //owner and owner retrieved components
        protected GameObject owner;
        protected AttackColliderCollisionDetection AttackColliderCollisionDetection;
        protected AnimationHandler animationHandler;
        protected EntityBase entityBase;
        private float _timeOfAttack;

        public bool IsAttacking { get; internal set; }

        public float FinalDamageMultiplier { get; set; } = 1;

        #endregion

        #region Combo System & Action Control

        //attack results
        protected const bool AttackCompleted = true;
        protected const bool AttackInterrupted = false;

        //continuation results
        private const bool AttackCommited = true;
        private const bool AttackCancelled = false;

        //combo link results
        private const bool ComboContinued = true;
        private const bool ComboDropped = false;

        public void OnDisable()
        {
            if (animationHandler)
            {
                animationHandler.OnAttackCompleted = null;
                animationHandler.OnAttackInterrupted = null;
                animationHandler.OnComboWindowEnded = null;
                animationHandler.OnAttackClimaxed = null;
            }
        }

        /// <summary>
        ///     Assigns the owner GameObject.
        ///     The owner needs to contain an AnimationHandler component and
        ///     an AttackColliderCollisionDetection component.
        ///     The owner should assign itself during initialization.
        /// </summary>
        /// <param name="owner">Owner whose animator and colliders will be used to display and calculate attacks.</param>
        public void SetOwner(GameObject owner)
        {
            this.owner = owner;
            entityBase = entityBase == null ? this.owner.GetComponent<EntityBase>() : entityBase;
            AttackColliderCollisionDetection = owner.GetComponentInChildren<AttackColliderCollisionDetection>();
            attackColliderRotator = AttackColliderCollisionDetection.transform.parent.transform;
            animationHandler = owner.GetComponentInChildren<AnimationHandler>();
            animationHandler.OnAttackCompleted += OnAttackCompleted;
            animationHandler.OnAttackInterrupted += OnAttackInterrupted;
            animationHandler.OnComboWindowEnded += OnComboWindowEnded;
            animationHandler.OnAttackClimaxed += OnAttackClimaxed;
            PostOwnerSet();
        }

        /// <summary>
        ///     If a subclass wishes to perform something immediately after the
        ///     owner retrieved components are assigned, it can do so here.
        /// </summary>
        protected virtual void PostOwnerSet()
        {
        }

        /// <summary>
        ///     Function for programatically triggering an attack, rather than using the input system.
        /// </summary>
        public void Trigger()
        {
            OnAttackPressed(true);
        }

        public void EnableAttackTrigger(object source = null)
        {
            attackCanBeTriggered = true;
            if (source != null)
            {
                Debug.Log(this + " inputs enabled by " + source);
            }
        }

        public virtual void DisableAttackTrigger()
        {
            attackCanBeTriggered = false;
        }

        protected void ResetInputs()
        {
            DisableAttackTrigger();
            if (isFollowup)
            {
                if (_origin != null)
                {
                    _origin.EnableAttackTrigger();
                }
            }
            else
            {
                EnableAttackTrigger();
            }
        }

        protected void TriggerMovement()
        {
            moveCondition.MoveDirection = owner.transform.forward;
            var dimensions = entityBase.GetDimensions();
            moveCondition.Radius = dimensions.Item1;
            moveCondition.Height = dimensions.Item2;
            ConditionManager.AddCondition(moveCondition, entityBase, entityBase);
        }

        private void OnFirstCheckSucceeded()
        {
        }

        protected virtual bool CanUse()
        {
            return true;
        }

        protected abstract void Attack();

        protected virtual void OnAttack()
        {
        }

        protected void RefreshCollider()
        {
            AttackColliderCollisionDetection.SetCollider(attackCollider, range.value, attackOffset, attackSize);
        }

        public string GetTitle()
        {
            return title;
        }

        public float GetTimeLeft()
        {
            return reuseAttackDelayMillis / 1000f - (Time.time - _timeOfAttack);
        }

        public void ForceAttack()
        {
            RefreshCollider();
            IsAttacking = true;
            Attack();
        }

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

        public virtual void TriggerWithInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                OnAttackPressed();
            }
        }

        /// <summary>
        ///     This should be ran when the by the implementing classes desired input action is performed.
        ///     <param name="programmatic">Whether or not the function was called by Trigger() or as a result of an input action.</param>
        /// </summary>
        protected async void OnAttackPressed(bool programmatic = false)
        {
            if (!CanUse() || !attackCanBeTriggered || ignoreInputSystem && !programmatic)
            {
                return;
            }

            OnFirstCheckSucceeded();
            DisableAttackTrigger();

            //check if this press was in a combo window spawned by a previous attack
            if (InComboWindow())
            {
                _pressedAttackInComboWindow.SetResult(true);
            }

            //check if we need to wait for a current attack
            if (!animationHandler.IsAttacking)
            {
                BeginAttack();
            }
            else
            {
                //check if attack was completed or interrupted
                _currentAttackFinished = new TaskCompletionSource<bool>();
                var currentAttackResult = await _currentAttackFinished.Task;
                _currentAttackFinished = null;

                switch (currentAttackResult)
                {
                    case AttackCompleted:
                    {
                        BeginAttack();
                    }
                        break;
                    case AttackInterrupted:
                    {
                        ResetInputs();
                        return;
                    }
                }
            }
        }

        /// <summary>
        ///     The actual performing of the attack, executed if various conditions are met,
        ///     such as the previous attack not being cancelled or no attack being in progress.
        /// </summary>
        private async void BeginAttack()
        {
            if (!CanUse())
            {
                ResetInputs();
                return;
            }

            IsAttacking = true;
            _timeOfAttack = Time.time;
            _readyToClimax = true;
            //-------------
            //start sfx, vfx, animations here, before the await
            //-------------
            RefreshCollider();
            animationHandler.AttackStartEvent?.Invoke(title, animationIDs);
            OnAttack();

            //wait until attack is ready, as overridable by the subclasses
            OnImmediateBegin?.Invoke();
            var delayResult = await Delay();
            OnBeginAttack?.Invoke();
            switch (delayResult)
            {
                case AttackCommited:
                {
                    if (HasFollowups())
                    {
                        foreach (var followup in followups)
                        {
                            //if this attack is an origin attack, i.e. the first light attack, assign self as origin to followups
                            //otherwise, propagate existing origin
                            if (!isFollowup)
                            {
                                followup._origin = this;
                                followup._previous = this;
                            }
                            else
                            {
                                followup._origin = _origin;
                                followup._previous = this;
                            }

                            followup.RunComboWindow();
                        }
                    }
                    else
                    {
                        if (isFollowup)
                        {
                            _origin.EnableAttackTrigger();
                        }
                        else
                        {
                            await Task.Delay(reuseAttackDelayMillis);
                            EnableAttackTrigger();
                        }
                    }
                }
                    break;
                case AttackCancelled:
                {
                    ResetInputs();
                    return;
                }
            }
        }

        protected virtual async Task<bool> Delay()
        {
            _cancelledBeforeComboWindow = new TaskCompletionSource<bool>();
            await Task.WhenAny(Task.Delay(comboWindowDelayMillis), _cancelledBeforeComboWindow.Task);
            var result = !_cancelledBeforeComboWindow.Task.IsCompleted || _cancelledBeforeComboWindow.Task.Result;
            _cancelledBeforeComboWindow = null;
            return result;
        }

        /// <summary>
        ///     Animation Event. When in-range entities are captured and damage is applied.
        /// </summary>
        private void OnAttackClimaxed()
        {
            if (_readyToClimax)
            {
                Attack();
                _readyToClimax = false;
            }
        }

        /// <summary>
        ///     Animation Event. After this point we can cancel into another attack.
        /// </summary>
        protected virtual void OnAttackCompleted()
        {
            IsAttacking = false;
            if (_currentAttackFinished != null)
            {
                _currentAttackFinished.SetResult(AttackCompleted);
            }
        }

        /// <summary>
        ///     Animation Event. If this happens, buffered attacks are aborted.
        /// </summary>
        protected virtual void OnAttackInterrupted()
        {
            IsAttacking = false;
            _readyToClimax = false;
            _currentAttackFinished?.SetResult(AttackInterrupted);
            _pressedAttackInComboWindow?.SetResult(ComboDropped);
            _cancelledBeforeComboWindow?.SetResult(AttackCancelled);
        }

        /// <summary>
        ///     Animation Event. After this point a combo will reset to its origin attack.
        /// </summary>
        protected virtual void OnComboWindowEnded()
        {
            _pressedAttackInComboWindow?.SetResult(false);
        }

        /// <summary>
        ///     Checks if any followup attack inputs are detected and sets attack buffer accordingly.
        /// </summary>
        private async void RunComboWindow()
        {
            EnableAttackTrigger();

            //check if attack was pressed in the combo window
            _pressedAttackInComboWindow = new TaskCompletionSource<bool>();
            var comboLinkResult = await _pressedAttackInComboWindow.Task;
            _pressedAttackInComboWindow = null;

            switch (comboLinkResult)
            {
                case ComboContinued:
                {
                    return;
                }
                case ComboDropped:
                {
                    DisableAttackTrigger();
                    await Task.Delay(_previous.comboDroppedPunishmentMillis);
                    _origin.EnableAttackTrigger();
                }
                    break;
            }
        }

        /// <summary>
        ///     Checks if the attack has any followup attacks to combo into.
        /// </summary>
        /// <returns>Whether or not the attack has any followups.</returns>
        private bool HasFollowups()
        {
            return followups != null && followups.Count > 0;
        }

        /// <summary>
        ///     Checks if the current attack is available for comboing.
        /// </summary>
        /// <returns>Whether or not the current attack is in its combo window.</returns>
        private bool InComboWindow()
        {
            return _pressedAttackInComboWindow != null;
        }
        
        #endregion
    }
}