using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Classes
{
    public abstract class BaseTurret : MonoBehaviour, IEventable
    {
        public enum TurretState
        {
            Idle,
            CoolDown,
            Attacking,
            Destroyed
        }

        public TurretState currentState;

        [SerializeField]
        protected int _turretCost;

        [Header("Attack Settings")]
        [SerializeField]
        protected List<GameObject> _activeTargetList = new List<GameObject>();
        [SerializeField]
        protected GameObject _activeTarget;
        [SerializeField]
        protected GameObject _rotationTarget;
        [SerializeField]
        protected float _attackStrength = 10f;

        protected bool _canFire;
        protected bool _hasFired;

        [Header("Primary Rotation")]
        [SerializeField]
        protected Transform _primaryRotationObj;
        [SerializeField]
        protected float _primaryRotationSpeed = 2.5f;
        [SerializeField]
        protected Vector2 _maxRotationAngle = new Vector2(45f, 45f);
        [SerializeField]
        protected Vector2 _minRotationAngle = new Vector2(-45f, -45f);

        protected Vector3 _targetDirection;
        protected float _primaryMovement;

        protected Quaternion _primaryInitialRotation;
        protected Vector3 _primaryRotateTowards;
        protected Quaternion _primaryLookRotation;

        protected float _xAngleCheck;
        protected float _yAngleCheck;

        protected float _xClamped;
        protected float _yClamped;

        // event to communicate with activeTarget
        public static event Action<GameObject, float> onTurretAttack;

        // event to aim at activeTarget's target
        //public static event Func<GameObject, GameObject> onRequestRotationTarget;

        public static event Action<GameObject> onCheckForRotationTarget;

        protected virtual void Awake()
        {
            if (_primaryRotationObj == null)
            {
                Debug.LogError("BaseTurret::Awake()::_primaryRotation is NULL on " + gameObject.name);
            }
        }

        public virtual void OnEnable()
        {
            _primaryInitialRotation = _primaryRotationObj.rotation;

            currentState = TurretState.Idle;

            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
            RotationTarget.onConfirmRotationTarget += AssignRotationTarget;
            Enemy.onEnemyDeath += AssignNewTarget;
        }

        public virtual void OnDisable()
        {
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
            RotationTarget.onConfirmRotationTarget -= AssignRotationTarget;
            Enemy.onEnemyDeath -= AssignNewTarget;
        }

        protected virtual void Update()
        {
            ControlTurretState();
        }

        protected abstract void ControlTurretState();

        protected virtual void UpdateTargetList(GameObject turret, GameObject target, bool addToList) // called each time AttackRadius entered
        {
            if (this.gameObject == turret)
            {
                if (addToList)
                {
                    _activeTargetList.Add(target);

                    if (_activeTarget == null)
                    {
                        _activeTarget = ReturnActiveTarget();
                    }                    
                }
                else
                {
                    if (_activeTargetList.Contains(target))
                    {
                        _activeTargetList.Remove(target);

                        if (_activeTarget == target)
                        {
                            _activeTarget = ReturnActiveTarget();
                        }
                    }
                }
            }
        }

        protected abstract void AssignNewTarget(GameObject activeTarget, int reward);

        protected abstract GameObject ReturnActiveTarget();

        protected virtual void OnCheckForRotationTarget(GameObject activeTarget)
        {
            onCheckForRotationTarget?.Invoke(activeTarget);
        }

        protected virtual void AssignRotationTarget(GameObject activeTarget, GameObject rotationTarget)
        {
            if (_activeTarget == activeTarget)
            {
                _rotationTarget = rotationTarget;
            }
        }

        protected abstract void RotateToTarget(Vector3 target);

        protected abstract void RotateToStart();

        protected float ReturnLocalEulerAngleCheck(float localEulerAngle)
        {
            return localEulerAngle <= 180 ? localEulerAngle : -(360 - localEulerAngle);
        }

        protected virtual void ActivateTurret(bool activate)
        {
            _canFire = activate;

            if (!activate)
            {
                DisengageTarget();
            }
        }

        protected abstract void EngageTarget();

        protected abstract void DisengageTarget();

        protected abstract void TurretAttack(GameObject activeTarget, float damageAmount);

        protected virtual void OnTurretAttack(GameObject activeTarget, float damageAmount)
        {
            onTurretAttack?.Invoke(activeTarget, damageAmount);
        }
    }
}

