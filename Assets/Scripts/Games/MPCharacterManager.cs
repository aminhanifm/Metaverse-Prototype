using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using Sirenix.OdinInspector;
using MetaversePrototype.Tools;
using Random = UnityEngine.Random;

namespace MetaversePrototype.Game
{
    public class MPCharacterManager : SerializedMonoBehaviour
    {
        public enum CharacterType { Player, OtherPlayer, NPC }
        public enum ControllerType { TPS, FPS }
        public enum SkinType { S1, S2, S3, S4, S5, S6, S7, S8, S9 }
        public GameObject characterModel;
        public CharacterType characterType;
        public SkinType curSkinType;
        private StarterAssetsInputs inputs;
        public StarterAssetsInputs _Inputs { get {return inputs;}}

        #region Player Component
        [ShowIf("characterType", CharacterType.Player)]
        public ControllerType curControllerType;
        [ShowIf("characterType", CharacterType.Player)]
        public Dictionary<ControllerType,CinemachineVirtualCamera> camDictionary;
        #endregion
        
        protected MPSpawner initialSpawner;
        public MPSpawner _InitialSpawner {
            get {return initialSpawner;} 
            set {initialSpawner = value;}
        }

        #region AI Component
        protected AIBrain AIBrain;
        public AIBrain _AIBrain {get {return AIBrain;}}
        protected NavMeshAgent agent;
        public NavMeshAgent _Agent {get {return agent;}}
        protected MPSpawner[] availablePatrolPoint;
        public MPSpawner[] _AvailablePatrolPoint {
            get {return availablePatrolPoint;} 
            set {availablePatrolPoint = value;}
        }
        #endregion
        
        public Dictionary<SkinType, GameObject> skinDictionary;
        protected ThirdPersonController TPSController;
        public ThirdPersonController _TPSController {get {return TPSController;}}
        protected FirstPersonController FPSController;
        public FirstPersonController _FPSController {get {return FPSController;}}
        protected int skinCount;
        public int _SkinCount {get {return skinCount;}}

        private void Awake() {
            PreInitalizeCharacter();
        }

        private void Start() {
            switch (characterType)
            {
                case CharacterType.Player:
                    InitalizeCamera();
                    InitalizeController();
                    break;
                case CharacterType.NPC:
                    InitalizeNPC();
                break;

            }
        }

        private void Update() {
            switch (characterType)
            {
                // case CharacterType.Player:
                // #if ENABLE_INPUT_SYSTEM && (UNITY_STANDALONE || UNITY_STANDALONE_WIN)
                //     // if(Input.GetKeyDown(KeyCode.Mouse1)) inputs.cursorLocked = true;
                //     // if(Input.GetKeyDown(KeyCode.Mouse2)) inputs.cursorLocked = false;
                //     print("Test");
                // #endif
                // break;
                case CharacterType.NPC:
                    ConvertAIInput();
                break;

            }

        }

        private void PreInitalizeCharacter(){
            TPSController = gameObject.MPGetComponentNoAlloc<ThirdPersonController>();
            FPSController = gameObject.MPGetComponentNoAlloc<FirstPersonController>();
            inputs = gameObject.MPGetComponentNoAlloc<StarterAssetsInputs>();
            skinCount = Enum.GetNames(typeof(SkinType)).Length;
        }

        #region Player Only
        private void InitalizeCamera(){
            CinemachineVirtualCamera[] vCams = GameObject.FindObjectsOfType<CinemachineVirtualCamera>(true);

            foreach (CinemachineVirtualCamera cam in vCams)
            {
                cam.Follow = transform.GetChild(0);
                if(cam.gameObject.MPGetComponentNoAlloc<MPTPSCamera>() != null) camDictionary.TryAdd(ControllerType.TPS, cam);
                if(cam.gameObject.MPGetComponentNoAlloc<MPFPSCamera>() != null) camDictionary.TryAdd(ControllerType.FPS, cam);
            }
        }

        private void InitalizeController(){
            MPCharacterUIController.Instance._CharacterManager = this;
            MPCharacterUIController.Instance._StarterAssetsInputs = gameObject.MPGetComponentNoAlloc<StarterAssetsInputs>();

            #if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)
                MobileDisableAutoSwitchControls.Instance._PlayerInput = gameObject.MPGetComponentNoAlloc<PlayerInput>();
                MobileDisableAutoSwitchControls.Instance.DisableAutoSwitchControls();
                MPCharacterUIController.Instance.mobileController.SetActive(true);
                inputs.cursorInputForLook = false;
                inputs.cursorLocked = false;
            #endif
        }
        #endregion

        #region NPC Only
            private void InitalizeNPC(){
                
                agent = gameObject.MPGetComponentAroundOrAdd<NavMeshAgent>();
                AIBrain = gameObject.MPGetComponentAroundOrAdd<AIBrain>();
                AIBrain.Owner = gameObject;

                RandomSkin();
            }

            private void ConvertAIInput(){
                // Get the agent's velocity and drop the y component
                Vector3 velocity = agent.velocity;
                Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

                // Normalize the vector to get a direction vector
                Vector2 direction = horizontalVelocity.normalized;

                // Set the input vector based on the direction and magnitude of the velocity
                inputs.move = direction * horizontalVelocity.magnitude;
            }
        #endregion

        #region Skins
        [ButtonGroup("Skin")]
        protected void SetSkin(){
            for (int i = 0; i < characterModel.transform.childCount-1; i++)
            {
                skinDictionary.TryAdd((SkinType)i, characterModel.transform.GetChild(i).gameObject);
            }
        }

        [ButtonGroup("Skin")]
        protected void ClearSkin(){
            skinDictionary.Clear();
        }

        [ButtonGroup("Skin")]
        public void RandomSkin(){
            List<SkinType> skinList = new List<SkinType>(skinDictionary.Keys);
            SkinType randomSkin = skinList[Random.Range(0, skinList.Count-1)];

            skinDictionary[curSkinType].gameObject.SetActive(false);
            skinDictionary[randomSkin].gameObject.SetActive(true);
            curSkinType = randomSkin;
        }
        #endregion
    }
}

