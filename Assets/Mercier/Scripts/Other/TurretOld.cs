using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Classes
{
    public abstract class TurretOld : MonoBehaviour, IEventable
    {
        public enum TurretState
        { 
            Idle,
            Searching,
            Attacking,
            Destroyed
        }

        public TurretState currentState;
        public int turretCost;

        [Header("Rotation Settings")]
        [SerializeField]
        protected Transform _baseRotationObj;
        [SerializeField]
        protected Transform _auxRotationObj;
        [SerializeField]
        protected float _rotationSpeed = 2.5f;
        [SerializeField]
        protected float _viewingAngle = 65f;
        [SerializeField]
        protected Vector2 _maxRotationAngle = new Vector2(45f, 45f);
        [SerializeField]
        protected Vector2 _minRotationAngle = new Vector2(-45f, -45f);

        private Vector3 _targetSighting;
        private float _cosAngle;
        private float _targetAngle;

        protected bool _isAuxRotationActive = false;

        protected Vector3 _targetDirection;
        protected float _movement;

        protected Quaternion _baseInitialRotation;
        protected Vector3 _baseRotateTowards;
        protected Quaternion _baseLookRotation;

        protected Quaternion _auxInitialRotation;
        protected Vector3 _auxRotateTowards;
        protected Quaternion _auxLookRotation;

        protected float _xRotAngleCheck;
        protected float _yRotAngleCheck;

        protected float _xClamped;
        protected float _yClamped;
        
        protected float _yAngleOffset = 180f;

        [Header("Attack Settings")]
        [SerializeField]
        protected List<GameObject> _attackTargetList = new List<GameObject>();
        [SerializeField]
        protected GameObject _attackTarget;
        [SerializeField]
        protected List<GameObject> _rotationTargetList = new List<GameObject>();
        [SerializeField]
        protected GameObject _rotationTarget;
        [SerializeField]
        protected float _attackStrength = 5f;
        [SerializeField]
        protected float _fireRate = 0.5f;
        protected float _lastFire;
        protected bool _hasFired;
        protected bool _canFire;

        // set event system to communicate with active target
        public static event Action<GameObject, float> onTurretAttack;

        // event to aim at enemy's target
        public static event Func<GameObject, GameObject> onRequestRotationTarget;

        protected virtual void Awake()
        {
            if (_baseRotationObj == null)
            {
                Debug.LogError("Turret::Start()::" + gameObject.name + "'s _rotationObj is NULL.");
            }

            if (_auxRotationObj != null)
            {
                _isAuxRotationActive = true;
            }
            else
            {
                _isAuxRotationActive = false;
            }
        }

        public virtual void OnEnable()
        {
            _baseInitialRotation = _baseRotationObj.rotation;

            if (_isAuxRotationActive)
            {
                _auxInitialRotation = _auxRotationObj.rotation;
            }

            currentState = TurretState.Idle;

            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
            Enemy.onEnemyDeath += AssignNewTarget;
        }

        public virtual void OnDisable()
        {
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
            Enemy.onEnemyDeath -= AssignNewTarget;
        }

        protected virtual void Update()
        {
            ControlTurretState();
        } 
        
        protected virtual void ControlTurretState()
        {
            switch(currentState)
            {
                case TurretState.Idle:

                    if (_attackTarget == null)
                    {
                        RotateToStart();
                    }
                    else
                    {
                        currentState = TurretState.Searching;
                    }

                    break;

                case TurretState.Searching:

                    if (!ReturnWithinLineOfSight(_attackTarget))
                    {
                        RotateToStart();
                    }
                    else if (_attackTarget == null)
                    {
                        currentState = TurretState.Idle;
                    }
                    else
                    {
                        currentState = TurretState.Attacking;
                    }

                    break;

                case TurretState.Attacking:

                    if (_attackTarget != null)
                    {
                        if (ReturnWithinLineOfSight(_attackTarget))
                        {
                            _hasFired = true;
                            ActivateTurret(true);

                            if (_rotationTarget != null)
                            {
                                RotateToTarget(_rotationTarget.transform.position);
                            }
                            
                            TurretAttack(_attackTarget, _attackStrength);
                        }
                        else
                        {
                            if (_hasFired)
                            {
                                _hasFired = false;
                                ActivateTurret(false);
                                AssignNewTarget(_attackTarget, 0);
                            }

                            RotateToStart();
                        }
                    }
                    else
                    {
                        currentState = TurretState.Idle;
                    }                    

                    break;

                case TurretState.Destroyed:
                    break;
            }
        }

        private bool ReturnWithinLineOfSight(GameObject target) // may not need for missile
        {
            _targetSighting = target.transform.position - _baseRotationObj.position;

            _cosAngle = Vector3.Dot(_targetSighting.normalized, this.transform.forward);

            _targetAngle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;
            
            return _targetAngle <= _maxRotationAngle.y; //_viewingAngle;
        }

        protected abstract void RotateToTarget(Vector3 target); // late update may work better

        protected abstract void RotateToStart();

        protected float ReturnRotationAngleCheck(float localEulerAngle)
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

        private void AssignNewTarget(GameObject currentAttackTarget, int reward)
        {
            if (_attackTarget == currentAttackTarget)
            {
                if (!ReturnWithinLineOfSight(currentAttackTarget))
                {
                    _attackTargetList.Remove(currentAttackTarget);

                    _attackTarget = null;
                    _rotationTarget = null;

                    if (_attackTargetList.Any())
                    {
                        _attackTarget = _attackTargetList.Where(t => ReturnWithinLineOfSight(t)).OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();

                        if (_attackTarget != null)
                        {
                            _rotationTarget = OnRequestRotationTarget(_attackTarget);
                        }
                    }
                }

                ActivateTurret(false);
            }
        }

        protected abstract void TurretAttack(GameObject activeRotationTarget, float damageAmount);
        
        protected virtual void OnTurretAttack(GameObject activeTarget, float damageAmount)
        {
            onTurretAttack?.Invoke(activeTarget, damageAmount);
        }

        protected virtual GameObject OnRequestRotationTarget(GameObject activeTarget)
        {
            return onRequestRotationTarget?.Invoke(activeTarget);
        }
        
        protected virtual void UpdateTargetList(GameObject turret, GameObject target, bool addTo)
        {
            if (this.gameObject == turret)
            {
                if (addTo)
                {
                    _attackTargetList.Add(target);

                    foreach (var obj in _attackTargetList)
                    {
                        if (ReturnWithinLineOfSight(obj))
                        {
                            _attackTarget = obj;

                            _rotationTarget = OnRequestRotationTarget(_attackTarget);
                            //Debug.Log("rotationTarget: " + _rotationTarget.name);

                            return;
                        }
                        else
                        {
                            _attackTarget = null;
                        }
                    }
                }
                else
                {
                    if (_attackTargetList.Contains(target))
                    {
                        _attackTargetList.Remove(target);
                    }
                }
            }
        }
    }
}

