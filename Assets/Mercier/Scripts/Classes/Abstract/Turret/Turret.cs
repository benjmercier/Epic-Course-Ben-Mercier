using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Turret.TurretStates;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class Turret : MonoBehaviour, IEventable
    {
        private TurretBaseState _currentTurretState;
        public TurretBaseState CurrentTurretStat { get { return _currentTurretState; } }
        public readonly TurretIdleState turretIdleState = new TurretIdleState();
        public readonly TurretCoolDownState turretCoolDownState = new TurretCoolDownState();
        public readonly TurretAttackingState turretAttackingState = new TurretAttackingState();
        public readonly TurretDestroyedState turretDestroyedState = new TurretDestroyedState();

        [SerializeField]
        public int _turretCost;

        [Header("Attack Settings")]
        [SerializeField]
        protected List<GameObject> _activeTargetList = new List<GameObject>();
        [SerializeField]
        protected GameObject _activeTarget;
        public GameObject ActiveTarget { get { return _activeTarget; } }
        [SerializeField]
        protected GameObject _rotationTarget;
        public GameObject RotationTarget { get { return _rotationTarget; } }
        [SerializeField]
        protected float _attackStrength = 10f;
        public float AttackStrength { get { return _attackStrength; } }

        protected bool _canFire;
        protected bool _hasFired;

        [Header("Primary Rotation")]
        [SerializeField]
        protected Transform _primaryRotationObj;
        [SerializeField]
        protected float _primaryRotationSpeed = 2.5f;
        [SerializeField]
        protected Vector2 _maxRotationAngle = new Vector2(65f, 65f);
        [SerializeField]
        protected Vector2 _minRotationAngle = new Vector2(-65f, -65f);

        protected Vector3 _primaryTargetDirection;
        protected float _primaryMovement;

        protected Quaternion _primaryInitialRotation;
        public Quaternion PrimaryInitialRotation { get { return _primaryInitialRotation; } }
        protected Vector3 _primaryRotateTowards;
        protected Quaternion _primaryLookRotation;

        protected float _xAngleCheck;
        protected float _yAngleCheck;

        protected float _xClamped;
        protected float _yClamped;

        [Range(0, 2)]
        [SerializeField]
        protected float _minAngleToStartRotation = 1f;
        protected float _primaryAngleToStartRotation;
        

        // event to communicate with activeTarget
        public static event Action<GameObject, float> onTurretAttack;

        // event to check rotationTarget
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

            TransitionToState(turretIdleState);

            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
            Classes.RotationTarget.onConfirmRotationTarget += AssignRotationTarget;
            Enemy.onEnemyDeath += AssignNewTarget;
        }

        public virtual void OnDisable()
        {
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
            Classes.RotationTarget.onConfirmRotationTarget -= AssignRotationTarget;
            Enemy.onEnemyDeath -= AssignNewTarget;
        }

        protected virtual void Update()
        {
            _currentTurretState.Update(this);

            Debug.Log("Current State: " + _currentTurretState.ToString());
        }

        protected virtual void LateUpdate()
        {
            _currentTurretState.LateUpdate(this);
        }

        public void TransitionToState(TurretBaseState state)
        {
            _currentTurretState = state;
            _currentTurretState.EnterState(this);
        }

        // registered to AttackRadius OnTriggerEnter/Exit
        protected virtual void UpdateTargetList(GameObject turret, GameObject target, bool addToList) 
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

        public abstract void RotateToTarget(Vector3 target);

        public abstract void RotateToStart();

        public abstract bool IsAtStart();

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

        public abstract void TurretAttack();

        protected virtual void OnTurretAttack()
        {
            onTurretAttack?.Invoke(_activeTarget, _attackStrength);
        }
    }
}

