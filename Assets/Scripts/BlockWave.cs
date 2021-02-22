using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockWave
{
    public string objName;
    public GameObject objPrefab;
    public int prefabID;
    public int spawnAmount;

    public BlockWave(string name, GameObject obj, int iD, int amount)
    {
        this.objName = name;
        this.objPrefab = obj;
        this.prefabID = iD;
        this.spawnAmount = amount;
    }
}
