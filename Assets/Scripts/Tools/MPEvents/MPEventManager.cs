//#define EVENTROUTER_THROWEXCEPTIONS 
#if EVENTROUTER_THROWEXCEPTIONS
//#define EVENTROUTER_REQUIRELISTENER // Uncomment this if you want listeners to be required for sending events.
#endif

using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace MetaversePrototype.Tools
{	
	public struct MPGameEvent
	{
		public string EventName;
		public MPGameEvent(string newName)
		{
			EventName = newName;
		}
		static MPGameEvent e;
		public static void Trigger(string newName)
		{
			e.EventName = newName;
			MPEventManager.TriggerEvent(e);
		}
	}
    
	[ExecuteAlways]
	public static class MPEventManager 
	{
		private static Dictionary<Type, List<MPEventListenerBase>> _subscribersList;

		static MPEventManager()
		{
			_subscribersList = new Dictionary<Type, List<MPEventListenerBase>>();
		}

		public static void AddListener<MPEvent>( MPEventListener<MPEvent> listener ) where MPEvent : struct
		{
			Type eventType = typeof( MPEvent );

			if (!_subscribersList.ContainsKey(eventType))
			{
				_subscribersList[eventType] = new List<MPEventListenerBase>();
			}

			if (!SubscriptionExists(eventType, listener))
			{
				_subscribersList[eventType].Add( listener );
			}
		}

		public static void RemoveListener<MPEvent>( MPEventListener<MPEvent> listener ) where MPEvent : struct
		{
			Type eventType = typeof( MPEvent );

			if( !_subscribersList.ContainsKey( eventType ) )
			{
				#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
				#else
				return;
				#endif
			}

			List<MPEventListenerBase> subscriberList = _subscribersList[eventType];

			#if EVENTROUTER_THROWEXCEPTIONS
	            bool listenerFound = false;
			#endif

			for (int i = subscriberList.Count-1; i >= 0; i--)
			{
				if( subscriberList[i] == listener )
				{
					subscriberList.Remove( subscriberList[i] );
					#if EVENTROUTER_THROWEXCEPTIONS
					    listenerFound = true;
					#endif

					if ( subscriberList.Count == 0 )
					{
						_subscribersList.Remove(eventType);
					}						

					return;
				}
			}

			#if EVENTROUTER_THROWEXCEPTIONS
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		        }
			#endif
		}

		public static void TriggerEvent<MPEvent>( MPEvent newEvent ) where MPEvent : struct
		{
			List<MPEventListenerBase> list;
			if( !_subscribersList.TryGetValue( typeof( MPEvent ), out list ) )
				#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MPEvent ).ToString() ) );
				#else
				return;
			#endif
			
			for (int i=list.Count-1; i >= 0; i--)
			{
				( list[i] as MPEventListener<MPEvent> ).OnMPEvent( newEvent );
			}
		}

		private static bool SubscriptionExists( Type type, MPEventListenerBase receiver )
		{
			List<MPEventListenerBase> receivers;

			if( !_subscribersList.TryGetValue( type, out receivers ) ) return false;

			bool exists = false;

			for (int i = receivers.Count-1; i >= 0; i--)
			{
				if( receivers[i] == receiver )
				{
					exists = true;
					break;
				}
			}

			return exists;
		}
	}

	public static class EventRegister
	{
		public delegate void Delegate<T>( T eventType );

		public static void MPEventStartListening<EventType>( this MPEventListener<EventType> caller ) where EventType : struct
		{
			MPEventManager.AddListener<EventType>( caller );
		}

		public static void MPEventStopListening<EventType>( this MPEventListener<EventType> caller ) where EventType : struct
		{
			MPEventManager.RemoveListener<EventType>( caller );
		}
	}

	public interface MPEventListenerBase { };

	public interface MPEventListener<T> : MPEventListenerBase
	{
		void OnMPEvent( T eventType );
	}

	public class MPEventListenerWrapper<TOwner, TTarget, TEvent> : MPEventListener<TEvent>, IDisposable
		where TEvent : struct
	{
		private Action<TTarget> _callback;

		private TOwner _owner;
		public MPEventListenerWrapper(TOwner owner, Action<TTarget> callback)
		{
			_owner = owner;
			_callback = callback;
			RegisterCallbacks(true);
		}

		public void Dispose()
		{
			RegisterCallbacks(false);
			_callback = null;
		}

		protected virtual TTarget OnEvent(TEvent eventType) => default;
		public void OnMPEvent(TEvent eventType)
		{
			var item = OnEvent(eventType);
			_callback?.Invoke(item);
		}

		private void RegisterCallbacks(bool b)
		{
			if (b)
			{
				this.MPEventStartListening<TEvent>();
			}
			else
			{
				this.MPEventStopListening<TEvent>();
			}
		}
	}
}