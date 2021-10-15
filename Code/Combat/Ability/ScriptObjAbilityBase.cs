//Primary Author : Maximiliam Rosén - maka4519

using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Combat.Ability
{
    /// <summary>
    /// A SuperClass for the Ability
    /// </summary>
    public abstract class ScriptObjAbilityBase : ScriptObjAttackBase
    {   
        [Header("VFX")] [SerializeField] 
        protected GameObject[] visualEffectPrefabs = default;
        [SerializeField] 
        protected float vfxDestroyTimeAfterAttack = default;
        [SerializeField] 
        public Vector3 visualEffectOffset = default;
        [Header("Essence")] [SerializeField] 
        protected float cost = default;
        [SerializeField] 
        protected ScriptObjVar<float> gaugeCurrent = default;
        [SerializeField] 
        private ScriptObjVar<bool> acquired = default;

        protected GameObject[] visualEffectInstances;

        protected override bool CanUse()
        {
            return (acquired != null && acquired || acquired == null) && (gaugeCurrent == null || gaugeCurrent >= cost);
        }

        protected override void OnAttackCompleted()
        {
            if (visualEffectInstances != null)
                for (var i = 0; i < visualEffectInstances.Length; i++)
                    Destroy(visualEffectInstances[i], vfxDestroyTimeAfterAttack);

            base.OnAttackCompleted();
        }

        protected override void Attack()
        {
            SpawnVFX();
            if (gaugeCurrent != null) gaugeCurrent.SetValueNotify(gaugeCurrent.value - cost);
        }

        protected virtual void SpawnVFX()
        {
            visualEffectInstances = new GameObject[visualEffectPrefabs.Length];
            for (var i = 0; i < visualEffectPrefabs.Length; i++)
            {
                var transform = AttackColliderCollisionDetection.transform;
                visualEffectInstances[i] = Instantiate(
                    visualEffectPrefabs[i],
                    transform.position,
                    transform.parent.rotation
                );
                visualEffectInstances[i].SetActive(true);
            }
        }

        public void SetCost(float cost)
        {
            this.cost = cost;
        }
    }
}