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
        [Range(-1f, 1f), Tooltip("-1f is directly behind and 1f is directly in front.")]
        public float maxDotAngleOffset = 0f;

        [HideInInspector]
        public Vector3 targetVector;
        [HideInInspector]
        public float dotAngle, targetAngle;
    }
}


