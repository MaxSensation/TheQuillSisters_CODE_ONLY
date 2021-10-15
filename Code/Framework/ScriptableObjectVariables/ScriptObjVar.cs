// Primary Author : Viktor Dahlberg - vida6631

using System;
using UnityEngine;

namespace Framework.ScriptableObjectVariables
{
	/// <summary>
	///     The base class for all typed Scriptable Object Variables.
	/// </summary>
	/// <typeparam name="T">Any - preferably serializeable - type.</typeparam>
	public class ScriptObjVar<T> : ScriptObjVar
    {
        public new T value;

        public static implicit operator T(ScriptObjVar<T> var)
        {
            return var.value;
        }

        public override object GetValue()
        {
            return value;
        }

        public override void SetValue(object value)
        {
            this.value = (T) value;
        }

        public override void SetValueNotify(object value)
        {
            this.value = (T) value;
            ValueChanged?.Invoke();
        }
    }

	/// <summary>
	///     The base serializable class used for saving.
	/// </summary>
	[Serializable]
    public abstract class ScriptObjVar : ScriptableObject
    {
        public object value;

        public Action ValueChanged;

        public virtual object GetValue()
        {
            return value;
        }

        public virtual void SetValue(object value)
        {
            this.value = value;
        }

        public virtual void SetValueNotify(object value)
        {
            this.value = value;
            ValueChanged?.Invoke();
        }
    }
}