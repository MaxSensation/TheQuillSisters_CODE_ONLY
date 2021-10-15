// Primary Author : Maximiliam Rosén - maka4519

using System;
using Entity.Player;
using UnityEngine;

namespace Environment.RoomManager
{
    [RequireComponent(typeof(BoxCollider))]
    public class RoomKeyTrigger : MonoBehaviour
    {
        public Action OnPickedUp;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>())
            {
                OnPickedUp?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}