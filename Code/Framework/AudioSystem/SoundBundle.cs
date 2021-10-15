// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Framework.AudioSystem
{
    [CreateAssetMenu(menuName = "Scriptable Object Audio/SoundBundle")]
    public class SoundBundle : ScriptableObject
    {
        [SerializeField] 
        private AudioClip[] audioClips = default;

        internal AudioClip GetRandom()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}