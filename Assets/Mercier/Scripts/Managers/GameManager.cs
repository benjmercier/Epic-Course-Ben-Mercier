using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.ScriptableObjects;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Managers
{
    public class GameManager : MonoSingleton<GameManager>, IEventable
    {
        [SerializeField]
        private int _maxPlayerHealth = 100;
        [ReadOnly, SerializeField]
        private int _currentPlayerHealth;

        public enum PlayerStatus
        {
            Good,
            Fair,
            Danger,
            Destroyed
        }

        [ReadOnly]
        public PlayerStatus currentPlayerStatus;

        [SerializeField]
        private int _minGoodStatus = 60,
            _minFairStatus = 20,
            _minDangerStatus = 0;

        [SerializeField]
        private int _startingWarFunds = 5000;
        [SerializeField]
        private int _warFundsAvailable;
        public int WarFundsAvailable { get { return _warFundsAvailable; } set { _warFundsAvailable = value; } }

        [SerializeField]
        private float _timeToStart = 10f;
        [ReadOnly, SerializeField]
        private float _countdown;
        private bool _gameStarted = false;


        [Range(0f, 5f)]
        [SerializeField]
        private float _timescale = 1f;

        public static event Action<int> onUpdatePlayerStatus;
        public static event Action<int> onUpdateWarFunds;

        // change to struct
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

        // change to struct
        #region Layers
        [Header("Game Layers")]
        [SerializeField]
        private LayerMask _enemyLayer;
        public LayerMask EnemyLayer { get { return _enemyLayer.value; } }

        [SerializeField]
        private LayerMask _turretLayer;
        public LayerMask TurretLayer { get { return _turretLayer.value; } }

        [SerializeField]
        private LayerMask _turretPosLayer;
        public LayerMask TurretPosLayer { get { return _turretPosLayer; } }
        #endregion

        private void Start()
        {
            _currentPlayerHealth = _maxPlayerHealth;
            currentPlayerStatus = PlayerStatus.Good;
            _warFundsAvailable = _startingWarFunds;

            OnUpdatePlayerStatus((int)currentPlayerStatus);
            OnUpdateWarFunds();
        }

        private void Update()
        {
            Time.timeScale = _timescale;

            //OnUpdatePlayerStatus((int)currentPlayerStatus);

            if (Input.GetKeyDown(KeyCode.B))
            {
                ItemPurchased(2000);
            }

            if (Input.GetKeyDown(KeyCode.S) && !_gameStarted)
            {
                StartCoroutine(StartGameRoutine());
            }
        }

        public void OnEnable()
        {
            TowerManager.onTurretEnabled += ItemPurchased;
            TargetDestination.onEnemyReachedTarget += UpdatePlayerHealth;
        }

        public void OnDisable()
        {
            TowerManager.onTurretEnabled -= ItemPurchased;
            TargetDestination.onEnemyReachedTarget += UpdatePlayerHealth;
        }

        private IEnumerator StartGameRoutine(Action onComplete = null)
        {
            _countdown = _timeToStart;

            while (_countdown > 0)
            {
                _countdown -= Time.deltaTime;

                yield return null;
            }

            _gameStarted = true;

            onComplete?.Invoke();
        }

        public void ItemPurchased(int cost)
        {
            if (_warFundsAvailable - cost >= 0)
            {
                _warFundsAvailable -= cost;
            }

            OnUpdateWarFunds();
        }

        private void OnUpdateWarFunds()
        {
            onUpdateWarFunds?.Invoke(_warFundsAvailable);
        }

        private void UpdatePlayerHealth(int amount)
        {
            _currentPlayerHealth -= amount;
            // optimize by not calling OnUpdatePlayerStatus if status is the same as last time
            if (CheckHealthBetween(_maxPlayerHealth, _minGoodStatus))
            {
                currentPlayerStatus = PlayerStatus.Good;
            }
            else if (CheckHealthBetween(_minGoodStatus, _minFairStatus))
            {
                currentPlayerStatus = PlayerStatus.Fair;
            }
            else if (CheckHealthBetween(_minFairStatus, _minDangerStatus))
            {
                currentPlayerStatus = PlayerStatus.Danger;
            }
            else
            {
                currentPlayerStatus = PlayerStatus.Destroyed;
            }

            OnUpdatePlayerStatus((int)currentPlayerStatus);
        }

        private bool CheckHealthBetween(int max, int min)
        {
            return _currentPlayerHealth < max && _currentPlayerHealth > min;
        }

        private void OnUpdatePlayerStatus(int playerStatus)
        {
            onUpdatePlayerStatus?.Invoke(playerStatus);
        }

        
    }
}
