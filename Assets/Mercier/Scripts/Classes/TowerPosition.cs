using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Classes
{
    public class TowerPosition : MonoBehaviour, IEventable
    {
        [SerializeField]
        private Renderer _towerRenderer;
        [SerializeField]
        private ParticleSystem _towerParticleSystem;

        [SerializeField]
        private bool _isSearchingForPos = false;
        [SerializeField]
        private bool _isPosAvailable = false;

        public static event Action<bool> onTowerAvailable;
        public static event Action onEnableTurret;
        

        public void OnEnable()
        {
            TowerManager.onDecoyTurretSelected += CheckTowerAvailability;
            UIManager.onDisableTurret += TurretDisabled;
        }

        public void OnDisable()
        {
            TowerManager.onDecoyTurretSelected -= CheckTowerAvailability;
            UIManager.onDisableTurret -= TurretDisabled;
        }

        private void Start()
        {
            if (_towerRenderer == null)
            {
                Debug.Log("TowerPosition::Start()::" + gameObject.name + " _towerRenderer is NULL");
            }
            if (_towerParticleSystem == null)
            {
                Debug.Log("TowerPosition::Start()::" + gameObject.name + " _towerAvailablePS is NULL");
            }

            _isPosAvailable = true;
        }

        private void CheckTowerAvailability(bool isSelected, int decoyIndex)
        {
            _isSearchingForPos = isSelected;

            if (!_isPosAvailable)
            {
                _towerParticleSystem.gameObject.SetActive(_isPosAvailable);
            }
            else
            {
                _towerParticleSystem.gameObject.SetActive(isSelected);
            }
        }

        private void OnMouseEnter()
        {
            if (_isSearchingForPos && _isPosAvailable)
            {
                OnTowerAvailable(true);
            }
        }

        private void OnMouseDown()
        {
            if (_isSearchingForPos && _isPosAvailable)
            {
                OnEnableTurret();
            }
        }

        private void OnMouseExit()
        {
            OnTowerAvailable(false);
        }

        private void OnTowerAvailable(bool isAvailable)
        {
            onTowerAvailable?.Invoke(isAvailable);
        }

        private void OnEnableTurret()
        {            
            if (onEnableTurret != null)
            {
                onEnableTurret();
                TurretEnabled(true);
            }
        }

        private void TurretEnabled(bool enable)
        {
            _isPosAvailable = !enable;

            _towerRenderer.enabled = enable;
        }

        private void TurretDisabled(GameObject turret)
        {
            if (gameObject == turret)
            {
                TurretEnabled(false);
            }
        }

        /// <summary>
        /// on mouse down if isSearching && isAvailable
        /// broadcast event that trying to place tower == OnTryEnableTurret()
        /// tower manager needs to listen to if trying to place
        /// tell TM trying to place
        /// TM checks with InventoryManager for cost of _activeDecoy
        /// TM checks cost with war funds from GameManager
        /// tower manager needs to return bool to this position 
        /// if war funds >= cost, TM return true
        /// if true, TurretEnabled()
        /// TM activates turret from PoolManager
        /// </summary>
    }
}

