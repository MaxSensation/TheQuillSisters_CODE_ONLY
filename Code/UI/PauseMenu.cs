// Primary Author : Andreas Berzelius - anbe5918

using System.Collections;
using Framework.SceneSystem;
using Framework.ScriptableObjectVariables;
using Framework.ScriptableObjectVariables.Custom;
using Scripts.Camera;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    ///     a script for handling the pauseMenu
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Tooltip("playerInput component on player")] [SerializeField] 
        private PlayerInput playerInput = default;
        [SerializeField]
        private GameObject playerUI = default;
        [SerializeField]
        private Button resumeButton = default;
        [SerializeField]
        private ScriptObjRef<ScriptObjBoolGroup> sceneCaptureReady = default;

        private bool _active;
        private readonly InputAction.CallbackContext _emptyAction = default;
        private bool _paused;
        private GameObject _pauseMenu;
        private ControllerInput _uiControls;

        private void Start()
        {
            _pauseMenu = transform.GetChild(0).gameObject;
            _uiControls = new ControllerInput();
            _uiControls.Enable();
            _uiControls.UI.Pause.started += ShowOrDisableSettings;
        }

        private void OnEnable()
        {
            _active = true;
        }

        private void OnDisable()
        {
            _active = false;
        }

        /// <summary>
        ///     Disable or enable Menu if it's active. takes in ctx to work with ControllerInput.
        /// </summary>
        /// <param name="ctx"></param>
        private void ShowOrDisableSettings(InputAction.CallbackContext ctx)
        {
            if (!_active)
            {
                return;
            }

            if (!_paused)
            {
                _pauseMenu.SetActive(_paused = true);
                resumeButton.Select();
                playerUI.SetActive(false);
                playerInput.DeactivateInput();
                CursorManager.ToggleAutomatic(false);
                CursorManager.Unlock();
            }
            else if (_paused)
            {
                _pauseMenu.SetActive(_paused = false);
                playerUI.SetActive(true);
                CursorManager.ToggleAutomatic(true);
                CursorManager.Lock();
                playerInput.ActivateInput();
            }
        }

        //just for resume button
        public void OnClick()
        {
            ShowOrDisableSettings(_emptyAction);
        }

        public void ExitToMainMenu()
        {
            StartCoroutine(LoadMainMenu());
        }

        private IEnumerator LoadMainMenu()
        {
            while (sceneCaptureReady.Value != true)
            {
                yield return null;
            }

            playerInput.ActivateInput();
            CursorManager.ToggleAutomatic(false);
            CursorManager.Unlock();
            _pauseMenu.SetActive(_paused = false);
            ScenePreLoader.UnloadActive?.Invoke();
        }
    }
}