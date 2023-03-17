using UnityEngine;

namespace MetaversePrototype.Tools
{

	public class MPPersistentSingleton<T> : MonoBehaviour	where T : Component
	{
		[Header("Persistent Singleton")]
		/// if this is true, this singleton will auto detach if it finds itself parented on awake
		[Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
		public bool AutomaticallyUnparentOnAwake = true;
		
		public static bool HasInstance => _instance != null;
		public static T Current => _instance;
		
		protected static T _instance;
		protected bool _enabled;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject ();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T> ();
					}
				}
				return _instance;
			}
		}

		protected virtual void Awake ()
		{
			InitializeSingleton();
		}

		protected virtual void InitializeSingleton()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (AutomaticallyUnparentOnAwake)
			{
				this.transform.SetParent(null);
			}

			if (_instance == null)
			{
				//If I am the first instance, make me the Singleton
				_instance = this as T;
				DontDestroyOnLoad (transform.gameObject);
				_enabled = true;
			}
			else
			{
				//If a Singleton already exists and you find
				//another reference in scene, destroy it!
				if(this != _instance)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
}