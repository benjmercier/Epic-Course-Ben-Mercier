using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    [System.Serializable]
    public class TurretInventory
    {
        public string name;
        public int iD;
        public GameObject prefab;
        public int cost;

        public TurretInventory(string name, int iD, GameObject prefab, int cost)
        {
            this.name = name;
            this.iD = iD;
            this.prefab = prefab;
            this.cost = cost;
        }
    }
}

