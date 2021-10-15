// Primary Author : Viktor Dahlberg - vida6631

using Entity.HealthSystem;
using Environment.Pickup;
using Framework.ScriptableObjectEvent;
using UnityEngine;

namespace UI.Bar
{
    public class PlayerHealthBar : HealthBar
    {
        [SerializeField] 
        private GameEventGroup onSceneLoaded = default;

        private new void Start()
        {
            base.Start();
            EyeOfHorus.HealedPlayer += Refresh;
            PlayerHealth.PlayerHealed += Refresh;
            onSceneLoaded.OnEvent += Refresh;
        }
    }
}