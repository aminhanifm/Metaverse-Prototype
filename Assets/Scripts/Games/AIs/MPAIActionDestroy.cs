using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using MetaversePrototype.Tools;
using Photon.Pun;

namespace MetaversePrototype.Game
{
    public class MPAIActionDestroy : AIAction
    {
		// protected const string idleState = "Idle";

        public override void PerformAction(){
            if (OnlyRunOnce && _alreadyRan)
			{
				return;
			}

			_alreadyRan = true;

			// _brain.TransitionToState(idleState);
			// MPGameManager.Instance._NPCCount--; //Keep track of NPC count
            // MPPoolableObject selfObject = gameObject.MPGetComponentAroundOrAdd<MPPoolableObject>();
			// selfObject.Destroy();
			if(PhotonNetwork.IsMasterClient) _brain.Owner.MPGetComponentNoAlloc<PhotonView>().RPC("DestroyNPC", RpcTarget.AllBufferedViaServer);
        }

		[Button]
		public void DestroyNow(){
			if(PhotonNetwork.IsMasterClient) _brain.Owner.MPGetComponentNoAlloc<PhotonView>().RPC("DestroyNPC", RpcTarget.AllBufferedViaServer);
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
