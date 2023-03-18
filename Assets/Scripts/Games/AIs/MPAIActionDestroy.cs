using UnityEngine;
using UnityEngine.AI;
using MetaversePrototype.Tools;

namespace MetaversePrototype.Game
{
    public class MPAIActionDestroy : AIAction
    {
		protected const string idleState = "Idle";

        public override void PerformAction(){
            if (OnlyRunOnce && _alreadyRan)
			{
				return;
			}

			_alreadyRan = true;

			_brain.TransitionToState(idleState);
			MPGameManager.Instance._NPCCount--; //Keep track of NPC count
            MPPoolableObject selfObject = gameObject.MPGetComponentAroundOrAdd<MPPoolableObject>();
			selfObject.Destroy();

        }

        protected override void Awake()
		{
			base.Awake();
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
