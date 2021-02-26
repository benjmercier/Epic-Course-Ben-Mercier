using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public class TowerPosition : MonoBehaviour
    {
        [SerializeField]
        private GameObject _towerPos;

        [SerializeField]
        private bool _isActive;

        public bool ReturnActiveState()
        {
            return _isActive;
        }
    }
}

