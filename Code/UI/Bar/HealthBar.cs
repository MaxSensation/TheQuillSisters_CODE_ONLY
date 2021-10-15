// Primary Author : Viktor Dahlberg - vida6631

using Entity.HealthSystem;
using UnityEngine;

namespace UI.Bar
{
    public class HealthBar : Bar
    {
        private Health _health;

        protected void Start()
        {
            Health.TookDamage += UpdateSliders;
        }

        public override void Refresh()
        {
            mainSlider.value = echoSlider.value = _health.GetCurrentHealth() / _health.GetMaxHealth() * 100f;
        }

        private void UpdateSliders(GameObject entity, float _)
        {
            var health = entity.GetComponent<Health>();
            if (health != null && health == this._health)
            {
                mainSlider.value = health.GetCurrentHealth() / health.GetMaxHealth() * 100f;
                ResetWait();
            }
        }

        public void SetHealthComponentTarget(Health healthComponent)
        {
            _health = healthComponent;
            if (_health != null)
            {
                mainSlider.value = echoSlider.value = _health.GetCurrentHealth() / _health.GetMaxHealth() * 100f;
            }
        }
    }
}