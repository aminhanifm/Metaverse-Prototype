using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        protected StarterAssetsInputs starterAssetsInputs;
        public StarterAssetsInputs _StarterAssetsInputs
        {
            get {return starterAssetsInputs;}
            set {starterAssetsInputs = value;}
        }

        public virtual void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public virtual void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public virtual void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public virtual void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public virtual void VirtualSkinInput()
        {
            starterAssetsInputs.SkinInput();
        }

        public virtual void VirtualCameraInput()
        {
            starterAssetsInputs.CameraInput();
        }
        
    }

}
