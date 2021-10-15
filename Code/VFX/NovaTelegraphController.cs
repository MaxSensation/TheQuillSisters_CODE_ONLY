// Primary Author : Viktor Dahlberg - vida6631

using System;
using UnityEngine;
using UnityEngine.VFX;

namespace VFX
{
	/// <summary>
	///     Manages NovaTelegraph VFX.
	/// </summary>
	public class NovaTelegraphController : MonoBehaviour
    {
        [SerializeField]
        private VisualEffect VFX = default;
        [SerializeField]
        private LightController leftCast = default;
        [SerializeField]
        private LightController rightCast = default;

        public Action Kill;

        public Transform LeftHand { get; set; }
        public Transform RightHand { get; set; }
        public Vector3 Target { get; set; }
        public float SpawnRate { get; set; }

        private void Awake()
        {
            Kill += FadeOut;
        }

        private void Update()
        {
            if (LeftHand == null || RightHand == null)
            {
                return;
            }

            VFX.SetVector3("Target", Target);
            VFX.SetFloat("SpawnRate", SpawnRate);
            var leftPosition = LeftHand.position;
            var rightPosition = RightHand.position;
            VFX.SetVector3("StartPos1", leftPosition);
            VFX.SetVector3("StartPos2", rightPosition);
            leftCast.transform.position = transform.InverseTransformPoint(leftPosition);
            rightCast.transform.position = transform.InverseTransformPoint(rightPosition);
        }

        private void FadeOut()
        {
            leftCast.FadeOut();
            rightCast.FadeOut();
        }
    }
}