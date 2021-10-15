// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.ScriptableObjectEvent
{
	/// <summary>
	///     Holds some amount of events and fires an event of its own when all source events have been fired.
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Events/Event Group")]
    public class GameEventGroup : ScriptableObject
    {
        [SerializeField]
        private GameEvent[] gameEvents = default;
        [SerializeField]
        private GameEvent resetEvent = default;
        [SerializeField]
        private bool echo = default;

        public Action OnEvent;
        private HashSet<string> _triggeredEvents;

        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            _triggeredEvents = new HashSet<string>();
            if (echo)
            {
                Debug.Log(name + " was reset");
            }
        }

        private void OnEnable()
        {
            if (gameEvents.Length > 0)
                foreach (var gameEvent in gameEvents)
                {
                    var name = gameEvent.name;
                    gameEvent.OnEvent += () => Log(name);
                }
            if (resetEvent != null)
            {
                resetEvent.OnEvent += Reset;
            }
        }

        private void OnDisable()
        {
            if (gameEvents.Length > 0)
                foreach (var gameEvent in gameEvents)
                {
                    var name = gameEvent.name;
                    gameEvent.OnEvent -= () => Log(name);
                }
            if (resetEvent != null) resetEvent.OnEvent -= Reset;
        }

        private void Log(string source)
        {
            if (_triggeredEvents == null)
            {
                Reset();
            }
            _triggeredEvents.Add(source);
            if (echo)
            {
                Debug.Log(name + " logged " + source + " (" + _triggeredEvents.Count + "/" + gameEvents.Length + ")");
            }
            if (_triggeredEvents.Count >= gameEvents.Length)
            {
                if (echo)
                {
                    Debug.Log(name + " fired");
                }

                try
                {
                    OnEvent.Invoke();
                }
                catch (NullReferenceException)
                {
                    Debug.Log(this + "nullref");
                }

                Reset();
            }
        }
    }
}