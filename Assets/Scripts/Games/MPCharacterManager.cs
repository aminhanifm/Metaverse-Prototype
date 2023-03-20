using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using Sirenix.OdinInspector;
using MetaversePrototype.Tools;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

namespace MetaversePrototype.Game
{
    public class MPCharacterManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        public enum CharacterType { Player, OtherPlayer, NPC }
        public enum ControllerType { TPS, FPS }
        public enum SkinType { S1, S2, S3, S4, S5, S6, S7, S8, S9 }
        public GameObject characterModel;
        public CharacterType characterType;
        public SkinType curSkinType;
        private CharacterController controller;
        public CharacterController _Controller { get {return controller;} set {controller = value;}}
        private StarterAssetsInputs inputs;
        public StarterAssetsInputs _Inputs { get {return inputs;}}
        private PlayerInput playerInputs;
        public PlayerInput _PlayerInputs { get {return playerInputs;}}

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
        protected Vector3[] availablePatrolPoint;
        public Vector3[] _AvailablePatrolPoint {
            get {return availablePatrolPoint;} 
            set {availablePatrolPoint = value;}
        }
        protected const string idleState = "Idle";

        #endregion
        
        public Dictionary<SkinType, GameObject> skinDictionary;
        protected ThirdPersonController TPSController;
        public ThirdPersonController _TPSController {get {return TPSController;}}
        protected FirstPersonController FPSController;
        public FirstPersonController _FPSController {get {return FPSController;}}
        protected int skinCount;
        public int _SkinCount {get {return skinCount;}}

        private void Awake() {
            SetInitialSkin();
            PreInitalizeCharacter();
        }

        private void Start() {
            // Set the initial skin
            SetSkin(curSkinType);

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

        private void FixedUpdate() {
            if(characterType == CharacterType.NPC && PhotonNetwork.IsMasterClient)
            {
                ConvertAIInput();
            }
        }

        private void PreInitalizeCharacter(){
            TPSController = gameObject.MPGetComponentNoAlloc<ThirdPersonController>();
            FPSController = gameObject.MPGetComponentNoAlloc<FirstPersonController>();
            _Controller = gameObject.MPGetComponentNoAlloc<CharacterController>();
            inputs = gameObject.MPGetComponentNoAlloc<StarterAssetsInputs>();
            playerInputs = gameObject.MPGetComponentNoAlloc<PlayerInput>();
            skinCount = Enum.GetNames(typeof(SkinType)).Length;
        }

        #region Player Only
        private void InitalizeCamera(){
            camDictionary = new Dictionary<ControllerType, CinemachineVirtualCamera>();
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

                // RandomSkinNPC();
            }

            private void ConvertAIInput(){
                // Get the agent's velocity and drop the y component
                Vector3 velocity = agent.desiredVelocity;
                Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);

                // Normalize the vector to get a direction vector
                Vector2 direction = horizontalVelocity.normalized;

                // Set the input vector based on the direction and magnitude of the velocity
                inputs.move = direction * horizontalVelocity.magnitude;
            }
        #endregion

        #region Skins
        [ButtonGroup("Skin")]
        protected void SetInitialSkin(){
            skinDictionary = new Dictionary<SkinType, GameObject>();
            for (int i = 0; i < characterModel.transform.childCount-1; i++)
            {
                skinDictionary.TryAdd((SkinType)i, characterModel.transform.GetChild(i).gameObject);
            }
        }

        [ButtonGroup("Skin")]
        protected void ClearSkin(){
            skinDictionary.Clear();
        }

        [PunRPC]
        public void SetSkin(SkinType newSkin) {
            // Set the new skin
            skinDictionary[curSkinType].gameObject.SetActive(false);
            skinDictionary[newSkin].gameObject.SetActive(true);
            curSkinType = newSkin;

            // Set the custom property on the PhotonPlayer
            Hashtable playerProps = new Hashtable();
            playerProps["skinType"] = curSkinType.ToString();
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }

        [ButtonGroup("Skin")]
        public void RandomSkin(){
            List<SkinType> skinList = new List<SkinType>(skinDictionary.Keys);
            SkinType randomSkin = skinList[Random.Range(0, skinList.Count-1)];

            // Set the new skin
            skinDictionary[curSkinType].gameObject.SetActive(false);
            skinDictionary[randomSkin].gameObject.SetActive(true);
            curSkinType = randomSkin;
            photonView.RPC("SetSkin", RpcTarget.AllBuffered, curSkinType);
        }

        public void RandomSkinNPC(){
            List<SkinType> skinList = new List<SkinType>(skinDictionary.Keys);
            SkinType randomSkin = skinList[Random.Range(0, skinList.Count-1)];

            // Set the new skin
        
            foreach (var skin in skinDictionary)
            {
                if(skin.Key != randomSkin) skin.Value.gameObject.SetActive(false);
                else skinDictionary[randomSkin].gameObject.SetActive(true);
            }
            
            curSkinType = randomSkin;
        }

        [PunRPC]
        public void DestroyNPC(){
            AIBrain.TransitionToState(idleState);
			MPGameManager.Instance._NPCCount--; //Keep track of NPC count
            MPPoolableObject selfObject = gameObject.MPGetComponentAroundOrAdd<MPPoolableObject>();
			selfObject.Destroy();
        }

        #endregion

        #region PUN2
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            // e.g. store this gameobject as this player's charater in Player.TagObject
            // info.Sender.TagObject = this.gameObject;
            
            if(photonView.IsMine){
                RandomSkin();
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "skinType", (int)curSkinType} });
                _Inputs.enabled = true;
                TPSController.enabled = true;
                // playerInputs.enabled = true;
                photonView.RPC("SetSkin", RpcTarget.AllBuffered, curSkinType);
            }

            if(!photonView.IsMine){
                TPSController._EnableAnimator = false;
                FPSController._EnableAnimator = false;
                
                switch (characterType)
                {
                    case CharacterType.Player:
                        characterType = CharacterType.OtherPlayer;
                        _Inputs.enabled = false;
                        // playerInputs.enabled = false;
                        TPSController.enabled = false;
                        FPSController.enabled = false;
                        // photonView.RPC("SetSkin", RpcTarget.Others, curSkinType);
                        break;

                    case CharacterType.NPC:
                        if(!PhotonNetwork.IsMasterClient){
                            InitalizeNPC();
                            if(MPGameManager.Instance.NPCPooler.WaitingPool.transform != null)
                            transform.SetParent(MPGameManager.Instance.NPCPooler.WaitingPool.transform);
                            
                            if(!MPGameManager.Instance.NPCPooler.ObjectPool.PooledGameObjects
                                .Contains(transform.gameObject)){
                                    MPGameManager.Instance.NPCPooler.ObjectPool.PooledGameObjects.Add(transform.gameObject);
                                    MPGameManager.Instance.NPCPooler.PooledGameObjects.Add(transform.gameObject);
                                }

                            gameObject.SetActive(false);
                            
                        }
                        break;
                }
            }
        }
        #endregion
    }
}

