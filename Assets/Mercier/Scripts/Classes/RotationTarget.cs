using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes.Abstract;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Classes
{
    public class RotationTarget : MonoBehaviour, IEventable
    {
        [SerializeField]
        private GameObject _targetParent;

        public static event Action<GameObject, GameObject> onConfirmRotationTarget;

        public void OnEnable()
        {
            if (_targetParent == null)
            {
                Debug.LogError("AttackTarget::OnEnable()::targetParent is null on " + gameObject.transform.parent.name);
            }

            Turret.onCheckForRotationTarget += VerifyRotationTarget;
            BaseTurret.onCheckForRotationTarget += VerifyRotationTarget;            
        }

        public void OnDisable()
        {
            Turret.onCheckForRotationTarget -= VerifyRotationTarget;
            BaseTurret.onCheckForRotationTarget -= VerifyRotationTarget;
        }

        private void VerifyRotationTarget(GameObject parent)
        {
            if (_targetParent == parent)
            {
                OnConfirmRotationTarget(parent, this.gameObject);
            }
        }

        private void OnConfirmRotationTarget(GameObject parent, GameObject rotationTarget)
        {
            onConfirmRotationTarget?.Invoke(parent, rotationTarget);
        }
    }
}

