using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.ScriptableObjects;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Managers
{
    public class GameManager : MonoSingleton<GameManager>, IEventable
    {
        public int _warFundsAvailable;        

        public void OnEnable()
        {
            TowerManager.onTurretEnabled += ItemPurchased;
        }

        public void OnDisable()
        {
            TowerManager.onTurretEnabled -= ItemPurchased;
        }

        public void ItemPurchased(int cost)
        {
            _warFundsAvailable -= cost;
        }
    }
}
