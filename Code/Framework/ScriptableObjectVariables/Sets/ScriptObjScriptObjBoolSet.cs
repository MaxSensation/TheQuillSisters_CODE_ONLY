// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Sets
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjSet
	///     <ScriptObjBool>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Sets/ScriptObjBool")]
    public class ScriptObjScriptObjBoolSet : ScriptObjScriptObjVarSet<ScriptObjVar<bool>>
    {
    }
}