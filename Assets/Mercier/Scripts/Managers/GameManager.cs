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
        [Range(0f, 5f)]
        [SerializeField]
        float timescale = 2.5f;

        #region Tags
        [Header("Game Tags")]
        [SerializeField]
        private string _enemyTag = "Enemy";
        public string EnemyTag { get { return _enemyTag; } }

        [SerializeField]
        private string _turretTag = "Turret";
        public string TurretTag { get { return _turretTag; } }   

        [SerializeField]
        private string _rotationTargetTag = "RotationTarget";
        public string RotationTargetTag { get { return _rotationTargetTag; } }
        #endregion

        private void Update()
        {
            Time.timeScale = timescale;
        }

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
