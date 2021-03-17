using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Custom
{
    [System.Serializable]
    public class Rotation
    {
        public Transform objTransform;
        public float speed = 2.5f;
        public Vector2 maxAngle = new Vector2(90f, 90f);
        public Vector2 minAngle = new Vector2(-90f, -90f);
        [Range(0f, 2f)]
        public float maxIdleOffset = 1f;
        [Range(0f, 1f)]
        public float maxLookOffset = 0.95f;

        [HideInInspector]
        public float angleToIdle;

        [HideInInspector]
        public Vector3 targetDirection;
        [HideInInspector]
        public float movement;

        [HideInInspector]
        public Quaternion idleRotation;
        //public Quaternion InitialRotation { get { return initialRotation; } }
        [HideInInspector]
        public Vector3 rotateTowards;
        [HideInInspector]
        public Quaternion lookRotation;

        [HideInInspector]
        public float xAxisCheck, yAxisCheck, xAxisClamp, yAxisClamp;

        public void CalculateMovement()
        {
            movement = speed * Time.deltaTime;
        }

        public void CalculateDirection(Vector3 targetPos)
        {
            targetDirection = targetPos - objTransform.position;
        }

        public void CalculateLookRotation()
        {
            lookRotation = Quaternion.LookRotation(targetDirection);
        }
    }
}

