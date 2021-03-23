using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mercier.Scripts.ScriptableObjects;
using Mercier.Scripts.Classes;

namespace Mercier.Scripts.Managers
{
    public class WaveManager : MonoSingleton<WaveManager>
    {
        [SerializeField]
        private List<Wave> _waves = new List<Wave>();
        public List<Wave> Waves { get { return _waves; } }
        private Wave _currentWave;

        private int _waveIndex = 0;

        private WaitForSeconds _spawnWait;

        public static event Action<int> onUpdateCurrentWave;

        public void StartWave(Wave wave)
        {
            wave.waveBoolDictionary = wave.GenerateBoolDictionary();

            _spawnWait = UtilityHelper.AssignWaitForSeconds(wave.spawnWaitTime);

            for (int i = 0; i < wave.waveBoolDictionary.Count; i++)
            {
                if (wave.waveBoolDictionary[i])
                {
                    switch (i)
                    {
                        case 0:
                            StartCoroutine(BlockWaveSequenceRoutine(wave.blockWaveSequence, _spawnWait));
                            break;
                        case 1:
                            StartCoroutine(CustomWaveSequenceRoutine(wave.customWaveSequence, _spawnWait));
                            break;
                        case 2:
                            StartCoroutine(RandomWaveSequenceRoutine(wave.randomWaveTotal, _spawnWait));
                            break;
                        default:
                            Debug.LogError("WaveManager::WaveRoutine::Assign single sequence to Wave " + CurrentWave());
                            break;
                    }
                }
            }
        }

        private IEnumerator BlockWaveSequenceRoutine(List<BlockWave> sequenceList, WaitForSeconds waitTime)
        {
            foreach (var obj in sequenceList)
            {
                for (int i = 0; i < obj.spawnAmount; i++)
                {
                    SpawnManager.Instance.ActivateEnemyPrefab(false, obj.prefabKey);

                    yield return waitTime;
                }
            }

            WaveEnd();
        }

        private IEnumerator CustomWaveSequenceRoutine(List<int> sequenceList, WaitForSeconds waitTime)
        {
            for (int i = 0; i < sequenceList.Count; i++)
            {
                SpawnManager.Instance.ActivateEnemyPrefab(false, sequenceList[i]);

                yield return waitTime;
            }

            WaveEnd();
        }

        private IEnumerator RandomWaveSequenceRoutine(int sequenceTotal, WaitForSeconds waitTime)
        {
            for (int i = 0; i < sequenceTotal; i++)
            {
                SpawnManager.Instance.ActivateEnemyPrefab(true, 0);

                yield return waitTime;
            }

            WaveEnd();
        }

        private void WaveEnd()
        {
            if (_waveIndex < _waves.Count - 1)
            {
                StartNextWave();
            }
            else
            {
                Debug.Log("WaveManager::RandomWaveSequenceRoutine()::Waves Complete");
            }
        }

        public void StartNextWave()
        {
            _waveIndex++;

            StartWave(CurrentWave());

            Debug.Log("Starting " + CurrentWave());
        }

        public Wave CurrentWave()
        {
            _currentWave = _waves[_waveIndex];

            OnUpdateCurrentWave(_waveIndex);

            return _currentWave;
        }

        public void OnUpdateCurrentWave(int waveIndex)
        {
            onUpdateCurrentWave?.Invoke(waveIndex);
        }

        public List<Wave> ReturnWaveList()
        {
            var waveList = _waves;

            return waveList;
        }
    }
}




