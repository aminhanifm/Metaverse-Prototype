using UnityEngine;
using Cinemachine;
using StarterAssets;
using Photon.Pun;

namespace MetaversePrototype.Game
{
    public class MPCharacterUIController : UICanvasControllerInput
    {
        #region Self-Singleton
        private static MPCharacterUIController instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static MPCharacterUIController Instance
        {
            get { return instance; }
        }
        #endregion

        public Camera mainCam;
        public GameObject mobileController;
        protected MPCharacterManager characterManager;
        public MPCharacterManager _CharacterManager {
            get {return characterManager;} 
            set { characterManager = value; }
        }
        
        protected CinemachineVirtualCamera FPScam, TPScam; // Shorten Purpose

        public override void VirtualJumpInput(bool virtualJumpState)
        {
            base.VirtualJumpInput(virtualJumpState);
        }

        public override void VirtualLookInput(Vector2 virtualLookDirection)
        {
            base.VirtualLookInput(virtualLookDirection);
        }

        public override void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            base.VirtualMoveInput(virtualMoveDirection);
        }

        public override void VirtualSprintInput(bool virtualSprintState)
        {
            base.VirtualSprintInput(virtualSprintState);
        }

        public override void VirtualSkinInput()
        {
            base.VirtualSkinInput();

            int curSkin = (int)characterManager.curSkinType;
            
            characterManager.skinDictionary[characterManager.curSkinType].gameObject.SetActive(false);

            curSkin++;
            if(curSkin >= characterManager._SkinCount) curSkin = 0;

            characterManager.skinDictionary[(MPCharacterManager.SkinType)curSkin].gameObject.SetActive(true);
            characterManager.curSkinType = (MPCharacterManager.SkinType)curSkin;

            characterManager.photonView.RPC("SetSkin", RpcTarget.AllBuffered, characterManager.curSkinType);
        }

        public override void VirtualCameraInput()
        {
            base.VirtualCameraInput();

            if(FPScam == null)
            FPScam = characterManager?.camDictionary[characterManager.curControllerType = MPCharacterManager.ControllerType.FPS];
            if(TPScam == null)
            TPScam = characterManager?.camDictionary[characterManager.curControllerType = MPCharacterManager.ControllerType.TPS];

            switch (characterManager.curControllerType)
            {
                case MPCharacterManager.ControllerType.FPS:
                    characterManager._FPSController.enabled = false;
                    characterManager._TPSController.enabled = true;
                    characterManager._TPSController.CinemachineTargetYaw = characterManager._FPSController.CinemachineTargetYaw;
                    characterManager._TPSController.CinemachineTargetPitch = characterManager._FPSController.CinemachineTargetPitch;
                    FPScam.gameObject.SetActive(false);
                    TPScam.gameObject.SetActive(true);
                    characterManager.curControllerType = MPCharacterManager.ControllerType.TPS;
                    break;
                case MPCharacterManager.ControllerType.TPS:
                
                    Vector3 cameraForward = new Vector3(mainCam.transform.forward.x, 0f, mainCam.transform.forward.z);
                    characterManager.transform.LookAt(characterManager.transform.position + cameraForward, Vector3.up);
                    
                    characterManager._TPSController.enabled = false;
                    characterManager._FPSController.enabled = true;
                    characterManager._FPSController.CinemachineTargetYaw = characterManager._TPSController.CinemachineTargetYaw;
                    characterManager._FPSController.CinemachineTargetPitch = characterManager._TPSController.CinemachineTargetPitch;
                    TPScam.gameObject.SetActive(false);
                    FPScam.gameObject.SetActive(true);
                    characterManager.curControllerType = MPCharacterManager.ControllerType.FPS;
                    break;
            }
        }

        // public void ToggleCamera()
        // {
        //     if(FPScam == null)
        //     FPScam = characterManager?.camDictionary[characterManager.curControllerType = MPCharacterManager.ControllerType.FPS];
        //     if(TPScam == null)
        //     TPScam = characterManager?.camDictionary[characterManager.curControllerType = MPCharacterManager.ControllerType.TPS];

        //     switch (characterManager.curControllerType)
        //     {
        //         case MPCharacterManager.ControllerType.FPS:
        //             characterManager._FPSController.enabled = false;
        //             characterManager._TPSController.enabled = true;
        //             characterManager._TPSController.CinemachineTargetYaw = characterManager._FPSController.CinemachineTargetYaw;
        //             characterManager._TPSController.CinemachineTargetPitch = characterManager._FPSController.CinemachineTargetPitch;
        //             FPScam.gameObject.SetActive(false);
        //             TPScam.gameObject.SetActive(true);
        //             characterManager.curControllerType = MPCharacterManager.ControllerType.TPS;
        //             break;
        //         case MPCharacterManager.ControllerType.TPS:
                
        //             Vector3 cameraForward = mainCam.transform.forward;
        //             characterManager.transform.LookAt(characterManager.transform.position + cameraForward, Vector3.up);
                    
        //             characterManager._TPSController.enabled = false;
        //             characterManager._FPSController.enabled = true;
        //             characterManager._FPSController.CinemachineTargetYaw = characterManager._TPSController.CinemachineTargetYaw;
        //             characterManager._FPSController.CinemachineTargetPitch = characterManager._TPSController.CinemachineTargetPitch;
        //             TPScam.gameObject.SetActive(false);
        //             FPScam.gameObject.SetActive(true);
        //             characterManager.curControllerType = MPCharacterManager.ControllerType.FPS;
        //             break;
        //     }
            
        // }

        // public void SwitchSkin(){
        //     int curSkin = (int)characterManager.curSkinType;
            
        //     characterManager.skinDictionary[characterManager.curSkinType].gameObject.SetActive(false);

        //     curSkin++;
        //     if(curSkin >= characterManager._SkinCount) curSkin = 0;

        //     characterManager.skinDictionary[(MPCharacterManager.SkinType)curSkin].gameObject.SetActive(true);
        //     characterManager.curSkinType = (MPCharacterManager.SkinType)curSkin;
        // }

    }
}

