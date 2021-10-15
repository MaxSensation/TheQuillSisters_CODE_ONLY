// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectVariables.Custom
{
	/// <summary>
	///     Use this class to create actual ScriptableObject assets, that later can be referred to as ScriptObjVar<Quaternion>
	/// </summary>
	[CreateAssetMenu(menuName = "Scriptable Object Variables/Custom/Quaternion")]
    public class ScriptObjQuaternion : ScriptObjVar<Quaternion>
    {
        public override object GetValue()
        {
            return new float[4] {value.x, value.y, value.z, value.w};
        }

        public override void SetValue(object value)
        {
            var vector = (float[]) value;
            this.value.x = vector[0];
            this.value.y = vector[1];
            this.value.z = vector[2];
            this.value.w = vector[3];
        }
    }
}