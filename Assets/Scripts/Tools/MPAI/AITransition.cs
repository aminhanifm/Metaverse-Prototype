﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	[System.Serializable]
	public class AITransition 
	{
		/// this transition's decision
		public AIDecision Decision;
		/// the state to transition to if this Decision returns true
		public string TrueState;
		/// the state to transition to if this Decision returns false
		public string FalseState;
	}
}