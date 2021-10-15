// Primary Author : Maximiliam Rosén - maka4519

using Framework.SceneSystem;
using Framework.ScriptableObjectEvent;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace Framework
{
    [RequireComponent(typeof(PlayableDirector))]
    [RequireComponent(typeof(SubtitlesTrigger))]
    public class CinematicSkipper : MonoBehaviour
    {
        [FormerlySerializedAs("_gameEvent")] [SerializeField] 
        private GameEvent gameEvent = default;
        
        private PlayableDirector _director;
        private SceneManager _sceneManager;
        private SubtitlesTrigger _subtitlesTrigger;
        private bool _used;

        private void Start()
        {
            _subtitlesTrigger = GetComponent<SubtitlesTrigger>();
            _director = GetComponent<PlayableDirector>();
            _sceneManager = GetComponent<SceneManager>();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard.spaceKey.wasPressedThisFrame && !_used)
            {
                Skip();
            }
        }

        public void Skip()
        {
            _used = true;
            _director.Stop();
            _subtitlesTrigger.StopDialog();
            if (gameEvent)
            {
                gameEvent.Raise();
            }
            else
            {
                _sceneManager.SetForced();
                _sceneManager.UnloadLoad();
            }
        }
    }
}