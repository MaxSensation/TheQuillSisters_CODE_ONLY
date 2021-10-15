// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Andreas Berzelius - anbe5918

using System;
using System.Collections;
using Entity.AI.Bosses.Zoe.States;
using Entity.HealthSystem;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class RoomGas : MonoBehaviour
    {
        [SerializeField]
        private float startSize = default;
        [SerializeField]
        private float endSize =default;
        [SerializeField]
        private float fillPercentage =default;
        [SerializeField]
        private float transitionSpeed = default;
        [SerializeField]
        private float tickTime = default;
        [SerializeField]
        private float damagePerTick = default;

        public Action Filled;
        private bool _isFilling;
        private SphereCollider _collider;
        private PlayerHealth _playerHealth;
        private Coroutine _tickDamageCoroutine;
        private Vector3 _wantedSize;

        public void Start()
        {
            _collider = GetComponent<SphereCollider>();
            _playerHealth = FindObjectOfType<PlayerHealth>();
            BossZoe.IncreaseGas += () => FillRoom();
        }

        private void Update()
        {
            var distance = (_playerHealth.transform.position - transform.position).magnitude;
            if (distance >= _collider.radius * transform.localScale.x)
            {
                if (_tickDamageCoroutine == null)
                {
                    _tickDamageCoroutine = StartCoroutine(StartTickDamage());
                }
            }
            else
            {
                if (_tickDamageCoroutine != null)
                {
                    StopCoroutine(_tickDamageCoroutine);
                    _tickDamageCoroutine = null;
                }
            }

            if (_isFilling)
            {
                transform.localScale = Vector3.Slerp(transform.localScale, _wantedSize, transitionSpeed);
                if (transform.localScale.magnitude - 0.5 <= _wantedSize.magnitude)
                {
                    _isFilling = false;
                    Filled?.Invoke();
                }
            }
        }

        private void OnDisable()
        {
            if (_tickDamageCoroutine != null)
            {
                StopCoroutine(_tickDamageCoroutine);
            }
        }

        private void OnDestroy()
        {
            BossZoe.IncreaseGas = null;
        }

        [ContextMenu("Fill")]
        private void FillRoom(bool firstUse = false)
        {
            var newSize = transform.localScale.x - transform.localScale.x * (fillPercentage / 100f);
            if (newSize < endSize)
            {
                newSize = endSize;
            }
            _wantedSize = firstUse ? startSize * Vector3.one : Vector3.one * newSize;
            _isFilling = true;
        }

        private IEnumerator StartTickDamage()
        {
            while (true)
            {
                _playerHealth.TakeDamage(damagePerTick);
                yield return new WaitForSeconds(tickTime);
            }
        }
    }
}