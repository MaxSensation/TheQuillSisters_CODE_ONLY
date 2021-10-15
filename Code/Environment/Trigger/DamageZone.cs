// Primary Author : Maximiliam Rosén - maka4519

using System.Collections;
using Entity.HealthSystem;
using UnityEngine;

namespace Environment.Trigger
{
    internal enum DamageZoneType
    {
        InstaKill,
        Tick,
        OneUse
    }

    public class DamageZone : MonoBehaviour
    {
        [SerializeField] 
        private DamageZoneType damageZoneType = default;
        [Header("OneUse")] [SerializeField] 
        private int oneUseDamage = default;
        [Header("TickDamage")] [SerializeField] 
        private float tickTimeInSeconds = default;
        [SerializeField] 
        private float damagePerTick = default;

        private Health _killable;
        private Coroutine _ticker;
        private bool _used;

        private void OnDestroy()
        {
            if (_ticker != null)
            {
                StopCoroutine(_ticker);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_used)
            {
                return;
            }

            var killable = other.GetComponent<Health>();
            if (!killable)
            {
                return;
            }

            switch (damageZoneType)
            {
                case DamageZoneType.InstaKill:
                    killable.TakeDamage(Mathf.Infinity);
                    break;
                case DamageZoneType.Tick:
                    var entityHealth = other.GetComponent<Health>();
                    if (_ticker == null)
                    {
                        _ticker = StartCoroutine(Ticker(entityHealth));
                    }

                    break;
                case DamageZoneType.OneUse:
                    _used = true;
                    killable.TakeDamage(oneUseDamage);
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var killable = other.GetComponent<Health>();
            if (!killable)
            {
                return;
            }

            if (_ticker != null)
            {
                StopCoroutine(_ticker);
                _ticker = null;
            }
        }

        private IEnumerator Ticker(Health entityHealth)
        {
            while (true)
            {
                entityHealth.TakeDamage(damagePerTick);
                yield return new WaitForSeconds(tickTimeInSeconds);
            }
        }
    }
}