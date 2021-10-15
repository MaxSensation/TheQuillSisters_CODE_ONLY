// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections.Generic;
using Entity.AI.Bosses;
using Environment.Trigger;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Pickup
{
	/// <summary>
	///     Grants an ability on pickup.
	/// </summary>
	public class AbilityOrb : MonoBehaviour
    {
        [SerializeField]
        private List<Boss> bossEntitites = new List<Boss>();
        [SerializeField]
        private GameObject orb = default;
        [SerializeField]
        private AreaTrigger trigger = default;
        [SerializeField]
        private ScriptObjVar<bool> abilityToAcquire = default;

        public static Action gotAbility;
        public Action onBossCompleted;
        private int _bosses;

        private void OnEnable()
        {
            Boss.OnBossDied += CheckIfAllBossesDead;
            trigger.OnTrigger += Triggered;
            _bosses = bossEntitites.Count;
        }

        private void OnDisable()
        {
            Boss.OnBossDied -= CheckIfAllBossesDead;
            trigger.OnTrigger -= Triggered;
        }

        private void Triggered(bool triggered)
        {
            abilityToAcquire.value = true;
            gameObject.SetActive(false);
            gotAbility?.Invoke();
        }

        private void CheckIfAllBossesDead()
        {
            if (--_bosses == 0)
            {
                onBossCompleted?.Invoke();
                orb.SetActive(true);
            }
        }
    }
}