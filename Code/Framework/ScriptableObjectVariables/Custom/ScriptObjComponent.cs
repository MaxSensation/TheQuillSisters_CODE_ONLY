// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<Component>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/Component")]
    public class ScriptObjComponent : ScriptObjVar<Component>
    {
    }
}