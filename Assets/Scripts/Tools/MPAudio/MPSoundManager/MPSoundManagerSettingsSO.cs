using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace MetaversePrototype.Tools
{
	[Serializable]
	public class MPSoundManagerSettingsSO : ScriptableObject
	{
		[Header("Audio Mixer")] 
		/// the audio mixer to use when playing sounds 
		[Tooltip("the audio mixer to use when playing sounds")]
		public AudioMixer TargetAudioMixer;
		/// the master group
		[Tooltip("the master group")]
		public AudioMixerGroup MasterAudioMixerGroup;
		/// the group on which to play all music sounds
		[Tooltip("the group on which to play all music sounds")]
		public AudioMixerGroup MusicAudioMixerGroup;
		/// the group on which to play all sound effects
		[Tooltip("the group on which to play all sound effects")]
		public AudioMixerGroup SfxAudioMixerGroup;
		/// the group on which to play all UI sounds
		[Tooltip("the group on which to play all UI sounds")]
		public AudioMixerGroup UIAudioMixerGroup;
		/// the multiplier to apply when converting normalized volume values to audio mixer values
		[Tooltip("the multiplier to apply when converting normalized volume values to audio mixer values")]
		public float MixerValuesMultiplier = 20;
        
		[Header("Settings Unfold")]
		/// the full settings for this MPSoundManager
		[Tooltip("the full settings for this MPSoundManager")]
		public MPSoundManagerSettings Settings;

		protected const string _saveFolderName = "MPSoundManager/";
		protected const string _saveFileName = "MPsound.settings";
    
		#region SaveAndLoad
        
		public virtual void SaveSoundSettings()
		{
			MPSaveLoadManager.Save(this.Settings, _saveFileName, _saveFolderName);
		}

		public virtual void LoadSoundSettings()
		{
			if (Settings.OverrideMixerSettings)
			{
				MPSoundManagerSettings settings =
					(MPSoundManagerSettings) MPSaveLoadManager.Load(typeof(MPSoundManagerSettings), _saveFileName,
						_saveFolderName);
				if (settings != null)
				{
					this.Settings = settings;
					ApplyTrackVolumes();
				}
			}
		}

		public virtual void ResetSoundSettings()
		{
			MPSaveLoadManager.DeleteSave(_saveFileName, _saveFolderName);
		}
        
		#endregion
        
		#region Volume

		public virtual void SetTrackVolume(MPSoundManager.MPSoundManagerTracks track, float volume)
		{
			if (volume <= 0f)
			{
				volume = MPSoundManagerSettings._minimalVolume;
			}
            
			switch (track)
			{
				case MPSoundManager.MPSoundManagerTracks.Master:
					TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.MasterVolume = volume;
					break;
				case MPSoundManager.MPSoundManagerTracks.Music:
					TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.MusicVolume = volume;
					break;
				case MPSoundManager.MPSoundManagerTracks.Sfx:
					TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.SfxVolume = volume;
					break;
				case MPSoundManager.MPSoundManagerTracks.UI:
					TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.UIVolume = volume;
					break;
			}

			if (Settings.AutoSave)
			{
				SaveSoundSettings();
			}
		}

		public virtual float GetTrackVolume(MPSoundManager.MPSoundManagerTracks track)
		{
			float volume = 1f;
			switch (track)
			{
				case MPSoundManager.MPSoundManagerTracks.Master:
					TargetAudioMixer.GetFloat(Settings.MasterVolumeParameter, out volume);
					break;
				case MPSoundManager.MPSoundManagerTracks.Music:
					TargetAudioMixer.GetFloat(Settings.MusicVolumeParameter, out volume);
					break;
				case MPSoundManager.MPSoundManagerTracks.Sfx:
					TargetAudioMixer.GetFloat(Settings.SfxVolumeParameter, out volume);
					break;
				case MPSoundManager.MPSoundManagerTracks.UI:
					TargetAudioMixer.GetFloat(Settings.UIVolumeParameter, out volume);
					break;
			}

			return MixerVolumeToNormalized(volume);
		}

		public virtual void GetTrackVolumes()
		{
			Settings.MasterVolume = GetTrackVolume(MPSoundManager.MPSoundManagerTracks.Master);
			Settings.MusicVolume = GetTrackVolume(MPSoundManager.MPSoundManagerTracks.Music);
			Settings.SfxVolume = GetTrackVolume(MPSoundManager.MPSoundManagerTracks.Sfx);
			Settings.UIVolume = GetTrackVolume(MPSoundManager.MPSoundManagerTracks.UI);
		}

		protected virtual void ApplyTrackVolumes()
		{
			if (Settings.OverrideMixerSettings)
			{
				TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(Settings.MasterVolume));
				TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(Settings.MusicVolume));
				TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(Settings.SfxVolume));
				TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(Settings.UIVolume));

				if (!Settings.MasterOn) { TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, -80f); }
				if (!Settings.MusicOn) { TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, -80f); }
				if (!Settings.SfxOn) { TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, -80f); }
				if (!Settings.UIOn) { TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, -80f); }

				if (Settings.AutoSave)
				{
					SaveSoundSettings();
				}
			}
		}
        
		public virtual float NormalizedToMixerVolume(float normalizedVolume)
		{
			return Mathf.Log10(normalizedVolume) * MixerValuesMultiplier;
		}

		public virtual float MixerVolumeToNormalized(float mixerVolume)
		{
			return (float)Math.Pow(10, (mixerVolume / MixerValuesMultiplier));
		}
        
		#endregion Volume
	}
}