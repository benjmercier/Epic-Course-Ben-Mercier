using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes;
using Mercier.Scripts.Classes.Abstract.Turret;
using Mercier.Scripts.Classes.Custom;

namespace Mercier.Scripts.Managers
{
    public class TowerManager : MonoSingleton<TowerManager>, IEventable
    {
        [SerializeField]
        private List<GameObject> _activeTurretList = new List<GameObject>(); 
        
        private GameObject _activatedDecoy;
        private Vector3 _selectedTowerPos;
        private Quaternion _selectedTowerRot;

        [SerializeField]
        private float _yPosOffset = 0.5f;

        private Ray _rayOrigin;
        private RaycastHit _rayHit;

        [SerializeField]
        private int _turretIndex = 0;
        private GameObject _newTurret;
        private int _newTurretCost;

        [SerializeField]
        private bool _isDecoySelected = false;
        [SerializeField]
        private bool _canActivateTurret = false;

        private Color32 _enablePlacementColor = new Color32(35, 255, 0, 20); // green
        private Color32 _disablePlacementColor = new Color32(255, 35, 0, 20); // red

        public static event Action<bool, int> onDecoyTurretSelected;
        public static event Action<Color32> onTurretPlacementColor;
        public static event Action<BaseTurret> onSelectActiveTurret;
        public static event Action<int> onTurretEnabled;
        public static event Action<int> onTurretSold;
        public static event Action<GameObject> onDisableTurret;


        // turret upgrade/sell
        [SerializeField]
        private bool _enableSelectActiveTurret = false;
        private bool _isUpgrading;
        [SerializeField]
        private GameObject _activeTurretSelected;
        private Transform _upgradeTransform;
        private BaseTurret _turretToModify;

        public void OnEnable()
        {
            TowerPosition.onTowerAvailable += CanActivateTurret;
            TowerPosition.onEnableTurret += EnableTurret;

            UIManager.onDecoyTurretSelectedFromArmory += DecoyTurretSelectedFromArmory;
            UIManager.onUpgradeSelectedTurret += UpgradeSelectedTurret;
            UIManager.onSellSelectedTurret += SellSelectedTurret;
            UIManager.onModifyTurretMenuDisabled += CanSelectActiveTurret;
        }

        public void OnDisable()
        {
            TowerPosition.onTowerAvailable -= CanActivateTurret;
            TowerPosition.onEnableTurret -= EnableTurret;

            UIManager.onDecoyTurretSelectedFromArmory -= DecoyTurretSelectedFromArmory;
            UIManager.onUpgradeSelectedTurret -= UpgradeSelectedTurret;
            UIManager.onSellSelectedTurret -= SellSelectedTurret;
            UIManager.onModifyTurretMenuDisabled -= CanSelectActiveTurret;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && _enableSelectActiveTurret)
            {
                SelectActiveTurret();
            }

            if (_isDecoySelected)
            {
                CastRayToMousePos();

                if (Input.GetMouseButton(1))
                {
                    DeSelectDecoyTurret();
                }
            }
            else
            {
                HotkeySelectDecoyTurret();
            }
        }

