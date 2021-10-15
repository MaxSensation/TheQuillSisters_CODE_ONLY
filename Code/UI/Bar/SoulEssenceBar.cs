// Primary Author : Viktor Dahlberg - vida6631

using Environment.Pickup;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace UI.Bar
{
    public class SoulEssenceBar : Bar
    {
        [SerializeField]
        private ScriptObjVar<float> soulEssenceGauge = default;
        [SerializeField]
        private GameEventGroup onSceneLoaded = default;
        [SerializeField]
        private GameObject primarySlider = default;
        [SerializeField]
        private GameObject secondarySlider = default;
        [SerializeField]
        private ScriptObjVar<bool> isEnabled = default;

        private void OnEnable()
        {
            AbilityOrb.gotAbility += Enable;
            mainSlider.value = echoSlider.value = soulEssenceGauge.value;
            soulEssenceGauge.ValueChanged += Refresh;
            onSceneLoaded.OnEvent += Refresh;
            if (isEnabled.value)
            {
                primarySlider.SetActive(true);
                secondarySlider.SetActive(true);
            }
        }

        private void OnDisable()
        {
            AbilityOrb.gotAbility -= Enable;
            soulEssenceGauge.ValueChanged -= Refresh;
            onSceneLoaded.OnEvent -= Refresh;
        }

        private void Enable()
        {
            if (isEnabled.value)
            {
                return;
            }

            primarySlider.SetActive(true);
            secondarySlider.SetActive(true);
            isEnabled.value = true;
        }

        public override void Refresh()
        {
            mainSlider.value = soulEssenceGauge.value;
            ResetWait();
        }
    }
}