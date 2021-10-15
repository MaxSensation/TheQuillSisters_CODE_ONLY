// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace VFX
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed = default;
        [SerializeField]
        private Vector3 axis = default;

        private void Update()
        {
            transform.RotateAround(transform.position, axis, rotationSpeed * Time.deltaTime);
        }
    }
}