using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace MetaversePrototype.Tools
{
	public class MPObjectPool : MonoBehaviour
	{
		[ReadOnly]
		public List<GameObject> PooledGameObjects;
	}
}