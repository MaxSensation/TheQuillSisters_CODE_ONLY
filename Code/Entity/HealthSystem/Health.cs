// Primary Author : Erik Pilström - erpi3245

using System;
using Combat.Interfaces;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.HealthSystem
{
    public abstract class Health : MonoBehaviour, IInvincible
    {
        [SerializeField] 
        protected ScriptObjRef<float> maxHealth;
        
        public static Action<GameObject, float> TookDamage;
        public static Action<GameObject> EntityDied;
        protected bool IsInvincible;

        public void SetInvincibility(bool active)
        {
            IsInvincible = active;
        }

        public abstract void TakeDamage(float damage);

        public abstract void Die();

        //Made this method virtual as it currently serves its purpose as written in both of the derived classes
        //PlayerHealth and EnemyHealth, but in the future there may be a need to alter the behaviour and thus we may want it overrideable
        public float GetMaxHealth()
        {
            return maxHealth.Value;
        }

        public abstract float GetCurrentHealth();
    }
}