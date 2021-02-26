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

        private int _decoyIndex = 0;
        private string _inputString;
        [SerializeField]
        private bool _canRaycast = false;

        private Ray _rayOrigin;
        private RaycastHit _hit;

        // Start is called before the first frame update
        void Start()
        {
            _decoyTurretPrefabs.ToList().ForEach(d => d.SetActive(false));
        }

        // Update is called once per frame
        void Update()
        {
            CheckInput();

            if (_canRaycast)
            {
                
            }

            Ray rayOrigin = _mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, out hit))
            {
                _decoyTurretPrefabs[_decoyIndex].transform.position = hit.point;
            }
        }

        private void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _canRaycast = true;
                _decoyIndex = 0;
                _decoyTurretPrefabs[_decoyIndex].SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _canRaycast = true;
                _decoyIndex = 1;
                _decoyTurretPrefabs[_decoyIndex].SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _canRaycast = true;
                _decoyIndex = 2;
                _decoyTurretPrefabs[_decoyIndex].SetActive(true);
            }
        }

    }
}

