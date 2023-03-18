using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MetaversePrototype.Tools
{
	public class MPTween : MonoBehaviour
	{
		public enum MPTweenCurve
		{
			LinearTween,        
			EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
			EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
			EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
			EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
			EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
			EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
			EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
			EaseInExponential,  EaseOutExponential, EaseInOutExponential,
			EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
			EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
			AntiLinearTween,    AlmostIdentity
		}
		
		public static TweenDelegate[] TweenDelegateArray = new TweenDelegate[]
		{
			LinearTween,        
			EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
			EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
			EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
			EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
			EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
			EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
			EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
			EaseInExponential,  EaseOutExponential, EaseInOutExponential,
			EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
			EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
			AntiLinearTween,    AlmostIdentity
		};

		// Core methods ---------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, MPTweenCurve curve)
		{
			currentTime = MPMaths.Remap(currentTime, initialTime, endTime, 0f, 1f);
			currentTime = TweenDelegateArray[(int)curve](currentTime);
			return startValue + currentTime * (endValue - startValue);
		}

		public static float Evaluate(float t, MPTweenCurve curve)
		{
			return TweenDelegateArray[(int)curve](t);
		}

		public static float Evaluate(float t, MPTweenType tweenType)
		{
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.MPTween)
			{
				return Evaluate(t, tweenType.MPTweenCurve);
			}
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.AnimationCurve)
			{
				return tweenType.Curve.Evaluate(t);
			}
			return 0f;
		}

		public delegate float TweenDelegate(float currentTime);
		
		public static float LinearTween(float currentTime) { return MPTweenDefinitions.Linear_Tween(currentTime); }
		public static float AntiLinearTween(float currentTime) { return MPTweenDefinitions.LinearAnti_Tween(currentTime); }
		public static float EaseInQuadratic(float currentTime) { return MPTweenDefinitions.EaseIn_Quadratic(currentTime); }
		public static float EaseOutQuadratic(float currentTime) { return MPTweenDefinitions.EaseOut_Quadratic(currentTime); }
		public static float EaseInOutQuadratic(float currentTime) { return MPTweenDefinitions.EaseInOut_Quadratic(currentTime); }
		public static float EaseInCubic(float currentTime) { return MPTweenDefinitions.EaseIn_Cubic(currentTime); }
		public static float EaseOutCubic(float currentTime) { return MPTweenDefinitions.EaseOut_Cubic(currentTime); }
		public static float EaseInOutCubic(float currentTime) { return MPTweenDefinitions.EaseInOut_Cubic(currentTime); }
		public static float EaseInQuartic(float currentTime) { return MPTweenDefinitions.EaseIn_Quartic(currentTime); }
		public static float EaseOutQuartic(float currentTime) { return MPTweenDefinitions.EaseOut_Quartic(currentTime); }
		public static float EaseInOutQuartic(float currentTime) { return MPTweenDefinitions.EaseInOut_Quartic(currentTime); }
		public static float EaseInQuintic(float currentTime) { return MPTweenDefinitions.EaseIn_Quintic(currentTime); }
		public static float EaseOutQuintic(float currentTime) { return MPTweenDefinitions.EaseOut_Quintic(currentTime); }
		public static float EaseInOutQuintic(float currentTime) { return MPTweenDefinitions.EaseInOut_Quintic(currentTime); }
		public static float EaseInSinusoidal(float currentTime) { return MPTweenDefinitions.EaseIn_Sinusoidal(currentTime); }
		public static float EaseOutSinusoidal(float currentTime) { return MPTweenDefinitions.EaseOut_Sinusoidal(currentTime); }
		public static float EaseInOutSinusoidal(float currentTime) { return MPTweenDefinitions.EaseInOut_Sinusoidal(currentTime); }
		public static float EaseInBounce(float currentTime) { return MPTweenDefinitions.EaseIn_Bounce(currentTime); }
		public static float EaseOutBounce(float currentTime) { return MPTweenDefinitions.EaseOut_Bounce(currentTime); }
		public static float EaseInOutBounce(float currentTime) { return MPTweenDefinitions.EaseInOut_Bounce(currentTime); }
		public static float EaseInOverhead(float currentTime) { return MPTweenDefinitions.EaseIn_Overhead(currentTime); }
		public static float EaseOutOverhead(float currentTime) { return MPTweenDefinitions.EaseOut_Overhead(currentTime); }
		public static float EaseInOutOverhead(float currentTime) { return MPTweenDefinitions.EaseInOut_Overhead(currentTime); }
		public static float EaseInExponential(float currentTime) { return MPTweenDefinitions.EaseIn_Exponential(currentTime); }
		public static float EaseOutExponential(float currentTime) { return MPTweenDefinitions.EaseOut_Exponential(currentTime); }
		public static float EaseInOutExponential(float currentTime) { return MPTweenDefinitions.EaseInOut_Exponential(currentTime); }
		public static float EaseInElastic(float currentTime) { return MPTweenDefinitions.EaseIn_Elastic(currentTime); }
		public static float EaseOutElastic(float currentTime) { return MPTweenDefinitions.EaseOut_Elastic(currentTime); }
		public static float EaseInOutElastic(float currentTime) { return MPTweenDefinitions.EaseInOut_Elastic(currentTime); }
		public static float EaseInCircular(float currentTime) { return MPTweenDefinitions.EaseIn_Circular(currentTime); }
		public static float EaseOutCircular(float currentTime) { return MPTweenDefinitions.EaseOut_Circular(currentTime); }
		public static float EaseInOutCircular(float currentTime) { return MPTweenDefinitions.EaseInOut_Circular(currentTime); }
		public static float AlmostIdentity(float currentTime) { return MPTweenDefinitions.AlmostIdentity(currentTime); }

		public static TweenDelegate GetTweenMethod(MPTweenCurve tween)
		{
			switch (tween)
			{
				case MPTweenCurve.LinearTween: return LinearTween;
				case MPTweenCurve.AntiLinearTween: return AntiLinearTween;
				case MPTweenCurve.EaseInQuadratic: return EaseInQuadratic;
				case MPTweenCurve.EaseOutQuadratic: return EaseOutQuadratic;
				case MPTweenCurve.EaseInOutQuadratic: return EaseInOutQuadratic;
				case MPTweenCurve.EaseInCubic: return EaseInCubic;
				case MPTweenCurve.EaseOutCubic: return EaseOutCubic;
				case MPTweenCurve.EaseInOutCubic: return EaseInOutCubic;
				case MPTweenCurve.EaseInQuartic: return EaseInQuartic;
				case MPTweenCurve.EaseOutQuartic: return EaseOutQuartic;
				case MPTweenCurve.EaseInOutQuartic: return EaseInOutQuartic;
				case MPTweenCurve.EaseInQuintic: return EaseInQuintic;
				case MPTweenCurve.EaseOutQuintic: return EaseOutQuintic;
				case MPTweenCurve.EaseInOutQuintic: return EaseInOutQuintic;
				case MPTweenCurve.EaseInSinusoidal: return EaseInSinusoidal;
				case MPTweenCurve.EaseOutSinusoidal: return EaseOutSinusoidal;
				case MPTweenCurve.EaseInOutSinusoidal: return EaseInOutSinusoidal;
				case MPTweenCurve.EaseInBounce: return EaseInBounce;
				case MPTweenCurve.EaseOutBounce: return EaseOutBounce;
				case MPTweenCurve.EaseInOutBounce: return EaseInOutBounce;
				case MPTweenCurve.EaseInOverhead: return EaseInOverhead;
				case MPTweenCurve.EaseOutOverhead: return EaseOutOverhead;
				case MPTweenCurve.EaseInOutOverhead: return EaseInOutOverhead;
				case MPTweenCurve.EaseInExponential: return EaseInExponential;
				case MPTweenCurve.EaseOutExponential: return EaseOutExponential;
				case MPTweenCurve.EaseInOutExponential: return EaseInOutExponential;
				case MPTweenCurve.EaseInElastic: return EaseInElastic;
				case MPTweenCurve.EaseOutElastic: return EaseOutElastic;
				case MPTweenCurve.EaseInOutElastic: return EaseInOutElastic;
				case MPTweenCurve.EaseInCircular: return EaseInCircular;
				case MPTweenCurve.EaseOutCircular: return EaseOutCircular;
				case MPTweenCurve.EaseInOutCircular: return EaseInOutCircular;
				case MPTweenCurve.AlmostIdentity: return AlmostIdentity;
			}
			return LinearTween;
		}

		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, MPTweenCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			return startValue;
		}

		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, MPTweenCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
			return startValue;
		}

		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, MPTweenCurve curve)
		{
			float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
			startValue = Quaternion.Slerp(startValue, endValue, turningRate);
			return startValue;
		}

		// Animation curve methods --------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
		{
			currentTime = MPMaths.Remap(currentTime, initialTime, endTime, 0f, 1f);
			currentTime = curve.Evaluate(currentTime);
			return startValue + currentTime * (endValue - startValue);
		}

		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			return startValue;
		}

		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
			return startValue;
		}

		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
		{
			float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
			startValue = Quaternion.Slerp(startValue, endValue, turningRate);
			return startValue;
		}

		// Tween type methods ------------------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, MPTweenType tweenType)
		{
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.MPTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MPTweenCurve);
			}
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return 0f;
		}
		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, MPTweenType tweenType)
		{
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.MPTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MPTweenCurve);
			}
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Vector2.zero;
		}
		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, MPTweenType tweenType)
		{
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.MPTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MPTweenCurve);
			}
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Vector3.zero;
		}
		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, MPTweenType tweenType)
		{
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.MPTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MPTweenCurve);
			}
			if (tweenType.MPTweenDefinitionType == MPTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Quaternion.identity;
		}

		// MOVE METHODS ---------------------------------------------------------------------------------------------------------
		public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Vector3 origin, Vector3 destination, 
			WaitForSeconds delay, float delayDuration, float duration, MPTween.MPTweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		public static Coroutine MoveRectTransform(MonoBehaviour mono, RectTransform targetTransform, Vector3 origin, Vector3 destination,
			WaitForSeconds delay, float delayDuration, float duration, MPTween.MPTweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveRectTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
			MPTween.MPTweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation, ignoreTimescale));
		}

		public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, 
			float duration, MPTween.MPTweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		protected static IEnumerator MoveRectTransformCo(RectTransform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
			float delayDuration, float duration, MPTween.MPTweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				targetTransform.localPosition = MPTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.localPosition = destination;
		}

		protected static IEnumerator MoveTransformCo(Transform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay, 
			float delayDuration, float duration, MPTween.MPTweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				targetTransform.transform.position = MPTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.transform.position = destination;
		}

		protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
			MPTween.MPTweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				if (updatePosition)
				{
					targetTransform.transform.position = MPTween.Tween(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
				}
				if (updateRotation)
				{
					targetTransform.transform.rotation = MPTween.Tween(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
				}
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			if (updatePosition) { targetTransform.transform.position = destination.position; }
			if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
		}

		protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration, 
			MPTween.MPTweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}

			Vector3 initialRotationPosition = targetTransform.transform.position;
			Quaternion initialRotationRotation = targetTransform.transform.rotation;

			float rate = 1f / duration;

			float timeSpent = 0f;
			while (timeSpent < duration)
			{

				float newAngle = MPTween.Tween(timeSpent, 0f, duration, 0f, angle, curve);

				targetTransform.transform.position = initialRotationPosition;
				initialRotationRotation = targetTransform.transform.rotation;
				targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
				targetTransform.transform.rotation = initialRotationRotation;

				timeSpent += ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.transform.position = destination.position;
		}
	}
}