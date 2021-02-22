using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave.asset", menuName = "Scriptable Objects/New Wave")]
public class Wave : ScriptableObject
{
    public bool useBlockWave;
    public List<BlockWave> blockWaveSequence = new List<BlockWave>();

    public bool useCustomWave;
    public List<GameObject> customWaveSequence = new List<GameObject>();

    public bool useRandomWave;
    public int randomWaveTotal;

    public float spawnWaitTime;

    public Dictionary<int, bool> waveBoolDictionary = new Dictionary<int, bool>();

    public Dictionary<int, bool> GenerateBoolDictionary()
    {
        waveBoolDictionary.Clear();

        waveBoolDictionary.Add(0, useBlockWave);
        waveBoolDictionary.Add(1, useCustomWave);
        waveBoolDictionary.Add(2, useRandomWave);

        return waveBoolDictionary;
    }
}
