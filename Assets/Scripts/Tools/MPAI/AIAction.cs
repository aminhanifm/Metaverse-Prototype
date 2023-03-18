using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MetaversePrototype.Tools
{
	public abstract class AIAction : MonoBehaviour
	{
		public enum InitializationModes { EveryTime, OnlyOnce, }

		public InitializationModes InitializationMode;
		protected bool _initialized;
		public bool OnlyRunOnce = true;
		protected bool _alreadyRan = false;
		public string Label;
		public abstract void PerformAction();
		public bool ActionInProgress { get; set; }
		protected AIBrain _brain;
		protected Animator _animator;
		protected NavMeshAgent _agent;

		protected virtual bool ShouldInitialize
		{
			get
			{
				switch (InitializationMode)
				{
					case InitializationModes.EveryTime:
						return true;
					case InitializationModes.OnlyOnce:
						return _initialized == false;
				}
				return true;
			}
		}

		protected virtual void Awake()
		{
			_brain = this.gameObject.MPGetComponentAroundOrAdd<AIBrain>();
			_animator = this.gameObject.MPGetComponentAroundOrAdd<Animator>();
			_agent = this.gameObject.MPGetComponentAroundOrAdd<NavMeshAgent>();
		}

		public virtual void Initialization()
		{
			_initialized = true;
		}

		public virtual void OnEnterState()
		{
			ActionInProgress = true;
		}

		public virtual void OnExitState()
		{
			ActionInProgress = false;
		}
	}
}