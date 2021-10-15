// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Enum
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<EDifficulty>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Enum/Difficulty")]
    public class ScriptObjDifficulty : ScriptObjVar<EDifficulty>
    {
    }
}