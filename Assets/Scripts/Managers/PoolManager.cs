using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField]
    private int _poolBuffer = 10;

    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private List<GameObject> _enemyPool;

    private int _randomIndex;

    private void Start()
    {
        _enemyPool = GenerateObjectPool(_poolBuffer, _enemyPrefabs, _enemyContainer, _enemyPool);
    }

    private List<GameObject> GenerateObjectPool(int amount, GameObject[] prefabs, GameObject container, List<GameObject> pool)
    {
        for (int i = 0; i < amount; i++)
        {
            pool.Add(PrefabToAdd(prefabs, container));
        }

        return pool;
    }

    private GameObject PrefabToAdd(GameObject[] prefabs, GameObject container)
    {
        _randomIndex = Random.Range(0, prefabs.Length);

        GameObject prefab = Instantiate(prefabs[_randomIndex]);
        prefab.transform.parent = container.transform;
        prefab.SetActive(false);

        return prefab;
    }

    public List<GameObject> ReturnEnemyPool()
    {
        return _enemyPool;
    }
}
