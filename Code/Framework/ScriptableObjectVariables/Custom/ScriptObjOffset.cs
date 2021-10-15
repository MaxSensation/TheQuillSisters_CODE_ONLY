// Primary Author : Viktor Dahlberg - vida6631

using Scripts.Camera;
using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<Offset>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/Offset")]
    public class ScriptObjOffset : ScriptObjVar<Offset>
    {
    }
}