// Primary Author : Andreas Berzelius - anbe5918

using System.Collections.Generic;
using Entity.HealthSystem;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class PoisonThrown : MonoBehaviour
    {
        [SerializeField] 
        private float damage = default;

        private readonly List<ParticleSystem.Particle> _enter = new List<ParticleSystem.Particle>();
        private CharacterController _playerCollider;
        private PlayerHealth _playerHealth;
        private ParticleSystem _poison;

        private void Start()
        {
            _poison = GetComponent<ParticleSystem>();
            _playerHealth = FindObjectOfType<PlayerHealth>();
            _playerCollider = _playerHealth.gameObject.GetComponent<CharacterController>();
            _poison.trigger.SetCollider(0, _playerCollider);
        }

        private void OnParticleTrigger()
        {
            var enterPs = _poison.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _enter);
            if (enterPs >= 1)
            {
                _playerHealth.TakeDamage(damage);
            }
        }
    }
}