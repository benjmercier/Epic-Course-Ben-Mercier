using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom
{
    [System.Serializable]
    public class EnemyStats
    {
        public int iD;

        public float maxHealth = 100f;
        [ReadOnly]
        public float currentHealth;

        public float maxArmor = 100f;
        [ReadOnly]
        public float currentArmor;

        public float speed;
        public float attackStrength;

        public int reward;
    }
}

