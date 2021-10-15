// Primary Author : Viktor Dahlberg - vida6631

using Entity.HealthSystem;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

namespace Environment.Pickup
{
    public class SoulEssence : MonoBehaviour
    {
        
        [Header("Meta")]
        
        [SerializeField]  
        private Collider collision = default;
        [SerializeField]
        private Collider inner = default;
        [SerializeField]
        private float chaseSpeedIncrease = default;
        [SerializeField]
        private float yeetForce = default;
        [SerializeField]
        private PuffController VFX = default;
        
        [Header("Essence")]
        
        [SerializeField]  private float essenceAmount = default;
        [SerializeField]
        private ScriptObjVar<float> gaugeMax = default;
        [SerializeField]
        private ScriptObjVar<float> gaugeCurrent = default;
        [SerializeField]
        private float healAmount = default;
        
        private bool _pickedUp;
        private Rigidbody _rigidbody;
        private float _speed;
        private float _speedIncrease;
        private Transform _target;

        private void LateUpdate()
        {
            if (_target)
            {
                var offsetTargetPosition = _target.position + Vector3.up;
                transform.position = Vector3.MoveTowards(transform.position, offsetTargetPosition, (_speed += _speedIncrease += 0.01f) * Time.deltaTime);
                if (transform.position == offsetTargetPosition)
                {
                    Pickup();
                }
            }
        }

        private void OnEnable()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            _pickedUp = false;
            _speed = 0;
            _speedIncrease = chaseSpeedIncrease;
            Yeet();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerHealth>())
            {
                if (inner.bounds.Intersects(other.bounds))
                {
                    Pickup();
                    other.GetComponent<PlayerHealth>().Heal(healAmount);
                }
                else
                {
                    _target = other.transform;
                    _rigidbody.useGravity = false;
                    _rigidbody.isKinematic = true;
                    collision.enabled = false;
                }
            }
        }

        private void Pickup()
        {
            if (!_pickedUp)
            {
                _pickedUp = true;
                Add(essenceAmount);
                VFX.SetParentTarget(transform);
                VFX.transform.position = transform.position;
                VFX.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        ///     Adds force in a random direction on the northern hemisphere.
        /// </summary>
        private void Yeet()
        {
            var direction = Random.insideUnitSphere;
            direction.y = Mathf.Abs(direction.y);
            _rigidbody.AddForce(direction * yeetForce);
        }

        /// <summary>
        ///     Adds soul essence and clamps to 0,max.
        /// </summary>
        /// <param name="essenceAmount">The amount of soul essence to add.</param>
        private void Add(float essenceAmount)
        {
            gaugeCurrent.SetValueNotify(Mathf.Clamp(gaugeCurrent + essenceAmount, 0f, gaugeMax));
        }
    }
}