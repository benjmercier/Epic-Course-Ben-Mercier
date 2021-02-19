using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField]
    private Transform _spawnPos;
    [SerializeField]
    private Transform _targetPos;

    private Vector3 _spawnLookPos;
    private Quaternion _spawnRotation;

    [SerializeField]
    private int _initialSpawnCount = 10;
    private int _currentSpawnCount;

    [SerializeField]
    private int _maxWaves = 5;
    private int _waveIndex = 0;
    private int _activatedIndex = 0;
    private int _destroyedIndex = 0;

    private bool _canSpawn = false;

    private WaitForSeconds _spawnWait;
    [SerializeField]
    private float _spawnWaitTime = 5f;

    void Start()
    {
        if (_targetPos == null)
        {
            Debug.LogError("Your TargetPos is NULL, please try again.");
        }

        _currentSpawnCount = _initialSpawnCount;

        _canSpawn = true;
    }

    void Update()
    {
        if (_canSpawn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _waveIndex++;
                
                _currentSpawnCount *= _waveIndex;

                SpawnPrefabs(PoolManager.Instance.ReturnEnemyPool(), _currentSpawnCount);
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
        _spawnWait = AssignWait(_spawnWaitTime);

        for (int i = 0; i < spawnTotal; i++)
        {
            ActivatePrefabFromPool(pool);

            _activatedIndex++;
            Debug.Log("Enemies Activated: " + _activatedIndex);

            yield return _spawnWait;
        }

        while (_destroyedIndex < _activatedIndex)
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

                _spawnLookPos = _targetPos.position - prefab.transform.position;
                _spawnLookPos.y = 0;

                _spawnRotation = Quaternion.LookRotation(_spawnLookPos);

                prefab.transform.rotation = _spawnRotation;

                prefab.SetActive(true);

                return;
            }
        }
    }

    public int EnemySpawnCount() // may not need
    {
        return _initialSpawnCount;
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
