using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    [System.Serializable]
    public class Turret
    {
        public string turretName;
        public int turretID;
        public GameObject turretPrefab;
        public int turretCost;

        public Turret(string name, int iD, GameObject prefab, int cost)
        {
            this.turretName = name;
            this.turretID = iD;
            this.turretPrefab = prefab;
            this.turretCost = cost;
        }
    }
}

