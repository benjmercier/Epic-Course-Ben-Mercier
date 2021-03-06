using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public class AttackRadius : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                /*
                _attackList.Add(other.gameObject);

                if (_attackList.Count <= 1)
                {
                    _activeTarget = _attackList.FirstOrDefault();
                }*/
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                //_attackList.Remove(other.gameObject);
            }
        }
    }
}


