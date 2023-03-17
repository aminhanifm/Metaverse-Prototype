using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace MetaversePrototype.Tools
{
	public class MPSimpleObjectPooler : MPObjectPooler 
	{
		// the game object we'll instantiate 
		public GameObject GameObjectToPool;
		// the number of objects we'll add to the pool
		public int PoolSize = 20;
		// if true, the pool will automatically add objects to the itself if needed
		public bool PoolCanExpand = true;

		// the actual object pool
		protected List<GameObject> _pooledGameObjects;
	    
		public List<MPSimpleObjectPooler> Owner { get; set; }
		private void OnDestroy() { Owner?.Remove(this); }

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
			// we go through the pool looking for an inactive object
			for (int i=0; i< _pooledGameObjects.Count; i++)
			{
				if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
				{
					// if we find one, we return it
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
		
		protected virtual GameObject AddOneObjectToThePool()
		{
			if (GameObjectToPool == null)
			{
				Debug.LogWarning("The "+gameObject.name+" ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
				return null;
			}

			bool initialStatus = GameObjectToPool.activeSelf;
			GameObjectToPool.SetActive(false);
			GameObject newGameObject = (GameObject)Instantiate(GameObjectToPool);
			GameObjectToPool.SetActive(initialStatus);
			SceneManager.MoveGameObjectToScene(newGameObject, this.gameObject.scene);
			if (NestWaitingPool)
			{
				newGameObject.transform.SetParent(_waitingPool.transform);	
			}
			newGameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;

			_pooledGameObjects.Add(newGameObject);

			_objectPool.PooledGameObjects.Add(newGameObject);

			return newGameObject;
		}
	}
}