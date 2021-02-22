using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField]
    private List<Wave> _waves = new List<Wave>();
    private Wave _currentWave;

    private int _waveIndex = 0;

    private WaitForSeconds _spawnWait;

    private Wave CurrentWave()
    {
        var wave = _waves[_waveIndex];

        return wave;
    }

    public int NextWave()
    {
        _waveIndex++;

        return _waveIndex;
    }

    public List<Wave> WaveList()
    {
        return _waves;
    }

    public void StartWave()
    {
        _currentWave = CurrentWave();

        _currentWave.waveBoolDictionary = _currentWave.GenerateBoolDictionary();

        _spawnWait = AssignWait(_currentWave.spawnWaitTime);
                
        for (int i = 0; i < _currentWave.waveBoolDictionary.Count; i++)
        {
            if(_currentWave.waveBoolDictionary[i])
            {
                switch (i)
                {
                    case 0:
                        StartCoroutine(BlockWaveSequenceRoutine(_currentWave.blockWaveSequence, _spawnWait));
                        break;
                    case 1:
                        StartCoroutine(CustomWaveSequence(_currentWave.customWaveSequence, _spawnWait));
                        break;
                    case 2:
                        StartCoroutine(RandomWaveSequence(_currentWave.randomWaveTotal, _spawnWait));
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
                SpawnManager.Instance.ActivatePrefab(false, obj.prefabKey);

                yield return waitTime;
            }
        }
    }

    private IEnumerator CustomWaveSequence(List<int> sequenceList, WaitForSeconds waitTime)
    {
        for (int i = 0; i < sequenceList.Count; i++)
        {
            SpawnManager.Instance.ActivatePrefab(false, sequenceList[i]);

            yield return waitTime;
        }
    }

    private IEnumerator RandomWaveSequence(int sequenceTotal, WaitForSeconds waitTime)
    {
        for (int i = 0; i < sequenceTotal; i++)
        {
            SpawnManager.Instance.ActivatePrefab(true, 0);

            yield return waitTime;
        }
    }

    private WaitForSeconds AssignWait(float wait) // change to helper?
    {
        return new WaitForSeconds(wait);
    }
}
