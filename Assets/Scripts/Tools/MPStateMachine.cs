using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace MetaversePrototype.Tools
{
	public struct MPStateChangeEvent<T> where T: struct, IComparable, IConvertible, IFormattable
	{
		public GameObject Target;
		public MPStateMachine<T> TargetStateMachine;
		public T NewState;
		public T PreviousState;

		public MPStateChangeEvent(MPStateMachine<T> stateMachine)
		{
			Target = stateMachine.Target;
			TargetStateMachine = stateMachine;
			NewState = stateMachine.CurrentState;
			PreviousState = stateMachine.PreviousState;
		}
	}

	public interface MPIStateMachine
	{
		bool TriggerEvents { get; set; }
	}

	public class MPStateMachine<T> : MPIStateMachine where T : struct, IComparable, IConvertible, IFormattable
	{

		public bool TriggerEvents { get; set; }
		/// the name of the target gameobject
		public GameObject Target;
		/// the current character's movement state
		public T CurrentState { get; protected set; }
		/// the character's movement state before entering the current one
		public T PreviousState { get; protected set; }

		public delegate void OnStateChangeDelegate();

		public OnStateChangeDelegate OnStateChange;
		public MPStateMachine(GameObject target, bool triggerEvents)
		{
			this.Target = target;
			this.TriggerEvents = triggerEvents;
		} 

		public virtual void ChangeState(T newState)
		{
			// if the "new state" is the current one, we do nothing and exit
			if (EqualityComparer<T>.Default.Equals(newState, CurrentState))
			{
				return;
			}

			// we store our previous character movement state
			PreviousState = CurrentState;
			CurrentState = newState;

			OnStateChange?.Invoke();

			if (TriggerEvents)
			{
				MPEventManager.TriggerEvent (new MPStateChangeEvent<T> (this));
			}
		}

		public virtual void RestorePreviousState()
		{
			// we restore our previous state
			CurrentState = PreviousState;

			OnStateChange?.Invoke();

			if (TriggerEvents)
			{
				MPEventManager.TriggerEvent (new MPStateChangeEvent<T> (this));
			}
		}	
	}
}