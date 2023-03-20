using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaversePrototype.Tools;
using Photon.Pun;

namespace MetaversePrototype.Game
{
	public class MPAIDecisionArrivedToLocation : AIDecision
	{

		public override bool Decide()
		{
			return EvaluateDistance();
		}

		protected virtual bool EvaluateDistance()
		{
			
			if(PhotonNetwork.IsMasterClient){
				if (_agent.velocity.magnitude > 0 && _agent.remainingDistance <= _agent.stoppingDistance)
				{
					_agent.velocity = Vector3.zero;
					// print("Arrived");
					return true;
				}
			}
			


			return false;
		}

		public override void Initialization()
		{
			base.Initialization();
		}

		public override void OnEnterState()
		{
			base.OnEnterState();
		}

	}
}