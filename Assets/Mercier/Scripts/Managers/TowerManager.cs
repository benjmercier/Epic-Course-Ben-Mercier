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

        private int _availableFunds;
        private int _turretCost;

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
                    if (OnCheckingWarFunds(0))
                    {
                        OnDecoyTurretSelected(true, _activeDecoyIndex);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    if (OnCheckingWarFunds(1))
                    {
                        OnDecoyTurretSelected(true, _activeDecoyIndex);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (OnCheckingWarFunds(2))
                    {
                        OnDecoyTurretSelected(true, _activeDecoyIndex);
                    }
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

        private bool OnCheckingWarFunds(int selectedIndex)
        {
            _activeDecoyIndex = selectedIndex;

            _availableFunds = GameManager.Instance._currentWarFunds;
            _turretCost = InventoryManager.Instance._turretInventory.turretInventory[selectedIndex].turretCost;

            return _availableFunds >= _turretCost;
        }


        private void OnDecoyTurretSelected(bool isSelected, int decoyIndex)
        {
            _isDecoySelected = isSelected;

            if (onDecoyTurretSelected != null)
            {
                onDecoyTurretSelected(isSelected, decoyIndex);

                if (isSelected)
                {   // 2 = decoy dictionary
                    _activeDecoy = PoolManager.Instance.ReturnPrefabFromPool(!isSelected, 2, decoyIndex); // set event for PoolManager
                }

                _activeDecoy.SetActive(isSelected);
            }
        }

        private void CastRayToMousePos()
        {
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _rayHit))
            {
                if (_canActivateTurret)
                {
                    _activeDecoyPos = _rayHit.transform.position;

                    _activeDecoy.transform.position = _activeDecoyPos;
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
            OnDecoyTurretSelected(false, _activeDecoyIndex);
            _newTurret.SetActive(true);

            GameManager.Instance.Purchased(_turretCost);
        }
    }
}

