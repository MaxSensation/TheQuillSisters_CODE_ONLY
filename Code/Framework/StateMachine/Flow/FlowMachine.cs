// Primary Author : Viktor Dahlberg - vida6631

using UnityEngine;

namespace Framework.StateMachine.Flow
{
	/// <summary>
	///     Runs state flows and handles switching between them.
	/// </summary>
	public class FlowMachine
    {
        private FlowState _activeState;
        private bool _failed;

        public FlowMachine(FlowState initialState, object param = null)
        {
            ChangeState(initialState, param);
        }

        /// <summary>
        ///     Runs the active state, should be ran every frame.
        /// </summary>
        public void Run()
        {
            if (!_failed)
            {
                _activeState.Run();
            }
        }

        /// <summary>
        ///     Aborts the Flow Machine, so that it needs to be Restart()-ed.
        /// </summary>
        public void Fail()
        {
            _failed = true;
            _activeState = null;
        }

        /// <summary>
        ///     Restarts the Flow Machine after a failure.
        /// </summary>
        /// <param name="initialState">The flow state to restart with.</param>
        /// <param name="param">Optional param for bookkeeping in state flows.</param>
        public void Restart(FlowState initialState, object param = null)
        {
            _failed = false;
            ChangeState(initialState, param);
        }

        /// <summary>
        ///     Changes state in the active state flow.
        /// </summary>
        /// <param name="next">The flow state to switch to.</param>
        /// <param name="param">Optional param for bookkeeping in state flows.</param>
        public void Goto(FlowState next, object param = null)
        {
            if (_failed)
            {
                Debug.LogWarning("Flow Machine has failed. If you want to start a new process, use Restart()");
                return;
            }

            if (_activeState != null)
            {
                _activeState.Exit();
            }

            ChangeState(next, param);
        }

        private void ChangeState(FlowState next, object param = null)
        {
            _activeState = next;
            _activeState.flowMachine = this;
            _activeState.Enter();
            _activeState.param = param;
        }
    }
}