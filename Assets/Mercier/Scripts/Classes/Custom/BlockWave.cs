using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    [System.Serializable]
    public class BlockWave
    {
        public string prefabName;
        public int prefabKey;
        public int spawnAmount;

        public BlockWave(string name, int iD, int amount)
        {
            this.prefabName = name;
            this.prefabKey = iD;
            this.spawnAmount = amount;
        }
    }
}




