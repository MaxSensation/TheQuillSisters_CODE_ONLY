// Primary Author : Maximiliam Rosén - maka4519

using System;
using Environment.Trigger;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitlesTrigger : MonoBehaviour
{
    [SerializeField]
    public string dialog = default;

    [SerializeField]
    private PlayableAsset playable = default;

    [SerializeField]
    private AreaTrigger areaTrigger = default;

    public static Action<string, PlayableAsset> OnDialog;
    public static Action OnStopped;

    private void Start()
    {
        if (areaTrigger)
        {
            areaTrigger.OnTrigger += _ => StartDialog();
        }
    }

    public void StartDialog()
    {
        OnDialog?.Invoke(dialog, playable);
    }

    public void StopDialog()
    {
        OnStopped?.Invoke();
    }
}