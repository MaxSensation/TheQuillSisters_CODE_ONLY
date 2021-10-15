// Primary Author : Maximiliam Rosén - maka4519

using UnityEngine;

namespace Framework
{
    public readonly struct Vector3Wrapper
    {
        private readonly Vector3 _value;

        public Vector3Wrapper(Vector3 value, Axis axis, float newAxisValue)
        {
            _value = value;
            switch (axis)
            {
                case Axis.X:
                    value.x = newAxisValue;
                    break;
                case Axis.Y:
                    value.y = newAxisValue;
                    break;
                case Axis.Z:
                    value.z = newAxisValue;
                    break;
            }
        }

        public static implicit operator Vector3(Vector3Wrapper vectorWrapper)
        {
            return vectorWrapper._value;
        }
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }
}