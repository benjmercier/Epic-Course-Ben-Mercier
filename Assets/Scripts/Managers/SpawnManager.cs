using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField]
    private int _enemiesToSpawn = 10;
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private List<GameObject> _enemyPool;

    [SerializeField]
    private Transform _spawnPos;
    [SerializeField]
    private Transform _targetPos;

    private int _randomIndex;
    private int _waveIndex;
    private int _activatedIndex = 0;
    private int _destroyedIndex = 0;

    private bool _canSpawn = false;

    private WaitForSeconds _spawnWait;
    [SerializeField]
    private float _waitTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (_targetPos == null)
        {
            _targetPos = GameObject.Find("TargetPos").GetComponent<Transform>();
        }

        _enemyPool = GenerateObjectPool(_enemiesToSpawn, _enemyPrefabs, _enemyContainer, _enemyPool);

        _canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_canSpawn)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnPrefabs(_enemyPool, _enemiesToSpawn);
            }
        }
    }

    private List<GameObject> GenerateObjectPool(int amount, GameObject[] prefabs, GameObject container, List<GameObject> pool)
    {
        for (int i = 0; i < amount; i++)
        {
            pool.Add(PrefabToAdd(prefabs, container, pool));
        }

        return pool;
    }

    private GameObject PrefabToAdd(GameObject[] prefabs, GameObject container, List<GameObject> pool)
    {
        _randomIndex = Random.Range(0, prefabs.Length);

        GameObject prefab = Instantiate(prefabs[_randomIndex]);
        prefab.transform.parent = container.transform;
        prefab.SetActive(false);

        return prefab;
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
            ActivatePrefab(pool);

            _activatedIndex++;
            Debug.Log("Enemies Activated: " + _activatedIndex);

            yield return _spawnWait;
        }

        while (_destroyedIndex != _activatedIndex)
        {
            ActivatePrefab(pool);

            yield return _spawnWait;
        }
    }

    private void ActivatePrefab(List<GameObject> pool)
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

    private WaitForSeconds AssignWait(float wait)
    {
        return new WaitForSeconds(wait);
    }

    public Vector3 AssignTargetPos()
    {
        return _targetPos.position;
    }
}
