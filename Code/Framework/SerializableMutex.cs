// Primary Author : Viktor Dahlberg - vida6631

using System;
using UnityEngine;

namespace Framework
{
	/// <summary>
	///     Mutex class used to determine whose turn it is to do something.
	/// </summary>
	[Serializable]
    public class SerializableMutex
    {
	    /// <summary>
	    ///     Whether or not the mutex is available for locking.
	    /// </summary>
	    public bool locked;

	    /// <summary>
	    ///     Whether or not warnings should be printed to console.
	    /// </summary>
	    public bool echo;

	    /// <summary>
	    ///     Timestamp when locked
	    /// </summary>
	    private float _timeStamp;

	    /// <summary>
	    ///     Interrupt all mutex users
	    /// </summary>
	    public Action OnInterrupted;

	    /// <summary>
	    ///     The owner of the mutex.
	    /// </summary>

	    public string Owner { get; private set; }

	    /// <summary>
	    ///     Calculates the time since the mutex was locked
	    /// </summary>
	    /// <returns>Time since mutex was locked</returns>
	    public float GetTimeDelta()
        {
            return Time.time - _timeStamp;
        }

	    /// <summary>
	    ///     Locks the mutex if it isn't already locked.
	    ///     Otherwise, sends a warning message and ends the function.
	    /// </summary>
	    /// <typeparam name="T">The locking class.</typeparam>
	    public void Lock(string newOwner)
        {
            if (locked)
            {
                if (echo)
                {
	                Debug.LogWarning(
		                newOwner
		                + " can't lock Mutex("
		                + GetHashCode()
		                + "): already locked by "
		                + Owner);
                }

                return;
            }

            locked = true;
            Owner = newOwner;
            UpdateTimeStamp();
        }

	    /// <summary>
	    ///     Unlocks the mutex if it isn't already unlocked, and T is the owner.
	    ///     Otherwise, sends a warning message and ends the function.
	    /// </summary>
	    /// <typeparam name="T">The unlocking class.</typeparam>
	    public void Unlock(string newOwner)
        {
            if (!locked || !Owner.Equals(newOwner))
            {
                if (echo)
                {
	                Debug.LogWarning(
		                newOwner
		                + " can't unlock Mutex("
		                + GetHashCode() + "): "
		                + (Owner != null && !Owner.Equals(newOwner) ? "Mutex owned by " + Owner : "already unlocked"));
                }

                return;
            }

            locked = false;
            Owner = null;
        }

	    /// <summary>
	    ///     Forces the mutex into the desired state.
	    /// </summary>
	    /// <typeparam name="T">New owner if mutex is forced open.</typeparam>
	    /// <param name="locked">Whether the mutex should be forced open or closed.</param>
	    public void Force(string newOwner, bool locked)
        {
            this.locked = locked;
            Owner = locked ? newOwner : null;
            if (locked)
            {
                OnInterrupted?.Invoke();
                UpdateTimeStamp();
            }
        }

	    /// <summary>
	    ///     Checks whether or not T is the owner.
	    /// </summary>
	    /// <typeparam name="T">The checking class.</typeparam>
	    /// <returns>Whether or not T is the owner.</returns>
	    public bool IsMe(string potentialOwner)
        {
            return Owner != null && Owner.Equals(potentialOwner);
        }

        public void UpdateTimeStamp()
        {
            _timeStamp = Time.time;
        }
    }
}