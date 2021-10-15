// Primary Author : Andreas Berzelius - anbe5918

using Framework.ScriptableObjectEvent;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment.Trigger
{
    public class TutorialTrigger : MonoBehaviour
    {
        [FormerlySerializedAs("activationtrigger")] [SerializeField] [Tooltip("An areatrigger to activate the event \nyou don´t need to have this")]
        private AreaTrigger activationTrigger = default;

        [FormerlySerializedAs("deactivationtrigger")] [SerializeField] [Tooltip("An areatrigger to deactivate the event \nyou don´t need to have this")]
        private AreaTrigger deactivationTrigger = default;

        [SerializeField] [Tooltip("The tutorial event to trigger deactivation or activation")]
        private GameEvent gameEvent = default;

        private void Start()
        {
            if (activationTrigger != null)
            {
                activationTrigger.OnTrigger += ActivateGameEvent;
            }

            if (deactivationTrigger != null)
            {
                deactivationTrigger.OnTrigger += ActivateGameEvent;
            }
        }

        private void OnDisable()
        {
            if (activationTrigger != null)
            {
                activationTrigger.OnTrigger -= ActivateGameEvent;
            }

            if (deactivationTrigger != null)
            {
                deactivationTrigger.OnTrigger -= ActivateGameEvent;
            }
        }

        private void ActivateGameEvent(bool obj)
        {
            gameEvent.Raise();
        }
    }
}