// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using UnityEngine;

namespace VFX
{
    public class LightController : MonoBehaviour
    {
        [SerializeField]
        private Light targetLight = default;
        [SerializeField]
        private float fadeSpeed = default;

        private float _origin;

        private void Awake()
        {
            _origin = targetLight.intensity;
        }

        private void OnEnable()
        {
            targetLight.intensity = 0f;
            FadeIn();
        }

        public void FadeIn()
        {
            StartCoroutine(PerformFadeIn());
        }

        public void FadeOut()
        {
            StartCoroutine(PerformFadeOut());
        }

        private IEnumerator PerformFadeIn()
        {
            do
            {
                targetLight.intensity = Mathf.Lerp(targetLight.intensity, _origin, fadeSpeed * Time.deltaTime);
                yield return null;
            } while (targetLight.intensity < _origin - 0.05f);
        }

        private IEnumerator PerformFadeOut()
        {
            do
            {
                targetLight.intensity = Mathf.Lerp(targetLight.intensity, 0, fadeSpeed * Time.deltaTime);
                yield return null;
            } while (targetLight.intensity > 0 + 0.05f);

            gameObject.SetActive(false);
        }
    }
}