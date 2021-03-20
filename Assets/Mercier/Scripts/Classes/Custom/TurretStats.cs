using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom
{
    [System.Serializable]
    public class TurretStats
    {
        public int iD;

        public float maxHealth = 100f;
        //[ReadOnly]
        public float currentHealth;

        public float maxArmor = 100f;
        [ReadOnly]
        public float currentArmor;

        public float attackStrength;
        public float fireRate = 0.5f;

        public int cost;
        public int UpgradeCost { get { return CalculateUpgradeCost(); } }
        public int SellAmount { get { return CalculateSellAmount(); } }
        public int destroyedPenalty;

        public Sprite currentSprite;
        public Sprite upgradeSprite;

        private int CalculateUpgradeCost()
        {
            return Mathf.RoundToInt((cost - SellAmount) + cost);
        }

        private int CalculateSellAmount()
        {
            return Mathf.RoundToInt((currentHealth / maxHealth) * cost);
        }
    }
}

