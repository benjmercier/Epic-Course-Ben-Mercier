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
        [SerializeField]
        private bool _isTurretSelected = false;
        [SerializeField]
        private bool _canActivateTurret = false;

        private GameObject _activeDecoy;
        private int _activeDecoyIndex = 0;
        private Vector3 _activeDecoyPos;
        [SerializeField]
        private float _yPosOffset = 0.5f;

        private GameObject _newTurret;

        private Ray _rayOrigin;
        private RaycastHit _rayHit;

        public static event Action<bool, int> onDecoyTurretSelected;

        public void OnEnable()
        {
            TowerPosition.onTowerAvailable += CanActivateTurret;
            TowerPosition.onSetTurretPos += AttemptTurretActivation;
        }

        public void OnDisable()
        {
            TowerPosition.onTowerAvailable -= CanActivateTurret;
            TowerPosition.onSetTurretPos -= AttemptTurretActivation;
        }

        void Update()
        {
            CheckSelectionInput();

            if (_isTurretSelected)
            {
                CastRay();
            }
        }

        private void CheckSelectionInput()
        {
            if (!_isTurretSelected)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    TurretSelected(true, 0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    TurretSelected(true, 1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    TurretSelected(true, 2);
                }
            }
            else
            {
                if (Input.GetMouseButton(1))
                {
                    TurretSelected(false, _activeDecoyIndex);
                }
            }
        }

        
        private void TurretSelected(bool isSelected, int decoyIndex)
        {
            _isTurretSelected = isSelected;
            _activeDecoyIndex = decoyIndex;

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

        private void CastRay()
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
        }

        private void AttemptTurretActivation()
        {
            _newTurret = PoolManager.Instance.ReturnPrefabFromPool(false, 1, _activeDecoyIndex);// ReturnTurretFromPool(false, _activeDecoyIndex);
            _newTurret.transform.position = _activeDecoyPos;
            TurretSelected(false, _activeDecoyIndex);
            _newTurret.SetActive(true);
        }
    }
}

