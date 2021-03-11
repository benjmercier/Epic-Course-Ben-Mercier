using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes;

namespace Mercier.Scripts.Managers
{
    public class TowerManager : MonoSingleton<TowerManager>, IEventable
    {
        private GameObject _activeDecoy;
        private int _activeDecoyIndex = 0;
        private Vector3 _activeDecoyPos;
        private Quaternion _activeDecoyRot;
        [SerializeField]
        private float _yPosOffset = 0.5f;        

        private GameObject _newTurret;

        private Ray _rayOrigin;
        private RaycastHit _rayHit;

        public static event Action<bool, int> onDecoyTurretSelected;
        [SerializeField]
        private bool _isDecoySelected = false;
        [SerializeField]
        private bool _canActivateTurret = false;

        public static event Action<Color32> onTurretPlacementColor;
        private Color32 _enablePlacementColor = new Color32(35, 255, 0, 20); // green
        private Color32 _disablePlacementColor = new Color32(255, 35, 0, 20); // red

        public static event Action<int> onTurretEnabled;

        private int _availableFunds;
        private int _turretCost;
        private bool _enoughFunds;

        public void OnEnable()
        {
            TowerPosition.onTowerAvailable += CanActivateTurret;
            TowerPosition.onEnableTurret += EnableTurret;
        }

        public void OnDisable()
        {
            TowerPosition.onTowerAvailable -= CanActivateTurret;
            TowerPosition.onEnableTurret -= EnableTurret;
        }

        void Update()
        {
            CheckSelectionInput();

            if (_isDecoySelected)
            {
                CastRayToMousePos();
            }
        }

        private void CheckSelectionInput()
        {
            if (!_isDecoySelected)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    OnDecoyTurretSelected(true, 0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    OnDecoyTurretSelected(true, 1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    OnDecoyTurretSelected(true, 2);
                }
            }
            else
            {
                if (Input.GetMouseButton(1))
                {
                    OnDecoyTurretSelected(false, _activeDecoyIndex);
                }
            }
        }

        private void OnDecoyTurretSelected(bool isSelected, int selectedIndex)
        {
            if (CalculateFundsAvailable(selectedIndex))
            {
                _activeDecoyIndex = selectedIndex;
                _isDecoySelected = isSelected;

                if (onDecoyTurretSelected != null)
                {
                    onDecoyTurretSelected(isSelected, selectedIndex);

                    if (isSelected)
                    {   // 2 = decoy dictionary
                        _activeDecoy = PoolManager.Instance.ReturnPrefabFromPool(!isSelected, 2, selectedIndex); // set event for PoolManager
                    }

                    _activeDecoy.SetActive(isSelected);
                }
            }
        }

        private bool CalculateFundsAvailable(int selectedIndex)
        {
            _availableFunds = GameManager.Instance._warFundsAvailable;
            //_turretCost = DatabaseManager.Instance.turretDatabase.databaseList[selectedIndex].cost;
            // set cost on decoy turret?

            _enoughFunds = _availableFunds >= _turretCost;

            if (!_enoughFunds)
            {
                Debug.Log("Not enough War Funds. Amount Available: " + _availableFunds);
            }

            return _enoughFunds;
        }

        private void CastRayToMousePos()
        {
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _rayHit))
            {
                if (_canActivateTurret)
                {
                    _activeDecoyPos = _rayHit.transform.position;
                    _activeDecoyRot = _rayHit.transform.rotation;

                    _activeDecoy.transform.position = _activeDecoyPos;
                    _activeDecoy.transform.rotation = _activeDecoyRot;
                }
                else
                {
                    _activeDecoyPos = _rayHit.point;
                    _activeDecoyPos.y += _yPosOffset;

                    _activeDecoy.transform.position = _activeDecoyPos;
                }
            }
        }  
        
        private void CanActivateTurret(bool isActive)
        {
            _canActivateTurret = isActive;

            if (isActive)
            {
                OnTurretPlacementColor(_enablePlacementColor);
            }
            else
            {
                OnTurretPlacementColor(_disablePlacementColor);
            }
        }

        private void OnTurretPlacementColor(Color32 color)
        {
            onTurretPlacementColor?.Invoke(color);
        }

        private void EnableTurret()
        {
            _newTurret = PoolManager.Instance.ReturnPrefabFromPool(false, 1, _activeDecoyIndex);
            _newTurret.transform.position = _activeDecoyPos;
            _newTurret.transform.rotation = _activeDecoyRot;

            OnDecoyTurretSelected(false, _activeDecoyIndex);
            _newTurret.SetActive(true);

            onTurretEnabled?.Invoke(_turretCost);
        }
    }
}

