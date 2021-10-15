// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Sets
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjSet<GameObject>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Sets/GameObject")]
    public class ScriptObjGameObjectSet : ScriptObjSet<GameObject>
    {
    }
}