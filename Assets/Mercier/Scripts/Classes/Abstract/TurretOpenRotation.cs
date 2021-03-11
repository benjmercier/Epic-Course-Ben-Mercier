using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public abstract class TurretOpenRotation : BaseTurret
    {
        [Header("Auxiliary Rotation")]
        [SerializeField]
        protected Transform _auxiliaryRotationObj;
        [SerializeField]
        protected float _auxiliaryRotationSpeed = 2.5f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        protected float _minFacingAccuracy = 0.95f;

        protected float _auxiliaryMovement;

        protected Quaternion _auxiliaryInitialRotation;
        protected Vector3 _auxiliaryRotateTowards;
        protected Quaternion _auxiliaryLookRotation;

        protected Vector3 _forwardView;
        protected Vector3 _directionToFace;
        protected float _dotAngle;

        protected override void Awake()
        {
            base.Awake();

            if (_auxiliaryRotationObj == null)
            {
                Debug.LogError("TurretOpenRotation::Awake()::_auxiliaryRotation is NULL on " + gameObject.name);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _auxiliaryInitialRotation = _auxiliaryRotationObj.rotation;

            _forwardView = _auxiliaryRotationObj.forward;
        }


        protected override void ControlTurretState()
        {
            switch (currentState)
            {
                case TurretState.Idle:
                    // if no target or target not acive
                    if (_activeTarget == null || !_activeTarget.activeInHierarchy)
                    {
                        RotateToStart();
                    }
                    else // start attacking
                    {
                        currentState = TurretState.Attacking;
                    }
                    break;

                case TurretState.Attacking:
                    // if target and target is active
                    if (_activeTarget != null && _activeTarget.activeInHierarchy)
                    {
                        if (_rotationTarget != null)
                        {
                            RotateToTarget(_rotationTarget.transform.position);
                        }
                        else
                        {
                            OnCheckForRotationTarget(_activeTarget);
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

        protected override void AssignNewTarget(GameObject activeTarget, int reward) // also registered to onEnemyDeath
        {
            if (_activeTarget == activeTarget)
            {
                _activeTargetList.Remove(activeTarget);

                _activeTarget = null;
                _rotationTarget = null;

                _activeTarget = ReturnActiveTarget();

                ActivateTurret(false);
            }
        }

        protected override GameObject ReturnActiveTarget()
        {
            if (_activeTargetList.Any())
            {
                _activeTarget = _activeTargetList.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).
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
            _targetDirection = target - _primaryRotationObj.position;

            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;
            _auxiliaryMovement = _auxiliaryRotationSpeed * Time.deltaTime;

            _primaryRotateTowards = Vector3.RotateTowards(_primaryRotationObj.forward, _targetDirection, _primaryMovement, 0f);
            _primaryRotateTowards.y = 0f;            

            _auxiliaryRotateTowards = Vector3.RotateTowards(_auxiliaryRotationObj.forward, _targetDirection, _auxiliaryMovement, 0f);

            _primaryLookRotation = Quaternion.LookRotation(_primaryRotateTowards); // clamp y
            _auxiliaryLookRotation = Quaternion.LookRotation(_auxiliaryRotateTowards); // clamp x

            _primaryRotationObj.rotation = _primaryLookRotation;
            _auxiliaryRotationObj.rotation = _auxiliaryLookRotation;

            _xAngleCheck = ReturnLocalEulerAngleCheck(_auxiliaryRotationObj.localEulerAngles.x);

            _xClamped = Mathf.Clamp(_xAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);

            _auxiliaryRotationObj.localRotation = Quaternion.Euler(_xClamped, _auxiliaryRotationObj.localEulerAngles.y, _auxiliaryRotationObj.localEulerAngles.z);
            _auxiliaryRotationObj.localRotation = RestrictLocalEulerAngleY(_auxiliaryRotationObj.localEulerAngles);
        }

        protected override void RotateToStart()
        {
            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;
            _auxiliaryMovement = _auxiliaryRotationSpeed * Time.deltaTime;

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryInitialRotation, _primaryMovement);
            _auxiliaryRotationObj.rotation = Quaternion.Slerp(_auxiliaryRotationObj.rotation, _auxiliaryInitialRotation, _auxiliaryMovement);

            _auxiliaryRotationObj.localRotation = RestrictLocalEulerAngleY(_auxiliaryRotationObj.localEulerAngles);
        }

        protected virtual Quaternion RestrictLocalEulerAngleY(Vector3 localEulerAngles)
        {
            return Quaternion.Euler(localEulerAngles.x, 0, localEulerAngles.z);
        }

        protected virtual bool IsFacingTarget(Vector3 target)
        {
            _directionToFace = target - _auxiliaryRotationObj.position;

            _dotAngle = Vector3.Dot(_forwardView, _directionToFace.normalized);

            return _dotAngle < _minFacingAccuracy ? false : true;
        }
    }
}

