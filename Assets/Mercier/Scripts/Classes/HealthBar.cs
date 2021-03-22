using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Enemy;

namespace Mercier.Scripts.Classes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetTransform;

        [SerializeField]
        private BaseEnemy _parentEnemy;

        [SerializeField]
        private Renderer _objRenderer;

        private void Start()
        {
            if (_objRenderer == null)
            {
                Debug.LogError("HealthBar::Start()::_objRenderer is NULL on " + gameObject.name);
            }
        }

        private void OnEnable()
        {
            _objRenderer.material.SetFloat("_Health", _parentEnemy.EnemyStats.maxHealth);

            _targetTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(_targetTransform);

            UpdateHealth();
        }

        private void UpdateHealth()
        {
            _objRenderer.material.SetFloat("_Health", _parentEnemy.EnemyStats.currentHealth);
        }
    }
}

