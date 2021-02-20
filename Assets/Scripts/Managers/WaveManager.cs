using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField]
    private int _maxWaves = 5;
    private int _waveIndex = 0;

    public int NextWave()
    {
        _waveIndex++;

        return _waveIndex;
    }
}
