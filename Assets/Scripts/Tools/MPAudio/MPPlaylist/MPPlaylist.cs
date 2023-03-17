using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{

	public struct MPPlaylistPlayEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(int channel);
		static public void Trigger(int channel)
		{
			OnEvent?.Invoke(channel);
		}
	}
	public struct MPPlaylistStopEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(int channel);
		static public void Trigger(int channel)
		{
			OnEvent?.Invoke(channel);
		}
	}
	public struct MPPlaylistPauseEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(int channel);
		static public void Trigger(int channel)
		{
			OnEvent?.Invoke(channel);
		}
	}
	public struct MPPlaylistPlayNextEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(int channel);
		static public void Trigger(int channel)
		{
			OnEvent?.Invoke(channel);
		}
	}
	public struct MPPlaylistPlayPreviousEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(int channel);
		static public void Trigger(int channel)
		{
			OnEvent?.Invoke(channel);
		}
	}

	public struct MPPlaylistPlayIndexEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(int channel, int index);
		static public void Trigger(int channel, int index)
		{
			OnEvent?.Invoke(channel, index);
		}
	}

	[System.Serializable]
	public class MPPlaylistSong
	{
		/// the audiosource that contains the audio clip we want to play
		public AudioSource TargetAudioSource;
		/// the min (when it's off) and max (when it's playing) volume for this source
		public Vector2 Volume = new Vector2(1f, 1f);
		/// a random delay in seconds to apply, between its RMin and RMax
		public Vector2 InitialDelay = Vector2.zero;
		/// a random crossfade duration (in seconds) to apply when transitioning to this song, between its RMin and RMax
		public Vector2 CrossFadeDuration = new Vector2(2f, 2f);
		/// a random pitch to apply to this song, between its RMin and RMax
		public Vector2 Pitch = Vector2.one;
		/// the stereo pan for this song
		[Range(-1f, 1f)]
		public float StereoPan = 0f;
		/// the spatial blend for this song (0 is 2D, 1 is 3D)
		[Range(0f, 1f)]
		public float SpatialBlend = 0f;
		/// whether this song should loop or not
		public bool Loop = false;
		/// whether this song is playing right now or not
		[ReadOnly]
		public bool Playing = false;
		/// whether this song is fading right now or not
		[ReadOnly]
		public bool Fading = false;

		public virtual void Initialization()
		{
			this.Volume = new Vector2(1f, 1f);
			this.InitialDelay = Vector2.zero;
			this.CrossFadeDuration = new Vector2(2f, 2f);
			this.Pitch = Vector2.one;
			this.StereoPan = 0f;
			this.SpatialBlend = 0f;
			this.Loop = false;
		}
	}

	public class MPPlaylist : MonoBehaviour
	{
		/// the possible states this playlist can be in
		public enum PlaylistStates
		{
			Idle,
			Playing,
			Paused
		}
		
		[BoxGroup("Playlist Songs")]
        
		/// the channel on which to broadcast orders for this playlist
		[Tooltip("the channel on which to broadcast orders for this playlist")]
		public int Channel = 0;
		/// the songs that this playlist will play
		[Tooltip("the songs that this playlist will play")]
		public List<MPPlaylistSong> Songs;

		[BoxGroup("Settings")]
		
		/// whether this should play in random order or not
		[Tooltip("whether this should play in random order or not")]
		public bool RandomOrder = false;
		/// if this is true, random seed will be randomized by the system clock
		[Tooltip("if this is true, random seed will be randomized by the system clock")]
		[ShowIf("RandomOrder", true)]
		public bool RandomizeOrderSeed = true;
		/// whether this playlist should play and loop as a whole forever or not
		[Tooltip("whether this playlist should play and loop as a whole forever or not")]
		public bool Endless = true;
		/// whether this playlist should auto play on start or not
		[Tooltip("whether this playlist should auto play on start or not")]
		public bool PlayOnStart = true;
		/// a global volume multiplier to apply when playing a song
		[Tooltip("a global volume multiplier to apply when playing a song")]
		public float VolumeMultiplier = 1f;
		/// if this is true, this playlist will automatically pause/resume OnApplicationPause, useful if you've prevented your game from running in the background
		[Tooltip("if this is true, this playlist will automatically pause/resume OnApplicationPause, useful if you've prevented your game from running in the background")]
		public bool AutoHandleApplicationPause = true;
		
		[BoxGroup("Persistence")]
		/// if this is true, this playlist will persist from scene to scene
		[Tooltip("if this is true, this playlist will persist from scene to scene")]
		public bool Persistent = false;
		/// if this is true, this singleton will auto detach if it finds itself parented on awake
		[Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
		[ShowIf("Persistent", true)]
		public bool AutomaticallyUnparentOnAwake = true;

		[BoxGroup("Status")]
		
		/// the current state of the playlist, debug display only
		[Tooltip("the current state of the playlist, debug display only")]
		[ReadOnly]
		public PlaylistStates DebugCurrentState = PlaylistStates.Idle;
		/// the index we're currently playing
		[Tooltip("the index we're currently playing")]
		[ReadOnly]
		public int CurrentlyPlayingIndex = -1;
		/// the name of the song that is currently playing
		[Tooltip("the name of the song that is currently playing")]
		[ReadOnly]
		public string CurrentSongName;
		/// the current state of this playlist
		[ReadOnly]
		public MPStateMachine<MPPlaylist.PlaylistStates> PlaylistState;

		[BoxGroup("Tests")]
		
		/// a play test button
		[Button("Play")]
		public bool PlayButton;
		/// a pause test button
		[Button("Pause")]
		public bool PauseButton;
		/// a stop test button
		[Button("Stop")]
		public bool StopButton;
		/// a next song test button
		[Button("PlayNextSong")]
		public bool NextButton;
		/// the index of the song to play when pressing the PlayTargetSong button
		[Tooltip("the index of the song to play when pressing the PlayTargetSong button")]
		public int TargetSongIndex = 0;
		/// a next song test button
		[Button("PlayTargetSong")]
		public bool TargetSongButton;
        
		protected int _songsPlayedSoFar = 0;
		protected int _songsPlayedThisCycle = 0;
		protected Coroutine _coroutine;
		protected bool _shouldResumeOnApplicationPause = false;
		
		public static bool HasInstance => _instance != null;
		public static MPPlaylist Current => _instance;
		protected static MPPlaylist _instance;
		protected bool _enabled;
		
		public static MPPlaylist Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<MPPlaylist> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject ();
						obj.name = typeof(MPPlaylist).Name + "_AutoCreated";
						_instance = obj.AddComponent<MPPlaylist> ();
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

			if (!Persistent)
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
				_instance = this;
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
        
		protected virtual void Start()
		{
			Initialization();
		}

		protected virtual void Initialization()
		{
			if (RandomOrder && RandomizeOrderSeed)
			{
				Random.InitState(System.Environment.TickCount);
			}
			_songsPlayedSoFar = 0;
			PlaylistState = new MPStateMachine<MPPlaylist.PlaylistStates>(this.gameObject, true);
			ChangePlaylistState(PlaylistStates.Idle);
			if (Songs.Count == 0)
			{
				return;
			}
			if (PlayOnStart)
			{
				PlayFirstSong();
			}
		}

		protected virtual void ChangePlaylistState(PlaylistStates newState)
		{
			PlaylistState.ChangeState(newState);
			DebugCurrentState = newState;
		}

		protected virtual void PlayFirstSong()
		{
			_songsPlayedThisCycle = 0;
			CurrentlyPlayingIndex = -1;
			int newIndex = PickNextIndex();
			_coroutine = StartCoroutine(PlaySong(newIndex));
		}

		protected virtual IEnumerator PlaySong(int index)
		{
			// if we don't have a song, we stop
			if (Songs.Count == 0)
			{
				yield break;
			}

			// if we've played all our songs, we stop
			if (!Endless && (_songsPlayedThisCycle > Songs.Count))
			{
				yield break;
			}

			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
            
			// we stop our current song                        
			if ((PlaylistState.CurrentState == PlaylistStates.Playing) && (index >= 0 && index < Songs.Count))
			{
				StartCoroutine(Fade(CurrentlyPlayingIndex,
					Random.Range(Songs[index].CrossFadeDuration.x, Songs[index].CrossFadeDuration.y),
					Songs[CurrentlyPlayingIndex].Volume.y * VolumeMultiplier,
					Songs[CurrentlyPlayingIndex].Volume.x * VolumeMultiplier,
					true));
			}

			// we stop all other coroutines
			if ((CurrentlyPlayingIndex >= 0) && (Songs.Count > CurrentlyPlayingIndex))
			{
				foreach (MPPlaylistSong song in Songs)
				{
					if (song != Songs[CurrentlyPlayingIndex])
					{
						song.Fading = false;
					}
				}
			}     
            
			if (index < 0 || index >= Songs.Count)
			{
				yield break;
			}

			// initial delay
			yield return MPCoroutine.WaitFor(Random.Range(Songs[index].InitialDelay.x, Songs[index].InitialDelay.y));

			if (Songs[index].TargetAudioSource == null)
			{
				Debug.LogError(this.name + " : the playlist song you're trying to play is null");
				yield break;
			}

			Songs[index].TargetAudioSource.pitch = Random.Range(Songs[index].Pitch.x, Songs[index].Pitch.y);
			Songs[index].TargetAudioSource.panStereo = Songs[index].StereoPan;
			Songs[index].TargetAudioSource.spatialBlend = Songs[index].SpatialBlend;
			Songs[index].TargetAudioSource.loop = Songs[index].Loop;
            
			// fades the new song's volume
			StartCoroutine(Fade(index,
				Random.Range(Songs[index].CrossFadeDuration.x, Songs[index].CrossFadeDuration.y),
				Songs[index].Volume.x * VolumeMultiplier,
				Songs[index].Volume.y * VolumeMultiplier,
				false));

			// starts the new song
			Songs[index].TargetAudioSource.Play();

			// updates our state
			CurrentSongName = Songs[index].TargetAudioSource.clip.name;
			ChangePlaylistState(PlaylistStates.Playing);
			Songs[index].Playing = true;
			CurrentlyPlayingIndex = index;
			_songsPlayedSoFar++;
			_songsPlayedThisCycle++;

			while (Songs[index].TargetAudioSource.isPlaying || (PlaylistState.CurrentState == PlaylistStates.Paused) || _shouldResumeOnApplicationPause)
			{
				yield return null;
			}

			if (PlaylistState.CurrentState != PlaylistStates.Playing)
			{
				yield break;
			}
            
			if (_songsPlayedSoFar < Songs.Count)
			{
				_coroutine = StartCoroutine(PlaySong(PickNextIndex()));
			}
			else
			{
				if (Endless)
				{
					_coroutine = StartCoroutine(PlaySong(PickNextIndex()));
				}
				else
				{
					ChangePlaylistState(PlaylistStates.Idle);
				}
			}
		}

		protected virtual IEnumerator Fade(int index, float duration, float initialVolume, float endVolume, bool stopAtTheEnd)
		{
			if (index < 0 || index >= Songs.Count)
			{
				yield break;
			}

			float startTimestamp = Time.time;
			float progress = 0f;
			Songs[index].Fading = true;

			while ((Time.time - startTimestamp < duration) && (Songs[index].Fading))
			{
				progress = MPMaths.Remap(Time.time - startTimestamp, 0f, duration, 0f, 1f);
				Songs[index].TargetAudioSource.volume = Mathf.Lerp(initialVolume, endVolume, progress);
				yield return null;
			}

			Songs[index].TargetAudioSource.volume = endVolume;

			if (stopAtTheEnd)
			{
				Songs[index].TargetAudioSource.Stop();
				Songs[index].Playing = false;
				Songs[index].Fading = false;
			}
		}

		protected virtual int PickNextIndex()
		{
			if (Songs.Count == 0)
			{
				return -1;
			}

			int newIndex = CurrentlyPlayingIndex;
			if (RandomOrder)
			{
				while (newIndex == CurrentlyPlayingIndex)
				{
					newIndex = Random.Range(0, Songs.Count);
				}                
			}
			else
			{
				newIndex = (CurrentlyPlayingIndex + 1) % Songs.Count;
			}

			return newIndex;
		}

		protected virtual int PickPreviousIndex()
		{
			if (Songs.Count == 0)
			{
				return -1;
			}

			int newIndex = CurrentlyPlayingIndex;
			if (RandomOrder)
			{
				while (newIndex == CurrentlyPlayingIndex)
				{
					newIndex = Random.Range(0, Songs.Count);
				}                
			}
			else
			{
				newIndex = (CurrentlyPlayingIndex - 1);
				if (newIndex < 0)
				{
					newIndex = Songs.Count - 1;
				}
			}

			return newIndex;
		}


		public virtual void Play()
		{
			switch (PlaylistState.CurrentState)
			{
				case PlaylistStates.Idle:
					PlayFirstSong();
					break;

				case PlaylistStates.Paused:
					Songs[CurrentlyPlayingIndex].TargetAudioSource.UnPause();
					ChangePlaylistState(PlaylistStates.Playing);
					break;

				case PlaylistStates.Playing:
					// do nothing
					break;
			}
		}

		public virtual void PlayAtIndex(int songIndex)
		{
			_coroutine = StartCoroutine(PlaySong(songIndex));
		}
        
		public virtual void Pause()
		{
			if (PlaylistState.CurrentState != PlaylistStates.Playing)
			{
				return;
			}

			Songs[CurrentlyPlayingIndex].TargetAudioSource.Pause();
			ChangePlaylistState(PlaylistStates.Paused);
		}

		public virtual void Stop()
		{
			if (PlaylistState.CurrentState != PlaylistStates.Playing)
			{
				return;
			} 
	        
			Songs[CurrentlyPlayingIndex].TargetAudioSource.Stop();
			Songs[CurrentlyPlayingIndex].Playing = false;
			Songs[CurrentlyPlayingIndex].Fading = false;
			CurrentlyPlayingIndex = -1;
			ChangePlaylistState(PlaylistStates.Idle);
		}

		public virtual void PlayNextSong()
		{
			int newIndex = PickNextIndex();
			_coroutine = StartCoroutine(PlaySong(newIndex));
		}

		public virtual void PlayPreviousSong()
		{
			int newIndex = PickPreviousIndex();
			_coroutine = StartCoroutine(PlaySong(newIndex));
		}

		protected virtual void PlayTargetSong()
		{
			int newIndex = Mathf.Clamp(TargetSongIndex, 0, Songs.Count - 1);
			PlayAtIndex(newIndex);
		}

		protected virtual void OnPlayEvent(int channel)
		{
			if (channel != Channel) { return; }
			Play();
		}

		protected virtual void OnPauseEvent(int channel)
		{
			if (channel != Channel) { return; }
			Pause();
		}

		protected virtual void OnStopEvent(int channel)
		{
			if (channel != Channel) { return; }
			Stop();
		}

		protected virtual void OnPlayNextEvent(int channel)
		{
			if (channel != Channel) { return; }
			PlayNextSong();
		}

		protected virtual void OnPlayPreviousEvent(int channel)
		{
			if (channel != Channel) { return; }
			PlayPreviousSong();
		}

		protected virtual void OnPlayIndexEvent(int channel, int index)
		{
			if (channel != Channel) { return; }
			_coroutine = StartCoroutine(PlaySong(index));
		}

		protected virtual void OnEnable()
		{
			MPPlaylistPauseEvent.Register(OnPauseEvent);
			MPPlaylistPlayEvent.Register(OnPlayEvent);
			MPPlaylistPlayNextEvent.Register(OnPlayNextEvent);
			MPPlaylistPlayPreviousEvent.Register(OnPlayPreviousEvent);
			MPPlaylistStopEvent.Register(OnStopEvent);
			MPPlaylistPlayIndexEvent.Register(OnPlayIndexEvent);
		}

		protected virtual void OnDisable()
		{
			MPPlaylistPauseEvent.Unregister(OnPauseEvent);
			MPPlaylistPlayEvent.Unregister(OnPlayEvent);
			MPPlaylistPlayNextEvent.Unregister(OnPlayNextEvent);
			MPPlaylistPlayPreviousEvent.Unregister(OnPlayPreviousEvent);
			MPPlaylistStopEvent.Unregister(OnStopEvent);
			MPPlaylistPlayIndexEvent.Unregister(OnPlayIndexEvent);
		}
        
		protected bool _firstDeserialization = true;
		protected int _listCount = 0;

		protected virtual void OnValidate()
		{
			if (_firstDeserialization)
			{
				if (Songs == null)
				{
					_listCount = 0;
					_firstDeserialization = false;
				}
				else
				{
					_listCount = Songs.Count;
					_firstDeserialization = false;
				}                
			}
			else
			{
				if (Songs.Count != _listCount)
				{
					if (Songs.Count > _listCount)
					{
						foreach(MPPlaylistSong song in Songs)
						{
							song.Initialization();
						}                            
					}
					_listCount = Songs.Count;
				}
			}
		}

		protected virtual void OnApplicationPause(bool pauseStatus)
		{
			if (!AutoHandleApplicationPause)
			{
				return;
			}
			
			if (pauseStatus && PlaylistState.CurrentState == PlaylistStates.Playing)
			{
				Pause();
				_shouldResumeOnApplicationPause = true;
			}

			if (!pauseStatus && _shouldResumeOnApplicationPause)
			{
				_shouldResumeOnApplicationPause = false;
				Play();
			}
		}
	}
}