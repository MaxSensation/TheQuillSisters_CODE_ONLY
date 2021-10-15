// Primary Author : Viktor Dahlberg - vida6631
// Secondary Author : Andreas Berzelius - anbe5918 

using UnityEngine;
using UnityEngine.AI;

namespace Entity.AI.States.Behavior
{
    [CreateAssetMenu(menuName = "States/EnemyStates/BehaviorStates/Fleeing")]
    public class Fleeing : AIBaseState
    {
        [SerializeField]
        private float maxFleeDistance = default;
        [SerializeField]
        private float minDistanceBetween = default;

        private int _currentDepth;
        private Vector3 _fleePosition;

        public override void Enter()
        {
            _fleePosition = GetFleePosition();
            if (!AI.agent.isOnNavMesh)
            {
                return;
            }
            AI.agent.isStopped = false;
            AI.agent.SetDestination(_fleePosition);
        }

        public override void Run()
        {
            var distanceMagnitude = (AI.playerPosition.Value - AI.transform.position).magnitude;
            if ((_fleePosition - AI.transform.position).magnitude < 1.5f && distanceMagnitude < minDistanceBetween || AI.agent.velocity.magnitude < 0.01f)
            {
                Enter();
            }
            if (distanceMagnitude > minDistanceBetween)
            {
                StateMachine.TransitionTo<Hunting>();
            }
            base.Run();
        }

        public override void Exit()
        {
        }

        private Vector3 GetFleePosition()
        {
            var newPosition = RandomInCone(140f, ((AI.transform.position - AI.playerPosition.Value).normalized * maxFleeDistance) + AI.transform.position);
            if (NavMesh.SamplePosition(newPosition, out NavMeshHit hit1, maxFleeDistance, NavMesh.AllAreas))
            {
                return hit1.position;
            }
            NavMesh.SamplePosition(newPosition, out NavMeshHit hit2, maxFleeDistance, NavMesh.AllAreas);
            return hit2.position;
        }

        private Vector3 RandomInCone(float spreadAngle, Vector3 axis)
        {
            var radians = Mathf.PI / 180 * spreadAngle / 2;
            var dotProduct = Random.Range(Mathf.Cos(radians), 1);
            var polarAngle = Mathf.Acos(dotProduct);
            var azimuth = Random.Range(-Mathf.PI, Mathf.PI);
            var yProjection = Vector3.ProjectOnPlane(Vector3.up, axis);
            if (Vector3.Dot(axis, Vector3.up) > 0.9f)
            {
                yProjection = Vector3.ProjectOnPlane(Vector3.forward, axis);
            }
            var y = yProjection.normalized;
            var x = Vector3.Cross(y, axis);
            var pointOnSphericalCap = Mathf.Sin(polarAngle) * (Mathf.Cos(azimuth) * x + Mathf.Sin(azimuth) * y) + Mathf.Cos(polarAngle) * axis;
            return pointOnSphericalCap;
        }

    }
}