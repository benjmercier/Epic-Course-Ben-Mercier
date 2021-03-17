using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom
{
    [System.Serializable]
    public class Stats
    {
        public float max = 100f;
        [ReadOnly]
        public float current;
    }
}

