// Primary Author : Maximiliam Rosén - maka4519

using System.Collections.Generic;
using Framework.ScriptableObjectEvent;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class EndCinematicSkipper : MonoBehaviour
{
    [SerializeField] 
    private SubtitlesTrigger subtitlesTrigger = default;
    [SerializeField] 
    private PlayableDirector director = default;
    [SerializeField] 
    private GameEvent startBoss = default;
    [SerializeField] 
    private GameEvent enableControls = default;
    [SerializeField] 
    private List<GameObject> gameObjects = default;

    private bool _used;

    private void Start()
    {
        subtitlesTrigger = GetComponent<SubtitlesTrigger>();
        director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (director.state == PlayState.Playing && keyboard.spaceKey.wasPressedThisFrame && !_used)
        {
            Skip();
        }
    }

    private void Skip()
    {
        _used = true;
        director.Stop();
        subtitlesTrigger.StopDialog();
        startBoss.Raise();
        enableControls.Raise();
        gameObjects.ForEach(g => g.SetActive(false));
    }
}