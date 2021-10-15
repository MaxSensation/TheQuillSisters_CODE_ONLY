// Primary Author : Maximiliam Rosén - maka4519

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.ScriptableObjectEvent
{
    [CreateAssetMenu(menuName = "Scriptable Object Events/Event")]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _eventListeners = new List<GameEventListener>();
        public Action OnEvent;

        public void Raise()
        {
            OnEvent?.Invoke();
            for (var i = _eventListeners.Count - 1; i >= 0; i--)
            {
                _eventListeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
            {
                _eventListeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (_eventListeners.Contains(listener))
            {
                _eventListeners.Remove(listener);
            }
        }
    }
}