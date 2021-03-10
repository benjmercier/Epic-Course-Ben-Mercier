using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public class MissileLauncher : Turret
    {
        public enum MissileType
        {
            Normal,
            Homing
        }

        [Header("Missle Launcher Settings")]
        [SerializeField]
        private GameObject _missilePrefab; //holds the missle gameobject to clone
        [SerializeField]
        private MissileType _missileType; //type of missle to be launched
        [SerializeField]
        private GameObject[] _missilePositions; //array to hold the rocket positions on the turret
        [SerializeField]
        private float _fireDelay; //fire delay between rockets
        [SerializeField]
        private float _launchSpeed; //initial launch speed of the rocket
        [SerializeField]
        private float _power; //power to apply to the force of the rocket
        [SerializeField]
        private float _fuseDelay; //fuse delay before the rocket launches
        [SerializeField]
        private float _reloadTime; //time in between reloading the rockets
        [SerializeField]
        private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        private bool _launched; //bool to check if we launched the rockets
                
        private int _missileLaunchIndex = 0;
        private GameObject _missileToLaunch;
        private Vector3 _missileLaunchRotation = new Vector3(-90f, 0f, 0f);

        //private bool _lockedOn = false;
        private Vector3 _forwardView;
        private Vector3 _targetDirectionNorm;
        private float _dotAngle;

        protected override void Update()
        {
            base.Update();
        }

        private IEnumerator MissileLaunchRoutine()
        {
            if (_missileLaunchIndex < _missilePositions.Length)
            {
                for (int i = _missileLaunchIndex; i < _missilePositions.Length; i++)
                {
                    if (_activeTarget == null)
                    {
                        StartCoroutine(MissileReloadRoutine());

                        break;
                    }

                    _missileToLaunch = Instantiate(_missilePrefab, _missilePositions[i].transform); // change to pool
                    _missileToLaunch.transform.localPosition = Vector3.zero;
                    _missileToLaunch.transform.localEulerAngles = _missileLaunchRotation;
                    _missileToLaunch.transform.parent = null;

                    // change to event
                    _missileToLaunch.GetComponent<Missile>().AssignMissileRules(_missileType, _activeTarget.transform, _launchSpeed, _power, _fuseDelay, _destroyTime);

                    _missilePositions[i].SetActive(false);

                    _missileLaunchIndex++;

                    yield return new WaitForSeconds(_fireDelay); // cache and call to helper 
                }
            }   
        }

        private IEnumerator MissileReloadRoutine()
        {
            while (_activeTarget == null)
            {
                for (int i = 0; i <= _missilePositions.Length; i++)
                {
                    yield return new WaitForSeconds(_reloadTime);

                    _missilePositions[i].SetActive(true);

                    _missileLaunchIndex--;

                    if (_missileLaunchIndex < 0)
                    {
                        _missileLaunchIndex = 0;
                    }
                }
            }            

            _launched = false;
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _missilePositions.Length; i++) //for loop to iterate through each missle position
            {
                GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

                rocket.transform.parent = _missilePositions[i].transform; //set the rockets parent to the missle launch position 
                rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocket.transform.parent = null; //set the rocket parent to null

                rocket.GetComponent<Missile>().AssignMissileRules(_missileType, _activeTarget.transform, _launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

                _missilePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            for (int i = 0; i < _missilePositions.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _missilePositions[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
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
            
            /*
            _xRotAngleCheck = _auxLookRotation.eulerAngles.x <= 180 ?
                _auxLookRotation.eulerAngles.x :
                -(360 - _auxLookRotation.eulerAngles.x); // is condition true ? yes : no  (conditional operator)

            _xClamped = Mathf.Clamp(_xRotAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);
            */
            _baseRotationObj.rotation = _baseLookRotation;
            _auxRotationObj.rotation = _auxLookRotation; //Quaternion.Euler(_xClamped, _auxLookRotation.eulerAngles.y, _auxLookRotation.eulerAngles.z);

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

            //OnTurretAttack(activeTarget, damageAmount);  // calls event on Turret class
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