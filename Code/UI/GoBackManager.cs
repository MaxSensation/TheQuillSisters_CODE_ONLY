// Primary Author : Andreas Berzelius - anbe5918

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    ///     Handle going back in menus.
    /// </summary>
    public class GoBackManager : MonoBehaviour
    {
        [SerializeField] [Tooltip("The menu Element to Disable")]
        private GameObject menuElement = default;

        [SerializeField] [Tooltip("if rebind overlay is ongoing don't go back, \nYou don't have to use this")]
        private GameObject rebindingOverlay = default;

        private Button _button;
        private ControllerInput _controller;
        private Dropdown _dropdown;
        private Slider _slider;

        private void OnEnable()
        {
            _controller = new ControllerInput();
            _controller.UI.Enable();
            if (TryGetComponent(out Button button))
            {
                _button = button;
            }

            if (TryGetComponent(out Slider slider))
            {
                _slider = slider;
            }

            if (TryGetComponent(out Dropdown dropDown))
            {
                _dropdown = dropDown;
            }

            _controller.UI.Cancel.started += GoBack;
            VoiceCommandManager.Back += VoiceCommandBack;
        }

        private void OnDisable()
        {
            _controller.UI.Cancel.started -= GoBack;
            _controller.UI.Disable();
        }

        private void VoiceCommandBack()
        {
            if (rebindingOverlay != null && rebindingOverlay.activeSelf)
            {
                return;
            }

            if (_button)
            {
                _button.navigation.selectOnLeft.Select();
            }
            else if (_slider)
            {
                _slider.navigation.selectOnUp.Select();
            }
            else if (_dropdown)
            {
                _dropdown.navigation.selectOnLeft.Select();
            }

            menuElement.SetActive(false);
        }

        private void GoBack(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
            {
                return;
            }

            if (rebindingOverlay != null && rebindingOverlay.activeSelf)
            {
                return;
            }

            if (_button)
            {
                _button.navigation.selectOnLeft.Select();
            }
            else if (_slider)
            {
                _slider.navigation.selectOnUp.Select();
            }
            else if (_dropdown)
            {
                _dropdown.navigation.selectOnLeft.Select();
            }

            menuElement.SetActive(false);
        }
    }
}