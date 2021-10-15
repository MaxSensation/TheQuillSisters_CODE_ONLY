// Primary Author : Maximiliam Rosén - maka4519

using System;
using Combat.AttackCollider;
using Entity.HealthSystem;
using Framework.ScriptableObjectEvent;
using UnityEngine;
using UnityEngine.VFX;

namespace Entity.AI.Bosses.TwinofRa
{
    public class TwinOfRaLifeLine : EntityBase
    {
        [Header("StartInfo")] 
        
        [SerializeField]
        private bool isStartOrb = default;
        [SerializeField]
        private GameEvent startTrigger = default;
        [SerializeField]
        private Color startOrbColor = default;

        [Header("General")] 
        
        [SerializeField]
        private bool isExploding = true;
        [SerializeField]
        private int bossDamage = default;
        [SerializeField]
        private int damage = default;
        [SerializeField]
        private float damageRadius = default;
        [SerializeField]
        private GameObject vfx = default;
        [SerializeField]
        private AttackColliderCollisionDetection attackCollider = default;

        private static readonly int ColorMain = Shader.PropertyToID("Color_Main");
        private Health _health;
        private TwinOfRa _twinOfRa;

        private void Start()
        {
            if (!isStartOrb)
            {
                _health = GetComponent<Health>();
                Health.EntityDied += OnDied;
            }
            else
            {
                var renderers = GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    var newMaterial = new Material(renderer.material);
                    newMaterial.SetColor(ColorMain, startOrbColor * 100);
                    renderer.material = newMaterial;
                }
            }
        }

        private void OnDestroy()
        {
            Health.EntityDied -= OnDied;
        }

        private void OnDied(GameObject obj)
        {
            if (obj == gameObject)
            {
                _twinOfRa.LifeLineDestroyed(bossDamage);
                Destroy(gameObject);
            }
        }

        public void SetLink(TwinOfRa twinOfRa)
        {
            _twinOfRa = twinOfRa;
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var newMaterial = new Material(renderer.material);
                newMaterial.SetColor(ColorMain, twinOfRa.Color * 100);
                renderer.material = newMaterial;
            }
        }

        public override Tuple<float, float> GetDimensions()
        {
            return new Tuple<float, float>(0f, 0f);
        }

        public override void TakeDamage(float damage)
        {
            if (isStartOrb)
            {
                startTrigger.Raise();
                Destroy(gameObject);
            }
            else
            {
                _health.TakeDamage(damage);
            }
        }

        public void Explode()
        {
            if (isExploding)
            {
                var nova = Instantiate(vfx, transform.position, Quaternion.identity);
                var visual = nova.GetComponent<VisualEffect>();
                var gradient = visual.GetGradient("Color over Life");
                var colorKeys = gradient.colorKeys;
                colorKeys[0].color = _twinOfRa.Color * 100;
                gradient.SetKeys(colorKeys, gradient.alphaKeys);
                visual.SetGradient("Color over Life", gradient);
                nova.SetActive(true);
                DoDamage();
            }

            Destroy(gameObject);
        }

        private void DoDamage()
        {
            attackCollider.SetCollider(AttackColliderType.Sphere, damageRadius, Vector3.zero, Vector2.zero);
            var list = attackCollider.GetDamageables();
            foreach (var damageable in list)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}