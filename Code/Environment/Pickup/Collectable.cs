// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Threading.Tasks;
using Entity.Player;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Pickup
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource = default;
        [SerializeField]
        private AudioClip audioClip = default;
        [SerializeField]
        private ScriptObjVar<bool> collectableCollected = default;
        [SerializeField]
        private GameObject pickupVFX = default;

        private void Awake()
        {
            if (transform.parent == null)
            {
                Debug.LogError($"The collectable {name} in scene {gameObject.scene} is missing a parent.");
            }
            else if (collectableCollected == null)
            {
                Debug.LogError($"The collectable {transform.parent.gameObject.name} in scene {transform.parent.gameObject.scene} is missing a ScriptObjBool.");
            }
            else
            {
                transform.parent.gameObject.SetActive(!collectableCollected);
            }
        }

        private async void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerController>())
            {
                return;
            }

            GetComponent<Collider>().enabled = false;
            GetComponentInParent<MeshRenderer>().enabled = false;
            collectableCollected.SetValueNotify(true);
            audioSource.PlayOneShot(audioClip);
            Instantiate(pickupVFX, transform.parent.position, Quaternion.identity).SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(audioClip.length));
            transform.parent.gameObject.SetActive(false);
        }
    }
}