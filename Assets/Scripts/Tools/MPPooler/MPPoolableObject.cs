using UnityEngine;
using UnityEngine.Events;

namespace MetaversePrototype.Tools
{
	public class MPPoolableObject : MPObjectBounds
	{
		[Header("Events")]
		public UnityEvent ExecuteOnEnable;
		public UnityEvent ExecuteOnDisable;
		
		public delegate void Events();
		public event Events OnSpawnComplete;

		[Header("Poolable Object")]
		// The life time, in seconds, of the object. If set to 0 it'll live forever, if set to any positive value it'll be set inactive after that time.
		public float LifeTime = 0f;

		public virtual void Destroy()
		{
			gameObject.SetActive(false);
		}

		protected virtual void Update()
		{

		}

		protected virtual void OnEnable()
		{
			Size = GetBounds().extents * 2;
			if (LifeTime > 0f)
			{
				Invoke("Destroy", LifeTime);	
			}
			ExecuteOnEnable?.Invoke();
		}

		protected virtual void OnDisable()
		{
			ExecuteOnDisable?.Invoke();
			CancelInvoke();
		}

		public virtual void TriggerOnSpawnComplete()
		{
			OnSpawnComplete?.Invoke();
		}
	}
}