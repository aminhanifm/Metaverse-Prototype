using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	public struct MPSoundManagerSoundFadeEvent
	{
		/// the ID of the sound to fade
		public int SoundID;
		/// the duration of the fade (in seconds)
		public float FadeDuration;
		/// the volume towards which to fade this sound
		public float FinalVolume;
		/// the tween over which to fade this sound
		public MPTweenType FadeTween;
        
		public MPSoundManagerSoundFadeEvent(int soundID, float fadeDuration, float finalVolume, MPTweenType fadeTween)
		{
			SoundID = soundID;
			FadeDuration = fadeDuration;
			FinalVolume = finalVolume;
			FadeTween = fadeTween;
		}

		static MPSoundManagerSoundFadeEvent e;
		public static void Trigger(int soundID, float fadeDuration, float finalVolume, MPTweenType fadeTween)
		{
			e.SoundID = soundID;
			e.FadeDuration = fadeDuration;
			e.FinalVolume = finalVolume;
			e.FadeTween = fadeTween;
			MPEventManager.TriggerEvent(e);
		}
	}
}