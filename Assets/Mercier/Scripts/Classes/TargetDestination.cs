using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;
using Mercier.Scripts.Classes.Abstract.Enemy;

namespace Mercier.Scripts
{
    public class TargetDestination : MonoBehaviour
    {
        public static event Action<int> onEnemyReachedTarget;

        private void OnEnemyReachedTarget(int penalty)
        {
            onEnemyReachedTarget?.Invoke(penalty);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(GameManager.Instance.EnemyTag)) // RotationTarget tagged as Enemy
            {
                SpawnManager.Instance.EnemyDestroyed();

                OnEnemyReachedTarget(20);

                other.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}

