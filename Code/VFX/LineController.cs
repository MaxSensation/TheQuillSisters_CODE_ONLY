// Primary Author : Viktor Dahlberg - vida6631

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace VFX
{
    public class LineController : MonoBehaviour
    {
        [Header("VFX")] 
        
        [SerializeField]
        private LineRenderer lineRenderer = default;
        [SerializeField]
        private VisualEffect VFX = default;
        
        [Header("Width")]
        
        [SerializeField]
        private float width = default;
        [SerializeField]
        private float widthScaleSpeed = default;
        [SerializeField]
        private float widthDecayWait = default;
        
        [Header("Distance")] 
        
        [SerializeField]
        private float distance = default;
        [SerializeField]
        private float distanceScaleSpeed = default;
        [SerializeField]
        private float distanceDecayWait = default;
        [SerializeField]
        private bool decayDistance = default;
        
        [Header("Collision")]
        
        [SerializeField]
        private bool pierceWalls = default;
        [SerializeField]
        private LayerMask layerMask = default;
        
        [Header("Lighting")]
        
        [SerializeField]
        private Light glow = default;
        [SerializeField]
        private HDAdditionalLightData hdAdditionalLightData = default;
        [SerializeField] [Tooltip("Needs to be equal to light component intensity.")]
        private float baseIntensity = default;

        private void Awake()
        {
            lineRenderer.widthMultiplier = 0f;
            lineRenderer.SetPosition(1, Vector3.zero);
            distance = !pierceWalls && Physics.Raycast(transform.position, transform.forward, out var hit, distance, layerMask)
                    ? Vector3.Distance(transform.position, hit.point) + width / 4f
                    : distance;
            if (VFX != null)
            {
                VFX.SetFloat("DistanceMultiplier", distance / 25f);
            }
            StartCoroutine(ScaleSizeUp());
            StartCoroutine(ScaleDistanceUp());
            Destroy(gameObject, 5f);
        }

        private void Update()
        {
            var z = lineRenderer.GetPosition(1).z;
            glow.transform.localPosition = new Vector3(0, 0, z);
            hdAdditionalLightData.SetAreaLightSize(new Vector2(z * 2, z * 2));
            hdAdditionalLightData.intensity = lineRenderer.widthMultiplier * baseIntensity;
        }

        private IEnumerator ScaleSizeUp()
        {
            while (lineRenderer.widthMultiplier < 0.95f)
            {
                lineRenderer.widthMultiplier =
                    Mathf.Lerp(lineRenderer.widthMultiplier, width, widthScaleSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(widthDecayWait);

            StartCoroutine(ScaleSizeDown());
        }

        private IEnumerator ScaleSizeDown()
        {
            while (lineRenderer.widthMultiplier > 0f)
            {
                lineRenderer.widthMultiplier =
                    Mathf.Lerp(lineRenderer.widthMultiplier, 0, widthScaleSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator ScaleDistanceUp()
        {
            while (lineRenderer.GetPosition(1).z < distance - 0.5f)
            {
                lineRenderer.SetPosition(1,
                    Vector3.MoveTowards(lineRenderer.GetPosition(1), Vector3.forward * distance,
                        distanceScaleSpeed * Time.deltaTime));
                yield return null;
            }

            yield return new WaitForSeconds(distanceDecayWait);

            if (decayDistance)
            {
                StartCoroutine(ScaleDistanceDown());
            }
        }

        private IEnumerator ScaleDistanceDown()
        {
            while (lineRenderer.GetPosition(0).z < distance)
            {
                lineRenderer.SetPosition(0,
                    Vector3.MoveTowards(lineRenderer.GetPosition(1), Vector3.forward * distance,
                        distanceScaleSpeed * Time.deltaTime));
                yield return null;
            }
        }
    }
}