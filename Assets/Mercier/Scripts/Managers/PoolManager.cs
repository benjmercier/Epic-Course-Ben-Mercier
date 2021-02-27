using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Managers
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [Header("Enemy Pool")]
        [SerializeField]
        private int _enemyBuffer = 10;
        [SerializeField]
        private GameObject _enemyContainer;
        [SerializeField]
        private GameObject[] _enemyPrefabs;
        private Dictionary<int, List<GameObject>> _enemyPoolDictionary = new Dictionary<int, List<GameObject>>();
        private List<GameObject> _enemyPool;

        [Header("TurretPool")]
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

        private Dictionary<int, List<GameObject>> _activePoolDictionary;

        private GameObject _prefabFromPool;
        private int _poolIndex;
        private int _randomIndex;

        private void Start()
        {
            _enemyPoolDictionary = GeneratePoolDictionary(_enemyBuffer, _enemyPrefabs, _enemyContainer, _enemyPool);
            _turretPoolDictionary = GeneratePoolDictionary(_turretBuffer, _turretPrefabs, _turretContainer, _turretPool);
            _decoyTurretPoolDictionary = GeneratePoolDictionary(_decoyTurretBuffer, _decoyTurretPrefabs, _decoyTurretContainer, _decoyTurretPool);
        }

        #region Generate GameObject Pool
        private Dictionary<int, List<GameObject>> GeneratePoolDictionary(int buffer, GameObject[] prefabArray, GameObject container, List<GameObject> pool)
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
        #endregion

        #region Return GameObject from Pool
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
        }

        private GameObject ReturnPrefabFromPool(bool isRandom, Dictionary<int, List<GameObject>> dictionary, int dictionaryKey, GameObject[] prefabs, GameObject container)
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
        #endregion
    }
}



