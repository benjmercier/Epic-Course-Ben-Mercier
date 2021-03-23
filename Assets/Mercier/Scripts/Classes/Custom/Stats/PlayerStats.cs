using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom.Stats
{
    [System.Serializable]
    public class PlayerStats
    {
        public int maxLives = 10;
        [ReadOnly]
        public int currentLives;

        public float maxHealth = 100f;
        [ReadOnly]
        public float currentHealth;

        public enum PlayerStatus { Good, Fair, Danger, Destroyed }
        [ReadOnly]
        public PlayerStatus currentStatus;

        public float minGoodStatus = 60,
            minFairStatus = 20,
            minDangerStatus = 0;

        public int startingWarFunds = 1500,
            currentWarFunds;

        public void SetCurrentValues()
        {
            currentLives = maxLives;
            currentHealth = maxHealth;
            currentStatus = PlayerStatus.Good;
            currentWarFunds = startingWarFunds;
        }

        private bool CheckHealthBetween(float max, float min)
        {
            return currentHealth < max && currentHealth >= min;
        }

        public void UpdateStatus()
        {
            if (CheckHealthBetween(maxHealth, minGoodStatus))
            {
                currentStatus = PlayerStatus.Good;
            }
            else if (CheckHealthBetween(minGoodStatus, minFairStatus))
            {
                currentStatus = PlayerStatus.Fair;
            }
            else if (CheckHealthBetween(minFairStatus, minDangerStatus))
            {
                currentStatus = PlayerStatus.Danger;
            }
            else
            {
                currentStatus = PlayerStatus.Destroyed;
            }
        }
    }
}

