using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	public enum MPSoundManagerEventTypes
	{
		SaveSettings,
		LoadSettings,
		ResetSettings
	}
    
	public struct MPSoundManagerEvent
	{
		public MPSoundManagerEventTypes EventType;
        
		public MPSoundManagerEvent(MPSoundManagerEventTypes eventType)
		{
			EventType = eventType;
		}

		static MPSoundManagerEvent e;
		public static void Trigger(MPSoundManagerEventTypes eventType)
		{
			e.EventType = eventType;
			MPEventManager.TriggerEvent(e);
		}
	}
}