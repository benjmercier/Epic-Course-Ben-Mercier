using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewPool.asset", menuName = "Scriptable Objects/New Pool")]
    public class Pool : ScriptableObject
    {
        public string name;
        public int index;
        public int buffer;
        public GameObject[] prefabs;

        public Dictionary<int, List<GameObject>> dictionary;
        [HideInInspector]
        public List<GameObject> list;
    }
}


