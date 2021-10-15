// Primary Author : Viktor Dahlberg - vida6631

using UnityEditor;
using UnityEngine;

namespace Scripts.Camera
{
    /// <summary>
    ///     A class that builds a path of guide points that the camera can use to direct the player.
    /// </summary>
    public class Guideline : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask = default;
        [SerializeField]
        private float vertexRadius = default;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, vertexRadius);

            if (transform.childCount <= 0)
            {
                return;
            }

            Gizmos.DrawLine(transform.position, transform.GetChild(0).position);
            Gizmos.DrawSphere(transform.GetChild(0).position, vertexRadius);

            for (var i = 0; i < transform.childCount - 1; i++)
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                Gizmos.DrawSphere(transform.GetChild(i + 1).position, vertexRadius);
            }
        }

        /// <summary>
        ///     Adds a guide point to the guideline at the position of the latest existing guideline.
        /// </summary>
        public void AddGuidePoint()
        {
            var childCount = transform.childCount;

            var point = new GameObject {name = "GuidePoint " + GetInstanceID()};
            point.transform.SetParent(transform);
            point.transform.localPosition = childCount > 0 ? transform.GetChild(childCount - 1).localPosition : Vector3.zero;
#if UNITY_EDITOR
            Selection.activeObject = point;
#endif
        }

        /// <summary>
        ///     Returns the transform of the furthest guidepoint that is visible from the target.
        /// </summary>
        public Transform FindFurthestGuidePoint(Vector3 origin)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Physics.Linecast(origin, transform.GetChild(i).position, out var hit, layerMask);
                if (hit.collider == null) return transform.GetChild(i);
            }

            return null;
        }
    }
}