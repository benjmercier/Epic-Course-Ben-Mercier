using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mercier.Scripts.Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private List<Button> _armoryButtons = new List<Button>();

        [SerializeField]
        private GameObject _turretToModify;

        private Ray _ray;
        private RaycastHit _rayHit;

        public int DecoyToActivate { set { ActivateDecoyFromTowerManager(value); } }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CastRay();
            }
        }

        private void ActivateDecoyFromTowerManager(int index)
        {
            TowerManager.Instance.OnDecoyTurretSelected(true, index);
        }

        private void CastRay()
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Mouse down.");
            if (Physics.Raycast(_ray, out _rayHit, Mathf.Infinity))
            {
                Debug.Log("Hit Info: " + _rayHit.transform.name);

                if (_rayHit.transform.gameObject.CompareTag(_rayHit.transform.parent.tag))
                {
                    _turretToModify = _rayHit.transform.gameObject;
                }
            }
        }
    }
}

