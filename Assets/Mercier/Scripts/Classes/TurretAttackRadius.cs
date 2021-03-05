﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GameDevHQ.FileBase.Gatling_Gun;

namespace Mercier.Scripts.Classes
{
    public class TurretAttackRadius : MonoBehaviour
    {
        [SerializeField]
        private Gatling_Gun _gatlingGun;

        [SerializeField]
        private Transform _objToRotate;
        [SerializeField]
        private float _rotationSpeed = 5f;
        [SerializeField]
        private Vector2 _minMaxX = new Vector2(-25f, 35f);
        private float _xClamped;
        [SerializeField]
        private Vector2 _minMaxY = new Vector2(-45f, 45f);
        private float _yClamped;

        [SerializeField]
        private float _maxAngle = 45f;
        private float _cosAngle;
        private float _angle;

        [SerializeField]
        private List<GameObject> _attackList = new List<GameObject>();
        [SerializeField]
        private GameObject _activeTarget;

        private Quaternion _initialRotation;
        private Vector3 _castDirection;
        private Vector3 _targetDirection;
        private Vector3 _rotateTowards;
        private Quaternion _lookRotation;
        private float _movement;

        private bool _hasFired = false;

        public enum TurretState
        {
            Idle,
            Searching,
            Attacking,
            Destroyed
        }

        public TurretState currentState;

        void Start()
        {
            if (_gatlingGun == null)
            {
                Debug.LogError("TurretAttackRadius::Start()::" + this.transform.parent + "'s _gatlingGun is NULL");
            }

            if (_objToRotate == null)
            {
                Debug.LogError("TurretAttackRadius::Start()::" + gameObject.transform.parent.name + "'s _rotationObj is NULL.");
            }

            _initialRotation = _objToRotate.rotation;
        }

        void Update()
        {
            CalculateRotation();
        }

        private bool ReturnWithinLineOfSight()
        {
            _castDirection = _activeTarget.transform.position - _objToRotate.position;

            _cosAngle = Vector3.Dot(_castDirection.normalized, _objToRotate.forward);

            _angle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;

            return _angle <= _maxAngle;
        }

        private void CalculateRotation()
        {
            if (_activeTarget != null)
            {
                if (ReturnWithinLineOfSight())
                {
                    _hasFired = true;
                    _gatlingGun.ActivateTurret(true);
                    RotateToTargetPos();
                }
                else
                {
                    if (_hasFired)
                    {
                        _hasFired = false;
                        _gatlingGun.ActivateTurret(false);
                        AssignNewTarget();
                    }

                    RotateToDefaultPos();
                }
            }
            else
            {
                RotateToDefaultPos();
            }
        }

        private void RotateToTargetPos()
        {
            _targetDirection = _activeTarget.transform.position - _objToRotate.position;

            _movement = _rotationSpeed * Time.deltaTime;

            _rotateTowards = Vector3.RotateTowards(_objToRotate.forward, _targetDirection, _movement, 0f);
            _lookRotation = Quaternion.LookRotation(_rotateTowards);

            _xClamped = Mathf.Clamp(_lookRotation.eulerAngles.x, _minMaxX.x, _minMaxX.y);
            _yClamped = Mathf.Clamp(_lookRotation.eulerAngles.y, _minMaxY.x + 180, _minMaxY.y + 180);

            _objToRotate.rotation = Quaternion.Euler(_xClamped, _yClamped, _lookRotation.eulerAngles.z);
        }

        private void RotateToDefaultPos()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _objToRotate.rotation = Quaternion.Slerp(_objToRotate.rotation, _initialRotation, _movement);
        }

        private void AssignNewTarget()
        {
            if (_attackList.Contains(_activeTarget))
            {
                _attackList.Remove(_activeTarget);

                _activeTarget = null;

                if (_attackList.Any())
                {
                    _activeTarget = _attackList.FirstOrDefault();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy")) // if using Interface on enemy, can check if it implements Interface
            {
                _attackList.Add(other.gameObject);

                if (_attackList.Count <= 1)
                {
                    _activeTarget = _attackList.FirstOrDefault();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _attackList.Remove(other.gameObject);
            }
        }
    }
}

