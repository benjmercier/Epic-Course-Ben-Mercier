using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missle_Launcher.Missle;
using Mercier.Scripts.Classes;

namespace GameDevHQ.FileBase.Missle_Launcher
{
    public class Missle_Launcher : Turret
    {
        [Header("Missle Launcher Settings")]
        [SerializeField]
        private GameObject _missilePrefab; //holds the missle gameobject to clone
        [SerializeField]
        private GameObject[] _misslePositions; //array to hold the rocket positions on the turret
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

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositions.Length; i++) //for loop to iterate through each missle position
            {
                GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

                rocket.transform.parent = _misslePositions[i].transform; //set the rockets parent to the missle launch position 
                rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocket.transform.parent = null; //set the rocket parent to null

                rocket.GetComponent<GameDevHQ.FileBase.Missle_Launcher.Missle.Missle>().AssignMissleRules(_launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

                _misslePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
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
            Debug.Log("Rotating to target.");

            // take target direction and movement
            // separate _rotateTowards & _lookRotation by object
            // x rot obj has y set to 0
            // y rot obj has x set to 0
            // clamp each
            // combine to one and pass to rotation?

            _targetDirection = target - _baseRotationObj.position;

            _movement = _rotationSpeed * Time.deltaTime;

            var _baseRotateTowards = Vector3.RotateTowards(_baseRotationObj.forward, _targetDirection, _movement, 0f);
            _baseRotateTowards.y = 0;

            var _auxRotateTowards = Vector3.RotateTowards(_auxRotationObj.forward, _targetDirection, _movement, 0f);

            var _baseLookRotation = Quaternion.LookRotation(_baseRotateTowards); // clamp y
            var _auxLookRotation = Quaternion.LookRotation(_auxRotateTowards); // clamp x

            var xAngle = _auxLookRotation.eulerAngles.x <= 180 ? 
                _auxLookRotation.eulerAngles.x : 
                -(360 - _auxLookRotation.eulerAngles.x); // is condition true ? yes : no  (conditional operator)
            
            _xClamped = Mathf.Clamp(xAngle, _minRotationAngle.x, _maxRotationAngle.x);

            _baseRotationObj.rotation = _baseLookRotation;
            _auxRotationObj.rotation = Quaternion.Euler(_xClamped, _auxLookRotation.eulerAngles.y, _auxLookRotation.eulerAngles.z); //_auxLookRotation; 
        }

        protected override void RotateToStart()
        {
            Debug.Log("Rotating to start.");
        }
    }
}

