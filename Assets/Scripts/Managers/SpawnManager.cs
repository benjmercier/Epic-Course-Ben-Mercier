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
            Debug.LogError("SpawnManager::Start()::Your TargetPos is NULL.");
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
                _currentSpawnCount *= WaveManager.Instance.NextWave();

                SpawnPrefabs();
            }
        }
    }

    private IEnumerator WaveSpawnRoutine()
    {
        // get current wave
        // assign spawn wait to current wave spawn delay
        // check which sequence being used


        var activeWave = WaveManager.Instance.WaveList()[WaveManager.Instance.CurrentWave()];

        _spawnWait = AssignWait(activeWave.spawnWaitTime);

        if (activeWave.useBlockSequence && !activeWave.useCustomSequence && !activeWave.useRandomSequence)
        {
            // using block sequence
            // match prefabs to poolPrefabs 
            // retrieve prefab from pool 
            // amount to retrieve = spawn amount
        }
        else if (activeWave.useCustomSequence && !activeWave.useBlockSequence && !activeWave.useRandomSequence)
        {
            // using custom sequence
            // retrieve specific prefab from pool based on custom list prefabs
            // amount to retrieve = customWaveList.Length
        }
        else if (activeWave.useRandomSequence && !activeWave.useBlockSequence && !activeWave.useCustomSequence)
        {
            // using random sequence
            // retrieve random prefab from bool manager to set active
            // amount to retrieve = random wave total
        }
        else
        {
            Debug.LogError("SpawnManager::WaveSpawnRoutine()::Assign single sequence to Wave " + WaveManager.Instance.CurrentWave());
        }


        yield return _spawnWait; //
    }


    private void SpawnPrefabs()
    {
        _canSpawn = false;

        StartCoroutine(SpawnRoutine(_currentSpawnCount));
    }

    private IEnumerator SpawnRoutine(int spawnTotal)
    {
        _spawnWait = AssignWait(_spawnWaitTime);

        for (int i = 0; i < spawnTotal; i++)
        {
            ActivatePrefab(PoolManager.Instance.ReturnPrefabFromPool(true));

            yield return _spawnWait;
        }

        while (_destroyedIndex < _activatedIndex)
        {
            ActivatePrefab(PoolManager.Instance.ReturnPrefabFromPool(true));

            yield return _spawnWait;
        }
    }

    private void ActivatePrefab(GameObject prefab)
    {
        prefab.transform.position = _spawnPos.position;

        _spawnLookPos = _targetPos.position - prefab.transform.position;
        _spawnLookPos.y = 0;

        _spawnRotation = Quaternion.LookRotation(_spawnLookPos);

        prefab.transform.rotation = _spawnRotation;

        prefab.SetActive(true);
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

    public void EnemyDestroyed() // used for testing
    {
        _destroyedIndex++;

        Debug.Log("Enemies destroyed: " + _destroyedIndex);
    }

    private WaitForSeconds AssignWait(float wait)
    {
        return new WaitForSeconds(wait);
    }
}
