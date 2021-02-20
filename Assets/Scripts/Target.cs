using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
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
