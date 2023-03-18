using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;

namespace MetaversePrototype.Tools
{
	public class MPFPSUnlock : MonoBehaviour
	{
		/// the target FPS you want the game to run at, that's up to how many times Update will run every second
		[Tooltip("the target FPS you want the game to run at, that's up to how many times Update will run every second")]
		public int TargetFPS;
		/// the number of frames to wait before rendering the next one. 0 will render every frame, 1 will render every 2 frames, 5 will render every 5 frames, etc
		[Tooltip("the number of frames to wait before rendering the next one. 0 will render every frame, 1 will render every 2 frames, 5 will render every 5 frames, etc")]
		public int RenderFrameInterval = 0;
		[Range(0,2)]
		/// whether vsync should be enabled or not (on a 60Hz screen, 1 : 60fps, 2 : 30fps, 0 : don't wait for vsync)
		[Tooltip("whether vsync should be enabled or not (on a 60Hz screen, 1 : 60fps, 2 : 30fps, 0 : don't wait for vsync)")]
		public int VSyncCount = 0;

		protected virtual void Start()
		{
			UpdateSettings();
		}	
        
		protected virtual void OnValidate()
		{
			UpdateSettings();
		}

		protected virtual void UpdateSettings()
		{
			QualitySettings.vSyncCount = VSyncCount;
			Application.targetFrameRate = TargetFPS;
			OnDemandRendering.renderFrameInterval = RenderFrameInterval;
		}
	}
}