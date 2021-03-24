﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.PropertyAttributes;
using Mercier.Scripts.Classes.Abstract.Enemy;
using Mercier.Scripts.Classes.Custom.Stats;

namespace Mercier.Scripts.Managers
{
    public class GameManager : MonoSingleton<GameManager>, IEventable
    {
        public enum GameState { Idle, Playing, Paused, FastForward, Over }
        [ReadOnly]
        private GameState currentGameState;

        private bool _isGameRunning = false;
        private bool _isIdle = false;

        [SerializeField]
        private PlayerStats _playerStats;
        [SerializeField]
        private int _currentWave;

        private int _currentSceneIndex;

        #region Time Settings
        [Header("Time Settings")]
        [SerializeField]
        private float _startCountdown = 10f;
        [ReadOnly, SerializeField]
        private float _currentCountdown;
        private float _countdownMinutes, _countdownSeconds;
        [SerializeField]
        private float _maxTimeScale = 5f;
        [ReadOnly, SerializeField]
        private float _timeScale = 1f;
        #endregion

        #region Game Tags // change to struct?
        [Header("Game Tags")]
        [SerializeField]
        private string _enemyTag = "Enemy";
        public string EnemyTag { get { return _enemyTag; } }

        [SerializeField]
        private string _turretTag = "Turret";
        public string TurretTag { get { return _turretTag; } } 
        #endregion

        public static event Action<int, int> onUpdatePlayerStatus;
        public static event Action<int> onUpdateWarFunds;
        public static event Action<float, float> onUpdateCountdownTimer;
        public static event Action onGameOver;

        private void Start()
        {
            _playerStats.SetCurrentValues();
            OnUpdatePlayerStatus((int)_playerStats.currentStatus, _playerStats.currentLives);

            OnUpdateWarFunds();

            TransitionToState(GameState.Idle);
        }

        public void OnEnable()
        {
            TowerManager.onTurretEnabled += DecreaseWarFunds;
            TowerManager.onTurretSold += IncreaseWarFunds;
            TargetDestination.onEnemyReachedTarget += UpdatePlayerHealth;
            BaseEnemy.onEnemyDestroyed += EnemyDestroyed;
            WaveManager.onUpdateCurrentWave += UpdateCurrentWave;
            UIManager.onReloadCurrentLevel += ReloadCurrentLevel;
        }

        public void OnDisable()
        {
            TowerManager.onTurretEnabled -= DecreaseWarFunds;
            TowerManager.onTurretSold -= IncreaseWarFunds;
            TargetDestination.onEnemyReachedTarget += UpdatePlayerHealth;
            BaseEnemy.onEnemyDestroyed -= EnemyDestroyed;
            WaveManager.onUpdateCurrentWave -= UpdateCurrentWave;
            UIManager.onReloadCurrentLevel -= ReloadCurrentLevel;
        }

        public void TransitionToState(GameState gameState)
        {
            this.currentGameState = gameState;

            switch(gameState)
            {
                case GameState.Idle:
                    StartCoroutine(IdleStateRoutine());
                    break;

                case GameState.Playing:
                    PlayingState();
                    break;

                case GameState.Paused:
                    PausedState();
                    break;

                case GameState.FastForward:
                    FastForwardState();
                    break;

                case GameState.Over:
                    GameOverState();
                    break;
            }

            Time.timeScale = _timeScale;
        }

        private IEnumerator IdleStateRoutine()
        {
            UIManager.Instance.ToggleInputToStart(true);
            
            while (currentGameState == GameState.Idle)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    UIManager.Instance.ToggleInputToStart(false);
                    UIManager.Instance.ToggleStartTMP(true);

                    TransitionToState(GameState.Playing);

                    StartLevel();
                }

                yield return null;
            }
        }

        private void PlayingState()
        {
            _timeScale = 1f;
        }

        private void PausedState()
        {
            _timeScale = 0f;
        }

        private void FastForwardState()
        {
            if (_timeScale < _maxTimeScale)
            {
                _timeScale++;
            }
        }

        private void GameOverState()
        {
            _isGameRunning = false;

            onGameOver?.Invoke();
        }

        public void StartLevel()
        {
            StartCoroutine(StartRoutine(() =>
            {
                UIManager.Instance.ToggleStartTMP(false);

                if (!_isGameRunning)
                {
                    _isGameRunning = true;

                    WaveManager.Instance.StartWave(WaveManager.Instance.CurrentWave());
                }
                else
                {
                    WaveManager.Instance.StartNextWave();
                }
            }));
        }

        private IEnumerator StartRoutine(Action onComplete = null)
        {
            _currentCountdown = _startCountdown;
            _countdownMinutes = Mathf.Floor(_currentCountdown / 60);
            _countdownSeconds = Mathf.Floor(_currentCountdown % 60);

            OnUpdateCountdownTimer(_countdownMinutes, _countdownSeconds);

            yield return new WaitForSeconds(1f);

            while (_currentCountdown > 0)
            {
                _currentCountdown -= Time.deltaTime;

                if (_currentCountdown > 0)
                {
                    _countdownMinutes = Mathf.Floor(_currentCountdown / 60);
                    _countdownSeconds = Mathf.Floor(_currentCountdown % 60);
                }
                else
                {
                    _countdownMinutes = 0f;
                    _countdownSeconds = 0f;
                }

                OnUpdateCountdownTimer(_countdownMinutes, _countdownSeconds);

                yield return null;
            }

            _isGameRunning = true;

            onComplete?.Invoke();
        }

        private void OnUpdateCountdownTimer(float min, float sec)
        {
            onUpdateCountdownTimer?.Invoke(min, sec);
        }

        public bool EnoughWarFundsAvailable(int cost)
        {
            if (cost > _playerStats.currentWarFunds)
            {
                return false;
            }

            return true;
        }

        public void DecreaseWarFunds(int cost)
        {
            if (_playerStats.currentWarFunds - cost >= 0)
            {
                _playerStats.currentWarFunds -= cost;
            }

            OnUpdateWarFunds();
        }

        private void EnemyDestroyed(GameObject objDestroyed, int currency)
        {
            IncreaseWarFunds(currency);
        }

        public void IncreaseWarFunds(int cost)
        {
            _playerStats.currentWarFunds += cost;

            OnUpdateWarFunds();
        }

        private void OnUpdateWarFunds()
        {
            onUpdateWarFunds?.Invoke(_playerStats.currentWarFunds);
        }

        private void UpdateCurrentWave(int currentWaveIndex)
        {
            _currentWave = currentWaveIndex;
        }

        private void UpdatePlayerHealth(int amount)
        {
            _playerStats.currentHealth -= amount;

            _playerStats.UpdateStatus();

            if (_playerStats.currentStatus == PlayerStats.PlayerStatus.Destroyed)
            {
                if (_playerStats.currentLives > 1)
                {
                    _playerStats.currentLives--;
                    // restart wave
                }
                else
                {
                    _playerStats.currentLives = 0;

                    TransitionToState(GameState.Over);
                    // restart game
                }
            }

            OnUpdatePlayerStatus((int)_playerStats.currentStatus, _playerStats.currentLives);
        }

        private void OnUpdatePlayerStatus(int playerStatus, int currentLives)
        {
            onUpdatePlayerStatus?.Invoke(playerStatus, currentLives);
        }

        private void ReloadCurrentLevel()
        {
            _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(_currentSceneIndex);
        }
    }
}
