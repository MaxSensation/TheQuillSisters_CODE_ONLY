// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Environment.Trigger;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.SceneSystem
{
	/// <summary>
	///     Load and unloads specified scenes
	///     secondsToUnload waits for x amounts of seconds before unloads is called
	///     areaTrigger is the trigger that will activate unload and load scenes
	///     unloadScenes is the scenes that will be unloaded
	///     loadScenes is the scenes that will be loaded
	/// </summary>
	public class SceneManager : MonoBehaviour
    {
        [SerializeField]
        private float secondsToUnload = 4f;
        [SerializeField]
        private AreaTrigger areaTrigger = default;
        [SerializeField]
        private List<string> unloadScenes = default;
        [SerializeField]
        private List<string> loadScenes = default;
        [SerializeField]
        private GameEvent sceneManagerDone = default;
        
        [Header("Bool Group")] 
        
        [SerializeField]
        private ScriptObjVar<bool> sceneManagerReady = default;

        private int _completedLoadCount;
        private bool _forced;

        /// <summary>
        ///     Check if trigger is assigned else throw execution.
        ///     If assigned subscribe to trigger events
        /// </summary>
        private void Start()
        {
            if (areaTrigger != null)
            {
                areaTrigger.OnTrigger += _ => UnloadLoad();
            }
        }

        public void SetForced()
        {
            _forced = true;
        }

        public void UnloadLoad()
        {
            sceneManagerReady.value = false;
            _completedLoadCount = 0;
            LoadScenes();
            UnloadSceneAfterDelay();
        }

        /// <summary>
        ///     Have a delay to prevent the player to see the scene disappear when the door is closing
        /// </summary>
        private async void UnloadSceneAfterDelay()
        {
            await Task.Delay(TimeSpan.FromSeconds(secondsToUnload));
            UnloadScenes();
        }

        /// <summary>
        ///     Unload all scenes specified in list
        /// </summary>
        private void UnloadScenes()
        {
            foreach (var scene in unloadScenes)
            {
                StartCoroutine(UnloadAsync(scene, loadScenes.Count + unloadScenes.Count));
            }
        }

        /// <summary>
        ///     Load all scenes specified in list
        /// </summary>
        private void LoadScenes()
        {
            foreach (var scene in loadScenes)
            {
                StartCoroutine(LoadAsync(scene, LoadSceneMode.Additive, loadScenes.Count + unloadScenes.Count));
            }

            if (_forced)
            {
                sceneManagerReady.value = true;
            }
        }

        private IEnumerator UnloadAsync(string scene, int target)
        {
            var load = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            while (!load.isDone)
            {
                yield return null;
            }

            _completedLoadCount++;
            if (_completedLoadCount >= target)
            {
                sceneManagerReady.value = true;
                if (sceneManagerDone != null)
                {
                    sceneManagerDone.Raise();
                }
            }
        }

        private IEnumerator LoadAsync(string scene, LoadSceneMode loadSceneMode, int target)
        {
            var load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene, loadSceneMode);
            while (!load.isDone)
            {
                yield return null;
            }

            _completedLoadCount++;
            if (_completedLoadCount >= target)
            {
                sceneManagerReady.value = true;
                if (sceneManagerDone != null)
                {
                    LightProbes.TetrahedralizeAsync();
                    sceneManagerDone.Raise();
                }
            }
        }
    }
}