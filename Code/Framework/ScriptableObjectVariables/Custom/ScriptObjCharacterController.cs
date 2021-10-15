// Primary Author : Andreas Berzelius - anbe5918

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
    /// <summary>
    ///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar
    ///     <CharacterController>
    /// </summary>
    [CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/CharacterController")]
    public class ScriptObjCharacterController : ScriptObjVar<CharacterController>
    {
    }
}