using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{
	public class MPPlaylistRemote : MonoBehaviour
	{
		public int Channel = 0;
		/// The track to play when calling PlaySelectedTrack
		public int TrackNumber = 0;

		[Header("Triggers")]
		/// if this is true, the selected track will be played on trigger enter (if you have a trigger collider on this)
		public bool PlaySelectedTrackOnTriggerEnter = true;
		/// if this is true, the selected track will be played on trigger exit (if you have a trigger collider on this)
		public bool PlaySelectedTrackOnTriggerExit = false;
		/// the tag to check for on trigger stuff
		public string TriggerTag = "Player";

		[Header("Test")]
		/// a play test button
		[Button("Play")]
		public bool PlayButton;
		/// a pause test button
		[Button("Pause")]
		public bool PauseButton;
		/// a stop test button
		[Button("Stop")]
		public bool StopButton;
		/// a next track test button
		[Button("PlayNextTrack")]
		public bool NextButton;
		/// a selected track test button
		[Button("PlaySelectedTrack")]
		public bool SelectedTrackButton;

		public virtual void Play()
		{
			MPPlaylistPlayEvent.Trigger(Channel);
		}

		public virtual void Pause()
		{
			MPPlaylistPauseEvent.Trigger(Channel);
		}

		public virtual void Stop()
		{
			MPPlaylistStopEvent.Trigger(Channel);
		}

		public virtual void PlayNextTrack()
		{
			MPPlaylistPlayNextEvent.Trigger(Channel);
		}

		public virtual void PlaySelectedTrack()
		{
			MPPlaylistPlayIndexEvent.Trigger(Channel, TrackNumber);
		}

		public virtual void PlayTrack(int trackIndex)
		{
			MPPlaylistPlayIndexEvent.Trigger(Channel, trackIndex);
		}

		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (PlaySelectedTrackOnTriggerEnter && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}

		protected virtual void OnTriggerExit(Collider collider)
		{
			if (PlaySelectedTrackOnTriggerExit && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}

		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			if (PlaySelectedTrackOnTriggerEnter && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}

		protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			if (PlaySelectedTrackOnTriggerExit && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}
	}
}