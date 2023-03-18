using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace MetaversePrototype.Tools
{
	public class MPSoundManager : MPPersistentSingleton<MPSoundManager>, 
		MPEventListener<MPSoundManagerTrackEvent>, 
		MPEventListener<MPSoundManagerEvent>,
		MPEventListener<MPSoundManagerSoundControlEvent>,
		MPEventListener<MPSoundManagerSoundFadeEvent>,
		MPEventListener<MPSoundManagerAllSoundsControlEvent>,
		MPEventListener<MPSoundManagerTrackFadeEvent>
	{
		/// the possible ways to manage a track
		public enum MPSoundManagerTracks { Sfx, Music, UI, Master, Other}
        
		[Header("Settings")]
		/// the current sound settings 
		[Tooltip("the current sound settings ")]
		public MPSoundManagerSettingsSO settingsSo;

		[Header("Pool")]
		/// the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once 
		[Tooltip("the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once")]
		public int AudioSourcePoolSize = 10;
		/// whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.
		[Tooltip("whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.")]
		public bool PoolCanExpand = true;
        
		protected MPSoundManagerAudioPool _pool;
		protected GameObject _tempAudioSourceGameObject;
		protected MPSoundManagerSound _sound;
		protected List<MPSoundManagerSound> _sounds; 
		protected AudioSource _tempAudioSource;

		#region Initialization

		protected override void Awake()
		{
			base.Awake();
			InitializeSoundManager();
		}
        
		protected virtual void Start()
		{
			if ((settingsSo != null) && (settingsSo.Settings.AutoLoad))
			{
				settingsSo.LoadSoundSettings();    
			}
		}

		protected virtual void InitializeSoundManager()
		{
			if (_pool == null)
			{
				_pool = new MPSoundManagerAudioPool();    
			}
			_sounds = new List<MPSoundManagerSound>();
			_pool.FillAudioSourcePool(AudioSourcePoolSize, this.transform);
		}
        
		#endregion
        
		#region PlaySound

		public virtual AudioSource PlaySound(AudioClip audioClip, MPSoundManagerPlayOptions options)
		{
			return PlaySound(audioClip, options.MPSoundManagerTrack, options.Location,
				options.Loop, options.Volume, options.ID,
				options.Fade, options.FadeInitialVolume, options.FadeDuration, options.FadeTween,
				options.Persistent,
				options.RecycleAudioSource, options.AudioGroup,
				options.Pitch, options.PanStereo, options.SpatialBlend,
				options.SoloSingleTrack, options.SoloAllTracks, options.AutoUnSoloOnEnd,
				options.BypassEffects, options.BypassListenerEffects, options.BypassReverbZones, options.Priority,
				options.ReverbZoneMix,
				options.DopplerLevel, options.Spread, options.RolloffMode, options.MinDistance, options.MaxDistance, 
				options.DoNotAutoRecycleIfNotDonePlaying, options.PlaybackTime, options.AttachToTransform,
				options.UseSpreadCurve, options.SpreadCurve, options.UseCustomRolloffCurve, options.CustomRolloffCurve,
				options.UseSpatialBlendCurve, options.SpatialBlendCurve, options.UseReverbZoneMixCurve, options.ReverbZoneMixCurve
			);
		}

		public virtual AudioSource PlaySound(AudioClip audioClip, MPSoundManagerTracks MPSoundManagerTrack, Vector3 location, 
			bool loop = false, float volume = 1.0f, int ID = 0,
			bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, MPTweenType fadeTween = null,
			bool persistent = false,
			AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
			float pitch = 1f, float panStereo = 0f, float spatialBlend = 0.0f,  
			bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,  
			bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false, int priority = 128, float reverbZoneMix = 1f,
			float dopplerLevel = 1f, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic, float minDistance = 1f, float maxDistance = 500f,
			bool doNotAutoRecycleIfNotDonePlaying = false, float playbackTime = 0f, Transform attachToTransform = null,
			bool useSpreadCurve = false, AnimationCurve spreadCurve = null, bool useCustomRolloffCurve = false, AnimationCurve customRolloffCurve = null,
			bool useSpatialBlendCurve = false, AnimationCurve spatialBlendCurve = null, bool useReverbZoneMixCurve = false, AnimationCurve reverbZoneMixCurve = null
		)
		{
			if (this == null) { return null; }
			if (!audioClip) { return null; }
            
			// audio source setup ---------------------------------------------------------------------------------
            
			// we reuse an audiosource if one is passed in parameters
			AudioSource audioSource = recycleAudioSource;   
            
			if (audioSource == null)
			{
				// we pick an idle audio source from the pool if possible
				audioSource = _pool.GetAvailableAudioSource(PoolCanExpand, this.transform);
				if ((audioSource != null) && (!loop))
				{
					recycleAudioSource = audioSource;
					// we destroy the host after the clip has played (if it not tag for reusability.
					StartCoroutine(_pool.AutoDisableAudioSource(audioClip.length / Mathf.Abs(pitch), audioSource, audioClip, doNotAutoRecycleIfNotDonePlaying));
				}
			}

			// we create an audio source if needed
			if (audioSource == null)
			{
				_tempAudioSourceGameObject = new GameObject("MPAudio_"+audioClip.name);
				SceneManager.MoveGameObjectToScene(_tempAudioSourceGameObject, this.gameObject.scene);
				audioSource = _tempAudioSourceGameObject.AddComponent<AudioSource>();
			}
            
			// audio source settings ---------------------------------------------------------------------------------
            
			audioSource.transform.position = location;
			audioSource.clip = audioClip;
			audioSource.pitch = pitch;
			audioSource.spatialBlend = spatialBlend;
			audioSource.panStereo = panStereo;
			audioSource.loop = loop;
			audioSource.bypassEffects = bypassEffects;
			audioSource.bypassListenerEffects = bypassListenerEffects;
			audioSource.bypassReverbZones = bypassReverbZones;
			audioSource.priority = priority;
			audioSource.reverbZoneMix = reverbZoneMix;
			audioSource.dopplerLevel = dopplerLevel;
			audioSource.spread = spread;
			audioSource.rolloffMode = rolloffMode;
			audioSource.minDistance = minDistance;
			audioSource.maxDistance = maxDistance;
			audioSource.time = playbackTime; 
			
			// curves
			if (useSpreadCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.Spread, spreadCurve); }
			if (useCustomRolloffCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve); }
			if (useSpatialBlendCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialBlendCurve); }
			if (useReverbZoneMixCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, reverbZoneMixCurve); }
			
			// attaching to target
			if (attachToTransform != null)
			{
				MPFollowTarget followTarget = audioSource.gameObject.MPGetComponentNoAlloc<MPFollowTarget>();
				followTarget.Target = attachToTransform;
				followTarget.InterpolatePosition = false;
				followTarget.InterpolateRotation = false;
				followTarget.InterpolateScale = false;
				followTarget.FollowRotation = false;
				followTarget.FollowScale = false;
				followTarget.enabled = true;
			}
            
			// track and volume ---------------------------------------------------------------------------------
            
			if (settingsSo != null)
			{
				audioSource.outputAudioMixerGroup = settingsSo.MasterAudioMixerGroup;
				switch (MPSoundManagerTrack)
				{
					case MPSoundManagerTracks.Master:
						audioSource.outputAudioMixerGroup = settingsSo.MasterAudioMixerGroup;
						break;
					case MPSoundManagerTracks.Music:
						audioSource.outputAudioMixerGroup = settingsSo.MusicAudioMixerGroup;
						break;
					case MPSoundManagerTracks.Sfx:
						audioSource.outputAudioMixerGroup = settingsSo.SfxAudioMixerGroup;
						break;
					case MPSoundManagerTracks.UI:
						audioSource.outputAudioMixerGroup = settingsSo.UIAudioMixerGroup;
						break;
				}
			}
			if (audioGroup) { audioSource.outputAudioMixerGroup = audioGroup; }
			audioSource.volume = volume;  
            
			// we start playing the sound
			audioSource.Play();
            
			// we destroy the host after the clip has played if it was a one time AS.
			if (!loop && !recycleAudioSource)
			{
				Destroy(_tempAudioSourceGameObject, audioClip.length);
			}
            
			// we fade the sound in if needed
			if (fade)
			{
				FadeSound(audioSource, fadeDuration, fadeInitialVolume, volume, fadeTween);
			}
            
			// we handle soloing
			if (soloSingleTrack)
			{
				MuteSoundsOnTrack(MPSoundManagerTrack, true, 0f);
				audioSource.mute = false;
				if (autoUnSoloOnEnd)
				{
					MuteSoundsOnTrack(MPSoundManagerTrack, false, audioClip.length);
				}
			}
			else if (soloAllTracks)
			{
				MuteAllSounds();
				audioSource.mute = false;
				if (autoUnSoloOnEnd)
				{
					StartCoroutine(MuteAllSoundsCoroutine(audioClip.length - playbackTime, false));
				}
			}
            
			// we prepare for storage
			_sound.ID = ID;
			_sound.Track = MPSoundManagerTrack;
			_sound.Source = audioSource;
			_sound.Persistent = persistent;

			// we check if that audiosource is already being tracked in _sounds
			bool alreadyIn = false;
			for (int i = 0; i < _sounds.Count; i++)
			{
				if (_sounds[i].Source == audioSource)
				{
					_sounds[i] = _sound;
					alreadyIn = true;
				}
			}

			if (!alreadyIn)
			{
				_sounds.Add(_sound);    
			}

			// we return the audiosource reference
			return audioSource;
		}
        
		#endregion

		#region SoundControls

		public virtual void PauseSound(AudioSource source)
		{
			source.Pause();
		}

		public virtual void ResumeSound(AudioSource source)
		{
			source.Play();
		}

		public virtual void StopSound(AudioSource source)
		{
			source.Stop();
		}
        
		public virtual void FreeSound(AudioSource source)
		{
			source.Stop();
			if (!_pool.FreeSound(source))
			{
				Destroy(source.gameObject);    
			}
		}

		#endregion
        
		#region TrackControls
        
		public virtual void MuteTrack(MPSoundManagerTracks track)
		{
			ControlTrack(track, ControlTrackModes.Mute, 0f);
		}

		public virtual void UnmuteTrack(MPSoundManagerTracks track)
		{
			ControlTrack(track, ControlTrackModes.Unmute, 0f);
		}

		public virtual void SetTrackVolume(MPSoundManagerTracks track, float volume)
		{
			ControlTrack(track, ControlTrackModes.SetVolume, volume);
		}

		public virtual float GetTrackVolume(MPSoundManagerTracks track, bool mutedVolume)
		{
			switch (track)
			{
				case MPSoundManagerTracks.Master:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedMasterVolume;
					}
					else
					{
						return settingsSo.Settings.MasterVolume;
					}
				case MPSoundManagerTracks.Music:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedMusicVolume;
					}
					else
					{
						return settingsSo.Settings.MusicVolume;
					}
				case MPSoundManagerTracks.Sfx:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedSfxVolume;
					}
					else
					{
						return settingsSo.Settings.SfxVolume;
					}
				case MPSoundManagerTracks.UI:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedUIVolume;
					}
					else
					{
						return settingsSo.Settings.UIVolume;
					}
			}

			return 1f;
		}
        
		public virtual void PauseTrack(MPSoundManagerTracks track)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Pause();
				}
			}    
		}

		public virtual void PlayTrack(MPSoundManagerTracks track)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Play();
				}
			}    
		}

		public virtual void StopTrack(MPSoundManagerTracks track)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Stop();
				}
			}
		}

		public virtual bool HasSoundsPlaying(MPSoundManagerTracks track)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if ((sound.Track == track) && (sound.Source.isPlaying))
				{
					return true;
				}
			}
			return false;
		}

		public virtual void FreeTrack(MPSoundManagerTracks track)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Stop();
					sound.Source.gameObject.SetActive(false);
				}
			}
		}
        
		public virtual void MuteMusic() { MuteTrack(MPSoundManagerTracks.Music); }
        
		public virtual void UnmuteMusic() { UnmuteTrack(MPSoundManagerTracks.Music); }
        
		public virtual void MuteSfx() { MuteTrack(MPSoundManagerTracks.Sfx); }
        
		public virtual void UnmuteSfx() { UnmuteTrack(MPSoundManagerTracks.Sfx); }
        
		public virtual void MuteUI() { MuteTrack(MPSoundManagerTracks.UI); }

		public virtual void UnmuteUI() { UnmuteTrack(MPSoundManagerTracks.UI); }
        
		public virtual void MuteMaster() { MuteTrack(MPSoundManagerTracks.Master); }
        
		public virtual void UnmuteMaster() { UnmuteTrack(MPSoundManagerTracks.Master); }
        
		public virtual void SetVolumeMusic(float newVolume) { SetTrackVolume(MPSoundManagerTracks.Music, newVolume);}

		public virtual void SetVolumeSfx(float newVolume) { SetTrackVolume(MPSoundManagerTracks.Sfx, newVolume);}

		public virtual void SetVolumeUI(float newVolume) { SetTrackVolume(MPSoundManagerTracks.UI, newVolume);}

		public virtual void SetVolumeMaster(float newVolume) { SetTrackVolume(MPSoundManagerTracks.Master, newVolume);}

		public virtual bool IsMuted(MPSoundManagerTracks track)
		{
			switch (track)
			{
				case MPSoundManagerTracks.Master:
					return settingsSo.Settings.MasterOn; 
				case MPSoundManagerTracks.Music:
					return settingsSo.Settings.MusicOn;
				case MPSoundManagerTracks.Sfx:
					return settingsSo.Settings.SfxOn;
				case MPSoundManagerTracks.UI:
					return settingsSo.Settings.UIOn;
			}
			return false;
		}

		public enum ControlTrackModes { Mute, Unmute, SetVolume }
		protected virtual void ControlTrack(MPSoundManagerTracks track, ControlTrackModes trackMode, float volume = 0.5f)
		{
			string target = "";
			float savedVolume = 0f; 
            
			switch (track)
			{
				case MPSoundManagerTracks.Master:
					target = settingsSo.Settings.MasterVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedMasterVolume); settingsSo.Settings.MasterOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedMasterVolume; settingsSo.Settings.MasterOn = true; }
					break;
				case MPSoundManagerTracks.Music:
					target = settingsSo.Settings.MusicVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedMusicVolume);  settingsSo.Settings.MusicOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedMusicVolume;  settingsSo.Settings.MusicOn = true; }
					break;
				case MPSoundManagerTracks.Sfx:
					target = settingsSo.Settings.SfxVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedSfxVolume);  settingsSo.Settings.SfxOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedSfxVolume;  settingsSo.Settings.SfxOn = true; }
					break;
				case MPSoundManagerTracks.UI:
					target = settingsSo.Settings.UIVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedUIVolume);  settingsSo.Settings.UIOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedUIVolume;  settingsSo.Settings.UIOn = true; }
					break;
			}

			switch (trackMode)
			{
				case ControlTrackModes.Mute:
					settingsSo.SetTrackVolume(track, 0f);
					break;
				case ControlTrackModes.Unmute:
					settingsSo.SetTrackVolume(track, settingsSo.MixerVolumeToNormalized(savedVolume));
					break;
				case ControlTrackModes.SetVolume:
					settingsSo.SetTrackVolume(track, volume);
					break;
			}

			settingsSo.GetTrackVolumes();

			if (settingsSo.Settings.AutoSave)
			{
				settingsSo.SaveSoundSettings();
			}
		}
        
		#endregion

		#region Fades
        
		public virtual void FadeTrack(MPSoundManagerTracks track, float duration, float initialVolume = 0f, float finalVolume = 1f, MPTweenType tweenType = null)
		{
			StartCoroutine(FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tweenType));
		}
        
		public virtual void FadeSound(AudioSource source, float duration, float initialVolume, float finalVolume, MPTweenType tweenType)
		{
			StartCoroutine(FadeCoroutine(source, duration, initialVolume, finalVolume, tweenType));
		}

		protected virtual IEnumerator FadeTrackCoroutine(MPSoundManagerTracks track, float duration, float initialVolume, float finalVolume, MPTweenType tweenType)
		{
			float startedAt = Time.unscaledTime;
			if (tweenType == null)
			{
				tweenType = new MPTweenType(MPTween.MPTweenCurve.EaseInOutQuartic);
			}
			while (Time.unscaledTime - startedAt <= duration)
			{
				float elapsedTime = Time.unscaledTime - startedAt;
				float newVolume = MPTween.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
				settingsSo.SetTrackVolume(track, newVolume);
				yield return null;
			}
			settingsSo.SetTrackVolume(track, finalVolume);
		}

		protected virtual IEnumerator FadeCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume, MPTweenType tweenType)
		{
			float startedAt = Time.unscaledTime;
			if (tweenType == null)
			{
				tweenType = new MPTweenType(MPTween.MPTweenCurve.EaseInOutQuartic);
			}
			while (Time.unscaledTime - startedAt <= duration)
			{
				float elapsedTime = Time.unscaledTime - startedAt;
				float newVolume = MPTween.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
				source.volume = newVolume;
				yield return null;
			}
			source.volume = finalVolume;
		}
        
		#endregion

		#region Solo

		public virtual void MuteSoundsOnTrack(MPSoundManagerTracks track, bool mute, float delay = 0f)
		{
			StartCoroutine(MuteSoundsOnTrackCoroutine(track, mute, delay));
		}
        
		public virtual void MuteAllSounds(bool mute = true)
		{
			StartCoroutine(MuteAllSoundsCoroutine(0f, mute));
		}

		protected virtual IEnumerator MuteSoundsOnTrackCoroutine(MPSoundManagerTracks track, bool mute, float delay)
		{
			if (delay > 0)
			{
				yield return MPCoroutine.WaitForUnscaled(delay);    
			}
            
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.mute = mute;
				}
			}
		}

		protected  virtual IEnumerator MuteAllSoundsCoroutine(float delay, bool mute = true)
		{
			if (delay > 0)
			{
				yield return MPCoroutine.WaitForUnscaled(delay);    
			}
			foreach (MPSoundManagerSound sound in _sounds)
			{
				sound.Source.mute = mute;
			}   
		}

		#endregion

		#region Find

		public virtual AudioSource FindByID(int ID)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.ID == ID)
				{
					return sound.Source;
				}
			}

			return null;
		}

		public virtual AudioSource FindByClip(AudioClip clip)
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Source.clip == clip)
				{
					return sound.Source;
				}
			}

			return null;
		}

		#endregion

		#region AllSoundsControls

		public virtual void PauseAllSounds()
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				sound.Source.Pause();
			}    
		}

		public virtual void PlayAllSounds()
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				sound.Source.Play();
			}    
		}

		public virtual void StopAllSounds()
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				sound.Source.Stop();
			}
		}

		public virtual void FreeAllSounds()
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if (sound.Source != null)
				{
					FreeSound(sound.Source);    
				}
			}
		}

		public virtual void FreeAllSoundsButPersistent()
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if ((!sound.Persistent) && (sound.Source != null))
				{
					FreeSound(sound.Source);
				}
			}
		}

		public virtual void FreeAllLoopingSounds()
		{
			foreach (MPSoundManagerSound sound in _sounds)
			{
				if ((sound.Source.loop) && (sound.Source != null))
				{
					FreeSound(sound.Source);
				}
			}
		}

		#endregion

		#region Events

		protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
		{
			FreeAllSoundsButPersistent();
		}

		public virtual void OnMPEvent(MPSoundManagerTrackEvent soundManagerTrackEvent)
		{
			switch (soundManagerTrackEvent.TrackEventType)
			{
				case MPSoundManagerTrackEventTypes.MuteTrack:
					MuteTrack(soundManagerTrackEvent.Track);
					break;
				case MPSoundManagerTrackEventTypes.UnmuteTrack:
					UnmuteTrack(soundManagerTrackEvent.Track);
					break;
				case MPSoundManagerTrackEventTypes.SetVolumeTrack:
					SetTrackVolume(soundManagerTrackEvent.Track, soundManagerTrackEvent.Volume);
					break;
				case MPSoundManagerTrackEventTypes.PlayTrack:
					PlayTrack(soundManagerTrackEvent.Track);
					break;
				case MPSoundManagerTrackEventTypes.PauseTrack:
					PauseTrack(soundManagerTrackEvent.Track);
					break;
				case MPSoundManagerTrackEventTypes.StopTrack:
					StopTrack(soundManagerTrackEvent.Track);
					break;
				case MPSoundManagerTrackEventTypes.FreeTrack:
					FreeTrack(soundManagerTrackEvent.Track);
					break;
			}
		}
        
		public virtual void OnMPEvent(MPSoundManagerEvent soundManagerEvent)
		{
			switch (soundManagerEvent.EventType)
			{
				case MPSoundManagerEventTypes.SaveSettings:
					SaveSettings();
					break;
				case MPSoundManagerEventTypes.LoadSettings:
					settingsSo.LoadSoundSettings();
					break;
				case MPSoundManagerEventTypes.ResetSettings:
					settingsSo.ResetSoundSettings();
					break;
			}
		}

		public virtual void SaveSettings()
		{
			settingsSo.SaveSoundSettings();
		}

		public virtual void LoadSettings()
		{
			settingsSo.LoadSoundSettings();
		}

		public virtual void ResetSettings()
		{
			settingsSo.ResetSoundSettings();
		}
        
		public virtual void OnMPEvent(MPSoundManagerSoundControlEvent soundControlEvent)
		{
			if (soundControlEvent.TargetSource == null)
			{
				_tempAudioSource = FindByID(soundControlEvent.SoundID);    
			}
			else
			{
				_tempAudioSource = soundControlEvent.TargetSource;
			}

			if (_tempAudioSource != null)
			{
				switch (soundControlEvent.MPSoundManagerSoundControlEventType)
				{
					case MPSoundManagerSoundControlEventTypes.Pause:
						PauseSound(_tempAudioSource);
						break;
					case MPSoundManagerSoundControlEventTypes.Resume:
						ResumeSound(_tempAudioSource);
						break;
					case MPSoundManagerSoundControlEventTypes.Stop:
						StopSound(_tempAudioSource);
						break;
					case MPSoundManagerSoundControlEventTypes.Free:
						FreeSound(_tempAudioSource);
						break;
				}
			}
		}
        
		public virtual void OnMPEvent(MPSoundManagerTrackFadeEvent trackFadeEvent)
		{
			FadeTrack(trackFadeEvent.Track, trackFadeEvent.FadeDuration, settingsSo.GetTrackVolume(trackFadeEvent.Track), trackFadeEvent.FinalVolume, trackFadeEvent.FadeTween);
		}
        
		public virtual void OnMPEvent(MPSoundManagerSoundFadeEvent soundFadeEvent)
		{
			_tempAudioSource = FindByID(soundFadeEvent.SoundID);

			if (_tempAudioSource != null)
			{
				FadeSound(_tempAudioSource, soundFadeEvent.FadeDuration, _tempAudioSource.volume, soundFadeEvent.FinalVolume,
					soundFadeEvent.FadeTween);
			}
		}
        
		public virtual void OnMPEvent(MPSoundManagerAllSoundsControlEvent allSoundsControlEvent)
		{
			switch (allSoundsControlEvent.EventType)
			{
				case MPSoundManagerAllSoundsControlEventTypes.Pause:
					PauseAllSounds();
					break;
				case MPSoundManagerAllSoundsControlEventTypes.Play:
					PlayAllSounds();
					break;
				case MPSoundManagerAllSoundsControlEventTypes.Stop:
					StopAllSounds();
					break;
				case MPSoundManagerAllSoundsControlEventTypes.Free:
					FreeAllSounds();
					break;
				case MPSoundManagerAllSoundsControlEventTypes.FreeAllButPersistent:
					FreeAllSoundsButPersistent();
					break;
				case MPSoundManagerAllSoundsControlEventTypes.FreeAllLooping:
					FreeAllLoopingSounds();
					break;
			}
		}

		public virtual void OnMPSfxEvent(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f, int priority = 128)
		{
			MPSoundManagerPlayOptions options = MPSoundManagerPlayOptions.Default;
			options.Location = this.transform.position;
			options.AudioGroup = audioGroup;
			options.Volume = volume;
			options.Pitch = pitch;
			if (priority >= 0)
			{
				options.Priority = Mathf.Min(priority, 256);
			}
			options.MPSoundManagerTrack = MPSoundManagerTracks.Sfx;
			options.Loop = false;
            
			PlaySound(clipToPlay, options);
		}

		public virtual AudioSource OnMPSoundManagerSoundPlayEvent(AudioClip clip, MPSoundManagerPlayOptions options)
		{
			return PlaySound(clip, options);
		}

		protected virtual void OnEnable()
		{
			MPSfxEvent.Register(OnMPSfxEvent);
			MPSoundManagerSoundPlayEvent.Register(OnMPSoundManagerSoundPlayEvent);
			this.MPEventStartListening<MPSoundManagerEvent>();
			this.MPEventStartListening<MPSoundManagerTrackEvent>();
			this.MPEventStartListening<MPSoundManagerSoundControlEvent>();
			this.MPEventStartListening<MPSoundManagerTrackFadeEvent>();
			this.MPEventStartListening<MPSoundManagerSoundFadeEvent>();
			this.MPEventStartListening<MPSoundManagerAllSoundsControlEvent>();
            
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		protected virtual void OnDisable()
		{
			if (_enabled)
			{
				MPSfxEvent.Unregister(OnMPSfxEvent);
				MPSoundManagerSoundPlayEvent.Unregister(OnMPSoundManagerSoundPlayEvent);
				this.MPEventStopListening<MPSoundManagerEvent>();
				this.MPEventStopListening<MPSoundManagerTrackEvent>();
				this.MPEventStopListening<MPSoundManagerSoundControlEvent>();
				this.MPEventStopListening<MPSoundManagerTrackFadeEvent>();
				this.MPEventStopListening<MPSoundManagerSoundFadeEvent>();
				this.MPEventStopListening<MPSoundManagerAllSoundsControlEvent>();
            
				SceneManager.sceneLoaded -= OnSceneLoaded;
			}
		}
        
		#endregion
	}    
}