// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;
using UnityEngine.Events;

namespace Framework.ScriptableObjectEvent
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("Event to register")] [SerializeField]
        public GameEvent gameEvent = default;
        [Tooltip("Response to invoke when Event is raised")] [SerializeField]
        public UnityEvent response = default;

        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            response.Invoke();
        }
    }
}