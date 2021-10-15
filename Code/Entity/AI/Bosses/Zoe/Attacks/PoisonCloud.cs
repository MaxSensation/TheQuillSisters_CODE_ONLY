// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using Entity.HealthSystem;
using Entity.Player;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class PoisonCloud : MonoBehaviour
    {
        [SerializeField]
        private LayerMask groundMask = default;
        [SerializeField]
        private LayerMask damageMask = default;
        [SerializeField]
        private float maxRadius = default;
        [SerializeField]
        private float tickTime = default;
        [SerializeField]
        private float damage = default;
        [SerializeField]
        private float increaseSpeed = default;

        private bool _activated;
        private static GameObject _applier;
        private AudioSource _audioSource;
        private Coroutine _coroutine;
        private SphereCollider _damageArea;
        private PlayerController _player;
        private Rigidbody _rigidBody;
        private Transform _vfx;

        private void Start()
        {
            _vfx = transform.GetChild(0).transform;
            _damageArea = GetComponent<SphereCollider>();
            _audioSource = GetComponent<AudioSource>();
            _player = FindObjectOfType<PlayerController>();
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.AddForce((_player.transform.position - transform.position) * 2.5f + transform.up * 50f);
        }

        private void Update()
        {
            if (_activated)
            {
                if (_damageArea.radius <= maxRadius)
                {
                    _damageArea.radius += increaseSpeed;
                    _vfx.localScale = Vector3.one * (50f * _damageArea.radius);
                }
                else
                {
                    if (_coroutine != null)
                    {
                        StopCoroutine(_coroutine);
                    }

                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & groundMask) != 0 && !_activated)
            {
                Activate();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & damageMask) != 0 && _applier == gameObject)
            {
                StopCoroutine(_coroutine);
                _applier = null;
                _coroutine = null;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (((1 << other.gameObject.layer) & damageMask) != 0 && _activated && _coroutine == null && _applier == null)
            {
                _applier = gameObject;
                _coroutine = StartCoroutine(DamageOverTime());
            }
        }

        private IEnumerator DamageOverTime()
        {
            while (true)
            {
                _player.GetComponent<PlayerHealth>().TakeDamage(damage);
                yield return new WaitForSeconds(tickTime);
            }
        }

        private void Activate()
        {
            _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            _activated = true;
            _audioSource.Play();
        }
    }
}