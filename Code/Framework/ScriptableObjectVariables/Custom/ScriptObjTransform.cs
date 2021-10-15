// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<Transform>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/Transform")]
    public class ScriptObjTransform : ScriptObjVar<Transform>
    {
    }
}