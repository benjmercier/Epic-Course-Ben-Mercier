using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public abstract class TurretLimitedRotationOld : BaseTurret_old
    {
        [SerializeField]
        protected float _fireRate = 0.5f;
        protected float _lastFire;

        protected Vector3 _targetVector;
        protected float _cosAngle;
        protected float _targetAngle;
        [SerializeField]
        protected float _viewAngle = 75f;

        protected virtual void LateUpdate()
        {
            switch (currentState)
            {
                case TurretState.Idle:
                    RotateToStart();
                    break;

                case TurretState.Attacking:
                    RotateToTarget(_rotationTarget.transform.position);
                    break;
            }
        }

        protected override void ControlTurretState()
        {
            switch (currentState)
            {
                case TurretState.Idle:
                    if (_activeTarget == null || !_activeTarget.activeInHierarchy)
                    {
                        RotateToStart();
                    }
                    else
                    {
                        currentState = TurretState.Attacking;
                    }
                    break;

                case TurretState.Attacking:
                    if (_activeTarget != null)
                    {
                        if (ReturnTargetInLineOfSight(_activeTarget))
                        {
                            _hasFired = true;
                            ActivateTurret(true);

                            if (_rotationTarget != null)
                            {
                                RotateToTarget(_rotationTarget.transform.position);
                            }

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

        protected virtual bool ReturnTargetInLineOfSight(GameObject target)
        {
            _targetVector = target.transform.position - _primaryRotationObj.position;

            _cosAngle = Vector3.Dot(_targetVector.normalized, this.transform.forward);

            _targetAngle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;

            return _targetAngle <= _viewAngle;
        }   

        protected override void AssignNewTarget(GameObject activeTarget, int reward)
        {
            if (_activeTarget == activeTarget)
            {
                if (!ReturnTargetInLineOfSight(activeTarget))
                {
                    _activeTargetList.Remove(activeTarget);

                    _activeTarget = ReturnActiveTarget();
                }

                ActivateTurret(false);
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

        protected override void RotateToTarget(Vector3 target)
        {
            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

            _targetDirection = target - _primaryRotationObj.position;
            _primaryLookRotation = Quaternion.LookRotation(_targetDirection);

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryLookRotation, _primaryMovement);

            /*
            _targetDirection = target - _primaryRotationObj.position;

            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

            _primaryRotateTowards = Vector3.RotateTowards(_primaryRotationObj.forward, _targetDirection, _primaryMovement, 0f);
            _primaryLookRotation = Quaternion.LookRotation(_primaryRotateTowards);

            _primaryRotationObj.rotation = _primaryLookRotation;

            _xAngleCheck = ReturnLocalEulerAngleCheck(_primaryRotationObj.localEulerAngles.x);
            _yAngleCheck = ReturnLocalEulerAngleCheck(_primaryRotationObj.localEulerAngles.y);

            _xClamped = Mathf.Clamp(_xAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);
            _yClamped = Mathf.Clamp(_yAngleCheck, _minRotationAngle.y, _maxRotationAngle.y);

            _primaryRotationObj.localRotation = Quaternion.Euler(_xClamped, _yClamped, _primaryRotationObj.localEulerAngles.z);
            */
        }

        protected override void RotateToStart()
        {
            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryInitialRotation, _primaryMovement);
        }

        protected override void TurretAttack(GameObject activeTarget, float damageAmount)
        {
            if (Time.time > _lastFire)
            {
                _lastFire = Time.time + _fireRate;

                OnTurretAttack(activeTarget, damageAmount);
            }
        }
    }
}

