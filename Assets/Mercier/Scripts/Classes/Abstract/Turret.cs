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
        [Header("Rotation Settings")]
        [SerializeField]
        private Transform _objToRotate;
        [SerializeField]
        protected float _rotationSpeed = 2.5f;
        [SerializeField]
        protected float _viewingAngle = 65f;
        [SerializeField]
        protected Vector2 _maxRotationAngle = new Vector2(-25f, 35f);
        [SerializeField]
        protected Vector2 _minRotationAngle = new Vector2(-45f, 45f);

        private Vector3 _targetSighting;
        private float _cosAngle;
        private float _targetAngle;        

        private Quaternion _initialRotation;        
        private Vector3 _targetDirection;
        private Vector3 _rotateTowards;
        private Quaternion _lookRotation;
        private float _movement;
        private float _xClamped;
        private float _yClamped;
        private float _yAngleOffset = 180f;

        [Header("Attack Settings")]
        [SerializeField]
        protected List<GameObject> _activeList = new List<GameObject>();
        [SerializeField]
        protected GameObject _activeTarget;
        [SerializeField]
        protected float _attackStrength = 5f;
        [SerializeField]
        protected float _fireRate = 0.5f;
        private float _lastFire;
        private bool _hasFired;
        protected bool _canFire;

        // set event system to communicate with active target
        public static event Action<GameObject, float> onTurretAttack;

        protected virtual void Awake()
        {
            if (_objToRotate == null)
            {
                Debug.LogError("Turret::Start()::" + _objToRotate.ToString() + " is NULL.");
            }

            _initialRotation = _objToRotate.rotation;
        }

        public void OnEnable()
        {
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
            CalculateRotation();
        }        

        protected virtual void CalculateRotation()
        {
            if (_activeTarget != null)
            {
                if (ReturnWithinLineOfSight())
                {
                    _hasFired = true;
                    ActivateTurret(true);
                    RotateToTarget(_activeTarget.transform.position);

                    //AttackTarget();
                    OnTurretAttack(_activeTarget, _attackStrength);
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
                RotateToStart();
            }
        }

        private bool ReturnWithinLineOfSight()
        {
            _targetSighting = _activeTarget.transform.position - _objToRotate.position;

            _cosAngle = Vector3.Dot(_targetSighting.normalized, _objToRotate.forward);

            _targetAngle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;

            return _targetAngle <= _viewingAngle;
        }

        protected virtual void RotateToTarget(Vector3 target)
        {
            _targetDirection = target - _objToRotate.position;

            _movement = _rotationSpeed * Time.deltaTime;

            _rotateTowards = Vector3.RotateTowards(_objToRotate.forward, _targetDirection, _movement, 0f);
            _lookRotation = Quaternion.LookRotation(_rotateTowards);

            _xClamped = Mathf.Clamp(_lookRotation.eulerAngles.x, _minRotationAngle.x, _maxRotationAngle.x);
            _yClamped = Mathf.Clamp(_lookRotation.eulerAngles.y, _minRotationAngle.y + _yAngleOffset, _maxRotationAngle.y + _yAngleOffset);

            _objToRotate.rotation = Quaternion.Euler(_xClamped, _yClamped, _lookRotation.eulerAngles.z);
        }

        private void RotateToStart()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _objToRotate.rotation = Quaternion.Slerp(_objToRotate.rotation, _initialRotation, _movement);
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

        /*
        protected virtual void AttackTarget()
        {
            if (Time.time > _lastFire)
            {
                _lastFire = Time.time + _fireRate;

                // apply damage

                Debug.Log("Causing Damage");
            }
        }*/

        private void AssignNewTarget(GameObject currentTarget, int reward)
        {
            if (_activeList.Contains(currentTarget))
            {
                _activeList.Remove(currentTarget);

                _activeTarget = null;

                if (_activeList.Any())
                {
                    _activeTarget = _activeList.FirstOrDefault();
                }
            }
        }

        protected virtual void OnTurretAttack(GameObject activeTarget, float damageAmount)
        {
            if (Time.time > _lastFire)
            {
                _lastFire = Time.time + _fireRate;

                onTurretAttack?.Invoke(activeTarget, damageAmount);

                Debug.Log("Causing Damage");
            }
        }   
        
        protected virtual void UpdateAttackList(GameObject turret, GameObject activeTarget, bool addTo)
        {
            if (this.gameObject == turret)
            {
                if (addTo)
                {
                    _activeList.Add(activeTarget);

                    if (_activeList.Count <= 1)
                    {
                        _activeTarget = _activeList.FirstOrDefault();
                    }
                }
                else
                {
                    if (_activeList.Contains(activeTarget))
                    {
                        _activeList.Remove(activeTarget);
                    }
                }
            }
        }
    }
}

