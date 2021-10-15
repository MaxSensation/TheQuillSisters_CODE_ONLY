// Primary Author : Maximiliam Rosén - maka4519

using System.Collections.Generic;
using System.Linq;
using Entity;
using Entity.Player;
using Framework;
using UnityEditor;
using UnityEngine;
using ERV = Framework.EntityRendererViewer;


namespace Combat
{
    [RequireComponent(typeof(RotateWithCamera))]
    public class AutoAimController : MonoBehaviour
    {
        [SerializeField]
        private bool isDebug = true;
        [SerializeField]
        private bool isCameraForward = default;
        [SerializeField]
        private AnimationHandler animationHandler = default;
        [SerializeField]
        private float radius = default;
        [SerializeField]
        [Range(0f, 1f)] private float strength = default;
        [SerializeField]
        private Vector3 offset = default;
        [SerializeField]
        private LayerMask entityLayer = default;
        [SerializeField] [Range(0.0f, 360.0f)]
        private float priorityAngle = default;

        private Camera _mainCamera;
        private RotateWithCamera _rotateWithCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
            _rotateWithCamera = GetComponent<RotateWithCamera>();
            animationHandler.OnAttackStarted += RotateToClosestEnemy;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!isDebug) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + offset, radius);
            var mainCamera = Camera.main;
            var forward = isCameraForward && mainCamera != null
                ? Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized
                : transform.forward;
            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.blue;
                Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(forward), radius, EventType.Repaint);
            }

            Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            var up = transform.up;
            forward = Quaternion.AngleAxis(-priorityAngle * 0.5f, up) * forward;
            Handles.DrawSolidArc(transform.position, up, forward, priorityAngle, radius);
        }
#endif

        private void RotateToClosestEnemy()
        {
            var maxDotProduct = CMath.Map(priorityAngle, 360, 0, -1, 1);
            var forward = isCameraForward && _mainCamera != null ? _mainCamera.transform.forward : transform.forward;
            var entitiesInArea = GetEntitiesInArea(maxDotProduct, forward);
            if (entitiesInArea.Count == 0)
            {
                return;
            }

            var closestEnemy = GetClosestEntity(entitiesInArea);
            if (closestEnemy == null)
            {
                return;
            }

            _rotateWithCamera.enabled = false;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(GetEntityDirection(closestEnemy), Vector3.up), strength);
            _rotateWithCamera.enabled = true;
        }

        private Vector3 GetEntityDirection(EntityBase entity)
        {
            var direction = (entity.transform.position - transform.position).normalized;
            direction.y = 0;
            return direction;
        }

        private EntityBase GetClosestEntity(IReadOnlyList<EntityBase> entitiesInArea)
        {
            return entitiesInArea[
                CMath.ClosestPoint(entitiesInArea.Select(entity => entity.transform.position).ToList(),
                    transform.position)];
        }

        private List<EntityBase> GetEntitiesInArea(float maxDotProduct, Vector3 forward)
        {
            var thisPos = transform.position;
            return Physics.OverlapSphere(thisPos + offset, radius, entityLayer)
                .Select(entity => entity.GetComponent<EntityBase>() == null ? entity.transform.root.GetComponent<EntityBase>() : entity.GetComponent<EntityBase>())
                .Where(e => ERV.Entities.Contains(e) &&
                            Vector3.Dot(new Vector3Wrapper(e.transform.position - thisPos, Axis.Y, 0), forward) >=
                            maxDotProduct)
                .ToList();
        }
    }
}