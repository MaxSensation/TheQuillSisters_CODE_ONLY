// Primary Author : Maximiliam Rosén - maka4519

using System;
using Entity.Player;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Environment.Trigger
{
	/// <summary>
	///     A Simple trigger.
	/// </summary>
	internal class AreaTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool isOpenTrigger = default;
        [SerializeField]
        private ScriptObjVar<bool> eyeOfHorus = default;
        [SerializeField]
        private bool invertState = default;
        
        private bool _isUsed;
        public Action<bool> OnTrigger;

        private void Awake()
        {
            SetState();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isUsed && other.GetComponent<PlayerController>() != null)
            {
                OnTrigger?.Invoke(isOpenTrigger);
                _isUsed = true;
            }
        }

        private void SetState()
        {
            if (eyeOfHorus != null)
            {
                if (invertState)
                {
                    _isUsed = eyeOfHorus;
                }
                else
                {
                    if (eyeOfHorus)
                    {
                        _isUsed = !eyeOfHorus;
                    }
                    else
                    {
                        _isUsed = false;
                    }
                }
            }
            else
            {
                _isUsed = false;
            }
        }
    }
}