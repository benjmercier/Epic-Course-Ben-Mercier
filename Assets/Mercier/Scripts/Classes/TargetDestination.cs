using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;
using Mercier.Scripts.Classes.Abstract.Turret;
using Mercier.Scripts.Classes.Custom;

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
            if (other.gameObject.CompareTag("Enemy"))
            {
                SpawnManager.Instance.EnemyDestroyed();


                OnEnemyReachedTarget(30);

                other.gameObject.SetActive(false); // change to event
            }
        }
    }
}

