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

        _canSpawn = true;
    }

    void Update()
    {
        if (_canSpawn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _canSpawn = false;

                WaveManager.Instance.StartWave();
            }
        }
    }

    public void ActivatePrefab(bool isRandom, int dictionaryKey)
    {
        GameObject prefab = PoolManager.Instance.ReturnPrefabFromPool(isRandom, dictionaryKey);

        prefab.transform.position = _spawnPos.position;

        _spawnLookPos = _targetPos.position - prefab.transform.position;
        _spawnLookPos.y = 0;

        _spawnRotation = Quaternion.LookRotation(_spawnLookPos);

        prefab.transform.rotation = _spawnRotation;

        prefab.SetActive(true);
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
}
