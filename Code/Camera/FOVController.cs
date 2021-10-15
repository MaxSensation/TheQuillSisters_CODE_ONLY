// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Camera
{
    /// <summary>
    /// Controls camera FOV.
    /// </summary>
    public class FOVController : MonoBehaviour
    {
        [SerializeField] 
        private UnityEngine.Camera cam = default;
        [SerializeField] 
        private float stepSize = default;
        [SerializeField] 
        private SerializableTuple<float, float> fovRange = default;

        private float _currentFOV;
        private bool _flashing;
        private Coroutine _beginFlash;
        private Coroutine _endFlash;
        private const float Threshold = 0.005f;

        private void Update()
        {
            if (!_flashing)
            {
                _currentFOV = UpdateFOV(Mouse.current.scroll.y.ReadValue());
            }
        }

        /// <summary>
        /// Updates field of view based on mouse scrolling, clamped to fovRange.
        /// </summary>
        /// <param name="y">Scroll delta y</param>
        private float UpdateFOV(float y)
        {
            return cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - y * stepSize, fovRange.item1, fovRange.item2);
        }

        /// <summary>
        /// Performs a dynamic FOV change for juicing purposes.
        /// </summary>
        /// <param name="intensity">FOV change magnitude.</param>
        /// <param name="duration">How long the FOV should stay changed.</param>
        /// <param name="transitionSpeed">How quickly FOV should be changed.</param>
        public void FOVFlash(float intensity, float duration, float transitionSpeed)
        {
            _flashing = true;
            if (_beginFlash != null)
            {
                StopCoroutine(_beginFlash);
            }

            if (_endFlash != null)
            {
                StopCoroutine(_endFlash);
            }

            _beginFlash = StartCoroutine(BeginFlash(intensity, duration, transitionSpeed, _currentFOV));
        }

        private IEnumerator BeginFlash(float intensity, float duration, float transitionSpeed, float initialFOV)
        {
            var targetFOV = initialFOV * intensity;
            while (cam.fieldOfView < targetFOV - Threshold)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, transitionSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(duration);
            _endFlash = StartCoroutine(EndFlash(initialFOV, transitionSpeed));
        }

        private IEnumerator EndFlash(float initialFOV, float transitionSpeed)
        {
            do
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, initialFOV, transitionSpeed * Time.deltaTime);
                yield return null;
            } while (cam.fieldOfView > initialFOV + Threshold);

            _flashing = false;
        }
    }
}