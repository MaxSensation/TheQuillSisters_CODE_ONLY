// Primary Author : Maximiliam Rosén - maka4519
// Secondary Author : Viktor Dahlberg - vida6631

using System;
using System.Threading;
using System.Threading.Tasks;
using Combat.Interfaces;
using Framework;
using UnityEngine;

namespace Combat.ConditionSystem.Condition
{
    [CreateAssetMenu(menuName = "Scriptable Object Attacks/Condition/MoveCondition")]
    public class MoveCondition : ConditionBase
    {
        private static CharacterController _positionSeeker;

        [SerializeField]
        private bool isProjected = default;
        [SerializeField]
        private bool autoInterrupt = default;
        [SerializeField]
        private float radiusMultiplier = 1f;
        [SerializeField]
        private GameObject positionSeekerGO = default;
        [SerializeField]
        private float moveDistance = default;
        [SerializeField]
        private AnimationCurve speedCurve = default;

        private CancellationTokenSource _cancel;

        public Vector3 MoveDirection { get; set; }
        public float Height { get; set; }
        public float Radius { get; set; }

        public override void Modify(EntityBase applyingEntity, EntityBase affectedEntity)
        {
            if (Application.isPlaying && _positionSeeker == null)
            {
                var go = Instantiate(positionSeekerGO);
                DontDestroyOnLoad(go);
                _positionSeeker = go.GetComponent<CharacterController>();
                _positionSeeker.enableOverlapRecovery = false;
            }

            var movable = affectedEntity.GetComponent<IMovable>();
            if (movable != null)
            {
                movable.OnMoveStarted(this);
                SetCollider();
                ProgressiveMove(GetValidPosition(affectedEntity), movable, affectedEntity);
            }
        }

        private void SetCollider()
        {
            _positionSeeker.height = Height;
            _positionSeeker.radius = Radius * radiusMultiplier;
        }

        public override void UnModify(EntityBase entity)
        {
            entity.GetComponent<IMovable>().OnMoveCompleted(this);
        }

        private Vector3 GetValidPosition(EntityBase entity)
        {
            var groundChecker = entity.GetComponentInChildren<GroundChecker>();
            if (isProjected && groundChecker && groundChecker.IsGrounded)
            {
                MoveDirection = Vector3.ProjectOnPlane(MoveDirection, groundChecker.GroundNormal);
            }

            _positionSeeker.transform.position = entity.transform.position;
            _positionSeeker.enabled = true;
            _positionSeeker.Move(MoveDirection * moveDistance);
            _positionSeeker.enabled = false;
            return _positionSeeker.transform.position;
        }

        private async void ProgressiveMove(Vector3 targetPosition, IMovable movable, EntityBase affectedEntity)
        {
            var originalPosition = affectedEntity.transform.position;
            var moveDelta = Vector3.zero;
            var originalDelta = targetPosition - originalPosition;
            _cancel = new CancellationTokenSource();
            var x = Time.deltaTime;
            while ((targetPosition - affectedEntity.transform.position).magnitude > 0.05f &&
                   Vector3.Dot(originalDelta, targetPosition - affectedEntity.transform.position) > 0.99f &&
                   !_cancel.Token.IsCancellationRequested)
            {
                var oldPosition = affectedEntity.transform.position;
                var newPosition = Vector3.Lerp(oldPosition, targetPosition, speedCurve.Evaluate(x));
                var delta = Math.Abs(affectedEntity.transform.InverseTransformPoint(originalPosition).x -
                                     affectedEntity.transform.InverseTransformPoint(newPosition).x);
                var newMoveDelta = newPosition - oldPosition;
                if (autoInterrupt && oldPosition == newPosition || delta > 0.2f ||
                    newMoveDelta.magnitude < moveDelta.magnitude * 0.7f) break;
                moveDelta = newMoveDelta;
                movable.MoveToPosition(moveDelta);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime), _cancel.Token);
                }
                catch (TaskCanceledException)
                {
                }

                x += Time.deltaTime;
            }

            ConditionManager.RemoveCondition(this, affectedEntity);
        }

        public override void CancelCondition(EntityBase entity)
        {
            _cancel?.Cancel();
        }
    }
}