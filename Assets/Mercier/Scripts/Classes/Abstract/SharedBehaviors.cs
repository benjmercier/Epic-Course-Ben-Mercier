using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes.Custom;

namespace Mercier.Scripts.Classes.Abstract
{
    [SelectionBase]
    public abstract class SharedBehaviors : MonoBehaviour, IEventable
    {
        #region Stats
        [Header("Stats")]
        [SerializeField]
        protected Stats _health;
        public float Health { get { return _health.current; } set { _health.current = value; } }
        [SerializeField]
        protected Stats _armor;
        public float Armor { get { return _armor.current; } set { _armor.current = value; } }

        protected float _delta;
        protected float _zero = 0f;
        #endregion

        #region Rotation
        [Header("Rotation Settings")]
        [SerializeField]
        protected Rotation _primaryRotation;
        protected Rotation _auxiliaryRotation;
        #endregion

        #region Targeting
        [SerializeField]
        protected Targeting _targeting;
        #endregion

        #region Events
        public static event Action<GameObject> onCheckForRotationTarget;
        #endregion

        public virtual void OnEnable()
        {
            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
        }

        public virtual void OnDisable()
        {
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
        }

        protected virtual void UpdateTargetList(GameObject obj, GameObject target, bool addToList)
        {
            if (this.gameObject == obj)
            {
                if (addToList)
                {
                    _targeting.targetList.Add(target);
                }
                else
                {
                    _targeting.targetList.Remove(target);
                }

                ManageActiveTarget(target);
            }
        }

        protected virtual void ManageActiveTarget(GameObject target)
        {
            if (_targeting.activeTarget != null && _targeting.activeTarget != target)
            {
                return;
            }

            SetActiveTarget();
        }

        protected virtual void SetActiveTarget()
        {
            if (_targeting.targetList.Any(t => ReturnTargetInLineOfSight(t.transform.position)))
            {
                _targeting.activeTarget = _targeting.targetList.Where(t => ReturnTargetInLineOfSight(t.transform.position)).
                    OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();

                OnCheckForRotationTarget(_targeting.activeTarget);
            }
            else
            {
                _targeting.activeTarget = null;
                _targeting.rotationTarget = null;
            }
        }

        protected virtual bool ReturnTargetInLineOfSight(Vector3 targetPos)
        {
            var targetVector = targetPos - this.transform.position;

            var dotAngle = Vector3.Dot(targetVector.normalized, transform.forward);

            return dotAngle > 0 ? true : false; 
            // change for missile / full rotation = return true
            // also check with edge towers
        }

        protected virtual void OnCheckForRotationTarget(GameObject activeTarget)
        {
            onCheckForRotationTarget?.Invoke(activeTarget);
        }

        protected virtual void AssignRotationTarget(GameObject activeTarget, GameObject rotationTarget)
        {
            if (_targeting.activeTarget == activeTarget)
            {
                _targeting.rotationTarget = rotationTarget;
            }
        }

        public abstract void RotateToTarget(Vector3 target);

        public abstract void RotateToStart();

        public abstract bool IsAtStart();
    }
}

