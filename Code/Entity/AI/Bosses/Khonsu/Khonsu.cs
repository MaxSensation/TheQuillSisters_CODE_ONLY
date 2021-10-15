// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using System.Threading;
using Entity.AI.Bosses.Khonsu.Attacks;
using Entity.AI.Bosses.Khonsu.States;
using Entity.HealthSystem;
using Environment.RoomManager;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UI.Bar;
using UnityEngine;

namespace Entity.AI.Bosses.Khonsu
{
    /// <summary>
    ///     Main driver script for Khonsu.
    /// </summary>
    public class Khonsu : Boss
    {
        [Header("Async Reset")]
        
        [SerializeField]
        private GameEvent gameLoadReady = default;
        
        [Header("Looking/Turning")]
        
        [SerializeField]
        private Transform head = default;
        [SerializeField]
        private float dotTreshold = default;
        [SerializeField]
        private bool lookAtPlayer = default;
        [SerializeField]
        private float lookSpeed = default;
        [SerializeField]
        private bool turnTowardPlayer = default;
        [SerializeField]
        private float turnSpeed = default;
        
        [Header("Casting")]
        
        [SerializeField]
        public Transform leftHand = default;
        [SerializeField]
        public Transform rightHand = default;
        [SerializeField]
        public Spawner platformSpawner = default;
        [SerializeField]
        public Spawner groundSpawner = default;
        [SerializeField]
        private float healthGateFraction = default;
        
        [Header("Enrage")]
        
        [SerializeField]
        private ScriptObjVar<bool> enraged = default;
        [SerializeField]
        private float enrageFraction = default;
        
        [Header("Invulnerability")]
        
        [SerializeField]
        private SkinnedMeshRenderer mainMeshRenderer;
        
        private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");
        private Color _baseColor;
        private Collider _collider;
        private Vector3 _bodyForward;
        private Vector3 _headForward;
        private bool _resetting;
        private bool _dead;
        private float _currentHealthGateThreshold;
        private float _overlayAlpha;

        public CancellationTokenSource Cancel { get; private set; }

        public int HealthGateThresholds { get; set; }

        /// <summary>
        ///     Cleans up all invocation registrations and cancels async stuff.
        /// </summary>
        private void Reset()
        {
            AnimationHandler.OnMiscEvent1 = null;
            AnimationHandler.OnComboWindowEnded = null;
            AnimationHandler.OnAttackClimaxed = null;
            AnimationHandler.OnAttackCompleted = null;
            AnimationHandler.OnAttackAnimationEnded = null;
            Cancel.Cancel();
        }

        private new void Start()
        {
            base.Start();
            _headForward = head.transform.forward;
            _bodyForward = transform.forward;
            _baseColor = mainMeshRenderer.material.GetColor(EmissiveColor);
        }

        //since Khonsu doesn't care about being grounded, run BehaviorStateMachine here
        private new void Update()
        {
            BehaviorStateMachine.Run();
            base.Update();
        }

        //rotating in LateUpdate to override AnimationController
        private void LateUpdate()
        {
            if (lookAtPlayer)
                if (!Rotate(head, lookSpeed, ref _headForward, false, false, _resetting) && turnTowardPlayer &&
                    !AnimationHandler.IsAttacking)
                {
                    //since animator overrides head transform, a "tracker" forward vector is kept
                    //this vector needs to be updated to maintain its relation to the body rotation
                    var oldRotation = transform.rotation;
                    Rotate(transform, turnSpeed, ref _bodyForward, true, true);
                    var newRotation = transform.rotation;
                    var delta = newRotation * Quaternion.Inverse(oldRotation);
                    _headForward = delta * _headForward;
                }
        }

        private void OnEnable()
        {
            HealthGateThresholds = 0;
            _currentHealthGateThreshold = healthGateFraction;
            BossHealthBarManager.Instance.Add(displayName, GetComponent<Health>(), 0);
            enraged.value = false;
            HealthSystem.Health.TookDamage += CheckEnrage;
            gameLoadReady.OnEvent += Reset;
            BehaviorStateMachine.TransitionTo<KhonsuAwakening>();
            Cancel = new CancellationTokenSource();
            _dead = false;
            _resetting = false;
            _collider = GetComponent<Collider>();
            _collider.enabled = true;
        }

        private void OnDisable()
        {
            BossHealthBarManager.Instance.Remove(0);
            AnimationHandler.OnAttackInterrupted?.Invoke();
            HealthSystem.Health.TookDamage -= CheckEnrage;
            gameLoadReady.OnEvent -= Reset;
        }

