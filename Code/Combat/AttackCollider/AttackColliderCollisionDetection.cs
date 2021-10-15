//Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Interfaces;
using UnityEngine;

namespace Combat.AttackCollider
{
    /// <summary>
    ///     A Class to get the list of effected enemies
    /// </summary>
    public class AttackColliderCollisionDetection : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask = default;

        private BoxCollider _boxCollider;
        private AttackColliderType _currentAttackCollider;
        private Vector3 _currentAttackOffset;
        private Vector2 _currentAttackSize;
        private float _currentRange;
        private GameObject _owner;
        private SphereCollider _sphereCollider;
        public Action OnAttackHit;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
            _owner = transform.parent.gameObject;
        }

        public void SetCollider(AttackColliderType attackCollider, float range, Vector3 attackOffset,
            Vector2 attackSize)
        {
            _currentAttackCollider = attackCollider;
            _currentRange = range;
            _currentAttackSize = attackSize;
            _currentAttackOffset = attackOffset;
            CalculateRange();
        }

        public List<IDamageable> GetDamageables(bool pierceWalls = false)
        {
            var damageables = new List<IDamageable>();
            CalculateRange();
            var collidersInHitZone = GetCollidersInHitZone();

            var alreadyUsed = new List<GameObject>();
            foreach (var t in collidersInHitZone)
            {
                if (t.gameObject == _owner || t.gameObject == gameObject)
                {
                    continue;
                }

                var useRoot = false;
                var damageable = t.gameObject.GetComponent<IDamageable>();
                if (damageable == null)
                {
                    damageable = t.transform.root.gameObject.GetComponent<IDamageable>();
                    if (damageable == null)
                    {
                        continue;
                    }
                    useRoot = true;
                }

                if (alreadyUsed.Contains(useRoot ? t.transform.root.gameObject : t.gameObject))
                {
                    continue;
                }
                alreadyUsed.Add(useRoot ? t.transform.root.gameObject : t.gameObject);

                var casterPosition = transform.position + _currentAttackOffset;
                var position = (useRoot ? t.transform.root.position : t.transform.position) + Vector3.up;
                var distance = Vector3.Distance(casterPosition, position);
                var direction = position - casterPosition;
                var hits = Physics.RaycastAll(casterPosition, direction, distance, layerMask);
                var hasWall = hits.Any(hit => !hit.collider.CompareTag("Damagable"));
                if (!hasWall || pierceWalls)
                {
                    damageables.Add(damageable);
                }
            }

            if (damageables.Count > 0)
            {
                OnAttackHit?.Invoke();
            }
            return damageables;
        }

        private void CalculateRange()
        {
            switch (_currentAttackCollider)
            {
                case AttackColliderType.Sphere:
                    _sphereCollider.radius = _currentRange / 2;
                    _sphereCollider.center = _currentAttackOffset;
                    break;
                case AttackColliderType.Cube:
                    _boxCollider.size = new Vector3(_currentAttackSize.x, _currentAttackSize.y, _currentRange);
                    _boxCollider.center =
                        new Vector3(_currentAttackOffset.x, _currentAttackOffset.y, _currentRange / 2);
                    break;
            }
        }

        private Collider[] GetCollidersInHitZone()
        {
            switch (_currentAttackCollider)
            {
                case AttackColliderType.Sphere:
                    return Physics.OverlapSphere(transform.TransformPoint(_sphereCollider.center),
                        _sphereCollider.radius, layerMask);
                case AttackColliderType.Cube:
                    return Physics.OverlapBox(transform.TransformPoint(_boxCollider.center), _boxCollider.size / 2,
                        _boxCollider.transform.rotation, layerMask);
            }

            return new Collider[0];
        }
    }
}