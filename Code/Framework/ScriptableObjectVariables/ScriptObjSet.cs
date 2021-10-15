// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections.Generic;

namespace Framework.ScriptableObjectVariables
{
	/// <summary>
	///     The base class for all typed Scriptable Object sets.
	/// </summary>
	/// <typeparam name="T">Any - preferably serializeable - type.</typeparam>
	[Serializable]
    public class ScriptObjSet<T> : ScriptObjVar
    {
        public List<T> items = new List<T>();

        public void Add(T t)
        {
            if (!items.Contains(t))
            {
                items.Add(t);
            }
        }

        public override object GetValue()
        {
            return items;
        }

        public void Remove(T t)
        {
            if (items.Contains(t))
            {
                items.Remove(t);
            }
        }

        public override void SetValue(object value)
        {
            items = (List<T>) value;
        }
    }
}