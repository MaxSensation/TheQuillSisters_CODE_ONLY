// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace VFX
{
    public class ControlWithCamera : MonoBehaviour
    {
        [SerializeField]
        private float xOffset = default;
        [SerializeField]
        private float minValue = default;
        [SerializeField]
        private float maxValue = default;
        [SerializeField]
        private Transform playerTransform = default;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var newRotation = _camera.transform.rotation * Quaternion.Euler(new Vector3(xOffset, 0, 0));
            var eulerRotation = newRotation.eulerAngles;
            var rotation = transform.rotation;
            rotation = Quaternion.Euler(ClampAngle(eulerRotation.x, minValue, maxValue), rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;
            var playerTransformEulerAngles = playerTransform.eulerAngles;
            playerTransform.rotation = Quaternion.Euler(playerTransformEulerAngles.x, _camera.transform.eulerAngles.y, playerTransformEulerAngles.z);
        }

        private void OnDisable()
        {
            transform.rotation = new Quaternion();
        }

        private static float ClampAngle(float angle, float from, float to)
        {
            if (angle < 0f)
            {
                angle = 360 + angle;
            }

            return angle > 180f ? Mathf.Max(angle, 360 + from) : Mathf.Min(angle, to);
        }
    }
}