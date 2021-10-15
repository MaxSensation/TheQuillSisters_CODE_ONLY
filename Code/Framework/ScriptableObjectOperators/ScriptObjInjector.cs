// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.ScriptableObjectOperators
{
	/// <summary>
	///     A class that can apply some operation to some component as defined by implementing subclasses.
	/// </summary>
	/// <typeparam name="T">The type of component to operate on.</typeparam>
	public abstract class ScriptObjInjector<T> : ScriptableObject where T : Component
    {
        public T component;
        public abstract void Modify();
        public abstract void DeModify();
    }
}