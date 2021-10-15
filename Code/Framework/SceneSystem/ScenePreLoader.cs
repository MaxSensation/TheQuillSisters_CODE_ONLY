// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using Framework.ScriptableObjectVariables.Custom;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.SceneSystem
{
	/// <summary>
	///     Keeps track of the active scenes and does the first scene load when it's time to load a save.
	/// </summary>
	public class ScenePreLoader : MonoBehaviour
    {
        [SerializeField]
        private GameEvent gameSaveRequested = default;
        [SerializeField]
        private GameEvent saveManagerDone = default;
        [SerializeField]
        private GameEvent scenePreLoaderSaved = default;
        [SerializeField]
        private GameEvent scenePreLoaderLoaded = default;
        [SerializeField]
        private ScriptObjVar<string[]> scenesToLoad = default;
        [SerializeField]
        private ScriptObjSet<string> scenesToIgnore = default;
        [Header("Bool Group")] [SerializeField]
        private ScriptObjVar<bool> scenePreLoaderReady = default;
        [SerializeField]
        private ScriptObjBoolGroup sceneCapturingReady = default;

        public static Action UnloadActive;
        public static Action UnloadedActive;

        private int _completedLoadCount;
        private TaskCompletionSource<bool> _unloadWait;

        private void OnEnable()
        {
            UnloadActive += OnUnloadActiveScenes;
            gameSaveRequested.OnEvent += OnSaveActiveScenes;
            saveManagerDone.OnEvent += OnLoadNewScenes;
        }

        private void OnDisable()
        {
            gameSaveRequested.OnEvent -= OnSaveActiveScenes;
            saveManagerDone.OnEvent -= OnLoadNewScenes;
        }

        /// <summary>
        ///     Stores the currently active scenes to a ScriptObjVar.
        /// </summary>
        public async void OnSaveActiveScenes()
        {
            while (!sceneCapturingReady.IsAllTrue())
            {
                await Task.Delay(10);
            }

            scenePreLoaderReady.value = false;
            scenesToLoad.value = GetActiveScenes().ToArray();
            scenePreLoaderReady.value = true;
            scenePreLoaderSaved.Raise();
        }

        private void OnUnloadActiveScenes()
        {
            LoadScenes(true);
        }

        private void OnLoadNewScenes()
        {
            LoadScenes(false);
        }

        /// <summary>
        ///     Unloads all active scenes and replaces them with a new set of scenes.
        /// </summary>
        /// <param name="emptySet">Whether any new scenes should be loaded.</param>
        private async void LoadScenes(bool emptySet)
        {
            while (!sceneCapturingReady.IsAllTrue())
            {
                await Task.Delay(10);
            }

            scenePreLoaderReady.value = false;

            //unload
            _completedLoadCount = 0;
            var oldScenes = GetActiveScenes();
            if (oldScenes.Count != 0)
            {
                _unloadWait = new TaskCompletionSource<bool>();
                foreach (var scene in oldScenes)
                {
                    StartCoroutine(UnloadAsync(scene, oldScenes.Count));
                }
                await _unloadWait.Task;
                _unloadWait = null;
            }

            //if true don't load any new scenes
            if (emptySet)
            {
                scenePreLoaderReady.value = true;
                UnloadedActive?.Invoke();
                return;
            }

            //load
            _completedLoadCount = 0;
            foreach (var scene in scenesToLoad.value)
            {
                StartCoroutine(LoadAsync(scene, LoadSceneMode.Additive, scenesToLoad.value.Length));
            }
        }

        /// <summary>
        ///     Returns a list of all currenty active scenes excluding scenes in the exclusion list.
        /// </summary>
        /// <returns>A list of all active and whitelisted scenes.</returns>
        private List<string> GetActiveScenes()
        {
            var countLoaded = UnityEngine.SceneManagement.SceneManager.sceneCount;
            var loadedScenes = new List<string>(countLoaded);
            for (var i = 0; i < countLoaded; i++)
            {
                var sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name;
                if (!scenesToIgnore.items.Contains(sceneName))
                {
                    loadedScenes.Add(sceneName);
                }
            }
            return loadedScenes;
        }

        /// <summary>
        ///     Asynchronously unloads specified scene.
        /// </summary>
        /// <param name="scene">Scene to unload.</param>
        /// <param name="target">Bookkeeping.</param>
        /// <returns>Coroutine.</returns>
        private IEnumerator UnloadAsync(string scene, int target)
        {
            yield return null;
            var load = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            while (!load.isDone)
            {
                yield return null;
            }
            _completedLoadCount++;
            if (_completedLoadCount >= target)
            {
                _unloadWait.SetResult(true);
            }
        }

        /// <summary>
        ///     Asynchronously loads specified scene.
        /// </summary>
        /// <param name="scene">Scene to load.</param>
        /// <param name="loadSceneMode">Always Additive.</param>
        /// <param name="target">Bookkeeping.</param>
        /// <returns>Coroutine.</returns>
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
                scenePreLoaderReady.value = true;
                LightProbes.TetrahedralizeAsync();
                scenePreLoaderLoaded.Raise();
            }
        }
    }
}