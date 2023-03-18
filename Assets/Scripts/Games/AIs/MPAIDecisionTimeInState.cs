using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaversePrototype.Tools;

namespace MetaversePrototype.Game
{

	public class MPAIDecisionTimeInState : AIDecision
	{
		/// The minimum duration, in seconds, after which to return true
		[Tooltip("The minimum duration, in seconds, after which to return true")]
		public float AfterTimeMin = 2f;
		/// The maximum duration, in seconds, after which to return true
		[Tooltip("The maximum duration, in seconds, after which to return true")]
		public float AfterTimeMax = 2f;

		protected float _randomTime;

		public override bool Decide()
		{
			return EvaluateTime();
		}

		protected virtual bool EvaluateTime()
		{
			if (_brain == null) { return false; }
			return (_brain.TimeInThisState >= _randomTime);
		}

		public override void Initialization()
		{
			base.Initialization();
			RandomizeTime();
		}

		public override void OnEnterState()
		{
			base.OnEnterState();
			RandomizeTime();
		}

		protected virtual void RandomizeTime()
		{
			_randomTime = Random.Range(AfterTimeMin, AfterTimeMax);
		}
	}
}