        #region Select Turret from Armory
        private void HotkeySelectDecoyTurret()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DecoyTurretSelectedFromArmory(true, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                DecoyTurretSelectedFromArmory(true, 1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                DecoyTurretSelectedFromArmory(true, 2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                DecoyTurretSelectedFromArmory(true, 3);
            }
        }

        private void DeSelectDecoyTurret()
        {
            DecoyTurretSelectedFromArmory(false, _turretIndex);
        }
        
        private void DecoyTurretSelectedFromArmory(bool isSelected, int selectedIndex)
        {
            _turretIndex = selectedIndex;
            _isDecoySelected = isSelected;

            if (!isSelected)
            {
                StartCoroutine(EnableSelectActiveTurretRoutine());
            }
            else
            {
                _enableSelectActiveTurret = false;
            }

            if (onDecoyTurretSelected != null)
            {
                onDecoyTurretSelected(isSelected, selectedIndex);

                if (isSelected)
                {
                    // 2 = decoy dictionary
                    _activatedDecoy = PoolManager.Instance.ReturnPrefabFromPool(!isSelected, 2, selectedIndex); // set event for PoolManager
                }

                _activatedDecoy.SetActive(isSelected);
            }
        }

        private void CastRayToMousePos()
        {
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(_rayOrigin, out _rayHit))
            {
                if (_canActivateTurret)
                {
                    _selectedTowerPos = _rayHit.transform.position;
                    _selectedTowerRot = _rayHit.transform.rotation;

                    _activatedDecoy.transform.position = _selectedTowerPos;
                    _activatedDecoy.transform.rotation = _selectedTowerRot;
                }
                else
                {
                    _selectedTowerPos = _rayHit.point;
                    _selectedTowerPos.y += _yPosOffset;

                    _activatedDecoy.transform.position = _selectedTowerPos;
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
        #endregion

        #region Select Active Turret
        // registered to UIManager ModifyTurret cancel btn
        private void CanSelectActiveTurret(bool canSelect)
        {
            _enableSelectActiveTurret = canSelect;
        }

        private void SelectActiveTurret()
        {
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _rayHit))
            {
                Debug.Log("Ray Hit: " + _rayHit.transform.name);
                if (_activeTurretList.Contains(_rayHit.transform.gameObject))
                {
                    CanSelectActiveTurret(false);

                    _activeTurretSelected = _rayHit.transform.gameObject;

                    if (_activeTurretSelected.TryGetComponent(out BaseTurret baseTurret))
                    {
                        _turretToModify = baseTurret;

                        OnSelectActiveTurret(baseTurret);
                    }
                }
            }
        }

        private void OnSelectActiveTurret(BaseTurret selectedTurret)
        {
            onSelectActiveTurret?.Invoke(selectedTurret);
        }
        #endregion

        #region Upgrade Turret
        // registered to UI upgrade btn
        private void UpgradeSelectedTurret()
        {
            _isUpgrading = true;

            _activeTurretList.Remove(_activeTurretSelected);

            _upgradeTransform = _activeTurretSelected.transform;

            _activeTurretSelected.SetActive(false);

            _turretIndex = _turretToModify.TurretStats.upgradeTo.TurretStats.iD;

            EnableTurret(_upgradeTransform);
        }
        #endregion

        #region Sell Turret
        private void SellSelectedTurret()
        {
            _activeTurretList.Remove(_activeTurretSelected);
            //OnDisableTurret(get tower position);
            _activeTurretSelected.SetActive(false);

            TurretSold(_turretToModify.TurretStats.SellAmount);
        }

        private void TurretSold(int sellAmount)
        {
            onTurretSold?.Invoke(sellAmount);
        }

        private void OnDisableTurret(GameObject turret)
        {
            onDisableTurret?.Invoke(turret);
        }
        #endregion

        private void EnableTurret(Transform objTransform)
        {
            _newTurret = PoolManager.Instance.ReturnPrefabFromPool(false, 1, _turretIndex);
            _newTurret.transform.position = objTransform.position; //_activeDecoyPos;
            _newTurret.transform.rotation = objTransform.rotation; //_activeDecoyRot;

            _activeTurretList.Add(_newTurret);

            if (_activatedDecoy != null)
            {
                DecoyTurretSelectedFromArmory(false, _turretIndex);
            }
            
            _newTurret.SetActive(true);

            if (_isUpgrading)
            {
                _newTurretCost = _turretToModify.TurretStats.UpgradeCost;
            }
            else
            {
                if (_newTurret.TryGetComponent(out BaseTurret baseTurret))
                {
                    _newTurretCost = baseTurret.TurretStats.cost;
                }
            }

            onTurretEnabled?.Invoke(_newTurretCost);

            StartCoroutine(EnableSelectActiveTurretRoutine());
        }

        private IEnumerator EnableSelectActiveTurretRoutine()
        {
            yield return new WaitForSeconds(0.5f);

            _isUpgrading = false;
            _enableSelectActiveTurret = true;
        }
    }
}

