// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using UnityEngine;

namespace VFX
{
    public class ShockWaveController : MonoBehaviour
    {
        [SerializeField]
        private float expansionSpeed = default;
        [SerializeField]
        private float maxScale = default;
        [SerializeField]
        private MeshRenderer meshRenderer = default;
        [SerializeField]
        private float fadeSpeed = default;

        private static readonly int AlphaMul = Shader.PropertyToID("AlphaMul");
        private Material _material;
        private Vector3 _origin;

        protected void OnEnable()
        {
            _origin = transform.localScale;
            StartCoroutine(PerformExpansion());
            StartCoroutine(PerformFade());
        }

        protected void OnEnd()
        {
            gameObject.SetActive(false);
        }

        protected void OnForceReset()
        {
            transform.localScale = _origin;
            meshRenderer.material.SetFloat(AlphaMul, 1f);
        }

        /// <summary>
        ///     Linearly interpolates GameObject scale to maxScale at expansionSpeed speed.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PerformExpansion()
        {
            do
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxScale,
                    expansionSpeed * Time.deltaTime);
                yield return null;
            } while (transform.localScale.magnitude < (Vector3.one * maxScale - Vector3.one * 0.05f).magnitude);
        }

        /// <summary>
        ///     Linearly interpolates material alpha to 1 at fadeSpeed speed.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PerformFade()
        {
            do
            {
                _material = new Material(meshRenderer.material);
                _material.SetFloat(AlphaMul, Mathf.Lerp(_material.GetFloat(AlphaMul), 0f, fadeSpeed * Time.deltaTime));
                meshRenderer.material = _material;
                yield return null;
            } while (_material.GetFloat(AlphaMul) > 0.00005f);

            gameObject.SetActive(false);
        }
    }
}