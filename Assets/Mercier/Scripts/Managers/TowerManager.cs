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
        public List<GameObject> ActiveTurretList { get { return _activeTurretList; } }
        
        private GameObject _activeDecoy;
        private int _activeIndex = 0;
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
        public static event Action<BaseTurret> onSelectActiveTurret;
        private bool _canSelectTurret = false;

        // turret upgrade/sell
        private GameObject _activeTurretSelected;



        private int _availableFunds;
        private int _turretCost;
        private bool _enoughFunds;

        public void OnEnable()
        {
            TowerPosition.onTowerAvailable += CanActivateTurret;
            TowerPosition.onEnableTurret += EnableTurret;

            UIManager.onDecoyTurretSelectedFromArmory += DecoyTurretSelectedFromArmory;
            UIManager.onUpgradeSelectedTurret += UpgradeSelectedTurret;
        }

        public void OnDisable()
        {
            TowerPosition.onTowerAvailable -= CanActivateTurret;
            TowerPosition.onEnableTurret -= EnableTurret;

            UIManager.onDecoyTurretSelectedFromArmory -= DecoyTurretSelectedFromArmory;
            UIManager.onUpgradeSelectedTurret -= UpgradeSelectedTurret;
        }

        void Update()
        {
            // Test
            CheckSelectionInput();
            //

            if (Input.GetMouseButtonDown(0) && _canSelectTurret)
            {
                SelectActiveTurret();
            }

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
            else
            {
                if (Input.GetMouseButton(1))
                {
                    DecoyTurretSelectedFromArmory(false, _activeIndex);
                }
            }
        }

        private void DecoyTurretSelectedFromArmory(bool isSelected, int selectedIndex)
        {
            if (CalculateFundsAvailable(selectedIndex))
            {
                _activeIndex = selectedIndex;
                _isDecoySelected = isSelected;

                if (_canSelectTurret)
                {
                    _canSelectTurret = false;
                }

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

        private void SelectActiveTurret()
        {
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _rayHit))
            {
                Debug.Log("Ray Hit: " + _rayHit.transform.name);
                if (_activeTurretList.Contains(_rayHit.transform.gameObject))
                {
                    _activeTurretSelected = _rayHit.transform.gameObject;

                    if (_activeTurretSelected.TryGetComponent(out BaseTurret baseTurret))
                    {
                        _activeIndex = baseTurret.TurretStats.iD + 1; // add upgrade id to TurretStats

                        OnSelectActiveTurret(baseTurret);
                    }
                }
            }
        }

        private void OnSelectActiveTurret(BaseTurret selectedTurret)
        {
            onSelectActiveTurret?.Invoke(selectedTurret);
        }

        private void UpgradeSelectedTurret()
        {
            _activeTurretList.Remove(_activeTurretSelected);

            var pos = _activeTurretSelected.transform;

            _activeTurretSelected.SetActive(false);

            EnableTurret(pos);
        }


        private bool CalculateFundsAvailable(int selectedIndex)
        {
            _availableFunds = GameManager.Instance.WarFundsAvailable;

            switch (selectedIndex)
            {
                case 0:
                    _turretCost = 250;
                    break;
                case 1:
                    _turretCost = 500;
                    break;
                case 2:
                    _turretCost = 350;
                    break;
                case 3:
                    _turretCost = 700;
                    break;
            }

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
            // set layer for turretPos
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

        private void EnableTurret(Transform objTransform)
        {
            _newTurret = PoolManager.Instance.ReturnPrefabFromPool(false, 1, _activeIndex);
            _newTurret.transform.position = objTransform.position; //_activeDecoyPos;
            _newTurret.transform.rotation = objTransform.rotation; //_activeDecoyRot;

            _activeTurretList.Add(_newTurret);

            if (_activeDecoy != null)
            {
                DecoyTurretSelectedFromArmory(false, _activeIndex);
            }
            
            StartCoroutine(CanSelectTurretRoutine());
            _newTurret.SetActive(true);

            onTurretEnabled?.Invoke(_turretCost);
        }

        private IEnumerator CanSelectTurretRoutine()
        {
            yield return new WaitForSeconds(1.5f);

            _canSelectTurret = true;
        }
    }
}

