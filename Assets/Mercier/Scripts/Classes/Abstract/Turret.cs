using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mercier.Scripts.Interfaces;

namespace Mercier.Scripts.Classes
{
    public abstract class Turret : MonoBehaviour, IEventable
    {
        public enum TurretState
        { 
            Idle,
            Searching,
            Attacking,
            Destroyed
        }

        public TurretState currentState;

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
        protected List<GameObject> _activeList = new List<GameObject>();
        [SerializeField]
        protected GameObject _activeTarget;
        [SerializeField]
        protected float _attackStrength = 5f;
        [SerializeField]
        protected float _fireRate = 0.5f;
        protected float _lastFire;
        protected bool _hasFired;
        protected bool _canFire;

        // set event system to communicate with active target
        public static event Action<GameObject, float> onTurretAttack;

        protected virtual void Awake()
        {
            if (_baseRotationObj == null)
            {
                Debug.LogError("Turret::Start()::" + gameObject.name + "'s _rotationObj is NULL.");
            }

            if (_auxRotationObj != null)
            {
                _auxInitialRotation = _auxRotationObj.rotation;
                _isAuxRotationActive = true;
            }
            else
            {
                _isAuxRotationActive = false;
            }

            _baseInitialRotation = _baseRotationObj.rotation;
        }

        public void OnEnable()
        {
            currentState = TurretState.Idle;

            AttackRadius.onAttackRadiusTriggered += UpdateAttackList;
            Enemy.onEnemyDeath += AssignNewTarget;
        }

        public void OnDisable()
        {
            AttackRadius.onAttackRadiusTriggered -= UpdateAttackList;
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

                    if (_activeTarget == null)
                    {
                        RotateToStart();
                    }
                    else
                    {
                        currentState = TurretState.Searching;
                    }

                    break;

                case TurretState.Searching:

                    if (!ReturnWithinLineOfSight())
                    {
                        RotateToStart();
                    }
                    else if (_activeTarget == null)
                    {
                        currentState = TurretState.Idle;
                    }
                    else
                    {
                        currentState = TurretState.Attacking;
                    }

                    break;

                case TurretState.Attacking:

                    if (_activeTarget != null)
                    {
                        if (ReturnWithinLineOfSight())
                        {
                            _hasFired = true;
                            ActivateTurret(true);
                            RotateToTarget(_activeTarget.transform.position);
                            TurretAttack(_activeTarget, _attackStrength);
                        }
                        else
                        {
                            if (_hasFired)
                            {
                                _hasFired = false;
                                ActivateTurret(false);
                                AssignNewTarget(_activeTarget, 0);
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

        private bool ReturnWithinLineOfSight() // may not need for missile
        {
            _targetSighting = _activeTarget.transform.position - _baseRotationObj.position;

            _cosAngle = Vector3.Dot(_targetSighting.normalized, _baseRotationObj.forward);

            _targetAngle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;

            return _targetAngle <= _viewingAngle;
        }

        protected abstract void RotateToTarget(Vector3 target); // late update may work better

        protected abstract void RotateToStart();

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

        private void AssignNewTarget(GameObject currentTarget, int reward)
        {
            if (_activeList.Contains(currentTarget))
            {
                _activeList.Remove(currentTarget);

                if (_activeTarget == currentTarget)
                {
                    _activeTarget = null;
                }                

                if (_activeList.Any())
                {
                    _activeTarget = _activeList.FirstOrDefault();
                }

                ActivateTurret(false);
            }
        }

        protected abstract void TurretAttack(GameObject activeTarget, float damageAmount);
        
        protected virtual void OnTurretAttack(GameObject activeTarget, float damageAmount)
        {
            onTurretAttack?.Invoke(activeTarget, damageAmount);
        }
        
        protected virtual void UpdateAttackList(GameObject turret, GameObject target, bool addTo)
        {
            if (this.gameObject == turret)
            {
                if (addTo)
                {
                    _activeList.Add(target);

                    if (_activeList.Count <= 1)
                    {
                        _activeTarget = _activeList.FirstOrDefault();
                    }
                }
                else
                {
                    if (_activeList.Contains(target))
                    {
                        _activeList.Remove(target);
                    }
                }
            }
        }
    }
}

