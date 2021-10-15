// Primary Author : Viktor Dahlberg - vida6631

using Framework.SceneSystem;
using Framework.ScriptableObjectEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
	/// <summary>
	///     Toggles META scene between "menu mode" and "game mode".
	/// </summary>
	public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject mainMenu = default;
        [SerializeField]
        private GameObject[] inGameItemsToToggle = default;
        [SerializeField]
        private GameEvent scenePreloaderLoaded = default;
        [SerializeField]
        private PlayerInput playerInput = default;
        [SerializeField]
        private GameObject playerMesh = default;
        [SerializeField]
        private GameObject subtitleManager = default;

        private void Start()
        {
            playerInput.DeactivateInput();
            playerMesh.SetActive(false);
            ScenePreLoader.UnloadedActive += GoToMainMenu;
            scenePreloaderLoaded.OnEvent += GoToGame;
            LoadingScreen.CinematicLoadReady += GoToCinematic;
        }

        /// <summary>
        ///     Toggles "menu mode".
        /// </summary>
        private void GoToMainMenu()
        {
            foreach (var go in inGameItemsToToggle)
            {
                go.SetActive(false);
            }

            mainMenu.SetActive(true);
            playerInput.DeactivateInput();
            subtitleManager.SetActive(false);
        }

        /// <summary>
        ///     Toggles "game mode".
        /// </summary>
        private void GoToGame()
        {
            foreach (var go in inGameItemsToToggle)
            {
                go.SetActive(true);
            }

            mainMenu.SetActive(false);
            playerInput.ActivateInput();
            subtitleManager.SetActive(true);
        }

        /// <summary>
        ///     Toggles "cinematic mode".
        /// </summary>
        private void GoToCinematic()
        {
            foreach (var go in inGameItemsToToggle)
                go.SetActive(false);

            mainMenu.SetActive(false);
            subtitleManager.SetActive(true);
        }
    }
}