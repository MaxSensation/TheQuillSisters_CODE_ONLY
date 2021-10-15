// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<Vector3>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/Vector3")]
    public class ScriptObjVector3 : ScriptObjVar<Vector3>
    {
        public override object GetValue()
        {
            return new float[3] {value.x, value.y, value.z};
        }

        public override void SetValue(object value)
        {
            var vector = (float[]) value;
            this.value.x = vector[0];
            this.value.y = vector[1];
            this.value.z = vector[2];
        }

        public override string ToString()
        {
            return $"({value.x}|{value.y}|{value.z})";
        }
    }
}