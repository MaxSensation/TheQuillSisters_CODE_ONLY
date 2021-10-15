// Primary Author : Viktor Dahlberg - vida6631

using System;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	///     Holds some amount of actions and invokes an action of its own when all source actions have been fired.
	/// </summary>
	public class ActionGroup<T> : IDisposable
    {
        private readonly List<Action<T>> _actions;
        private bool _disposeOnFire;
        private HashSet<Action<T>> _invokedActions;
        public Action whenAll;
        private bool AutoReset { get; set; } = true;
        public bool HasActions => _actions != null && _actions.Count > 0;

        public ActionGroup(bool disposeOnFire = false)
        {
            _actions = new List<Action<T>>();
            SetDisposeOnFire(disposeOnFire);
        }

        public void Dispose()
        {
            whenAll = null;
        }

        public void AddAction(ref Action<T> action)
        {
            var act = action;
            action += _ => Log(act);
            _actions.Add(act);
        }

        public void RemoveAction(ref Action<T> action)
        {
            var act = action;
            action -= _ => Log(act);
            _actions.Remove(act);
        }

        private void Log(Action<T> action)
        {
            if (whenAll == null)
            {
            }
            else
            {
                if (_invokedActions != null)
                {
                    _invokedActions.Add(action);
                    if (_invokedActions.Count == _actions.Count)
                    {
                        whenAll?.Invoke();
                        if (AutoReset) Reset();
                    }
                }
                else
                {
                    Reset();
                    Log(action);
                }
            }
        }

        public void Reset()
        {
            _invokedActions = new HashSet<Action<T>>();
        }

        public void SetDisposeOnFire(bool dispose)
        {
            if (dispose && !_disposeOnFire)
                whenAll += () => Dispose();
            else if (!dispose && _disposeOnFire) whenAll -= () => Dispose();
            _disposeOnFire = dispose;
        }
    }
}