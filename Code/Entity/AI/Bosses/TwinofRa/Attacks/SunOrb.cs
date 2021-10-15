// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using Entity.HealthSystem;
using Environment.Pickup;
using Framework.ScriptableObjectEvent;
using UnityEngine;

namespace Entity.AI.Bosses.TwinofRa.Attacks
{
    public class SunOrb : MonoBehaviour
    {
        [SerializeField]
        private GameEvent startEvent = default;
        [SerializeField]
        private float damageTickTime = default;
        [SerializeField]
        private float maxDamage = default;
        [SerializeField]
        private float instaKillDistance = default;
        [SerializeField]
        private float speed = default;
        [SerializeField]
        private Vector3 targetOffset = default;
        [SerializeField]
        private AbilityOrb abilityOrb = default;

        private SphereCollider _collider;
        private bool _isActive;
        private PlayerHealth _playerHealth;
        private Rigidbody _rigidbody;
        private Transform _target;
        private Coroutine _tickDamageRoutine;
        private const float MINDamageMultiplier = 0.1f;
        private const float MAXDamageMultiplier = 1f;

        private void Start()
        {
            abilityOrb.onBossCompleted += DestroyMe;
            startEvent.OnEvent += () => _isActive = true;
            _collider = GetComponent<SphereCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _playerHealth = FindObjectOfType<PlayerHealth>();
            Physics.IgnoreCollision(_playerHealth.GetComponent<Collider>(), GetComponents<Collider>()[1]);
            _target = _playerHealth.transform;
        }

        private void FixedUpdate()
        {
            if (_isActive && SeesPlayer())
            {
                _rigidbody.MovePosition(
                    transform.position + (_target.position + targetOffset - transform.position).normalized * (speed * Time.fixedDeltaTime)
                    );
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") && _tickDamageRoutine == null)
            {
                _tickDamageRoutine = StartCoroutine(DamageTick());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") && _tickDamageRoutine != null)
            {
                StopCoroutine(_tickDamageRoutine);
                _tickDamageRoutine = null;
            }
        }

        private void DestroyMe()
        {
            Destroy(gameObject);
        }

        private bool SeesPlayer()
        {
            return Physics.Linecast(transform.position, _target.position + targetOffset);
        }

        private IEnumerator DamageTick()
        {
            while (true)
            {
                var distance = (_target.position + targetOffset - transform.position).magnitude - instaKillDistance;
                if (distance > instaKillDistance)
                {
                    _playerHealth.TakeDamage(Mathf.Clamp(1f - distance / _collider.radius, MINDamageMultiplier, MAXDamageMultiplier) * maxDamage);
                }
                else
                {
                    _playerHealth.TakeDamage(float.PositiveInfinity);
                }

                yield return new WaitForSeconds(damageTickTime);
            }
        }
    }
}