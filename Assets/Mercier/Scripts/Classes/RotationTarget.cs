using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Classes
{
    public class RotationTarget : MonoBehaviour, IEventable
    {
        [SerializeField]
        private GameObject _targetParent;

        public static event Action<GameObject> onConfirmRotationTarget;

        public void OnEnable()
        {
            if (_targetParent == null)
            {
                Debug.LogError("AttackTarget::OnEnable()::targetParent is null on " + gameObject.transform.parent.name);
            }

            BaseTurret.onCheckForRotationTarget += VerifyRotationTarget;
        }

        public void OnDisable()
        {
            BaseTurret.onCheckForRotationTarget -= VerifyRotationTarget;
        }

        private void VerifyRotationTarget(GameObject parent)
        {
            if (_targetParent == parent)
            {
                OnConfirmRotationTarget(this.gameObject);
            }
        }

        private void OnConfirmRotationTarget(GameObject rotationTarget)
        {
            onConfirmRotationTarget?.Invoke(rotationTarget);
        }
    }
}

