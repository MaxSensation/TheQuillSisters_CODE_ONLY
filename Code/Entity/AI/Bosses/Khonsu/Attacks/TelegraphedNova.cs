// Primary Author : Viktor Dahlberg - vida6631

using Combat.Ability;
using UnityEngine;
using VFX;

namespace Entity.AI.Bosses.Khonsu.Attacks
{
	/// <summary>
	///     Nova that also prefaces the attack with a charge VFX.
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Attacks/Bosses/Khonsu/TelegraphedNova")]
    public class TelegraphedNova : ScriptObjAbilityBase
    {
        [Header("Telegraph")] 
        
        [SerializeField]
        private GameObject telegraphVFX = default;
        [SerializeField]
        private GameObject visualCueVFX = default;
        [SerializeField]
        private int novaSpawnRate = 1000;
        
        private NovaTelegraphController _nova;
        private GameObject _telegraphVFXInstance;
        private GameObject _visualCueVFXInstance;

        public void TriggerWithCancel()
        {
            Trigger();
            animationHandler.OnMiscEvent2 += ShowTelegraph;
        }

        public void Refresh()
        {
            if (_nova != null)
            {
                RefreshCollider();
                _nova.Target = owner.transform.position + visualEffectOffset;
            }
        }

        private void ShowTelegraph()
        {
            animationHandler.OnMiscEvent2 -= ShowTelegraph;
            _telegraphVFXInstance = Instantiate(telegraphVFX, Vector3.zero, Quaternion.identity);
            _nova = _telegraphVFXInstance.GetComponentInChildren<NovaTelegraphController>();
            var khonsu = owner.GetComponent<Khonsu>();
            _nova.Target = owner.transform.position + visualEffectOffset;
            _nova.LeftHand = khonsu.leftHand;
            _nova.RightHand = khonsu.rightHand;
            _nova.SpawnRate = novaSpawnRate;
            _telegraphVFXInstance.SetActive(true);
        }

        protected override void OnComboWindowEnded()
        {
            if (_nova != null)
            {
                _nova.SpawnRate = 0;
                _nova.Kill?.Invoke();
            }

            if (_telegraphVFXInstance != null)
            {
                OnSpecial?.Invoke();
                _visualCueVFXInstance = Instantiate(visualCueVFX, owner.transform.position + visualEffectOffset, Quaternion.Euler(90, 0, 0));
            }
            
            base.OnComboWindowEnded();
        }

        protected override void OnAttackInterrupted()
        {
            base.OnAttackInterrupted();

            DisposeVFX();
        }

        protected override void OnAttackCompleted()
        {
            Destroy(_telegraphVFXInstance);
            base.OnAttackCompleted();
        }

        protected override void Attack()
        {
            if (_visualCueVFXInstance != null)
            {
                Destroy(_visualCueVFXInstance, 1f);
            }

            base.Attack();
            foreach (var entity in AttackColliderCollisionDetection.GetDamageables(true))
            {
                entity.TakeDamage(damage * FinalDamageMultiplier);
            }
        }

        protected override void SpawnVFX()
        {
            visualEffectInstances = new GameObject[visualEffectPrefabs.Length];
            for (var i = 0; i < visualEffectPrefabs.Length; i++)
            {
                var transform = AttackColliderCollisionDetection.transform;
                visualEffectInstances[i] = Instantiate(
                    visualEffectPrefabs[i],
                    transform.position + visualEffectOffset,
                    transform.parent.rotation
                );
                visualEffectInstances[i].SetActive(true);
            }
        }

        public void DisposeVFX()
        {
            if (_telegraphVFXInstance != null)
            {
                Destroy(_telegraphVFXInstance, 0.5f);
            }

            if (_visualCueVFXInstance != null)
            {
                Destroy(_visualCueVFXInstance);
            }

            if (_nova != null)
            {
                _nova.SpawnRate = 0;
                _nova.Kill?.Invoke();
            }
        }
    }
}