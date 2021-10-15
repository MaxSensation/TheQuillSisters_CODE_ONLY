// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.SceneSystem
{
    public class CinematicLoader : MonoBehaviour
    {
        [SerializeField]
        private string cinematicSceneName;

        public static Action CinematicLoaded;

        private void Start()
        {
            LoadingScreen.CinematicLoadReady += BeginLoad;
        }

        private void BeginLoad()
        {
            StartCoroutine(LoadAsync(cinematicSceneName, LoadSceneMode.Additive));
        }

        private IEnumerator LoadAsync(string scene, LoadSceneMode loadSceneMode)
        {
            var load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, loadSceneMode);
            while (!load.isDone)
            {
                yield return null;
            }
            CinematicLoaded?.Invoke();
        }
    }
}