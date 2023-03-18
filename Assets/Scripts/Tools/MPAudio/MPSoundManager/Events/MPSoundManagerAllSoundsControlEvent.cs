using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	public enum MPSoundManagerAllSoundsControlEventTypes
	{
		Pause, Play, Stop, Free, FreeAllButPersistent, FreeAllLooping
	}
    
	public struct MPSoundManagerAllSoundsControlEvent
	{
		public MPSoundManagerAllSoundsControlEventTypes EventType;
        
		public MPSoundManagerAllSoundsControlEvent(MPSoundManagerAllSoundsControlEventTypes eventType)
		{
			EventType = eventType;
		}

		static MPSoundManagerAllSoundsControlEvent e;
		public static void Trigger(MPSoundManagerAllSoundsControlEventTypes eventType)
		{
			e.EventType = eventType;
			MPEventManager.TriggerEvent(e);
		}
	}
}