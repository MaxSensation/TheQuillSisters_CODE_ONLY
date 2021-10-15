// Primary Author : Viktor Dahlberg - vida6631

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Framework.ScriptableObjectEvent;
using Framework.ScriptableObjectVariables;
using UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Framework.SaveSystem
{
	/// <summary>
	///     Handles savefile IO, and holds everything that needs to be saved and loaded.
	/// </summary>
	public class SaveManager : MonoBehaviour
    {
        [SerializeField] 
        private GameEventGroup gameSaveReady = default;
        [SerializeField] 
        private GameEvent gameLoadReady = default;
        [SerializeField] 
        private GameEvent saveManagerDone = default;
        [SerializeField] 
        private ScriptObjVar[] scriptableObjectVariablesToSave = default;
        [SerializeField] 
        private bool defaultWriteEnabled = default;

        private bool _isDefault;

        private bool _streamingInProgress;

        private void OnEnable()
        {
            LoadingScreen.GameLoadRequested += SetDefault;
            gameSaveReady.OnEvent += SaveGame;
            gameLoadReady.OnEvent += LoadGame;
        }

        /// <summary>
        ///     Toggles whether or not the default save should be used for saving and loading.
        /// </summary>
        /// <param name="isDefault">Whether to use default save.</param>
        private void SetDefault(bool isDefault)
        {
            _isDefault = isDefault;
        }

        /// <summary>
        ///     Editor function: Generates a save based on current state of saved ScriptObjVars.
        /// </summary>
        public void GenerateDefaultSave()
        {
            SaveGame(true);
        }

        /// <summary>
        ///     Editor function: Loads the default save.
        /// </summary>
        public void LoadDefaultSave()
        {
            LoadGame(true);
        }

        /// <summary>
        ///     Saves game based on current saving profile.
        /// </summary>
        private void SaveGame()
        {
            SaveGame(_isDefault && defaultWriteEnabled);
            _isDefault = false;
        }

        /// <summary>
        ///     Saves game with debugging in mind
        /// </summary>
        public void DebugSaveGame()
        {
            SaveGame(false);
        }

        /// <summary>
        ///     Loads game based on current loading profile.
        /// </summary>
        private void LoadGame()
        {
            LoadGame(_isDefault);
            _isDefault = false;
        }

        /// <summary>
        ///     Saves the values of all the passed Scriptable Objects, if iostreaming is available.
        /// </summary>
        private async void SaveGame(bool asDefault)
        {
            if (_streamingInProgress)
            {
                Debug.LogWarning("Failed to save: already saving");
            }
            else
            {
                _streamingInProgress = true;
                var path = asDefault
                    ? Path.Combine(Application.streamingAssetsPath, "DefaultSave.sav")
                    : Path.Combine(Application.persistentDataPath, "Save.sav");
                await Task.Run(() => OStream(path));

#if UNITY_EDITOR
                if (asDefault)
                {
                    AssetDatabase.Refresh();
                }
#endif
                _streamingInProgress = false;
                Debug.Log("Game Saved");
            }
        }

        /// <summary>
        ///     Loads the values of all the passed Scriptable Objects from the save file, if iostreaming is available.
        /// </summary>
        private async void LoadGame(bool asDefault)
        {
            if (_streamingInProgress)
            {
                Debug.LogWarning("Failed to load save: already loading.");
            }
            else
            {
                _streamingInProgress = true;
                var path = asDefault
                    ? Path.Combine(Application.streamingAssetsPath, "DefaultSave.sav")
                    : Path.Combine(Application.persistentDataPath, "Save.sav");
                await Task.Run(() => IStream(path));
                _streamingInProgress = false;
                saveManagerDone.Raise();
            }
        }

        /// <summary>
        ///     Outputs contents to save into a new save file,
        ///     or overwrites the existing one.
        /// </summary>
        /// <param name="path">Where the savefile should be created.</param>
        private void OStream(string path)
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, new Save(scriptableObjectVariablesToSave));
            stream.Close();
        }

        /// <summary>
        ///     Loads the contents of a savefile into the game.
        /// </summary>
        /// <param name="path">Where the savefile should be loaded from.</param>
        private void IStream(string path)
        {
            if (File.Exists(path))
            {
                var formatter = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Open);
                if (stream.Length > 0 && formatter.Deserialize(stream) is Save save)
                {
                    for (var i = 0; i < scriptableObjectVariablesToSave.Length; i++)
                    {
                        scriptableObjectVariablesToSave[i].SetValue(save.SaveData[i]);
                    }
                }
                stream.Close();
            }
            else
            {
                Debug.LogWarning($"Failed to load save: no such file exists at the specified path: {path}");
            }
        }

        public static bool HasSave()
        {
            var path = Path.Combine(Application.persistentDataPath, "Save.sav");
            return File.Exists(path);
        }

        public void DeleteSave()
        {
            var path = Path.Combine(Application.persistentDataPath, "Save.sav");
            if (File.Exists(path) && !_streamingInProgress)
            {
                File.Delete(path);
                Debug.Log("Save Deleted");
            }
        }
    }
}