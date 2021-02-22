using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave.asset", menuName = "Scriptable Objects/New Wave")]
public class Wave : ScriptableObject
{
    public bool useBlockSequence;
    public List<BlockWave> blockWaveList = new List<BlockWave>();

    public bool useCustomSequence;
    public List<GameObject> customWaveList = new List<GameObject>();

    public bool useRandomSequence;
    public int randomWaveTotal;

    public float spawnWaitTime;
}
