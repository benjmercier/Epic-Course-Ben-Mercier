using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes
{
    public class AttackRadius : MonoBehaviour
    {
        [SerializeField]
        private GameObject _parentObj;

        public static event Action<GameObject, GameObject, bool> onAttackRadiusTriggered;

        private void Awake()
        {
            if (_parentObj == null)
            {
                _parentObj = this.transform.parent.gameObject;
            }
        }
        
        private void OnAttackRadiusTriggered(GameObject parentObj, GameObject activeTarget, bool isEntering)
        {
            onAttackRadiusTriggered?.Invoke(parentObj, activeTarget, isEntering);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_parentObj.CompareTag(GameManager.Instance.EnemyTag))
            {
                if (other.CompareTag(GameManager.Instance.TurretTag))
                {
                    OnAttackRadiusTriggered(_parentObj, other.gameObject, true);
                }
            }
            else if (_parentObj.CompareTag(GameManager.Instance.TurretTag))
            {
                if (other.CompareTag(GameManager.Instance.EnemyTag))
                {
                    OnAttackRadiusTriggered(_parentObj, other.gameObject, true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_parentObj.CompareTag(GameManager.Instance.EnemyTag))
            {
                if (other.CompareTag(GameManager.Instance.TurretTag))
                {
                    OnAttackRadiusTriggered(_parentObj, other.gameObject, false);
                }
            }
            else if (_parentObj.CompareTag(GameManager.Instance.TurretTag))
            {
                if (other.CompareTag(GameManager.Instance.EnemyTag))
                {
                    OnAttackRadiusTriggered(_parentObj, other.gameObject, false);
                }
            }
        }
    }
}


