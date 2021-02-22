using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockWave
{
    public string objName;
    public GameObject objPrefab;
    public int prefabKey;
    public int spawnAmount;

    public BlockWave(string name, GameObject obj, int iD, int amount)
    {
        this.objName = name;
        this.objPrefab = obj;
        this.prefabKey = iD;
        this.spawnAmount = amount;
    }
}
