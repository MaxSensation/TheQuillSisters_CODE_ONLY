// Primary Author : Andeas Berzelius - anbe5918
// Secondary Author: Viktor Dahlberg - vida6631

using System.Collections;
using Entity.HealthSystem;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Entity.AI.Mummies.Ranged
{
    public class HomingProjectile : MonoBehaviour
    {
        [SerializeField]
        private ScriptObjRef<Vector3> playerPosition = default;
        [SerializeField]
        private float force = default;
        [SerializeField]
        private float rotationForce = default;
        [SerializeField]
        private float lifeTime = default;
        [SerializeField]
        private bool collideWithEnemies = default;

        private float _damage;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            StartCoroutine(ScaleDown());
        }

        private void FixedUpdate()
        {
            var direction = playerPosition.Value + Vector3.up - _rigidbody.position;
            if (direction.magnitude < 2.5f)
            {
                rotationForce = 0f;
            }
            direction.Normalize();
            var rotationAmount = Vector3.Cross(transform.forward, direction);
            _rigidbody.angularVelocity = rotationAmount * rotationForce;
            _rigidbody.velocity = transform.forward * force;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other)
            {
                return;
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.gameObject.GetComponent<Health>().TakeDamage(_damage);
            }

            if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                Destroy(gameObject);
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && collideWithEnemies)
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator ScaleDown()
        {
            float totalTime = 0;
            while (totalTime <= lifeTime)
            {
                totalTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        public void Init(float damage)
        {
            _damage = damage;
        }
    }
}