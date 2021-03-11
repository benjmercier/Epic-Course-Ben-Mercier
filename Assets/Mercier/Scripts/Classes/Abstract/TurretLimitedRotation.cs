using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public abstract class TurretLimitedRotation : BaseTurret
    {
        [SerializeField]
        protected float _fireRate = 0.5f;
        protected float _lastFire;

        protected Vector3 _targetVector;
        protected float _cosAngle;
        protected float _targetAngle;

        protected override void ControlTurretState()
        {
            switch (currentState)
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
                    if (ReturnTargetInLineOfSight(_activeTarget))
                    {
                        currentState = TurretState.Attacking;
                    }
                    else if (_activeTarget == null)
                    {
                        currentState = TurretState.Idle;
                    }
                    else
                    {
                        RotateToStart();
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
            _targetVector = target.transform.position - _primaryRotation.position;

            _cosAngle = Vector3.Dot(_targetVector.normalized, this.transform.forward);

            _targetAngle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;

            return _targetAngle <= _maxRotationAngle.y;
        }

        protected override void UpdateTargetList(GameObject turret, GameObject target, bool addToList)
        {
            if (this.gameObject == turret)
            {
                if (addToList)
                {
                    _activeTargetList.Add(target);

                    _activeTarget = ReturnActiveTarget();
                }
            }
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

        protected GameObject ReturnActiveTarget()
        {
            if (_activeTargetList.Any(t => ReturnTargetInLineOfSight(t)))
            {
                _activeTarget = _activeTargetList.Where(t => ReturnTargetInLineOfSight(t)).
                    OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).
                    FirstOrDefault();

                if (_activeTarget != null)
                {
                    _rotationTarget = OnRequestRotationTarget(_activeTarget);
                }

                return _activeTarget;
            }

            _rotationTarget = null;

            return null;
        }

        protected override void RotateToTarget(Vector3 target)
        {
            _targetDirection = target - _primaryRotation.position;

            _movement = _rotationSpeed * Time.deltaTime;

            _primaryRotateTowards = Vector3.RotateTowards(_primaryRotation.forward, _targetDirection, _movement, 0f);
            _primaryLookRotation = Quaternion.LookRotation(_primaryRotateTowards);

            _primaryRotation.rotation = _primaryLookRotation;

            _xAngleCheck = ReturnLocalEulerAngleCheck(_primaryRotation.localEulerAngles.x);
            _yAngleCheck = ReturnLocalEulerAngleCheck(_primaryRotation.localEulerAngles.y);

            _xClamped = Mathf.Clamp(_xAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);
            _yClamped = Mathf.Clamp(_yAngleCheck, _minRotationAngle.y, _maxRotationAngle.y);

            _primaryRotation.localRotation = Quaternion.Euler(_xClamped, _yClamped, _primaryRotation.localEulerAngles.z);
        }

        protected override void RotateToStart()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _primaryRotation.rotation = Quaternion.Slerp(_primaryRotation.rotation, _primaryInitialRotation, _movement);
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

