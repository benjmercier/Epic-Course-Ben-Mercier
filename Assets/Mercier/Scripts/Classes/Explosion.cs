using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    public class Explosion : MonoBehaviour
    {
        private void Start()
        {
            Destroy(this.gameObject, 3f);
        }
    }
}

