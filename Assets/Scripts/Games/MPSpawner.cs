using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Random = UnityEngine.Random;
using MetaversePrototype.Tools;
using Photon.Pun;
using Sirenix.OdinInspector;
using MSLIMA.Serializer;

namespace MetaversePrototype.Game
{
    public class MPSpawner : MonoBehaviourPunCallbacks
    {
        public struct NPCProperties{
            public Vector3 spawnPosition;
            public Vector3[] availableSpawnPoint;
            public float sprintSpeed;
            public bool isSprint;
            public MPCharacterManager.SkinType skinType;
        }
        public enum SpawnerTypes { Player, NPC }
        public SpawnerTypes spawnerTypes;
        [ShowIf("spawnerTypes", SpawnerTypes.NPC)]
        public MPSimpleObjectPooler objectPooler;
        private Collider _collider;
        private MPSpawnAroundProperties spawnerProperties;

        private void Awake()
        {
            _collider = gameObject.MPGetComponentNoAlloc<Collider>();
            Serializer.RegisterCustomType<MPSpawner>((byte)'B');
        }

        public void SpawnObjects(){
            Vector3 spawnPosition = MPBoundsExtensions.MPRandomPointInBounds(_collider.bounds);

            GameObject spawnedObject  = null;
            if(PhotonNetwork.IsConnected){
                switch (spawnerTypes)
                {
                    case SpawnerTypes.Player:
                        spawnedObject = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
                        break;
                    case SpawnerTypes.NPC:
                        // spawnedObject = PhotonNetwork.InstantiateRoomObject(objectPooler.GameObjectToPool.name, spawnPosition, Quaternion.identity);
                        spawnedObject = objectPooler.GetPooledGameObject();
                        break;
                }
                
            } else {
                print("PhotonNetwork is not connected!");
                spawnedObject = objectPooler.GetPooledGameObject();
            }   
            // spawnedObject = objectPooler.GetPooledGameObject();

            MPCharacterManager characterManager = spawnedObject.gameObject.MPGetComponentNoAlloc<MPCharacterManager>();

            if (spawnedObject == null) { return; }
			if (spawnedObject.GetComponent<MPPoolableObject>() == null)
			{
				throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
			}


            // #if UNITY_EDITOR
            //     EditorGUIUtility.PingObject(spawnedObject);
            // #endif

            spawnedObject.transform.position = spawnPosition;
            // spawnedObject.transform.rotation = Quaternion.AngleAxis(Random.Range(0,360), Vector3.up);;
            spawnedObject.gameObject.SetActive(true);
            characterManager._InitialSpawner = this;
            
            if(characterManager.characterType == MPCharacterManager.CharacterType.NPC){
                MPGameManager.Instance._NPCCount++; //Keep track of NPC count
                characterManager.RandomSkinNPC();
                
                if(Random.value <= .5f){
                    characterManager._Inputs.sprint = true;
                    characterManager._TPSController.SprintSpeed = Random.Range(3,5);
                }
                else characterManager._Inputs.sprint = false;
                
                Vector3[] availableSpawner = new Vector3[MPGameManager.Instance._SpawnerList.Length];
                for (var i = 0; i < availableSpawner.Length; i++)
                {
                    availableSpawner[i] = Vector3.zero;
                }

                for (var i = 0; i < MPGameManager.Instance._SpawnerList.Length; i++)
                {
                    if (MPGameManager.Instance._SpawnerList[i] != characterManager._InitialSpawner
                        && MPGameManager.Instance._SpawnerList[i].spawnerTypes == SpawnerTypes.NPC)
                    {
                        availableSpawner[i] = MPGameManager.Instance._SpawnerList[i].transform.position;
                    }
                }

                Vector3[] nonVectorZero = Array.FindAll(availableSpawner, spawner => spawner != Vector3.zero);

                characterManager._AvailablePatrolPoint = nonVectorZero;
                
                NPCProperties customProps = new NPCProperties();
                customProps.spawnPosition = spawnPosition;
                customProps.availableSpawnPoint = nonVectorZero;
                customProps.skinType = characterManager.curSkinType;
                customProps.isSprint = characterManager._Inputs.sprint;
                customProps.sprintSpeed = characterManager._TPSController.SprintSpeed;

                byte[] customPropsBytes = Serialize(customProps);

                photonView.RPC("SpawnObjectsRPC", RpcTarget.OthersBuffered, customPropsBytes);
            }
        }

        [PunRPC]
        public void SpawnObjectsRPC(byte[] customPropsBytes){
            NPCProperties customProps = (NPCProperties)Deserialize(customPropsBytes);
            
            GameObject spawnedObject = objectPooler.GetPooledGameObject();
            MPCharacterManager characterManager = spawnedObject.gameObject.MPGetComponentNoAlloc<MPCharacterManager>();

            spawnedObject.transform.position = customProps.spawnPosition;
            spawnedObject.gameObject.SetActive(true);
            characterManager.skinDictionary[characterManager.curSkinType].gameObject.SetActive(false);
            characterManager.curSkinType = customProps.skinType;
            characterManager.skinDictionary[customProps.skinType].gameObject.SetActive(true);
            characterManager._InitialSpawner = this;
            characterManager._Inputs.sprint = customProps.isSprint;
            characterManager._TPSController.SprintSpeed = customProps.sprintSpeed;
            characterManager._AvailablePatrolPoint = customProps.availableSpawnPoint;

            MPGameManager.Instance._NPCCount++;
        }
        

        public static byte[] Serialize(object customObject)
        {
            NPCProperties props = (NPCProperties)customObject;
            byte[] bytes = new byte[0];

            Serializer.Serialize(props.spawnPosition, ref bytes);
            Serializer.Serialize(props.availableSpawnPoint, ref bytes);
            Serializer.Serialize(props.sprintSpeed, ref bytes);
            Serializer.Serialize(props.isSprint, ref bytes);
            Serializer.Serialize((int)props.skinType, ref bytes);

            return bytes;
        }

        public static object Deserialize(byte[] bytes)
        {
            NPCProperties props = new NPCProperties();
            int offset = 0;

            props.spawnPosition = Serializer.DeserializeVector3(bytes, ref offset);
            props.availableSpawnPoint = Serializer.DeserializeVector3Array(bytes, ref offset);
            props.sprintSpeed = Serializer.DeserializeFloat(bytes, ref offset);
            props.isSprint = Serializer.DeserializeBool(bytes, ref offset);
            props.skinType = (MPCharacterManager.SkinType)Serializer.DeserializeInt(bytes, ref offset);

            return props;
        }
    }
}
