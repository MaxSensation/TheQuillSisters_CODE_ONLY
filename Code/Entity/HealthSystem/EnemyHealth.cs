// Primary Author : Erik Pilström - erpi3245

namespace Entity.HealthSystem
{
    public class EnemyHealth : Health
    {
        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth.Value;
        }

        public override void TakeDamage(float damage)
        {
            if (IsInvincible)
            {
                return;
            }

            _currentHealth -= damage;
            TookDamage?.Invoke(gameObject, damage);
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public override void Die()
        {
            EntityDied?.Invoke(gameObject);
        }

        public override float GetCurrentHealth()
        {
            return _currentHealth;
        }

        public void SetCurrentHealth(float health)
        {
            if (health >= _currentHealth)
            {
                _currentHealth = health;
            }
        }
    }
}