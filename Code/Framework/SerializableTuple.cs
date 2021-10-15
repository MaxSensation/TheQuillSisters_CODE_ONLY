// Primary Author : Viktor Dahlberg - vida6631

using System;
using UnityEngine.Serialization;

namespace Framework
{
	/// <summary>
	///     Tuple that can be serialized to the Unity inspector. Equality determined by Item1.
	/// </summary>
	/// <typeparam name="T1">Type of Item1.</typeparam>
	/// <typeparam name="T2">Type of Item2.</typeparam>
	[Serializable]
    public class SerializableTuple<T1, T2> : IEquatable<SerializableTuple<T1, T2>>
    {
        [FormerlySerializedAs("Item1")] public T1 item1;
        [FormerlySerializedAs("Item2")] public T2 item2;

        public bool Equals(SerializableTuple<T1, T2> other)
        {
            return item1.Equals(other.item1);
        }
    }

	/// <summary>
	///     Tuple that can be serialized to the Unity inspector. Equality determined by Item1.
	/// </summary>
	/// <typeparam name="T">Type of Item1 and Item2.</typeparam>
	[Serializable]
    public class SerializableTuple<T> : IEquatable<SerializableTuple<T>>
    {
        [FormerlySerializedAs("Item1")] public T item1;
        [FormerlySerializedAs("Item2")] public T item2;

        public bool Equals(SerializableTuple<T> other)
        {
            return item1.Equals(other.item1);
        }
    }
}