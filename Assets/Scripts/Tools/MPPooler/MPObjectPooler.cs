using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{	
	public abstract class MPObjectPooler : MonoBehaviour
	{
		// singleton pattern
		public static MPObjectPooler Instance;
		// if this is true, the pool will try not to create a new waiting pool if it finds one with the same name.
		public bool MutualizeWaitingPools = false;
		// if this is true, all waiting and active objects will be regrouped under an empty game object. Otherwise they'll just be at top level in the hierarchy
		public bool NestWaitingPool = true;
		// if this is true, the waiting pool will be nested under this object
		[ShowIf("NestWaitingPool", true)] 
		public bool NestUnderThis = false;

		// this object is just used to group the pooled objects
		protected GameObject _waitingPool = null;
		public GameObject WaitingPool {get {return _waitingPool;}}
		protected MPObjectPool _objectPool;
		public MPObjectPool ObjectPool {get {return _objectPool;}}
		protected const int _initialPoolsListCapacity = 5;
		protected bool _onSceneLoadedRegistered = false;
        
		public static List<MPObjectPool> _pools = new List<MPObjectPool>(_initialPoolsListCapacity);

		public static void AddPool(MPObjectPool pool)
		{
			if (_pools == null)
			{
				_pools = new List<MPObjectPool>(_initialPoolsListCapacity);    
			}
			if (!_pools.Contains(pool))
			{
				_pools.Add(pool);
			}
		}

		public static void RemovePool(MPObjectPool pool)
		{
			_pools?.Remove(pool);
		}

		protected virtual void Awake()
		{
			Instance = this;
			FillObjectPool();
			
		}

		protected virtual bool CreateWaitingPool()
		{
			if (!MutualizeWaitingPools)
			{
				// we create a container that will hold all the instances we create
				_waitingPool = new GameObject(DetermineObjectPoolName());
				SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
				_objectPool = _waitingPool.AddComponent<MPObjectPool>();
				_objectPool.PooledGameObjects = new List<GameObject>();
				ApplyNesting();
				return true;
			}
			else
			{
				MPObjectPool objectPool = ExistingPool(DetermineObjectPoolName());
				if (objectPool != null)
				{
					_objectPool = objectPool;
					_waitingPool = objectPool.gameObject;
					return false;
				}
				else
				{
					_waitingPool = new GameObject(DetermineObjectPoolName());
					SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
					_objectPool = _waitingPool.AddComponent<MPObjectPool>();
					_objectPool.PooledGameObjects = new List<GameObject>();
					ApplyNesting();
					AddPool(_objectPool);
					return true;
				}
			}
		}
        
		public virtual MPObjectPool ExistingPool(string poolName)
		{
			if (_pools == null)
			{
				_pools = new List<MPObjectPool>(_initialPoolsListCapacity);    
			}
			if (_pools.Count == 0)
			{
				var pools = FindObjectsOfType<MPObjectPool>();
				if (pools.Length > 0)
				{
					_pools.AddRange(pools);
				}
			}
			foreach (MPObjectPool pool in _pools)
			{
				if ((pool != null) && (pool.name == poolName)/* && (pool.gameObject.scene == this.gameObject.scene)*/)
				{
					return pool;
				}
			}
			return null;
		}

		protected virtual void ApplyNesting()
		{
			if (NestWaitingPool && NestUnderThis && (_waitingPool != null))
			{
				_waitingPool.transform.SetParent(this.transform);
			}
		}

		protected virtual string DetermineObjectPoolName()
		{
			return ("[ObjectPooler] " + this.name);	
		}

		public virtual void FillObjectPool()
		{
			return ;
		}

		public virtual GameObject GetPooledGameObject()
		{
			return null;
		}

		public virtual void DestroyObjectPool()
		{
			if (_waitingPool != null)
			{
				Destroy(_waitingPool.gameObject);
			}
		}

		protected virtual void OnEnable()
		{
			if (!_onSceneLoadedRegistered)
			{
				SceneManager.sceneLoaded += OnSceneLoaded;    
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (this == null)
			{
				return;
			}
			if ((_objectPool == null) || (_waitingPool == null))
			{
				if (this != null)
				{
					FillObjectPool();    
				}
			}
		}
        
		private void OnDestroy()
		{
			if ((_objectPool != null) && NestUnderThis)
			{
				RemovePool(_objectPool);    
			}

			if (_onSceneLoadedRegistered)
			{
				SceneManager.sceneLoaded -= OnSceneLoaded;
				_onSceneLoadedRegistered = false;
			}
		}
	}
}