using UnityEngine;
using UnityEngine.AI;
using MetaversePrototype.Tools;
using Random = UnityEngine.Random;

namespace MetaversePrototype.Game
{
    public class MPAIActionMoveToLocation : AIAction
    {
		protected MPCharacterManager characterManager;
        public override void PerformAction(){
            if (OnlyRunOnce && _alreadyRan)
			{
				return;
			}

			_alreadyRan = true;
			int patrolPoint = Random.Range(0, characterManager._AvailablePatrolPoint.Length);
			_agent.SetDestination(characterManager._AvailablePatrolPoint[patrolPoint].transform.position);

		}

        protected override void Awake()
		{
			base.Awake();

			characterManager = gameObject.MPGetComponentAroundOrAdd<MPCharacterManager>();
		}

		public override void Initialization()
		{
			base.Initialization();
		}

		public override void OnEnterState()
		{
			base.OnEnterState();
			_alreadyRan = false;
		}

		public override void OnExitState()
		{
			base.OnExitState();
		}
    }
}
