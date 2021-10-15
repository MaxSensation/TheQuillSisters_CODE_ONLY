// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Sets
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjSet
	///     <ScriptObjVar>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Sets/ScriptObjVar")]
    public class ScriptObjVarSet : ScriptObjScriptObjVarSet<ScriptObjVar>
    {
    }
}