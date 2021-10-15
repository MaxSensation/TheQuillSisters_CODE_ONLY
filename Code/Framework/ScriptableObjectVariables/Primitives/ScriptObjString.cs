// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Primitives
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<int>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Primitives/string")]
    public class ScriptObjString : ScriptObjVar<string>
    {
    }
}