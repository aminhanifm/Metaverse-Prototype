﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{
	public class MPFollowTarget : MonoBehaviour
	{
		/// the possible update modes
		public enum UpdateModes { Update, FixedUpdate, LateUpdate }
		/// the possible follow modes
		public enum FollowModes { RegularLerp, MPLerp, MPSpring }
		/// whether to operate in world or local space
		public enum PositionSpaces { World, Local }

		[Header("Follow Position")]
		/// whether or not the object is currently following its target's position
		public bool FollowPosition = true;
		/// whether this object should follow its target on the X axis
		[ShowIf("FollowPosition", true)]
		public bool FollowPositionX = true;
		/// whether this object should follow its target on the Y axis
		[ShowIf("FollowPosition", true)]
		public bool FollowPositionY = true;
		/// whether this object should follow its target on the Z axis
		[ShowIf("FollowPosition", true)]
		public bool FollowPositionZ = true;
		/// whether to operate in world or local space
		[ShowIf("FollowPosition", true)] 
		public PositionSpaces PositionSpace = PositionSpaces.World;

		[Header("Follow Rotation")]
		/// whether or not the object is currently following its target's rotation
		public bool FollowRotation = true;

		[Header("Follow Scale")]
		/// whether or not the object is currently following its target's rotation
		public bool FollowScale = true;
		/// the factor to apply to the scale when following
		[ShowIf("FollowScale", true)]
		public float FollowScaleFactor = 1f;

		[Header("Target")]
		/// the target to follow
		public Transform Target;
		/// the offset to apply to the followed target
		[ShowIf("FollowPosition", true)]
		public Vector3 Offset;
		///whether or not to add the initial x distance to the offset
		[ShowIf("FollowPosition", true)]
		public bool AddInitialDistanceXToXOffset = false;
		///whether or not to add the initial y distance to the offset
		[ShowIf("FollowPosition", true)]
		public bool AddInitialDistanceYToYOffset = false;
		///whether or not to add the initial z distance to the offset
		[ShowIf("FollowPosition", true)]
		public bool AddInitialDistanceZToZOffset = false;

		[Header("Position Interpolation")]
		/// whether or not we need to interpolate the movement
		public bool InterpolatePosition = true;
		/// the follow mode to use when following position
		[ShowIf("InterpolatePosition", true)]
		public FollowModes FollowPositionMode = FollowModes.MPLerp;
		/// the speed at which to interpolate the follower's movement
		[ShowIf("InterpolatePosition", true)]
		public float FollowPositionSpeed = 10f;
		/// higher values mean more damping, less spring, low values mean less damping, more spring
		[ShowIf("FollowPositionMode", (int)FollowModes.MPSpring)] 
		[Range(0.01f, 1.0f)]
		public float PositionSpringDamping = 0.3f;
		/// the frequency at which the spring should "vibrate", in Hz (1 : the spring will do one full period in one second)
		[ShowIf("FollowPositionMode", (int)FollowModes.MPSpring)]
		public float PositionSpringFrequency = 3f;

		[Header("Rotation Interpolation")]
		/// whether or not we need to interpolate the movement
		public bool InterpolateRotation = true;
		/// the follow mode to use when interpolating the rotation
		[ShowIf("InterpolateRotation", true)]
		public FollowModes FollowRotationMode = FollowModes.MPLerp;
		/// the speed at which to interpolate the follower's rotation
		[ShowIf("InterpolateRotation", true)]
		public float FollowRotationSpeed = 10f;

		[Header("Scale Interpolation")]
		/// whether or not we need to interpolate the scale
		public bool InterpolateScale = true;
		/// the follow mode to use when interpolating the scale
		[ShowIf("InterpolateScale", true)]
		public FollowModes FollowScaleMode = FollowModes.MPLerp;
		/// the speed at which to interpolate the follower's scale
		[ShowIf("InterpolateScale", true)]
		public float FollowScaleSpeed = 10f;

		[Header("Mode")]
		/// the update at which the movement happens
		public UpdateModes UpdateMode = UpdateModes.Update;
		/// if this is true, this component will self disable when its host game object gets disabled
		public bool DisableSelfOnSetActiveFalse = false;
        
		[Header("Distances")]
		/// whether or not to force a minimum distance between the object and its target before it starts following
		public bool UseMinimumDistanceBeforeFollow = false;
		/// the minimum distance to keep between the object and its target
		public float MinimumDistanceBeforeFollow = 1f;
		/// whether or not we want to make sure the object is never too far away from its target
		public bool UseMaximumDistance = false;
		/// the maximum distance at which the object can be away from its target
		public float MaximumDistance = 1f;

		[Header("Anchor")] 
		/// if this is true, the movement will be constrained around the initial position
		public bool AnchorToInitialPosition;
		/// the maximum distance around the initial position at which the transform can move
		[ShowIf("AnchorToInitialPosition", true)]
		public float MaxDistanceToAnchor = 1f;
        
		protected bool _localSpace { get { return PositionSpace == PositionSpaces.Local; } }

		protected Vector3 _velocity = Vector3.zero;
		protected Vector3 _newTargetPosition;        
		protected Vector3 _initialPosition;
		protected Vector3 _lastTargetPosition;
		protected Vector3 _direction;
		protected Vector3 _newPosition;
		protected Vector3 _newScale;
		protected Quaternion _newTargetRotation;
		protected Quaternion _initialRotation;
        
		protected virtual void Start()
		{
			Initialization();
		}

		public virtual void Initialization()
		{
			SetInitialPosition();
			SetOffset();
		}

		public virtual void StopFollowing()
		{
			FollowPosition = false;
		}

		public virtual void StartFollowing()
		{
			FollowPosition = true;
			SetInitialPosition();
		}

		protected virtual void SetInitialPosition()
		{
			_initialPosition = _localSpace ? this.transform.localPosition : this.transform.position;
			_initialRotation = this.transform.rotation;
			_lastTargetPosition = _localSpace ? this.transform.localPosition : this.transform.position;
		}

		protected virtual void SetOffset()
		{
			if (Target == null)
			{
				return;
			}
			Vector3 difference = this.transform.position - Target.transform.position;
			Offset.x = AddInitialDistanceXToXOffset ? difference.x : Offset.x;
			Offset.y = AddInitialDistanceYToYOffset ? difference.y : Offset.y;
			Offset.z = AddInitialDistanceZToZOffset ? difference.z : Offset.z;
		}

		protected virtual void Update()
		{
			if (Target == null)
			{
				return;
			}
			if (UpdateMode == UpdateModes.Update)
			{
				FollowTargetRotation();
				FollowTargetScale();
				FollowTargetPosition();
			}
		}

		protected virtual void FixedUpdate()
		{
			if (UpdateMode == UpdateModes.FixedUpdate)
			{
				FollowTargetRotation();
				FollowTargetScale();
				FollowTargetPosition();
			}
		}

		protected virtual void LateUpdate()
		{
			if (UpdateMode == UpdateModes.LateUpdate)
			{
				FollowTargetRotation();
				FollowTargetScale();
				FollowTargetPosition();
			}
		}

		protected virtual void FollowTargetPosition()
		{
			if (Target == null)
			{
				return;
			}

			if (!FollowPosition)
			{
				return;
			}

			_newTargetPosition = Target.position + Offset;
			if (!FollowPositionX) { _newTargetPosition.x = _initialPosition.x; }
			if (!FollowPositionY) { _newTargetPosition.y = _initialPosition.y; }
			if (!FollowPositionZ) { _newTargetPosition.z = _initialPosition.z; }

			float trueDistance = 0f;
			_direction = (_newTargetPosition - this.transform.position).normalized;
			trueDistance = Vector3.Distance(this.transform.position, _newTargetPosition);
            
			float interpolatedDistance = trueDistance;
			if (InterpolatePosition)
			{
				switch (FollowPositionMode)
				{
					case FollowModes.MPLerp:
						interpolatedDistance = MPMaths.Lerp(0f, trueDistance, FollowPositionSpeed, Time.deltaTime);
						interpolatedDistance = ApplyMinMaxDistancing(trueDistance, interpolatedDistance);
						this.transform.Translate(_direction * interpolatedDistance, Space.World);
						break;
					case FollowModes.RegularLerp:
						interpolatedDistance = Mathf.Lerp(0f, trueDistance, Time.deltaTime * FollowPositionSpeed);
						interpolatedDistance = ApplyMinMaxDistancing(trueDistance, interpolatedDistance);
						this.transform.Translate(_direction * interpolatedDistance, Space.World);
						break;
					case FollowModes.MPSpring:
						_newPosition = this.transform.position;
						MPMaths.Spring(ref _newPosition, _newTargetPosition, ref _velocity, PositionSpringDamping, PositionSpringFrequency, FollowPositionSpeed, Time.deltaTime);
						if (_localSpace)
						{
							this.transform.localPosition = _newPosition;   
						}
						else
						{
							this.transform.position = _newPosition;    
						}
						break;
				}                
			}
			else
			{
				interpolatedDistance = ApplyMinMaxDistancing(trueDistance, interpolatedDistance);
				this.transform.Translate(_direction * interpolatedDistance, Space.World);
			}

			if (AnchorToInitialPosition)
			{
				if (Vector3.Distance(this.transform.position, _initialPosition) > MaxDistanceToAnchor)
				{
					if (_localSpace)
					{
						this.transform.localPosition = _initialPosition + Vector3.ClampMagnitude(this.transform.localPosition - _initialPosition, MaxDistanceToAnchor);   
					}
					else
					{
						this.transform.position = _initialPosition + Vector3.ClampMagnitude(this.transform.position - _initialPosition, MaxDistanceToAnchor);    
					}
				}
			}
		}

		protected virtual float ApplyMinMaxDistancing(float trueDistance, float interpolatedDistance)
		{
			if (UseMinimumDistanceBeforeFollow && (trueDistance - interpolatedDistance < MinimumDistanceBeforeFollow))
			{
				interpolatedDistance = 0f;
			}

			if (UseMaximumDistance && (trueDistance - interpolatedDistance >= MaximumDistance))
			{
				interpolatedDistance = trueDistance - MaximumDistance;
			}

			return interpolatedDistance;
		}

		protected virtual void FollowTargetRotation()
		{
			if (Target == null)
			{
				return;
			}

			if (!FollowRotation)
			{
				return;
			}

			_newTargetRotation = Target.rotation;

			if (InterpolateRotation)
			{
				switch (FollowRotationMode)
				{
					case FollowModes.MPLerp:
						this.transform.rotation = MPMaths.Lerp(this.transform.rotation, _newTargetRotation, FollowRotationSpeed, Time.deltaTime);
						break;
					case FollowModes.RegularLerp:
						this.transform.rotation = Quaternion.Lerp(this.transform.rotation, _newTargetRotation, Time.deltaTime * FollowRotationSpeed);
						break;
					case FollowModes.MPSpring:
						this.transform.rotation = MPMaths.Lerp(this.transform.rotation, _newTargetRotation, FollowRotationSpeed, Time.deltaTime);
						break;
				}
			}
			else
			{
				this.transform.rotation = _newTargetRotation;
			}
		}

		protected virtual void FollowTargetScale()
		{
			if (Target == null)
			{
				return;
			}

			if (!FollowScale)
			{
				return;
			}

			_newScale = Target.localScale * FollowScaleFactor;

			if (InterpolateScale)
			{
				switch (FollowScaleMode)
				{
					case FollowModes.MPLerp:
						this.transform.localScale = MPMaths.Lerp(this.transform.localScale, _newScale, FollowScaleSpeed, Time.deltaTime);
						break;
					case FollowModes.RegularLerp:
						this.transform.localScale = Vector3.Lerp(this.transform.localScale, _newScale, Time.deltaTime * FollowScaleSpeed);
						break;
					case FollowModes.MPSpring:
						this.transform.localScale = MPMaths.Lerp(this.transform.localScale, _newScale, FollowScaleSpeed, Time.deltaTime);
						break;
				}
			}
			else
			{
				this.transform.localScale = _newScale;
			}
		}
        
		public virtual void ChangeFollowTarget(Transform newTarget) => Target = newTarget;

		protected virtual void OnDisable()
		{
			if (DisableSelfOnSetActiveFalse)
			{
				this.enabled = false;
			}
		}
	}
}