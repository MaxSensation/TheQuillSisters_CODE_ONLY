// Primary Author : Viktor Dahlberg - vida6631

using System.Threading.Tasks;
using Framework;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Camera
{
    /// <summary>
    ///     A configurable camera that follows a target and considers player input.
    /// </summary>
    public class GameplayCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            Transform cameraTransform;
            (cameraTransform = transform).rotation = Quaternion.Lerp(NormalRotation(), PointRotation(), lookAtFactor);
            rotation.value = cameraTransform.rotation;
            if (currentOffset != targetOffset.Value.offset && !_hadToAdjust)
            {
                TransitionOffset();
            }

            //Transform local offset to world, adjust for collision, then transform the new offset back to local so
            //TransitionOffset() may slerp it back to the target offset
            var adjustedOffset = AdjustForCollision(transform.TransformDirection(currentOffset));
            transform.position = GetHeightOffsetPosition(target.position) + adjustedOffset;
            currentOffset = transform.InverseTransformDirection(adjustedOffset);
        }

        public void Rotate(InputAction.CallbackContext context)
        {
            _inputRotation =
                onlyRotateWhenCursorIsLocked && Cursor.lockState == CursorLockMode.Locked ||
                !onlyRotateWhenCursorIsLocked
                    ? context.ReadValue<Vector2>()
                    : Vector2.zero;
        }

        /// <summary>
        ///     Sets whether camera should use KB&M or Gamepad sensitivity values.
        /// </summary>
        /// <param name="useGamepad">Whether to use KB&M or Gamepad sensitivity.</param>
        public void SetSensitivityMode(bool useGamepad)
        {
            _activeSensitivity = useGamepad ? gamepadSensitivity : mouseSensitivity;
        }

        #region Fields

        #region Serialized

        #region Accessing

        [Header("Accessing")] [SerializeField]
        private ScriptObjVar<Quaternion> rotation = default;
        [SerializeField]
        private ScriptObjVar<float> mouseSensitivity = default;
        [SerializeField]
        private ScriptObjVar<float> gamepadSensitivity = default;

        #endregion

        #region Targeting

        [Header("Targeting")] [SerializeField] 
        private Transform target = default;
        [SerializeField]
        private float targetHeightOffset = default;
        [SerializeField]
        private Vector3 currentOffset = default;
        [SerializeField]
        private ScriptObjRef<Offset> originOffset = default;
        [SerializeField]
        private ScriptObjRef<Offset> targetOffset = default;
        [SerializeField]
        private float defaultOffsetTransitionSpeed = default;

        #endregion

        #region Rotating

        [Header("Rotating")] [SerializeField]
        private bool onlyRotateWhenCursorIsLocked = default;
        [SerializeField]
        private bool invertX = default;
        [SerializeField]
        private bool invertY = default;
        [SerializeField]
        private SerializableTuple<float> xClamp = default;
        [Space] [SerializeField]
        private ScriptObjRef<Transform> lookAtTarget = default;
        [SerializeField]
        private Guideline guideline = default;
        [SerializeField]
        private bool lookAt = default;
        [SerializeField]
        private float lookAtSpeed = default;
        [SerializeField] [Range(0, 1f)]
        private float lookAtFactor = default;
        [SerializeField] [Range(0.95f, 1f)]
        private float alignmentDotTreshold = default;
        [SerializeField]
        private float resistanceMagnitudeTreshold = default;
        [SerializeField]
        private GameEvent forceRotationEvent = default;
        [SerializeField]
        private ScriptObjVar<Quaternion> forceTargetRotation = default;

        #endregion

        #region Colliding

        [Header("Colliding")] [SerializeField]
        private float cameraRadius;
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private bool useDefaultOffsetTransitionSpeedAfterCollision;

        #endregion

        #endregion

        #region Internal
        
        private float _activeSensitivity;
        private float _resistanceMagnitude;
        private bool _hadToAdjust;
        private bool _lookOnceActive;
        private Vector2 _inputRotation;
        private TaskCompletionSource<bool> _lookResult;

        #endregion

        #endregion

        #region Initialization

        private void Awake()
        {
            lookAtTarget.variableValue.value = null;
            _activeSensitivity = mouseSensitivity;
        }

        private void Start()
        {
            forceRotationEvent.OnEvent += () => transform.rotation = forceTargetRotation;
            currentOffset = originOffset.Value.offset;
            targetOffset.variableValue.value.offset = originOffset.Value.offset;
            gamepadSensitivity.ValueChanged += () => SetSensitivityMode(true);
            mouseSensitivity.ValueChanged += () => SetSensitivityMode(false);
        }

        #endregion

        #region Look At

        public void LookAtOnce(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                PerformLookOnce(guideline.FindFurthestGuidePoint(GetHeightOffsetPosition(target.position)));
        }

        /// <summary>
        ///     Causes camera to look at target until directions are aligned, then gives control back to player.
        /// </summary>
        /// <param name="target">Transform to look at.</param>
        private async void PerformLookOnce(Transform target)
        {
            if (target != null)
            {
                lookAtTarget.Value = target;
                _lookOnceActive = true;
                _resistanceMagnitude = 0;
                _lookResult = new TaskCompletionSource<bool>();
                await _lookResult.Task;
                _lookResult = null;
                _lookOnceActive = false;
            }
        }

        #endregion

        #region Rotation

        /// <summary>
        ///     Considers current player input, applies sensitivity factor and clamps the resulting rotation.
        /// </summary>
        /// <returns>The adjusted rotation.</returns>
        private Quaternion NormalRotation()
        {
            var adjustedRotation = _inputRotation * _activeSensitivity;
            if (_lookResult != null)
            {
                _resistanceMagnitude += adjustedRotation.magnitude;
                if (_resistanceMagnitude > resistanceMagnitudeTreshold)
                { 
                    _lookResult.SetResult(false);
                }
            }

            var xRotation = transform.eulerAngles.x + (invertX ? adjustedRotation.y : -adjustedRotation.y);
            //convert overflow into negative degrees i.e. 350 = -10, then clamp between -value and value
            xRotation = Mathf.Clamp(xRotation > 180 ? xRotation - 360 : xRotation, xClamp.item1, xClamp.item2);
            var wantedRotation = Quaternion.Euler(
                xRotation, 
                transform.eulerAngles.y + (invertY ? adjustedRotation.x : -adjustedRotation.x), 
                0);
            return wantedRotation;
        }

        /// <summary>
        ///     Returns a rotation toward the look at target, if it's assigned and looking at things is enabled.
        /// </summary>
        /// <returns>A rotation toward the look at target or a new Quaternion.</returns>
        private Quaternion PointRotation()
        {
            if (lookAtTarget != null && (lookAt || _lookOnceActive))
            {
                var position = transform.position;
                var direction = (lookAtTarget.Value.position - position).normalized;
                var goalRotation = Quaternion.LookRotation(direction, Vector3.up);
                var lookAtRotation = Quaternion.RotateTowards(transform.rotation, goalRotation, lookAtSpeed);

                if (!(Mathf.Abs(Quaternion.Dot(lookAtRotation, goalRotation)) >= alignmentDotTreshold))
                {
                    return lookAtRotation;
                }

                if (_lookResult != null)
                {
                    _lookResult.SetResult(true);
                }

                return new Quaternion();
            }

            return new Quaternion();
        }

        #endregion

        #region Offsetting

        /// <summary>
        ///     Slerps the current offset to the target offset at a speed specified by the target offset,
        ///     or at the default speed if target offset speed is less than or equal to zero.
        /// </summary>
        private void TransitionOffset()
        {
            var speed = targetOffset.Value.transitionSpeed <= 0
                ? defaultOffsetTransitionSpeed
                : targetOffset.Value.transitionSpeed;
            currentOffset = Vector3.Slerp(currentOffset, targetOffset.Value.offset, speed);
        }

        /// <summary>
        ///     Calculates a closer offset based on whether or not the cameras view of the target is blocked.
        /// </summary>
        /// <param name="offset">Offset to adjust.</param>
        /// <returns>The adjusted offset.</returns>
        private Vector3 AdjustForCollision(Vector3 offset)
        {
            if (Physics.SphereCast(
                GetHeightOffsetPosition(target.position),
                cameraRadius,
                offset.normalized,
                out var hit,
                offset.magnitude + cameraRadius,
                layerMask))
            {
                _hadToAdjust = true;
                if (hit.distance >= cameraRadius)
                {
                    if (useDefaultOffsetTransitionSpeedAfterCollision)
                    {
                        targetOffset.variableValue.value.transitionSpeed = defaultOffsetTransitionSpeed;
                    }

                    return offset.normalized * (hit.distance - cameraRadius);
                }

                return Vector3.zero;
            }

            _hadToAdjust = false;
            return offset;
        }

        /// <summary>
        ///     Adds Y offset to Vector3, to be used if pivot is at feet level.
        /// </summary>
        /// <param name="position">Vector3 to offset.</param>
        /// <returns>Offset position.</returns>
        private Vector3 GetHeightOffsetPosition(Vector3 position)
        {
            return new Vector3(position.x, position.y + targetHeightOffset, position.z);
        }

        #endregion
    }
}