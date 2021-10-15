// Primary Author : Erik Pilström - erpi3245

using System;
using Entity.HealthSystem;
using Framework.AudioSystem;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Pickup
{
    public class HealthOrb : Pickup
    {
        [SerializeField] 
        private ScriptObjRef<float> healPercentage = default;
        
        public Action OnPickup;
        private PlayerHealth _collidedPlayer;
        private bool _hasAudioManager;

        private void Start()
        {
            if (transform.parent.GetComponent<HealthOrbAudioManager>())
            {
                _hasAudioManager = true;
            }
        }

        private void OnEnable()
        {
            var container = transform.parent.gameObject;
            if (Physics.Raycast(container.transform.position, Vector3.down, out var hit, 0.5f))
            {
                container.transform.position = new Vector3(transform.position.x, transform.position.y + (0.5f - hit.distance), transform.position.z);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<PlayerHealth>())
            {
                _collidedPlayer = other.gameObject.GetComponent<PlayerHealth>(); //store as var to access correct gameobject in PickedUp()
                PickedUp();
            }
        }

        public virtual void PickedUp()
        {
            var playerMaxHealth = _collidedPlayer.GetMaxHealth();
            var playerCurrentHealth = _collidedPlayer.GetCurrentHealth();
            if (playerCurrentHealth < playerMaxHealth) //if the player has maximum health, we don't pick up the health orb
            {
                _collidedPlayer.Heal(playerMaxHealth * (healPercentage.Value / 100));
                OnPickup?.Invoke();
                if (!_hasAudioManager)
                {
                    Destroy(gameObject); //destroy itself after healing the player	
                }
            }
        }
    }
}