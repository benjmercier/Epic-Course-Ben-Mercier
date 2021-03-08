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
        //[SerializeField]
        //private Transform _target; //Who should the rocket fire at?

        private bool _lockedOn = false;
        private int _missileLaunchIndex = 0;
        private GameObject _missileToLaunch;
        private Vector3 _missileLaunchRotation = new Vector3(-90f, 0f, 0f);

        protected override void Update()
        {
            base.Update();
        }

        /*
        private void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Space) && _launched == false) //check for space key and if we launched the rockets
            {
                _launched = true; //set the launch bool to true
                StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets. 
            }
        }*/

        private IEnumerator MissileLaunchRoutine()
        {
            if (_missileLaunchIndex < _missilePositions.Length && _lockedOn)
            {
                for (int i = _missileLaunchIndex; i < _missilePositions.Length; i++)
                {
                    if (_activeTarget == null)
                    {
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


            //while (_activeTarget != null) //_missilePositions.All(a => !a.activeInHierarchy)) // is this too unperformant?
            //{
                
            //}

            for (int i = _missileLaunchIndex; i < _missilePositions.Length; i++)
            {
                yield return new WaitForSeconds(_reloadTime);

                _missilePositions[i].SetActive(true);

                _missileLaunchIndex--;

                if (_missileLaunchIndex < 0)
                {
                    _missileLaunchIndex = 0;
                }
            }
            /*
            foreach (var missle in _missilePositions)
            {
                if (!missle.activeInHierarchy)
                {
                    yield return new WaitForSeconds(_reloadTime); // cache

                    missle.SetActive(true);
                }
            }*/

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

            _xRotAngleCheck = _auxLookRotation.eulerAngles.x <= 180 ?
                _auxLookRotation.eulerAngles.x :
                -(360 - _auxLookRotation.eulerAngles.x); // is condition true ? yes : no  (conditional operator)

            _xClamped = Mathf.Clamp(_xRotAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);

            _baseRotationObj.rotation = _baseLookRotation;
            _auxRotationObj.rotation = Quaternion.Euler(_xClamped, _auxLookRotation.eulerAngles.y, _auxLookRotation.eulerAngles.z);

            _lockedOn = IsLockedOn(_activeTarget.transform.position);
            Debug.Log("Locked On: " + _lockedOn);
        }

        private bool IsLockedOn(Vector3 targetPos)
        {
            //Vector3 baseForward = _baseRotationObj.forward;
            Vector3 forward = _auxRotationObj.forward;

            //Vector3 baseTarget = (targetPos - _baseRotationObj.position).normalized;
            Vector3 targetNorm = (targetPos - _auxRotationObj.position).normalized;

            //var baseDot = Vector3.Dot(baseForward, baseTarget);
            var dotAngle = Vector3.Dot(forward, targetNorm);

            return dotAngle < 0.95f ? false : true;
        }

        protected override void RotateToStart()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotationObj.rotation = Quaternion.Slerp(_baseRotationObj.rotation, _baseInitialRotation, _movement);
            _auxRotationObj.rotation = Quaternion.Slerp(_auxRotationObj.rotation, _auxInitialRotation, _movement);
        }

        protected override void TurretAttack(GameObject activeTarget, float damageAmount)
        {
            Debug.Log("Turret Attacking");

            if (_launched == false)
            {
                if (_lockedOn)
                {
                    _launched = true;
                    StartCoroutine(MissileLaunchRoutine());
                }
            }

            //OnTurretAttack(activeTarget, damageAmount);  // calls event on Turret class
        }
    }
}