using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes.Abstract.Enemy;
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

            BaseTurret.onCheckForRotationTarget += VerifyRotationTarget;
            BaseTurret.onTurretAttack += NotifyParentOfDamage;
        }

        public void OnDisable()
        {
            BaseTurret.onCheckForRotationTarget -= VerifyRotationTarget;
            BaseTurret.onTurretAttack -= NotifyParentOfDamage;
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

        protected void NotifyParentOfDamage(GameObject damagedObj, float damageAmount)
        {
            if (this.gameObject == damagedObj)
            {
                //OnDamageReceived(_enemyStats.currentHealth, _enemyStats.currentArmor, damageAmount, out _enemyStats.currentHealth, out _enemyStats.currentArmor);

                if (_targetParent.TryGetComponent(out BaseEnemy parent))
                {
                    parent.ReceiveDamage(_targetParent, damageAmount);
                }
            }
        }
    }
}

