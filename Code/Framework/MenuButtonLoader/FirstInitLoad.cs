// Primary Author : Maximiliam Rosén - maka4519

using System.Collections.Generic;
using System.Linq;
using Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.MenuButtonLoader
{
    public class FirstInitLoad : MonoBehaviour
    {
        [SerializeField] 
        private List<string> scenesToLoad = default;
        [SerializeField]
        private string sceneToMakeActive = default;

        private bool _isUsed;

        private void Start()
        {
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (sceneToMakeActive == scene.name)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToMakeActive));
                SceneManager.UnloadSceneAsync("MainMenu");
            }
        }

        public void OnClick()
        {
            CursorManager.ToggleAutomatic(false);
            CursorManager.Lock();
            if (!_isUsed)
            {
                _isUsed = true;
                foreach (var scene in scenesToLoad.Where(scene => !SceneManager.GetSceneByName(scene).isLoaded))
                {
                    SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                }
            }
        }
    }
}