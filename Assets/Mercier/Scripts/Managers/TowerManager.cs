using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Managers
{
    public class TowerManager : MonoSingleton<TowerManager>
    {
        [SerializeField]
        private Camera _mainCamera;

        [SerializeField]
        private GameObject _decoyTurretContainer;

        [SerializeField]
        private GameObject[] _decoyTurretPrefabs;

        private GameObject _activeDecoy;
        private Vector3 _currentPos;
        [SerializeField]
        private float _yOffset = 0.5f;

        private int _decoyIndex = 0;
        [SerializeField]
        private bool _canCastRay = false;

        private Ray _rayOrigin;
        private RaycastHit _rayHit;

        // Start is called before the first frame update
        void Start()
        {
            _decoyTurretPrefabs.ToList().ForEach(t => t.SetActive(false));
        }

        // Update is called once per frame
        void Update()
        {
            if (_canCastRay)
            {
                CastRay();
            }
            else
            {
                CheckSelectionInput();
            }
        }

        private void CheckSelectionInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ActivateDecoy(0);
                
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivateDecoy(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivateDecoy(2);
            }
        }

        private void CastRay()
        {
            _rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_rayOrigin, out _rayHit))
            {
                _currentPos = _rayHit.point;
                _currentPos.y += _yOffset;

                _activeDecoy.transform.position = _currentPos;

                if (Input.GetMouseButton(1))
                {
                    DeactivateDecoy();
                }
            }
        }

        private void ActivateDecoy(int index)
        {
            _decoyIndex = index;

            _activeDecoy = PoolManager.Instance.ReturnDecoyTurretFromPool(false, index); // Instantiate(_decoyTurretPrefabs[index], _decoyTurretContainer.transform);
            _activeDecoy.SetActive(true);

            _canCastRay = true;
        }

        private void DeactivateDecoy()
        {
            _activeDecoy.SetActive(false);

            _canCastRay = false;
        }

    }
}

