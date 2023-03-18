using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{

	public struct MPSoundManagerTrackFadeEvent
	{
		/// the track to fade the volume of
		public MPSoundManager.MPSoundManagerTracks Track;
		/// the duration of the fade, in seconds
		public float FadeDuration;
		/// the final volume to fade towards
		public float FinalVolume;
		/// the tween to use when fading
		public MPTweenType FadeTween;
        
		public MPSoundManagerTrackFadeEvent(MPSoundManager.MPSoundManagerTracks track, float fadeDuration, float finalVolume, MPTweenType fadeTween)
		{
			Track = track;
			FadeDuration = fadeDuration;
			FinalVolume = finalVolume;
			FadeTween = fadeTween;
		}

		static MPSoundManagerTrackFadeEvent e;
		public static void Trigger(MPSoundManager.MPSoundManagerTracks track, float fadeDuration, float finalVolume, MPTweenType fadeTween)
		{
			e.Track = track;
			e.FadeDuration = fadeDuration;
			e.FinalVolume = finalVolume;
			e.FadeTween = fadeTween;
			MPEventManager.TriggerEvent(e);
		}
	}
}