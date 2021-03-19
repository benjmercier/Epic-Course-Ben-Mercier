using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class TurretLimitedRotation : Turret
    {
        [SerializeField]
        protected float _fireRate = 0.5f;
        protected float _lastFire;

        protected Vector3 _targetVector;
        protected float _cosAngle;
        protected float _targetAngle;

        protected virtual bool ReturnTargetInLineOfSight(GameObject target)
        {
            _targetVector = target.transform.position - _primaryRotationObj.position;

            _cosAngle = Vector3.Dot(_targetVector.normalized, this.transform.forward);

            _targetAngle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;
                        
            return _targetAngle <= _maxRotationAngle.y;
        }

        // registered to Enemy onDeath event
        protected override void AssignNewTarget(GameObject activeTarget, int reward)
        {
            if (_activeTarget == activeTarget)
            {
                _activeTargetList.Remove(activeTarget);

                _activeTarget = ReturnActiveTarget();
            }
            else
            {
                if (_activeTargetList.Contains(activeTarget))
                {
                    _activeTargetList.Remove(activeTarget);
                }
            }
        }

        protected override GameObject ReturnActiveTarget()
        {
            if (_activeTargetList.Any(t => ReturnTargetInLineOfSight(t)))
            {
                _activeTarget = _activeTargetList.Where(t => ReturnTargetInLineOfSight(t)).
                    OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).
                    FirstOrDefault();

                if (_activeTarget != null)
                {
                    OnCheckForRotationTarget(_activeTarget);
                }

                return _activeTarget;
            }

            _rotationTarget = null;

            return null;
        }

        public override void RotateToTarget(Vector3 target)
        {
            if (!_canFire)
            {
                ActivateTurret(true);
            }
            /*

            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

            _primaryTargetDirection = target - _primaryRotationObj.position;
            _primaryLookRotation = Quaternion.LookRotation(_primaryTargetDirection);

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryLookRotation, _primaryMovement);

            _xAngleCheck = ReturnLocalEulerAngleCheck(_primaryRotationObj.localEulerAngles.x);
            _yAngleCheck = ReturnLocalEulerAngleCheck(_primaryRotationObj.localEulerAngles.y);

            _xClamped = Mathf.Clamp(_xAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);
            _yClamped = Mathf.Clamp(_yAngleCheck, _minRotationAngle.y, _maxRotationAngle.y);

            _primaryRotationObj.localRotation = Quaternion.Euler(_xClamped, _yClamped, _primaryRotationObj.localEulerAngles.z);
            */

            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

            _primaryTargetDirection = target - _primaryRotationObj.position;
            _primaryLookRotation = Quaternion.LookRotation(_primaryTargetDirection);

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryLookRotation, _primaryMovement);
        }

        public override void RotateToStart()
        {
            if (_canFire)
            {
                ActivateTurret(false);
            }

            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryInitialRotation, _primaryMovement);
        }

        public override bool IsAtStart()
        {
            _primaryAngleToStartRotation = Quaternion.Angle(_primaryRotationObj.rotation, _primaryInitialRotation);

            return _primaryAngleToStartRotation < _minAngleToStartRotation;
        }

        public override void TurretAttack()
        {
            if (Time.time > _lastFire)
            {
                _lastFire = Time.time + _fireRate;

                OnTurretAttack();
            }
        }
    }
}

