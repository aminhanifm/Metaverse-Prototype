using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{

	[Serializable]
	public struct MPSoundManagerSound
	{
		/// the ID of the sound 
		public int ID;
		/// the track the sound is being played on
		public MPSoundManager.MPSoundManagerTracks Track;
		/// the associated audiosource
		public AudioSource Source;
		/// whether or not this sound will play over multiple scenes
		public bool Persistent;
	}
}