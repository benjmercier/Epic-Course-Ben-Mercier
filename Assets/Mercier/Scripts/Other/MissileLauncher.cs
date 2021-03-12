using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public class MissileLauncher : TurretOld
    {
        public enum MissileType
        {
            Normal,
            Homing
        }

        [Header("Missle Launcher Settings")]
        [SerializeField]
        private GameObject _missilePrefab;
        [SerializeField]
        private MissileType _missileType;
        [SerializeField]
        private GameObject[] _missilePositions;
        [SerializeField]
        private float _fireDelay;
        [SerializeField]
        private float _launchSpeed; 
        [SerializeField]
        private float _power; 
        [SerializeField]
        private float _fuseDelay;
        [SerializeField]
        private float _reloadTime; 
        [SerializeField]
        private float _destroyTime = 10.0f; 
        private bool _launched; 
                
        private int _missileLaunchIndex = 0;
        private GameObject _missileToLaunch;
        private Vector3 _missileLaunchRotation = new Vector3(-90f, 0f, 0f);

        private Vector3 _forwardView;
        private Vector3 _targetDirectionNorm;
        private float _dotAngle;

        private Queue<GameObject> _openPositionQueue = new Queue<GameObject>();
        private List<GameObject> _currentMissileSalvo = new List<GameObject>();

        public override void OnEnable()
        {
            base.OnEnable();

            MissileOld.onTargetEnemyHit += OnMissileHitTarget;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            MissileOld.onTargetEnemyHit -= OnMissileHitTarget;
        }

        protected override void Update()
        {
            base.Update();
        }

        private IEnumerator MissileLaunchRoutine()
        {
            if (_missileLaunchIndex < _missilePositions.Length)
            {
                _currentMissileSalvo.Clear();

                for (int i = _missileLaunchIndex; i < _missilePositions.Length; i++)
                {
                    if (_attackTarget == null)
                    {
                        StartCoroutine(MissileReloadRoutine());

                        break;
                    }

                    _missileToLaunch = Instantiate(_missilePrefab, _missilePositions[i].transform); // change to pool
                    _missileToLaunch.transform.localPosition = Vector3.zero;
                    _missileToLaunch.transform.localEulerAngles = _missileLaunchRotation;
                    _missileToLaunch.transform.parent = null;

                    // add active missiles to list
                    _currentMissileSalvo.Add(_missileToLaunch);

                    // change to event
                    _missileToLaunch.GetComponent<MissileOld>().AssignMissileRules(_missileType, _attackTarget.transform, _launchSpeed, _power, _fuseDelay, _destroyTime);

                    _missilePositions[i].SetActive(false);

                    // add inactive missilePositions to queue
                    _openPositionQueue.Enqueue(_missilePositions[i]);

                    _missileLaunchIndex++;

                    yield return new WaitForSeconds(_fireDelay); // cache and call to helper 
                }
            }   
        }

        private IEnumerator MissileReloadRoutine()
        {
            while (_attackTarget == null)
            {
                if (_openPositionQueue.Any(i => !i.activeInHierarchy))
                {
                    yield return new WaitForSeconds(_reloadTime);

                    _openPositionQueue.Peek().SetActive(true);

                    _openPositionQueue.Dequeue();

                    _missileLaunchIndex--;

                    if (_missileLaunchIndex < 0)
                    {
                        _missileLaunchIndex = 0;
                    }
                }
            }            

            _launched = false;
        }

        protected override void EngageTarget()
        {

        }

        protected override void DisengageTarget()
        {

        }

        protected override void RotateToTarget(Vector3 target)
        {
            _targetDirection = target - _baseRotationObj.position;
            
            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotateTowards = Vector3.RotateTowards(_baseRotationObj.forward, _targetDirection, _movement, 0f);
            _baseRotateTowards.y = 0;

            _auxRotateTowards = Vector3.RotateTowards(_auxRotationObj.forward, _targetDirection, _movement, 0f);

            _baseLookRotation = Quaternion.LookRotation(_baseRotateTowards); // clamp y
            _auxLookRotation = Quaternion.LookRotation(_auxRotateTowards); // clamp x
            
            _baseRotationObj.rotation = _baseLookRotation;
            _auxRotationObj.rotation = _auxLookRotation;

            _xRotAngleCheck = ReturnRotationAngleCheck(_auxRotationObj.localEulerAngles.x);

            _xClamped = Mathf.Clamp(_xRotAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);

            _auxRotationObj.localRotation = Quaternion.Euler(_xClamped, _auxRotationObj.localEulerAngles.y, _auxRotationObj.localEulerAngles.z);
            _auxRotationObj.localRotation = RestrictLocalRotaionY(_auxRotationObj.localEulerAngles);
        }

        private bool IsLockedOn(Vector3 targetPos)
        {
            _forwardView = _auxRotationObj.forward;

            _targetDirectionNorm = (targetPos - _auxRotationObj.position).normalized;

            _dotAngle = Vector3.Dot(_forwardView, _targetDirectionNorm);

            return _dotAngle < 0.95f ? false : true;
        }

        protected override void TurretAttack(GameObject activeTarget, float damageAmount)
        {
            Debug.Log("Turret Attacking");

            if (_launched == false)
            {
                if (IsLockedOn(activeTarget.transform.position))
                {
                    _launched = true;
                    StartCoroutine(MissileLaunchRoutine());
                }
            }
        }

        private void OnMissileHitTarget(GameObject missile, GameObject target)
        {
            if (_currentMissileSalvo.Contains(missile))
            {
                OnTurretAttack(target, _attackStrength);
                Debug.Log("MISSILE HIT!");
            }
        }

        protected override void RotateToStart()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotationObj.rotation = Quaternion.Slerp(_baseRotationObj.rotation, _baseInitialRotation, _movement);

            _auxRotationObj.rotation = Quaternion.Slerp(_auxRotationObj.rotation, _auxInitialRotation, _movement);
            _auxRotationObj.localRotation = RestrictLocalRotaionY(_auxRotationObj.localEulerAngles);
        }

        private Quaternion RestrictLocalRotaionY(Vector3 localEulerAngles)
        {
            return Quaternion.Euler(localEulerAngles.x, 0, localEulerAngles.z);
        }
    }
}