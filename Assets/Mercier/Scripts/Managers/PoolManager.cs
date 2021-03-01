using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mercier.Scripts.ScriptableObjects;

namespace Mercier.Scripts.Managers
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        #region Hard-Coded Pool Values (Not In Use)
        /*
        [Header("Enemy Pool")] // change pools to scriptable objects
        [SerializeField]
        private int _enemyBuffer = 10;
        [SerializeField]
        private GameObject _enemyContainer;
        [SerializeField]
        private GameObject[] _enemyPrefabs;
        private Dictionary<int, List<GameObject>> _enemyPoolDictionary = new Dictionary<int, List<GameObject>>();
        private List<GameObject> _enemyPool;

        [Header("Turret Pool")]
        [SerializeField]
        private int _turretBuffer = 20;
        [SerializeField]
        private GameObject _turretContainer;
        [SerializeField]
        private GameObject[] _turretPrefabs;
        private Dictionary<int, List<GameObject>> _turretPoolDictionary = new Dictionary<int, List<GameObject>>();
        private List<GameObject> _turretPool;

        [Header("Decoy Turret Pool")]
        [SerializeField]
        private int _decoyTurretBuffer = 2;
        [SerializeField]
        private GameObject _decoyTurretContainer;
        [SerializeField]
        private GameObject[] _decoyTurretPrefabs;
        private Dictionary<int, List<GameObject>> _decoyTurretPoolDictionary = new Dictionary<int, List<GameObject>>();
        private List<GameObject> _decoyTurretPool;
        */
        #endregion

        [SerializeField]
        private List<Pool> _activePools = new List<Pool>();
        [SerializeField]
        private List<GameObject> _poolContainers = new List<GameObject>();

        private List<Dictionary<int, List<GameObject>>> _activeDictionaryList = new List<Dictionary<int, List<GameObject>>>();

        private Dictionary<int, List<GameObject>> _tempPoolDictionary;

        private GameObject _prefabFromPool;
        private int _poolIndex;
        private int _randomIndex;

        private void Start()
        {
            _activeDictionaryList = GeneratePoolDictionaries(_activePools);
        }

        public List<Dictionary<int, List<GameObject>>> GeneratePoolDictionaries(List<Pool> poolList)
        {
            _activeDictionaryList = new List<Dictionary<int, List<GameObject>>>();

            for (int a = 0; a < poolList.Count; a++)
            {
                _tempPoolDictionary = new Dictionary<int, List<GameObject>>();

                for (int b = 0; b < poolList[a].prefabs.Length; b++)
                {
                    _poolIndex = System.Array.IndexOf(poolList[a].prefabs, poolList[a].prefabs[b]);

                    poolList[a].list = new List<GameObject>();
                    poolList[a].list = GeneratePool(poolList[a].buffer, poolList[a].prefabs[b], _poolContainers[a], poolList[a].list);

                    _tempPoolDictionary.Add(_poolIndex, poolList[a].list);
                }

                _activeDictionaryList.Add(_tempPoolDictionary);
            }

            return _activeDictionaryList;
        }

        private List<GameObject> GeneratePool(int buffer, GameObject prefab, GameObject container, List<GameObject> pool)
        {
            for (int i = 0; i < buffer; i++)
            {
                pool.Add(GeneratePrefab(prefab, container));
            }

            return pool;
        }

        private GameObject GeneratePrefab(GameObject prefab, GameObject container)
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.parent = container.transform;
            obj.SetActive(false);

            return obj;
        }

        public GameObject ReturnPrefabFromPool(bool isRandom, int dictionaryID, int dictionaryKey)
        {
            return PrefabToReturn(isRandom, _activeDictionaryList[dictionaryID], dictionaryKey, _activePools[dictionaryID].prefabs, _poolContainers[dictionaryID]);
        }

        private GameObject PrefabToReturn(bool isRandom, Dictionary<int, List<GameObject>> dictionary, int dictionaryKey, GameObject[] prefabs, GameObject container)
        {
            if (isRandom)
            {
                _randomIndex = Random.Range(0, dictionary.Count);

                CheckIfActive(dictionary, _randomIndex, prefabs, container);
            }
            else
            {
                CheckIfActive(dictionary, dictionaryKey, prefabs, container);
            }

            return _prefabFromPool;
        }

        private void CheckIfActive(Dictionary<int, List<GameObject>> dictionary, int dictionaryKey, GameObject[] prefabs, GameObject container)
        {
            if (dictionary[dictionaryKey].Any(a => !a.activeInHierarchy))
            {
                _prefabFromPool = dictionary[dictionaryKey].FirstOrDefault(f => !f.activeInHierarchy);
            }
            else
            {
                _prefabFromPool = GeneratePrefab(prefabs[dictionaryKey], container);

                dictionary[dictionaryKey].Add(_prefabFromPool);
            }
        }

        #region Generate & Return Code W/O Using Scriptable Objects (Not In Use)
        /*
        private void Start()
        {
            //_enemyPoolDictionary = GeneratePoolDictionary(_enemyBuffer, _enemyPrefabs, _enemyContainer, _enemyPool);
            //_turretPoolDictionary = GeneratePoolDictionary(_turretBuffer, _turretPrefabs, _turretContainer, _turretPool);
            //_decoyTurretPoolDictionary = GeneratePoolDictionary(_decoyTurretBuffer, _decoyTurretPrefabs, _decoyTurretContainer, _decoyTurretPool);
        }         
        
        ***Generate GameObject***
        public Dictionary<int, List<GameObject>> GeneratePoolDictionary(int buffer, GameObject[] prefabArray, GameObject container, List<GameObject> pool)
        {
            _activePoolDictionary = new Dictionary<int, List<GameObject>>();

            for (int i = 0; i < prefabArray.Length; i++)
            {
                _poolIndex = System.Array.IndexOf(prefabArray, prefabArray[i]);

                pool = new List<GameObject>();
                pool = GeneratePool(buffer, prefabArray[i], container, pool);

                _activePoolDictionary.Add(_poolIndex, pool);
            }

            return _activePoolDictionary;
        
        ***Return GameObject***
        public GameObject ReturnEnemyFromPool(bool isRandom, int dictionaryKey)
        {
            return ReturnPrefabFromPool(isRandom, _enemyPoolDictionary, dictionaryKey, _enemyPrefabs, _enemyContainer);
        }

        public GameObject ReturnTurretFromPool(bool isRandom, int dictionaryKey)
        {
            return ReturnPrefabFromPool(isRandom, _turretPoolDictionary, dictionaryKey, _turretPrefabs, _turretContainer);
        }

        public GameObject ReturnDecoyTurretFromPool(bool isRandom, int dictionaryKey)
        {
            return ReturnPrefabFromPool(isRandom, _decoyTurretPoolDictionary, dictionaryKey, _decoyTurretPrefabs, _decoyTurretContainer);
        }*/
        #endregion
    }
}



