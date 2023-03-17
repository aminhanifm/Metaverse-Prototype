using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaversePrototype.Tools;

namespace MetaversePrototype.Game
{
    public class MPSpawner : MPObjectBounds
    {
        public enum SpawnerTypes { Player, AI }
        public SpawnerTypes spawnerTypes;
        private Bounds bounds;

        private void Awake()
        {
            bounds = GetBounds();
        }

        public void SpawnObjects(GameObject spawnedObject){
            // generate a random position on the surface of the plane
            Vector3 randomPositionOnPlane = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0f,
                Random.Range(bounds.min.z, bounds.max.z)
            );
            
            // calculate the spawn position by adding the random position on the plane
            // multiplied by the spawn distance to the center of the plane
            Vector3 center = bounds.center;
            Vector3 spawnPosition = center + (randomPositionOnPlane - center).normalized;
            
            // instantiate the object at the calculated spawn position
            Instantiate(spawnedObject, spawnPosition, Quaternion.identity);
        }
    }
}
