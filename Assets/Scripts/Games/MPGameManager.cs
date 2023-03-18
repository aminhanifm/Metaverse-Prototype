using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MetaversePrototype.Tools;
using Random = UnityEngine.Random;

namespace MetaversePrototype.Game
{
    public class MPGameManager : MPSingleton<MPGameManager>
    {
        private MPSpawner[] spawnerList;
        public MPSpawner[] _SpawnerList {get {return spawnerList;}}
        private MPSpawner lastChoosenSpawner;

        [Header("Player")]
        [AssetsOnly] public GameObject playerPrefab;

        private float curTime;
        private float delayTime;
        [Header("NPC")]
        [ShowInInspector] [ReadOnly] protected int npcCount = 0;
        public int _NPCCount {get {return npcCount;} set {npcCount = value;}}

        protected override void Awake() {
            spawnerList = GameObject.FindObjectsOfType<MPSpawner>();
            delayTime = Random.Range(10,20);
        }

        private void Start() {
            SpawnCharacter(MPSpawner.SpawnerTypes.Player);
        }

        private void Update() {
            curTime = curTime + 1f * Time.deltaTime;

            if(curTime >= delayTime){
                curTime = 0;
                delayTime = Random.Range(10,20);

                if(npcCount < 10){
                    SpawnCharacter(MPSpawner.SpawnerTypes.NPC);
                }
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
        
        #region UTILITY
        [Button]
        public void TestSpawnNPC(){
            List<MPSpawner> relatedSpawner = new List<MPSpawner>();
            for (var i = 0; i < spawnerList.Length; i++)
            {
                if(spawnerList[i].spawnerTypes == MPSpawner.SpawnerTypes.NPC) relatedSpawner.Add(spawnerList[i]);
            }

            lastChoosenSpawner = relatedSpawner[Random.Range(0, relatedSpawner.Count)];
            lastChoosenSpawner.SpawnObjects();
        }

        [Button]
        public void RemoveSpawnedNPC(){
            lastChoosenSpawner?.objectPooler?.DestroyObjectPool();
            lastChoosenSpawner?.objectPooler?.FillObjectPool();
        }
        #endregion
    }
}

