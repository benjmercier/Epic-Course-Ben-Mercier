using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Managers
{
    public class SpawnManager : MonoSingleton<SpawnManager>
    {
        [SerializeField]
        private Transform _spawnPos;
        [SerializeField]
        private Transform _targetPos;

        private GameObject _spawnPrefab;
        private Vector3 _spawnLookPos;
        private Quaternion _spawnRotation;

        private int _activatedIndex = 0;
        private int _destroyedIndex = 0;

        private bool _canSpawn = false;

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

                    WaveManager.Instance.StartWave(WaveManager.Instance.CurrentWave());
                }
            }
        }

        public void ActivateEnemyPrefab(bool isRandom, int dictionaryKey)
        {
            _spawnPrefab = PoolManager.Instance.ReturnPrefabFromPool(isRandom, 0, dictionaryKey);

            _spawnPrefab.transform.position = _spawnPos.position;

            _spawnLookPos = _targetPos.position - _spawnPrefab.transform.position;
            _spawnLookPos.y = 0;

            _spawnRotation = Quaternion.LookRotation(_spawnLookPos);

            _spawnPrefab.transform.rotation = _spawnRotation;

            _spawnPrefab.SetActive(true);

            EnemyActivated();
        }

        public Vector3 AssignSpawnPos()
        {
            return _spawnPos.position;
        }

        public Vector3 AssignTargetPos()
        {
            return _targetPos.position;
        }

        public void EnemyActivated()
        {
            _activatedIndex++;
        }

        public int CurrentEnemiesActivated()
        {
            return _activatedIndex;
        }

        public void EnemyDestroyed() // used for testing
        {
            _destroyedIndex++;
        }

        public int CurrentEnemiesDestroyed()
        {
            return _destroyedIndex;
        }
    }
}





