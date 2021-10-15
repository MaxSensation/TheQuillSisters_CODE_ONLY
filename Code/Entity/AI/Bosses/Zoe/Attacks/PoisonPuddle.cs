// Primary Author : Andreas Berzelius - anbe5918

using Entity.HealthSystem;
using UnityEngine;

namespace Entity.AI.Bosses.Zoe.Attacks
{
    public class PoisonPuddle : MonoBehaviour
    {
        [SerializeField] 
        private float damage = default;

        private const int PlayerLayer = 9;
        private PlayerHealth _playerHealth;

        private void Start()
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.layer == PlayerLayer)
            {
                _playerHealth.TakeDamage(damage);
            }
        }
    }
}