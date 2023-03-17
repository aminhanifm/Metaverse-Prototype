using UnityEngine;
using UnityEngine.Events;

namespace MetaversePrototype.Tools
{
	public class MPGameEventListener : MonoBehaviour, MPEventListener<MPGameEvent>
	{
		[Header("MPGameEvent")] 
		public string EventName = "Load";
		public UnityEvent OnMPGameEvent;
		
		public void OnMPEvent(MPGameEvent gameEvent)
		{
			if (gameEvent.EventName == EventName)
			{
				OnMPGameEvent?.Invoke();
			}
		}

		protected virtual void OnEnable()
		{
			this.MPEventStartListening<MPGameEvent>();
		}

		protected virtual void OnDisable()
		{
			this.MPEventStopListening<MPGameEvent>();
		}
	}	
}