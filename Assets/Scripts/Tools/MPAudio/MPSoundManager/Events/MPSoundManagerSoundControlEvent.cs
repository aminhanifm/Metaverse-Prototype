using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	public enum MPSoundManagerSoundControlEventTypes
	{
		Pause,
		Resume,
		Stop,
		Free
	}
    
	public struct MPSoundManagerSoundControlEvent
	{
		/// the ID of the sound to control (has to match the one used to play it)
		public int SoundID;
		/// the control mode
		public MPSoundManagerSoundControlEventTypes MPSoundManagerSoundControlEventType;
		/// the audiosource to control (if specified)
		public AudioSource TargetSource;
        
		public MPSoundManagerSoundControlEvent(MPSoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
		{
			SoundID = soundID;
			TargetSource = source;
			MPSoundManagerSoundControlEventType = eventType;
		}

		static MPSoundManagerSoundControlEvent e;
		public static void Trigger(MPSoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
		{
			e.SoundID = soundID;
			e.TargetSource = source;
			e.MPSoundManagerSoundControlEventType = eventType;
			MPEventManager.TriggerEvent(e);
		}
	}
}