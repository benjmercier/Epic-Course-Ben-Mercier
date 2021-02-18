using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField]
    private int _enemiesToSpawn = 10;
    
    [SerializeField]
    private Transform _spawnPos;
    [SerializeField]
    private Transform _targetPos;

    private int _waveIndex;
    private int _activatedIndex = 0;
    private int _destroyedIndex = 0;

    private bool _canSpawn = false;

    private WaitForSeconds _spawnWait;
    [SerializeField]
    private float _waitTime = 10f;

    void Start()
    {
        if (_targetPos == null)
        {
            Debug.LogError("TargetPos is NULL.");
        }

        _canSpawn = true;
    }

    void Update()
    {
        if (_canSpawn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnPrefabs(PoolManager.Instance.ReturnEnemyPool(), _enemiesToSpawn);
            }
        }
    }

    private void SpawnPrefabs(List<GameObject> pool, int spawnTotal)
    {
        _canSpawn = false;

        if (pool.Any())
        {
            StartCoroutine(SpawnRoutine(pool, spawnTotal));
        }
    }

    IEnumerator SpawnRoutine(List<GameObject> pool, int spawnTotal)
    {
        _spawnWait = AssignWait(_waitTime);

        for (int i = 0; i < spawnTotal; i++)
        {
            ActivatePrefabFromPool(pool);

            _activatedIndex++;
            Debug.Log("Enemies Activated: " + _activatedIndex);

            yield return _spawnWait;
        }

        while (_destroyedIndex != _activatedIndex)
        {
            ActivatePrefabFromPool(pool);

            yield return _spawnWait;
        }
    }
    
    private void ActivatePrefabFromPool(List<GameObject> pool)
    {
        foreach (var prefab in pool)
        {
            if (!prefab.activeInHierarchy)
            {
                prefab.transform.position = _spawnPos.position;

                prefab.SetActive(true);

                return;
            }
        }
    }

    public int EnemySpawnCount()
    {
        return _enemiesToSpawn;
    }

    public Vector3 AssignSpawnPos()
    {
        return _spawnPos.position;
    }

    public Vector3 AssignTargetPos()
    {
        return _targetPos.position;
    }

    private WaitForSeconds AssignWait(float wait)
    {
        return new WaitForSeconds(wait);
    }
}
