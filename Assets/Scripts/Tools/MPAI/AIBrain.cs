using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{
	public class AIBrain : MonoBehaviour
	{
		[Header("Debug")]
		/// the owner of that AI Brain, usually the associated character
		[ReadOnly]
		public GameObject Owner;
		/// the collection of states
		public List<AIState> States;
		/// this brain's current state
		public AIState CurrentState { get; protected set; }
		/// the time we've spent in the current state
		[ReadOnly]
		public float TimeInThisState;
		/// the current target
		[ReadOnly]
		public Transform Target;
		/// the last known world position of the target
		[ReadOnly]
		public Vector3 _lastKnownTargetPosition = Vector3.zero;
		
		[Header("State")]
		/// whether or not this brain is active
		public bool BrainActive = true;
		public bool ResetBrainOnStart = true;
		public bool ResetBrainOnEnable = false;

		[Header("Frequencies")]
		/// the frequency (in seconds) at which to perform actions (lower values : higher frequency, high values : lower frequency but better performance)
		public float ActionsFrequency = 0f;
		/// the frequency (in seconds) at which to evaluate decisions
		public float DecisionFrequency = 0f;
        
		/// whether or not to randomize the action and decision frequencies
		public bool RandomizeFrequencies = false;
		/// the min and max values between which to randomize the action frequency
		public Vector2 RandomActionFrequency = new Vector2(0.5f, 1f);
		/// the min and max values between which to randomize the decision frequency
		public Vector2 RandomDecisionFrequency = new Vector2(0.5f, 1f);

		protected AIDecision[] _decisions;
		protected AIAction[] _actions;
		protected float _lastActionsUpdate = 0f;
		protected float _lastDecisionsUpdate = 0f;
		protected AIState _initialState;

		public virtual AIAction[] GetAttachedActions()
		{
			AIAction[] actions = this.gameObject.GetComponentsInChildren<AIAction>();
			return actions;
		}

		public virtual AIDecision[] GetAttachedDecisions()
		{
			AIDecision[] decisions = this.gameObject.GetComponentsInChildren<AIDecision>();
			return decisions;
		}

		protected void OnEnable()
		{
			if (ResetBrainOnEnable)
			{
				ResetBrain();
			}
		}

		protected virtual void Awake()
		{
			foreach (AIState state in States)
			{
				state.SetBrain(this);
			}
			_decisions = GetAttachedDecisions();
			_actions = GetAttachedActions();
			if (RandomizeFrequencies)
			{
				ActionsFrequency = Random.Range(RandomActionFrequency.x, RandomActionFrequency.y);
				DecisionFrequency = Random.Range(RandomDecisionFrequency.x, RandomDecisionFrequency.y);
			}
		}

		protected virtual void Start()
		{
			if (ResetBrainOnStart)
			{
				ResetBrain();	
			}
		}

		protected virtual void Update()
		{
			if (!BrainActive || (CurrentState == null) || (Time.timeScale == 0f))
			{
				return;
			}

			if (Time.time - _lastActionsUpdate > ActionsFrequency)
			{
				CurrentState.PerformActions();
				_lastActionsUpdate = Time.time;
			}
            
			if (!BrainActive)
			{
				return;
			}
            
			if (Time.time - _lastDecisionsUpdate > DecisionFrequency)
			{
				CurrentState.EvaluateTransitions();
				_lastDecisionsUpdate = Time.time;
			}
            
			TimeInThisState += Time.deltaTime;

			StoreLastKnownPosition();
		}
        
		public virtual void TransitionToState(string newStateName)
		{
			if (CurrentState == null)
			{
				CurrentState = FindState(newStateName);
				if (CurrentState != null)
				{
					CurrentState.EnterState();
				}
				return;
			}
			if (newStateName != CurrentState.StateName)
			{
				CurrentState.ExitState();
				OnExitState();

				CurrentState = FindState(newStateName);
				if (CurrentState != null)
				{
					CurrentState.EnterState();
				}                
			}
		}
        
		protected virtual void OnExitState()
		{
			TimeInThisState = 0f;
		}

		protected virtual void InitializeDecisions()
		{
			if (_decisions == null)
			{
				_decisions = GetAttachedDecisions();
			}
			foreach(AIDecision decision in _decisions)
			{
				decision.Initialization();
			}
		}

		protected virtual void InitializeActions()
		{
			if (_actions == null)
			{
				_actions = GetAttachedActions();
			}
			foreach(AIAction action in _actions)
			{
				action.Initialization();
			}
		}

		protected AIState FindState(string stateName)
		{
			foreach (AIState state in States)
			{
				if (state.StateName == stateName)
				{
					return state;
				}
			}
			if (stateName != "")
			{
				Debug.LogError("You're trying to transition to state '" + stateName + "' in " + this.gameObject.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
			}            
			return null;
		}

		protected virtual void StoreLastKnownPosition()
		{
			if (Target != null)
			{
				_lastKnownTargetPosition = Target.transform.position;
			}
		}

		public virtual void ResetBrain()
		{
			InitializeDecisions();
			InitializeActions();
			BrainActive = true;
			this.enabled = true;

			if (CurrentState != null)
			{
				CurrentState.ExitState();
				OnExitState();
			}
            
			if (States.Count > 0)
			{
				CurrentState = States[0];
				CurrentState?.EnterState();
			}  
		}
	}
}