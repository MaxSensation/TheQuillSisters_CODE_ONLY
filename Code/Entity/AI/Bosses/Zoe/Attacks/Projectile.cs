// Primary Author : Andreas Berzelius - anbe5918

using System;
using Entity.HealthSystem;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] 
        private float force = default;
        [SerializeField] 
        private float damage = 5f;

        public static Action Ready;
        private Rigidbody _rigidbody;
        private float _holdForce;
        private Transform _parentTransform;
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _parentTransform = transform.parent;
            transform.rotation = _parentTransform.rotation;
            _rigidbody = GetComponent<Rigidbody>();
            _holdForce = force;
            force = 0;
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(_parentTransform.forward * (force * Time.deltaTime), ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other)
            {
                return;
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.gameObject.GetComponent<Health>().TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        public void Release()
        {
            force = _holdForce;
            Ready?.Invoke();
            transform.parent = null;
            transform.rotation = _parentTransform.rotation;
            _audioSource.Play();
        }
    }
}