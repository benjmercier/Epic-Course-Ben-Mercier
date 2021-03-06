using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public class AttackRadius : MonoBehaviour
    {
        [SerializeField]
        private GameObject _parentTurret;

        public static event Action<GameObject, GameObject, bool> onAttackRadiusTriggered;

        private void Awake()
        {
            if (_parentTurret == null)
            {
                _parentTurret = this.transform.parent.gameObject;
            }
        }
        
        private void OnAttackRadiusTriggered(GameObject parentTurret, GameObject activeTarget, bool isEntering)
        {
            onAttackRadiusTriggered?.Invoke(parentTurret, activeTarget, isEntering);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                OnAttackRadiusTriggered(_parentTurret, other.gameObject, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                OnAttackRadiusTriggered(_parentTurret, other.gameObject, false);
            }
        }
    }
}


