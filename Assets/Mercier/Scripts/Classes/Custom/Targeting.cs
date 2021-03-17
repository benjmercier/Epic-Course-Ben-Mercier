using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom
{
    [System.Serializable]
    public class Targeting
    {
        [ReadOnly]
        public List<GameObject> targetList = new List<GameObject>();
        [ReadOnly]
        public GameObject activeTarget, rotationTarget;

        [HideInInspector]
        public Vector3 targetVector;
        [HideInInspector]
        public float dotAngle, targetAngle;
    }
}


