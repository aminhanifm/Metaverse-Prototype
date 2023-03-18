using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MetaversePrototype.Tools
{
	public abstract class AIDecision : MonoBehaviour
	{
		/// Decide will be performed every frame while the Brain is in a state this Decision is in. Should return true or false, which will then determine the transition's outcome.
		public abstract bool Decide();

		public string Label;
		public bool DecisionInProgress { get; set; }
		protected AIBrain _brain;
		protected Animator _animator;
		protected NavMeshAgent _agent;
        
		protected virtual void Awake()
		{
			_brain = this.gameObject.MPGetComponentAroundOrAdd<AIBrain>();
			_animator = this.gameObject.MPGetComponentAroundOrAdd<Animator>();
			_agent = this.gameObject.MPGetComponentAroundOrAdd<NavMeshAgent>();
		}

		public virtual void Initialization()
		{

		}

		public virtual void OnEnterState()
		{
			DecisionInProgress = true;
		}

		public virtual void OnExitState()
		{
			DecisionInProgress = false;
		}
	}
}