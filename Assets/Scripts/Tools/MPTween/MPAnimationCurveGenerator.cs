using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{
	public class MPAnimationCurveGenerator : MonoBehaviour
	{
		[Header("Save settings")]
		/// the path to save the asset at
		public string AnimationCurveFilePath = "Assets/MPTools/MPTween/Editor/";
		/// the name of the asset
		public string AnimationCurveFileName = "MPCurves.curves";

		[Header("Animation Curves")]
		/// the dots resolution (higher is better)
		public int Resolution = 50;
		/// whether to generate anti curves (y goes from 1 to 0) or regular ones (y goes from 0 to 1)
		public bool GenerateAntiCurves = false;

		[Button("GenerateAnimationCurvesAsset")]
		public bool GenerateAnimationCurvesButton;

		protected Type _scriptableObjectType;
		protected Keyframe _keyframe = new Keyframe();
		protected MethodInfo _addMethodInfo;
		protected object[] _parameters;
        
		public virtual void GenerateAnimationCurvesAsset()
		{
			// we get the method to add to our object
			_scriptableObjectType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
			_addMethodInfo = _scriptableObjectType.GetMethod("Add");

			// we create a new instance of our curve asset
			ScriptableObject curveAsset = ScriptableObject.CreateInstance(_scriptableObjectType);
            
			// for each type of curve, we create an animation curve
			foreach (MPTween.MPTweenCurve curve in Enum.GetValues(typeof(MPTween.MPTweenCurve)))
			{
				CreateAnimationCurve(curveAsset, curve, Resolution, GenerateAntiCurves);
			}

			// we save it to file
			#if UNITY_EDITOR
			AssetDatabase.CreateAsset(curveAsset, AnimationCurveFilePath + AnimationCurveFileName);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			#endif
		}

		protected virtual void CreateAnimationCurve(ScriptableObject asset, MPTween.MPTweenCurve curveType, int curveResolution, bool anti)
		{
			// generates an animation curve
			AnimationCurve animationCurve = new AnimationCurve();

			for (int i = 0; i < curveResolution; i++)
			{
				_keyframe.time = i / (curveResolution - 1f);
				if (anti)
				{
					_keyframe.value = MPTween.Tween(_keyframe.time, 0f, 1f, 1f, 0f, curveType);
				}
				else
				{
					_keyframe.value = MPTween.Tween(_keyframe.time, 0f, 1f, 0f, 1f, curveType);
				}                
				animationCurve.AddKey(_keyframe);
			}
			// smoothes the curve's tangents
			for (int j = 0; j < curveResolution; j++)
			{
				animationCurve.SmoothTangents(j, 0f);
			}

			// we add the curve to the scriptable object
			_parameters = new object[] { animationCurve, curveType.ToString() };
			_addMethodInfo.Invoke(asset, _parameters);

		}
	}
}