// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Viktor Dahlberg - vida6631
// Third Author : Maximiliam Rosén - maka4519
// Forth Author : Andreas Berzelius - anbe5918

using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.StateMachine
{
    public abstract class State : ScriptableObject
    {
        [FormerlySerializedAs("Owner")] public object owner;

        public StateMachine StateMachine;

        public abstract void Enter();
        public abstract void Run();
        public abstract void Exit();
    }
}