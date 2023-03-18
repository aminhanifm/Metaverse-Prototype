using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using MetaversePrototype.Tools;
using Random = UnityEngine.Random;

namespace MetaversePrototype.Game
{
    public class MPSpawner : MPObjectBounds
    {
        public enum SpawnerTypes { Player, NPC }
        public SpawnerTypes spawnerTypes;
        public MPSimpleObjectPooler objectPooler;
        private Collider _collider;
        private MPSpawnAroundProperties spawnerProperties;

        private void Awake()
        {
            _collider = gameObject.MPGetComponentNoAlloc<Collider>();
        }

        public void SpawnObjects(){

            GameObject spawnedObject  = objectPooler.GetPooledGameObject();
            MPCharacterManager characterManager = spawnedObject.gameObject.MPGetComponentNoAlloc<MPCharacterManager>();

            if (spawnedObject == null) { return; }
			if (spawnedObject.GetComponent<MPPoolableObject>() == null)
			{
				throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
			}

            Vector3 spawnPosition = MPBoundsExtensions.MPRandomPointInBounds(_collider.bounds);

            #if UNITY_EDITOR
                EditorGUIUtility.PingObject(spawnedObject);
            #endif

            spawnedObject.transform.position = spawnPosition;
            // spawnedObject.transform.rotation = Quaternion.AngleAxis(Random.Range(0,360), Vector3.up);;
            spawnedObject.gameObject.SetActive(true);
            characterManager.RandomSkin();
            characterManager._InitialSpawner = this;

            if(characterManager.characterType == MPCharacterManager.CharacterType.NPC){
                MPGameManager.Instance._NPCCount++; //Keep track of NPC count

                if(Random.value <= .5f){
                    characterManager._Inputs.sprint = true;
                    characterManager._TPSController.SprintSpeed = Random.Range(3,5);
                }
                else characterManager._Inputs.sprint = false;
                
                MPSpawner[] availableSpawner = new MPSpawner[MPGameManager.Instance._SpawnerList.Length];
                for (var i = 0; i < availableSpawner.Length; i++)
                {
                    availableSpawner[i] = null;
                }

                for (var i = 0; i < MPGameManager.Instance._SpawnerList.Length; i++)
                {
                    if (MPGameManager.Instance._SpawnerList[i] != characterManager._InitialSpawner
                        && MPGameManager.Instance._SpawnerList[i].spawnerTypes == SpawnerTypes.NPC)
                    {
                        availableSpawner[i] = MPGameManager.Instance._SpawnerList[i];
                    }
                }

                MPSpawner[] nonNullSpawnerArray = Array.FindAll(availableSpawner, spawner => spawner != null);

                characterManager._AvailablePatrolPoint = nonNullSpawnerArray;

                // for (var i = 0; i < characterManager._AvailablePatrolPoint.Length; i++)
                // {
                //     if (characterManager._AvailablePatrolPoint[i] != null)
                //     {
                //         Debug.Log("index of " + i + " is: " + characterManager._AvailablePatrolPoint[i].name);
                //     }
                // }
            
            }
        }

    }
}
