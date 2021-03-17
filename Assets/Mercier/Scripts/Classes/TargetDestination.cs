using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts
{
    public class TargetDestination : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                SpawnManager.Instance.EnemyDestroyed();

                other.gameObject.SetActive(false); // change to event
            }
        }
    }
}

