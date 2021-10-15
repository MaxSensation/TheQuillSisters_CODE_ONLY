// Primary Author : Erik Pilström - erpi3245
// Secondary Author : Viktor Dahlberg - vida6631
// Third Author : Maximiliam Rosén - maka4519
// Forth Author : Andreas Berzelius - anbe5918

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Framework.StateMachine
{
    public class StateMachine
    {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();

        public StateMachine(State[] states, object owner)
        {
            foreach (var state in states)
            {
                var instance = Object.Instantiate(state);
                instance.owner = owner;
                instance.StateMachine = this;
                _states.Add(instance.GetType(), instance);

                if (CurrentState == null)
                {
                    CurrentState = instance;
                }
            }

            if (CurrentState != null)
            {
                CurrentState.Enter();
            }
        }

        public State CurrentState { get; private set; }

        public void Run()
        {
            CurrentState.Run();
        }

        public void TransitionTo<T>() where T : State
        {
            if (CurrentState != null) CurrentState.Exit();
            CurrentState = _states[typeof(T)];
            CurrentState.Enter();
        }

        public bool HasState<T>() where T : State
        {
            return _states.ContainsKey(typeof(T));
        }
    }
}