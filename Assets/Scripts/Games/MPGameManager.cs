using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MetaversePrototype.Tools;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace MetaversePrototype.Game
{
    public class MPGameManager : MPSingleton<MPGameManager>, IPunObservable
    {
        [ShowInInspector] [ReadOnly] private int[] NPCListViewID;
        public int[] _NPCList {get {return NPCListViewID;} set {NPCListViewID = value;}}
        private MPSpawner[] spawnerList;
        public MPSpawner[] _SpawnerList {get {return spawnerList;}}
        private MPSpawner lastChoosenSpawner;
        protected PhotonView photonView;
        public PhotonView _PhotonView {get {return photonView;} set { photonView = value;}}
        public MPSimpleObjectPooler NPCPooler;

        [Header("Player")]
        [AssetsOnly] public GameObject playerPrefab;

        private float curTime;
        private float delayTime;
        [Header("NPC")]
        [ShowInInspector] [ReadOnly] protected int npcCount = 0;
        [AssetsOnly] public GameObject NPCPrefab;
        public int _NPCCount {get {return npcCount;} set {npcCount = value;}}

        protected override void Awake() {
            spawnerList = GameObject.FindObjectsOfType<MPSpawner>();
            delayTime = Random.Range(10,20);
            
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool != null && playerPrefab != null && NPCPrefab != null)
            {
                pool.ResourceCache.Add(playerPrefab.name, playerPrefab);
                pool.ResourceCache.Add(NPCPrefab.name, NPCPrefab);
            }
        }

        private void Start() {
            SpawnCharacter(MPSpawner.SpawnerTypes.Player);
            _PhotonView = gameObject.MPGetComponentNoAlloc<PhotonView>();
            _NPCList = NPCPooler._ViewIDList;
        }

        private void Update() {
            curTime = curTime + 1f * Time.deltaTime;

            if(PhotonNetwork.IsConnected){
                if(!PhotonNetwork.IsMasterClient) return;

                CheckSpawnNPC();
            } else {
                CheckSpawnNPC();
            }
            
        }

        public void SpawnCharacter(MPSpawner.SpawnerTypes characterType){
            List<MPSpawner> relatedSpawner = new List<MPSpawner>();
            for (var i = 0; i < spawnerList.Length; i++)
            {
                if(spawnerList[i].spawnerTypes == characterType) relatedSpawner.Add(spawnerList[i]);
            }

            lastChoosenSpawner = relatedSpawner[Random.Range(0, relatedSpawner.Count)];
            lastChoosenSpawner.SpawnObjects();
        }

        public void CheckSpawnNPC(){
            if(curTime >= delayTime){
                curTime = 0;
                delayTime = Random.Range(10,20);

                if(npcCount < 10){
                    SpawnCharacter(MPSpawner.SpawnerTypes.NPC);
                    // _PhotonView.RPC("SpawnCharacter", RpcTarget.AllBuffered, MPSpawner.SpawnerTypes.NPC);
                }
            }
        }
        
        [Button]
        public void TestSpawnNPC(){
            if(npcCount < 10){
                SpawnCharacter(MPSpawner.SpawnerTypes.NPC);
                // _PhotonView.RPC("SpawnCharacter", RpcTarget.AllBuffered, MPSpawner.SpawnerTypes.NPC);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
            var list = NPCListViewID;

            if (stream.IsWriting){  
                stream.SendNext(list);
            } else {
                NPCListViewID = (int[])stream.ReceiveNext();
            }
        }   
    }
}

