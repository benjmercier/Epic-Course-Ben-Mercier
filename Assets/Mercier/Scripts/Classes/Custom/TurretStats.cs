using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mercier.Scripts.PropertyAttributes;
using Mercier.Scripts.Classes.Abstract.Turret;

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
        public bool isUpgradeable = false;
        public BaseTurret upgradeTo;
        public Sprite upgradeSprite;

        [ReadOnly]
        public Renderer attachedTowerRenderer;

        private int CalculateUpgradeCost()
        {
            // upgradeTo.TurretStats.cost = total upgrade cost
            // find
            if (isUpgradeable)
            {
                return Mathf.RoundToInt(-((currentHealth / maxHealth) * cost) + upgradeTo.TurretStats.cost);  //(cost - SellAmount) + cost);
            }

            return 0;
        }

        private int CalculateSellAmount()
        {
            return Mathf.RoundToInt((currentHealth / maxHealth) * cost);
        }
    }
}

