// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace VFX
{
    public class DecalProjectorController : MonoBehaviour
    {
        [SerializeField]
        private DecalProjector decalProjector = default;
        [SerializeField]
        private float fadeSpeed = default;

        private void OnEnable()
        {
            decalProjector.fadeFactor = 0f;
            StopCoroutine(FadeIn());
            StopCoroutine(FadeOut());
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            do
            {
                decalProjector.fadeFactor =
                    Mathf.MoveTowards(decalProjector.fadeFactor, 1.05f, fadeSpeed * Time.deltaTime);
                yield return null;
            } while (decalProjector.fadeFactor < 0.995f);

            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            do
            {
                decalProjector.fadeFactor =
                    Mathf.MoveTowards(decalProjector.fadeFactor, 0f, fadeSpeed * Time.deltaTime);
                yield return null;
            } while (decalProjector.fadeFactor > 0.0005f);
        }
    }
}