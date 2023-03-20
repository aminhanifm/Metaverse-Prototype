using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace MetaversePrototype.Tools
{
	public class MPSimpleObjectPooler : MPObjectPooler 
	{
		public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();
		// the game object we'll instantiate 
		public GameObject GameObjectToPool;
		// the number of objects we'll add to the pool
		public int PoolSize = 20;
		// if true, the pool will automatically add objects to the itself if needed
		public bool PoolCanExpand = true;

		// the actual object pool
		protected List<GameObject> _pooledGameObjects;
		public List<GameObject> PooledGameObjects {get {return _pooledGameObjects;} set {_pooledGameObjects = value;}}
		protected int[] viewIDList;
		public int[] _ViewIDList {get {return viewIDList;}}
	    
		public List<MPSimpleObjectPooler> Owner { get; set; }
		private void OnDestroy() { Owner?.Remove(this); }
		private PhotonView photonView;

		protected override void Awake() {
			photonView = gameObject.MPGetComponentNoAlloc<PhotonView>();
			viewIDList = new int[0];	
		}

		public override void FillObjectPool()
		{
			if (GameObjectToPool == null)
			{
				return;
			}

			// if we've already created a pool, we exit
			if ((_objectPool != null) && (_objectPool.PooledGameObjects.Count > PoolSize))
			{
				return;
			}

			CreateWaitingPool ();

			// we initialize the list we'll use to 
			_pooledGameObjects = new List<GameObject>();

			int objectsToSpawn = PoolSize;

			if (_objectPool != null)
			{
				objectsToSpawn -= _objectPool.PooledGameObjects.Count;
				_pooledGameObjects = new List<GameObject>(_objectPool.PooledGameObjects);
			}

			// we add to the pool the specified number of objects
			for (int i = 0; i < objectsToSpawn; i++)
			{
				AddOneObjectToThePool ();
			}
		}

		protected override string DetermineObjectPoolName()
		{
			return ("[SimpleObjectPooler] " + GameObjectToPool.name);	
		}
	    	
		public override GameObject GetPooledGameObject()
		{
			// we go through the pool looking for an inactive object\
			for (int i=0; i< _pooledGameObjects.Count; i++)
			{
				if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
				{
					// if we find one, we return it
					#if UNITY_EDITOR
						EditorGUIUtility.PingObject(_pooledGameObjects[i]);
					#endif
					return _pooledGameObjects[i];
				}
			}
			// if we haven't found an inactive object (the pool is empty), and if we can extend it, we add one new object to the pool, and return it		
			if (PoolCanExpand)
			{
				return AddOneObjectToThePool();
			}
			// if the pool is empty and can't grow, we return nothing.
			return null;
		}
		
		[PunRPC]
		public void SetObjectsPool(int viewID){
			GameObject newGameObject = PhotonView.Find(viewID).gameObject;

			newGameObject.SetActive(false);
			// SceneManager.MoveGameObjectToScene(newGameObject, this.gameObject.scene);
			if (NestWaitingPool)
			{
				newGameObject.transform.SetParent(_waitingPool.transform);	
			}

			_pooledGameObjects.Add(newGameObject);

			_objectPool.PooledGameObjects.Add(newGameObject);
		}

		protected virtual GameObject AddOneObjectToThePool()
		{
			if(!PhotonNetwork.IsMasterClient){
				
				return null;
			}

			if (GameObjectToPool == null)
			{
				Debug.LogWarning("The "+gameObject.name+" ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
				return null;
			}

			if(_objectPool.PooledGameObjects.Count < PoolSize){
				bool initialStatus = GameObjectToPool.activeSelf;
				GameObjectToPool.SetActive(false);
				
				GameObject newGameObject = null;
				StringBuilder pooledObjName = new StringBuilder();
				pooledObjName.Append("NPC - " + _pooledGameObjects.Count);

				newGameObject = PhotonNetwork.InstantiateRoomObject("NPC", Vector3.zero, Quaternion.identity);
				
				PhotonView pvObj = newGameObject.MPGetComponentNoAlloc<PhotonView>();
				
				viewIDList = viewIDList.Append(pvObj.ViewID).ToArray();
				photonView.RPC("SetObjectsPool", RpcTarget.AllBufferedViaServer, pvObj.ViewID);
				// photonView.RPC("SetNPCListRPC", RpcTarget.MasterClient, pvObj.ViewID);
				return newGameObject;
			}
				
			return null;

		}
	
	}
}