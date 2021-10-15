//Author: Viktor Dahlberg
//Secondary Author: Andreas Berzelius

using System;
using Scripts.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Input
{
	/// <summary>
	/// Switches input schemes on a delay basis.
	/// </summary>
	public class SchemeSwitcher : MonoBehaviour
	{
		public static Action<int> switchedDevice;
		[SerializeField]
		private PlayerInput playerInput = default;
		[SerializeField]
		private string keyboardMouseSchemeName = default;
		[SerializeField]
		private string gamepadSchemeName = default;
		[SerializeField]
		private float switchDelay = default;
		[SerializeField]
		private GameplayCamera gameplayCamera = default;

		private float _currentDelay;

		private bool? _latestDevice;

		private void OnEnable()
		{
			//determine latest used input device
			InputSystem.onActionChange += SetLatestDevice;
		}

		private void OnDisable()
		{
			InputSystem.onActionChange -= SetLatestDevice;
		}

		private void LateUpdate()
		{
			_currentDelay = (_currentDelay -= Time.deltaTime) <= 0 && SwitchControlScheme() ? switchDelay : _currentDelay;
		}

		private void SetLatestDevice(object obj, InputActionChange change)
		{
			if (change == InputActionChange.ActionPerformed)
			{
				var inputAction = (InputAction) obj;
				var lastControl = inputAction.activeControl;
				var lastDevice = lastControl.device;
					
				//should swap to Keyboard&Mouse
				if ((lastDevice == Keyboard.current || lastDevice == Mouse.current) &&
				    !playerInput.currentControlScheme.Equals(keyboardMouseSchemeName))
				{
					_latestDevice = true;
				}
				//should swap to Gamepad
				else if (lastDevice == Gamepad.current &&
				         !playerInput.currentControlScheme.Equals(gamepadSchemeName))
				{
					_latestDevice = false;
				}
				//should not swap
				else
				{
					_latestDevice = null;
				}
			}
		}
		
		/// <summary>
		/// Switches control scheme if the latest actuated device is different from the one bound to the active input scheme.
		/// </summary>
		/// <returns>Whether the control scheme was switched.</returns>
		private bool SwitchControlScheme()
		{
			if (!_latestDevice.HasValue)
			{
				return false;
			}
			
			if (_latestDevice.Value)
			{
				playerInput.SwitchCurrentControlScheme(keyboardMouseSchemeName, Keyboard.current, Mouse.current);
				gameplayCamera.SetSensitivityMode(false);
				switchedDevice?.Invoke(1);
			}
			else if(Gamepad.current != null)
			{
				playerInput.SwitchCurrentControlScheme(gamepadSchemeName, Gamepad.current);
				gameplayCamera.SetSensitivityMode(true);
				switchedDevice?.Invoke(0);
			}

			return true;
		}
	}
}