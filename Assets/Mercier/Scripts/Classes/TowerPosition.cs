using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public class TowerPosition : MonoBehaviour
    {
        [SerializeField]
        private GameObject _turretPos;

        [SerializeField]
        private bool _isActive;

        private void Start()
        {
            if (_turretPos == null)
            {
                Debug.Log("TowerPosition::Start()::" + gameObject.name + " _turretPos is NULL");
            }
        }

        public bool ReturnActiveState()
        {
            return _isActive;
        }
    }
}

