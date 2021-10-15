// Primary Author : Andreas Berzelius - anbe5918
// Secondary Author : Erik Pilström - erpi3245

using Entity.HealthSystem;
using Framework.ScriptableObjectVariables;
using UI.Bar;
using UnityEngine;
using UnityEngine.UI;

namespace Entity.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Nova")] 
        
        [SerializeField]
        private GameObject nova = default;
        [SerializeField]
        private Image novaShade = default;
        [SerializeField]
        private ScriptObjVar<bool> novaAcquired = default;
        
        [Header("Beam")]

        [SerializeField]
        private GameObject beam = default;
        [SerializeField]
        private Image beamShade = default;
        [SerializeField]
        private ScriptObjVar<bool> beamAcquired = default;
        
        [Header("Stats")] 
        
        [SerializeField]
        private ScriptObjRef<float> currentAbilityEnergyGauge = default;
        [SerializeField]
        private ScriptObjRef<float> maxAbilityEnergyGauge = default;
        [SerializeField]
        private PlayerHealthBar playerHealthBar = default;

        private GameObject _player;
        private PlayerHealth _playerHealth;

        // Depending on how the player is re-enabled the unsubsription and subscription of tookDamage will look different.
        private void Start()
        {
            _player = FindObjectOfType<PlayerController>().gameObject;
            _playerHealth = _player.GetComponent<PlayerHealth>();
            playerHealthBar.SetHealthComponentTarget(_playerHealth);
        }

        private void Update()
        {
            nova.SetActive(novaAcquired);
            beam.SetActive(beamAcquired);
            if (beamAcquired)
            {
                if (currentAbilityEnergyGauge.Value >= 50)
                {
                    beamShade.fillAmount = 0;
                }

                if (currentAbilityEnergyGauge.Value <= 50)
                {
                    beamShade.fillAmount = 1;
                }
            }

            if (novaAcquired)
            {
                if (currentAbilityEnergyGauge.Value >= 30)
                {
                    novaShade.fillAmount = 0;
                }

                if (currentAbilityEnergyGauge.Value <= 30)
                {
                    novaShade.fillAmount = 1;
                }
            }
        }
    }
}