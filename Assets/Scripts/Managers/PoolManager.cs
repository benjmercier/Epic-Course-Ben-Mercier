using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoSingleton<PoolManager>
{
    private Dictionary<int, List<GameObject>> _poolDictionary = new Dictionary<int, List<GameObject>>();
    private string _poolName;
    private int _poolIndex;
    private List<GameObject> _pool;

    [SerializeField]
    private int _poolBuffer = 10;

    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _enemyPrefabs;

    /* Notes From 2/19/21 Coaching Call
     * can use a dictionary to store lists of game objects
     * dynamically create lists to then store in dictionary?
     * scriptable objects for waves
     */

    private int _randomIndex;

    private void Start()
    {
        _poolDictionary = GeneratePoolDictionary(_poolBuffer, _enemyPrefabs, _enemyContainer);
    }

    private Dictionary<int, List<GameObject>> GeneratePoolDictionary(int buffer, GameObject[] prefabArray, GameObject container)
    {
        for (int i = 0; i < prefabArray.Length; i++)
        {
            //_poolName = GeneratePoolName(prefabArray[i].name);
            _poolIndex = System.Array.IndexOf(prefabArray, prefabArray[i]);

            _pool = new List<GameObject>();
            _pool = GeneratePool(buffer, prefabArray[i], container, _pool);

            //_poolDictionary.Add(_poolName, _pool);
            _poolDictionary.Add(_poolIndex, _pool);
        }

        foreach (var item in _poolDictionary)
        {
            Debug.Log("Pool Index: " + item.Key);
            Debug.Log("Pool Count: " + item.Value.Count);
        }

        return _poolDictionary;
    }
    
    private string GeneratePoolName(string name)
    {
        return name + "Pool";
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

    public GameObject ReturnPrefabFromPool(bool isRandom)
    {
        GameObject enemy = new GameObject();

        if (isRandom)
        {
            // select random pool in dictionary
            // retrieve object in pool that isn't active
            _randomIndex = Random.Range(0, _poolDictionary.Count);

            if (_poolDictionary[_randomIndex].Any(a => !a.activeInHierarchy))
            {
                enemy = _poolDictionary[_randomIndex].FirstOrDefault(f => !f.activeInHierarchy);
            }
            else
            {
                // create object to return and add to pool in dictionary
                enemy = GeneratePrefab(_enemyPrefabs[_randomIndex], _enemyContainer);

                _poolDictionary[_randomIndex].Add(enemy);
            }
        }
        else
        {

        }

        return enemy;
    }
}
