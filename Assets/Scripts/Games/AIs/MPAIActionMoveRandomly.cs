using UnityEngine;
using UnityEngine.AI;
using MetaversePrototype.Tools;

namespace MetaversePrototype.Game
{
    public class MPAIActionMoveRandomly : AIAction
    {
        [SerializeField] private float radius = 10f;

        public override void PerformAction(){
            if (OnlyRunOnce && _alreadyRan)
			{
				return;
			}

			_alreadyRan = true;

            Vector3 randomPoint = Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
            {
                // hit.position is a point on the NavMesh
                // use it to set the agent's destination
                _agent.SetDestination(hit.position);
            }

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
