// Primary Author : Maximiliam Rosén - maka4519

using Combat;
using UnityEngine;

namespace Framework.AudioSystem
{
    [CreateAssetMenu(menuName = "Scriptable Object Audio/Foley/AttackSound")]
    public class AttackAudio : ScriptableObject
    {
        [SerializeField]
        internal ScriptObjAttackBase attack = default;
        [SerializeField]
        internal bool immediateBegin = default;
        [SerializeField]
        internal AudioClip beginAttack = default;
        [SerializeField]
        internal AudioClip hitAttack = default;
        [SerializeField]
        internal AudioClip specialAttack = default;
    }
}