        /// <summary>
        ///     Determines whether or not Khonsu is enraged, and how many health gate thresholds have been passed.
        /// </summary>
        private void CheckEnrage(GameObject g, float _)
        {
            if (Health.GetCurrentHealth() < Health.GetMaxHealth() / 4f * _currentHealthGateThreshold)
            {
                _currentHealthGateThreshold--;
                HealthGateThresholds++;
            }

            if (!enraged)
            {
                enraged.value = Health.GetCurrentHealth() < Health.GetMaxHealth() / enrageFraction;
                if (enraged)
                {
                    BossHealthBarManager.Instance.Refresh(0, "The Eternally Damned and Unceasingly Wrathful Avatar of Khonsu");
                }
            }
        }

        //override to make room for death animation
        protected override void EnemyDied(GameObject entity)
        {
            if (_dead || entity != gameObject)
            {
                return;
            }

            _dead = true;
            BossHealthBarManager.Instance.Remove(0);
            Cancel.Cancel();
            groundSpawner.KillAll();
            platformSpawner.KillAll();
            AnimationHandler.AttackInterrupted();
            AnimationHandler.SetBool("Null", false);
            AnimationHandler.SetBool("Falling", true);
            AnimationHandler.OnMiscEvent1 += Disable;
            turnTowardPlayer = false;
            _resetting = true;
            OnBossDied?.Invoke();
        }

        private void Disable()
        {
            AnimationHandler.OnMiscEvent1 -= Disable;
            AnimationHandler.SetFloat("SpeedModifier", 0f);
            enabled = false;
            _collider.enabled = false;
            foreach (var colliderInChildren in GetComponentsInChildren<Collider>())
            {
                if (!colliderInChildren.isTrigger)
                {
                    colliderInChildren.enabled = false;
                }
            }

            if (_overlayAlpha > 0.9f)
            {
                StopCoroutine("FadeIn");
                StopCoroutine("FadeOut");
                FadeOut(0.1f);
            }

            (localAttacks[0] as TelegraphedNova)?.DisposeVFX();
        }

        /// <summary>
        ///     Rotates a transform to some forward at some speed.
        /// </summary>
        /// <param name="toRotate">Transform to rotate.</param>
        /// <param name="speed">Speed at which to rotate.</param>
        /// <param name="intendedForward">Bookkeeping.</param>
        /// <param name="removeY">Whether or not the transform should rotate up and down.</param>
        /// <param name="ignoreTreshold">Whether or not the rotation should stop when the Dot product treshold is reached.</param>
        /// <param name="customTarget">Target override.</param>
        /// <returns>Whether the rotation could be executed.</returns>
        private bool Rotate(Transform toRotate, float speed, ref Vector3 intendedForward, bool removeY, bool ignoreTreshold, bool reset = false)
        {
            var initial = intendedForward;
            if (!reset) toRotate.LookAt(playerPosition.Value);
            var final = toRotate.forward;
            var actual = Vector3.Slerp(initial, final, speed * Time.deltaTime);
            
            if (!ignoreTreshold && Vector3.Dot(actual, transform.forward) < dotTreshold)
            {
                if (removeY)
                {
                    actual.y = 0;
                }

                toRotate.forward = intendedForward;
                return false;
            }

            if (removeY)
            {
                actual.y = 0;
            }

            toRotate.forward = actual;
            intendedForward = actual;
            return true;
        }

        private float SetEmissiveColorAlpha(float alpha)
        {
            mainMeshRenderer.materials[0].SetColor(EmissiveColor, _baseColor * alpha);
            mainMeshRenderer.materials[1].SetColor(EmissiveColor, _baseColor * alpha);
            return alpha;
        }

        public void FadeIn(float overlayFadeSpeed)
        {
            StartCoroutine(PerformFadeIn(overlayFadeSpeed));
        }

        public void FadeOut(float overlayFadeSpeed)
        {
            StartCoroutine(PerformFadeOut(overlayFadeSpeed));
        }

        private IEnumerator PerformFadeIn(float overlayFadeSpeed)
        {
            do
            {
                _overlayAlpha = SetEmissiveColorAlpha(Mathf.MoveTowards(_overlayAlpha, 1f, overlayFadeSpeed * Time.deltaTime));
                yield return null;
            } 
            while (_overlayAlpha < 0.9995f);
        }

        private IEnumerator PerformFadeOut(float overlayFadeSpeed)
        {
            do
            {
                _overlayAlpha = SetEmissiveColorAlpha(Mathf.MoveTowards(_overlayAlpha, -1f, overlayFadeSpeed * Time.deltaTime));
                yield return null;
            } 
            while (_overlayAlpha > -0.9995f);
        }
    }
}