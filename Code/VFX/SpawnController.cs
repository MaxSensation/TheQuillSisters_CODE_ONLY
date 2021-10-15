// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace VFX
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer = default;
        [SerializeField]
        private float scrollSpeed = default;
        [SerializeField]
        private DecalProjector decalProjector = default;
        [SerializeField]
        private Vector3 initialDecalScale = default;

        private const float MOffsetInvisible = 0.4f;
        private const float MOffsetPassed = -1f;
        private const float MOffsetObscured = -0.2f;
        private static readonly int MOffset = Shader.PropertyToID("MOffset");
        
        public Renderer ToEnable { get; set; }

        private void Update()
        {
            transform.position = ToEnable.transform.position;
        }

        private void OnEnable()
        {
            meshRenderer.material.SetFloat(MOffset, MOffsetInvisible);
            StartCoroutine(Scroll());
        }

        public void SetOtherScale()
        {
            decalProjector.size = new Vector3(transform.localScale.x * initialDecalScale.x, transform.localScale.z * initialDecalScale.y, initialDecalScale.z);
        }

        private IEnumerator Scroll()
        {
            float mo;
            do
            {
                meshRenderer.material.SetFloat(MOffset,
                    Mathf.MoveTowards(
                        mo = meshRenderer.material.GetFloat(MOffset),
                        MOffsetPassed,
                        scrollSpeed * Time.deltaTime));
                if (mo < MOffsetObscured)
                {
                    ToEnable.enabled = true;
                }

                yield return null;
            } while (mo > MOffsetPassed + 0.05f);

            gameObject.SetActive(false);
        }
    }
}