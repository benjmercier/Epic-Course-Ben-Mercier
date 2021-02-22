using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoSingleton<WaveManager>
{
    [SerializeField]
    private List<Wave> _waves = new List<Wave>();

    private int _waveIndex = 0;

    public int CurrentWave()
    {
        return _waveIndex;
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
}
