using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using MetaversePrototype.Tools;

namespace MetaversePrototype.Game
{
    public class MPAIDebugPathfind : MonoBehaviour
    {
        #if UNITY_EDITOR
        private NavMeshAgent _agent;

        private void Start()
        {
            _agent = gameObject.MPGetComponentAroundOrAdd<NavMeshAgent>();
        }

        private void Update()
        {
            if (_agent.hasPath)
            {
                Debug.DrawLine(transform.position, _agent.destination, Color.blue);
            }
        }
        #endif
    }
    
}
