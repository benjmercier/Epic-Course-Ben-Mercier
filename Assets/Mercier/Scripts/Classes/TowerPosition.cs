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
        private bool _detectTowerStatus = false;
        [SerializeField]
        private bool _canActivateTurret = false;

        public static event Action<bool> onTowerAvailable;
        public static event Action onActivateTurret;

        public void OnEnable()
        {
            TowerManager.onTurretSelection += CheckTowerAvailability;
        }

        public void OnDisable()
        {
            TowerManager.onTurretSelection -= CheckTowerAvailability;
        }

        private void Start()
        {
            if (_towerRenderer == null)
            {
                Debug.Log("TowerPosition::Start()::" + gameObject.name + " _towerRenderer is NULL");
            }

            _canActivateTurret = true;
        }

        private void CheckTowerAvailability(bool isSelected, int decoyIndex)
        {
            _detectTowerStatus = isSelected;

            if (_detectTowerStatus && _canActivateTurret)
            {
                // can set event to change color and activate particles
            }
        }

        private void OnTowerAvailable(bool isAvailable)
        {
            if (onTowerAvailable != null)
            {
                onTowerAvailable(isAvailable);
            }
        }

        private void OnActivateTurret()
        {
            if (onActivateTurret != null)
            {
                onActivateTurret();
            }
        }

        private void OnMouseEnter()
        {
            if (_detectTowerStatus && _canActivateTurret)
            {
                OnTowerAvailable(true);
            }
        }

        private void OnMouseDown()
        {
            if (_detectTowerStatus && _canActivateTurret)
            {
                OnActivateTurret();
            }
        }

        private void OnMouseExit()
        {
            OnTowerAvailable(false);
        }
    }
}

