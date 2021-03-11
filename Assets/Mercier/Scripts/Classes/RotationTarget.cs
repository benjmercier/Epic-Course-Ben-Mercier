using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Classes
{
    public class RotationTarget : MonoBehaviour, IEventable
    {
        [SerializeField]
        private GameObject targetParent;

        public void OnEnable()
        {
            if (targetParent == null)
            {
                Debug.LogError("AttackTarget::OnEnable()::targetParent is null on " + gameObject.transform.parent.name);
            }

            //Turret.onRequestRotationTarget += ReturnRotationTarget;
            BaseTurret.onRequestRotationTarget += ReturnRotationTarget;
        }

        public void OnDisable()
        {
            //Turret.onRequestRotationTarget -= ReturnRotationTarget;
            BaseTurret.onRequestRotationTarget -= ReturnRotationTarget;
        }

        private GameObject ReturnRotationTarget(GameObject parent)
        {
            if (targetParent == parent)
            {
                return this.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
}

