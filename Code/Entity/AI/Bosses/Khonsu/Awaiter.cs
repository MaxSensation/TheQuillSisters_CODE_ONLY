// Primary Author : Viktor Dahlberg - vida6631

using Environment.Trigger;
using Framework.ScriptableObjectEvent;
using UnityEngine;

namespace Entity.AI.Bosses.Khonsu
{
    public class Awaiter : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] scriptsToEnable = default;
        [SerializeField]
        private AreaTrigger areaTrigger = default;
        [SerializeField]
        private GameEvent gameEvent = default;

        private void OnEnable()
        {
            if (areaTrigger != null)
            {
                areaTrigger.OnTrigger += Trigger;
            }

            if (gameEvent != null)
            {
                gameEvent.OnEvent += Trigger;
            }
        }

        private void OnDisable()
        {
            if (areaTrigger != null)
            {
                areaTrigger.OnTrigger -= Trigger;
            }

            if (gameEvent != null)
            {
                gameEvent.OnEvent -= Trigger;
            }
        }

        private void Trigger()
        {
            Trigger(false);
        }

        private void Trigger(bool _)
        {
            foreach (var t in scriptsToEnable)
            {
                t.enabled = true;
            }
        }
    }
}