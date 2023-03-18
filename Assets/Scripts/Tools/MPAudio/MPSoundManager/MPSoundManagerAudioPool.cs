using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace MetaversePrototype.Tools
{
	[Serializable]
	public class MPSoundManagerAudioPool 
	{
		protected List<AudioSource> _pool;
        
		public virtual void FillAudioSourcePool(int poolSize, Transform parent)
		{
			if (_pool == null)
			{
				_pool = new List<AudioSource>();
			}
            
			if ((poolSize <= 0) || (_pool.Count >= poolSize))
			{
				return;
			}

			foreach (AudioSource source in _pool)
			{
				UnityEngine.Object.Destroy(source.gameObject);
			}

			for (int i = 0; i < poolSize; i++)
			{
				GameObject temporaryAudioHost = new GameObject("MPAudioSourcePool_"+i);
				SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, parent.gameObject.scene);
				AudioSource tempSource = temporaryAudioHost.AddComponent<AudioSource>();
				MPFollowTarget followTarget = temporaryAudioHost.AddComponent<MPFollowTarget>();
				followTarget.enabled = false;
				followTarget.DisableSelfOnSetActiveFalse = true;
				temporaryAudioHost.transform.SetParent(parent);
				temporaryAudioHost.SetActive(false);
				_pool.Add(tempSource);
			}
		}

		public virtual IEnumerator AutoDisableAudioSource(float duration, AudioSource source, AudioClip clip, bool doNotAutoRecycleIfNotDonePlaying)
		{
			yield return MPCoroutine.WaitFor(duration);
			if (source.clip != clip)
			{
				yield break;
			}
			if (doNotAutoRecycleIfNotDonePlaying)
			{
				while (source.time < source.clip.length)
				{
					yield return null;
				}
			}
			source.gameObject.SetActive(false);
		}

		public virtual AudioSource GetAvailableAudioSource(bool poolCanExpand, Transform parent)
		{
			foreach (AudioSource source in _pool)
			{
				if (!source.gameObject.activeInHierarchy)
				{
					source.gameObject.SetActive(true);
					return source;
				}
			}

			if (poolCanExpand)
			{
				GameObject temporaryAudioHost = new GameObject("MPAudioSourcePool_"+_pool.Count);
				SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, parent.gameObject.scene);
				AudioSource tempSource = temporaryAudioHost.AddComponent<AudioSource>();
				temporaryAudioHost.transform.SetParent(parent);
				temporaryAudioHost.SetActive(true);
				_pool.Add(tempSource);
				return tempSource;
			}

			return null;
		}

		public virtual bool FreeSound(AudioSource sourceToStop)
		{
			foreach (AudioSource source in _pool)
			{
				if (source == sourceToStop)
				{
					source.Stop();
					source.gameObject.SetActive(false);
					return true;
				}
			}
			return false;
		}
	}
}