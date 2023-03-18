using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	public enum MPSoundManagerTrackEventTypes
	{
		MuteTrack,
		UnmuteTrack,
		SetVolumeTrack,
		PlayTrack,
		PauseTrack,
		StopTrack,
		FreeTrack
	}
    
	public struct MPSoundManagerTrackEvent
	{
		/// the order to pass to the track
		public MPSoundManagerTrackEventTypes TrackEventType;
		/// the track to pass the order to
		public MPSoundManager.MPSoundManagerTracks Track;
		/// if in SetVolume mode, the volume to which to set the track to
		public float Volume;
        
		public MPSoundManagerTrackEvent(MPSoundManagerTrackEventTypes trackEventType, MPSoundManager.MPSoundManagerTracks track = MPSoundManager.MPSoundManagerTracks.Master, float volume = 1f)
		{
			TrackEventType = trackEventType;
			Track = track;
			Volume = volume;
		}

		static MPSoundManagerTrackEvent e;
		public static void Trigger(MPSoundManagerTrackEventTypes trackEventType, MPSoundManager.MPSoundManagerTracks track = MPSoundManager.MPSoundManagerTracks.Master, float volume = 1f)
		{
			e.TrackEventType = trackEventType;
			e.Track = track;
			e.Volume = volume;
			MPEventManager.TriggerEvent(e);
		}
	}
}