using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes.Abstract.Turret
{
    public abstract class TurretFullRotation : Turret
    {
        [Header("Auxiliary Rotation")]
        [SerializeField]
        protected Transform _auxiliaryRotationObj;
        [SerializeField]
        protected float _auxiliaryRotationSpeed = 2.5f;
        [Range(0.0f, 1.0f)]
        [SerializeField]
        protected float _minLookVariance = 0.95f;

        protected Vector3 _auxiliaryTargetDirection;
        protected float _auxiliaryMovement;

        protected Quaternion _auxiliaryInitialRotation;
        protected Vector3 _auxiliaryRotateTowards;
        protected Quaternion _auxiliaryLookRotation;

        protected float _auxiliaryAngleToStartRotation;

        protected Vector3 _forwardView;
        protected Vector3 _directionToLook;
        protected float _lookAngle;

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

        protected override void AssignNewTarget(GameObject activeTarget, int reward) // also registered to onEnemyDeath
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

        public override void RotateToTarget(Vector3 target)
        {
            _primaryTargetDirection = target - _primaryRotationObj.position;
            _auxiliaryTargetDirection = target - _auxiliaryRotationObj.position;

            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;
            _auxiliaryMovement = _auxiliaryRotationSpeed * Time.deltaTime;

            _primaryRotateTowards = Vector3.RotateTowards(_primaryRotationObj.forward, _primaryTargetDirection, _primaryMovement, 0f);
            _primaryRotateTowards.y = 0f;

            _auxiliaryRotateTowards = Vector3.RotateTowards(_auxiliaryRotationObj.forward, _auxiliaryTargetDirection, _auxiliaryMovement, 0f);

            _primaryLookRotation = Quaternion.LookRotation(_primaryRotateTowards); // clamp y
            _auxiliaryLookRotation = Quaternion.LookRotation(_auxiliaryRotateTowards); // clamp x

            _primaryRotationObj.rotation = _primaryLookRotation;
            _auxiliaryRotationObj.rotation = _auxiliaryLookRotation;
            
            _xAngleCheck = ReturnLocalEulerAngleCheck(_auxiliaryRotationObj.localEulerAngles.x);
            
            _xClamped = Mathf.Clamp(_xAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);
            
            _auxiliaryRotationObj.localRotation = Quaternion.Euler(_xClamped, _auxiliaryRotationObj.localEulerAngles.y, _auxiliaryRotationObj.localEulerAngles.z);
            _auxiliaryRotationObj.localRotation = RestrictLocalEulerAngleY(_auxiliaryRotationObj.localEulerAngles);
        }

        public override void RotateToStart()
        {
            _primaryMovement = _primaryRotationSpeed * Time.deltaTime;
            _auxiliaryMovement = _auxiliaryRotationSpeed * Time.deltaTime;

            _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryInitialRotation, _primaryMovement);
            _auxiliaryRotationObj.rotation = Quaternion.Slerp(_auxiliaryRotationObj.rotation, _auxiliaryInitialRotation, _auxiliaryMovement);

            _auxiliaryRotationObj.localRotation = RestrictLocalEulerAngleY(_auxiliaryRotationObj.localEulerAngles);
        }

        public override bool IsAtStart()
        {
            _primaryAngleToStartRotation = Quaternion.Angle(_primaryRotationObj.rotation, _primaryInitialRotation);
            _auxiliaryAngleToStartRotation = Quaternion.Angle(_auxiliaryRotationObj.rotation, _auxiliaryInitialRotation);

            return _primaryAngleToStartRotation < _minAngleToStartRotation && _auxiliaryAngleToStartRotation < _minAngleToStartRotation ;
        }

        protected virtual Quaternion RestrictLocalEulerAngleY(Vector3 localEulerAngles)
        {
            return Quaternion.Euler(localEulerAngles.x, 0, localEulerAngles.z);
        }

        protected virtual bool IsFacingTarget()
        {
            if (_rotationTarget != null)
            {
                _directionToLook = _rotationTarget.transform.position - _auxiliaryRotationObj.position;

                _lookAngle = Vector3.Angle(_auxiliaryRotationObj.forward, _directionToLook);
                //Debug.Log("lookAngle: " + _lookAngle);
                return _lookAngle < _minLookVariance ? true : false;
            }

            return false;
        }
    }
}

