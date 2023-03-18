using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaversePrototype.Tools
{
	public enum MPTweenDefinitionTypes { MPTween, AnimationCurve }

	[Serializable]
	public class MPTweenType
	{
		public MPTweenDefinitionTypes MPTweenDefinitionType = MPTweenDefinitionTypes.MPTween;
		public MPTween.MPTweenCurve MPTweenCurve = MPTween.MPTweenCurve.EaseInCubic;
		public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));
		public bool Initialized = false;

		public MPTweenType(MPTween.MPTweenCurve newCurve)
		{
			MPTweenCurve = newCurve;
			MPTweenDefinitionType = MPTweenDefinitionTypes.MPTween;
		}
		public MPTweenType(AnimationCurve newCurve)
		{
			Curve = newCurve;
			MPTweenDefinitionType = MPTweenDefinitionTypes.AnimationCurve;
		}

		public float Evaluate(float t)
		{
			return MPTween.Evaluate(t, this);
		}
	}
}