using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Managers
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private Dictionary<int, List<GameObject>> _poolDictionary = new Dictionary<int, List<GameObject>>();
        private int _poolIndex;
        private List<GameObject> _pool;

        [SerializeField]
        private int _poolBuffer = 10;

        [SerializeField]
        private GameObject _enemyContainer;
        [SerializeField]
        private GameObject[] _enemyPrefabs;
        private GameObject _enemy;

        private int _randomIndex;

        private void Start()
        {
            _poolDictionary = GeneratePoolDictionary(_poolBuffer, _enemyPrefabs, _enemyContainer);
        }

        private Dictionary<int, List<GameObject>> GeneratePoolDictionary(int buffer, GameObject[] prefabArray, GameObject container)
        {
            for (int i = 0; i < prefabArray.Length; i++)
            {
                _poolIndex = System.Array.IndexOf(prefabArray, prefabArray[i]);

                _pool = new List<GameObject>();
                _pool = GeneratePool(buffer, prefabArray[i], container, _pool);

                _poolDictionary.Add(_poolIndex, _pool);
            }

            return _poolDictionary;
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

        public GameObject ReturnPrefabFromPool(bool isRandom, int dictionaryKey)
        {
            if (isRandom)
            {
                _randomIndex = Random.Range(0, _poolDictionary.Count);

                if (_poolDictionary[_randomIndex].Any(a => !a.activeInHierarchy))
                {
                    _enemy = _poolDictionary[_randomIndex].FirstOrDefault(f => !f.activeInHierarchy);
                }
                else
                {
                    _enemy = GeneratePrefab(_enemyPrefabs[_randomIndex], _enemyContainer);

                    _poolDictionary[_randomIndex].Add(_enemy);
                }
            }
            else
            {
                if (_poolDictionary[dictionaryKey].Any(a => !a.activeInHierarchy))
                {
                    _enemy = _poolDictionary[dictionaryKey].FirstOrDefault(b => !b.activeInHierarchy);
                }
                else
                {
                    _enemy = GeneratePrefab(_enemyPrefabs[dictionaryKey], _enemyContainer);

                    _poolDictionary[dictionaryKey].Add(_enemy);
                }
            }

            return _enemy;
        }

        public Dictionary<int, List<GameObject>> ReturnPoolDictionary()
        {
            var dictionary = _poolDictionary;

            return dictionary;
        }
    }
}



