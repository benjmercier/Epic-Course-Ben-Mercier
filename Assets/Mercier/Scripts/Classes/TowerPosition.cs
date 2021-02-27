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
        private ParticleSystem _towerAvailablePS;

        [SerializeField]
        private bool _isSearchingForPos = false;
        [SerializeField]
        private bool _isPosAvailable = false;

        private GameObject _currentActiveTurret = null;

        public static event Action<bool> onTowerAvailable;
        public static event Action onSetTurretPos;

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
            if (_towerAvailablePS == null)
            {
                Debug.Log("TowerPosition::Start()::" + gameObject.name + " _towerAvailablePS is NULL");
            }

            _isPosAvailable = true;
        }

        private void CheckTowerAvailability(bool isSelected, int decoyIndex)
        {
            _isSearchingForPos = isSelected;

            if (_isSearchingForPos && _isPosAvailable)
            {
                // can activate particles
                _towerAvailablePS.Play();
            }
            else
            {
                _towerAvailablePS.Clear();
            }
        }

        private void OnTowerAvailable(bool isAvailable)
        {
            if (onTowerAvailable != null)
            {
                onTowerAvailable(isAvailable);
            }
        }

        private void OnSetTurretPos()
        {
            if (onSetTurretPos != null)
            {
                onSetTurretPos();
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
                _isPosAvailable = false;

                _towerAvailablePS.Clear();

                _towerRenderer.enabled = true;

                OnSetTurretPos();
            }
        }

        private void OnMouseExit()
        {
            OnTowerAvailable(false);
        }
    }
}

