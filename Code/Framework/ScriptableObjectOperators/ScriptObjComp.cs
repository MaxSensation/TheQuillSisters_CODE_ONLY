// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections.Generic;
using Framework.ScriptableObjectVariables;
using UnityEngine;

namespace Framework.ScriptableObjectOperators
{
	/// <summary>
	///     A class that can compare the values of two ScriptObjRef<T> variables with any standard comparison.
	/// </summary>
	/// <typeparam name="T">The types of the variables to be compared.</typeparam>
	public class ScriptObjComp<T> : ScriptableObject
    {
        [Serializable]
        public enum Comparator
        {
            LESS_THAN,
            GREATER_THAN,
            EQUAL,
            NOT_EQUAL,
            LESS_THAN_OR_EQUAL,
            GREATER_THAN_OR_EQUAL
        }

        public ScriptObjRef<T> item1;
        public Comparator comparator;
        public ScriptObjRef<T> item2;

        public bool Value => Compare();

        private bool Compare()
        {
            var comparison = Comparer<T>.Default.Compare(item1.Value, item2.Value);
            switch (comparator)
            {
                case Comparator.LESS_THAN: return comparison < 0;
                case Comparator.GREATER_THAN: return comparison > 0;
                case Comparator.EQUAL: return comparison == 0;
                case Comparator.NOT_EQUAL: return comparison != 0;
                case Comparator.LESS_THAN_OR_EQUAL: return comparison <= 0;
                case Comparator.GREATER_THAN_OR_EQUAL: return comparison >= 0;
                default: return false;
            }
        }
    }
}