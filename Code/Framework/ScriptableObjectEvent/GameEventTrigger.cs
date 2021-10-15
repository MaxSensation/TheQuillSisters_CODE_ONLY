// Primary Author : Maximiliam Rosén - maka4519

using Environment.Trigger;
using Framework.ScriptableObjectEvent;
using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    [SerializeField] 
    private AreaTrigger areaTrigger = default;
    [SerializeField] 
    private GameEvent gameEvent = default;

    private void Start()
    {
        areaTrigger.OnTrigger += TriggerEvent;
    }

    private void TriggerEvent(bool obj)
    {
        gameEvent.Raise();
    }
}