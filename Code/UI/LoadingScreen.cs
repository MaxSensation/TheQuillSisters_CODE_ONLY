// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Threading.Tasks;
using Framework.SceneSystem;
using Framework.ScriptableObjectEvent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	/// <summary>
	///     Manages the loading screen and schedules value and scene loading around its state.
	/// </summary>
	public class LoadingScreen : MonoBehaviour
    {
        [Header("Events")]
        
        [SerializeField]
        private GameEventGroup gameLoadCompleted = default;
        [SerializeField]
        private GameEvent gameLoadReady = default;
        [SerializeField]
        private GameEvent defaultLoadRequested = default;

        [Header("Loading Screen")] 
        
        [SerializeField]
        private Image loadingImage = default;
        [SerializeField]
        private float alphaFadeDuration = default;
        [SerializeField]
        private bool showScreen = default;
        [SerializeField]
        private bool startShown = default;
        [SerializeField]
        private bool freezeTimeDuringLoad = default;


        public static Action<bool> GameLoadRequested;
        public static Action CinematicLoadReady;
        private bool _loadingCinematic;

        private void OnEnable()
        {
            GameLoadRequested += BeginLoadingScreen;
            gameLoadCompleted.OnEvent += EndLoadingScreen;
            defaultLoadRequested.OnEvent += TriggerDefaultLoad;
            CinematicLoader.CinematicLoaded += ForceEndLoadingScreen;
            SetVisible(startShown, 0f);
            loadingImage.enabled = showScreen;
        }

        private void OnDisable()
        {
            GameLoadRequested -= BeginLoadingScreen;
            gameLoadCompleted.OnEvent -= EndLoadingScreen;
        }

        /// <summary>
        ///     UI OnClick function to load default save.
        /// </summary>
        public void TriggerDefaultLoad()
        {
            GameLoadRequested?.Invoke(true);
        }

        /// <summary>
        ///     UI OnClick function to load player save.
        /// </summary>
        public void TriggerNormalLoad()
        {
            GameLoadRequested?.Invoke(false);
        }

        /// <summary>
        ///     UI OnClick function to load cinematic.
        /// </summary>
        public void TriggerCinematic()
        {
            _loadingCinematic = true;
            BeginLoadingScreen(false);
        }

        /// <summary>
        ///     Toggles visibility of loading screen, transitioning over the specified duration.
        /// </summary>
        /// <param name="visible">Whether or not the loading screen should end up visible.</param>
        /// <param name="fadeDuration">Duration over which to fade the loading screen alpha.</param>
        public void SetVisible(bool visible, float fadeDuration)
        {
            loadingImage.CrossFadeAlpha(visible ? 1f : 0f, fadeDuration, true);
        }

        /// <summary>
        ///     Fades in the loading screen and sets timescale to 0.
        /// </summary>
        private async void BeginLoadingScreen(bool isDefault)
        {
            if (FadeInReady())
            {
                SetVisible(true, alphaFadeDuration);
                await Task.Delay(TimeSpan.FromSeconds(alphaFadeDuration));
                if (_loadingCinematic)
                {
                    CinematicLoadReady?.Invoke();
                    _loadingCinematic = false;
                }
                else
                {
                    gameLoadReady.Raise();
                }
            }
            else
            {
                if (_loadingCinematic)
                {
                    CinematicLoadReady?.Invoke();
                    _loadingCinematic = false;
                }
                else
                {
                    gameLoadReady.Raise();
                }
            }

            if (freezeTimeDuringLoad && !_loadingCinematic)
            {
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        ///     Fades out the loading screen and sets timescale to 1.
        /// </summary>
        private void EndLoadingScreen()
        {
            Debug.Log("Game Loaded");
            if (FadeOutReady())
            {
                SetVisible(false, alphaFadeDuration);
            }

            if (freezeTimeDuringLoad)
            {
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        ///     Ends loading screen instantly (basically).
        /// </summary>
        private void ForceEndLoadingScreen()
        {
            SetVisible(false, 0.01f);
        }

        private bool FadeInReady()
        {
            return showScreen || loadingImage.color.a == 1f;
        }

        private bool FadeOutReady()
        {
            return showScreen && loadingImage.color.a == 1f;
        }
    }
}