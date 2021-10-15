// Primary Author : Maximiliam Rosén - maka4519

using Environment.Trigger;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] 
    private AudioClip music = default;
    [SerializeField] 
    private AreaTrigger activator = default;
    [SerializeField] 
    private AreaTrigger deactivator = default;

    private void Start()
    {
        if (activator)
        {
            activator.OnTrigger += StartMusic;
        }
        if (deactivator)
        {
            deactivator.OnTrigger += StopMusic;
        }
    }

    private void OnDisable()
    {
        if (activator)
        {
            activator.OnTrigger -= StartMusic;
        }
        if (deactivator)
        {
            deactivator.OnTrigger -= StopMusic;
        }
    }

    private void StartMusic(bool invalid)
    {
        MusicManager.OnMusicStart?.Invoke(music);
    }

    private void StopMusic(bool obj)
    {
        MusicManager.OnMusicStop?.Invoke();
    }
}