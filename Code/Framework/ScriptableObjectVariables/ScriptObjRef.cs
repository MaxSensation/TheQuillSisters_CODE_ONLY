// Primary Author : Viktor Dahlberg - vida6631

using System;

namespace Framework.ScriptableObjectVariables
{
	/// <summary>
	///     A class that holds a reference to some value of some type and an optional override of that same type.
	/// </summary>
	/// <typeparam name="T">Any - preferably serializeable - type.</typeparam>
	[Serializable]
    public class ScriptObjRef<T>
    {
        public bool useConstant;
        public T constantValue;
        public ScriptObjVar<T> variableValue;

        /// <summary>
        ///     Returns <c>constantValue</c> if <c>useConstant</c> is <c>true</c> or <c>variableValue</c> is <c>null</c>,
        ///     otherwise returns <c>variableValue</c>.
        /// </summary>
        public T Value
        {
            get => useConstant || variableValue is null ? constantValue : variableValue.value;
            set
            {
                if (variableValue != null)
                {
                    variableValue.value = value;
                }
            }
        }
    }
}