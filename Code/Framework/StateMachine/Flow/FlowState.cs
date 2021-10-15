// Primary Author : Viktor Dahlberg - vida6631

using System;
using UnityEngine;

namespace Framework.StateMachine.Flow
{
	/// <summary>
	///     Base class for flow states. Specifies its own followup state, hence "flow".
	/// </summary>
	public abstract class FlowState : ScriptableObject
    {
	    /// <summary>
	    ///     The followup state.
	    /// </summary>
	    [SerializeField] 
	    protected FlowState flow = default;

	    /// <summary>
	    ///     Flow Machine running the state.
	    /// </summary>
	    [NonSerialized] 
	    public FlowMachine flowMachine;

	    /// <summary>
	    ///     Bookkeeping variable to be used within the state flow.
	    /// </summary>
	    [NonSerialized] 
	    public object param;

	    /// <summary>
	    ///     Is ran on state entry.
	    /// </summary>
	    public virtual void Enter()
        {
        }

	    /// <summary>
	    ///     Is ran by the Flow Machine whenever FlowMachine.Run() is called, which should be every frame.
	    /// </summary>
	    public abstract void Run();

	    /// <summary>
	    ///     Is ran on state exit.
	    /// </summary>
	    public virtual void Exit()
        {
        }
    }
}