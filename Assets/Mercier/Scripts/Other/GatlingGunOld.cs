using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    [RequireComponent(typeof(AudioSource))]
    public class GatlingGunOld : TurretOld
    {
        [Header("Gatling Gun Settings")]
        [SerializeField]
        private Transform[] _gunBarrel; 
        [SerializeField]
        private GameObject[] _muzzleFlash; 
        [SerializeField]
        private ParticleSystem[] _bulletCasings; 
        [SerializeField]
        private AudioClip _fireSound; 
        [SerializeField]
        private AudioSource _audioSource; 

        private bool _startWeaponNoise = true;

        protected override void Awake()
        {
            base.Awake();

            if (_gunBarrel == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::_gunBarrel is null.");
            }

            if (_muzzleFlash == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::_muzzleFlash is null.");
            }

            if (_bulletCasings == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::_bulletCasings is null.");
            }

            if (_fireSound == null)
            {
                Debug.LogError("Gatling_Gun::Awake():: _fireSound is null.");
            }

            if (_audioSource == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::_audioSource is null.");
            }
        }

        void Start()
        {
            _muzzleFlash.ToList().ForEach(i => i.SetActive(false));
            _audioSource.playOnAwake = false; 
            _audioSource.loop = true; 
            _audioSource.clip = _fireSound; 
        }

        protected override void Update()
        {
            base.Update();

            if (_canFire)
            {
                EngageTarget();
            }
        }

        protected override void EngageTarget()
        {
            RotateBarrel();
            _muzzleFlash.ToList().ForEach(i => i.SetActive(true));
            _bulletCasings.ToList().ForEach(i => i.Emit(1));

            if (_startWeaponNoise)
            {
                _audioSource.Play();
                _startWeaponNoise = false;
            }
        }

        protected override void DisengageTarget()
        {
            _muzzleFlash.ToList().ForEach(i => i.SetActive(false));
            _audioSource.Stop();
            _startWeaponNoise = true;
        }

        void RotateBarrel()
        {
            _gunBarrel.ToList().ForEach(i => i.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f));
        }

        protected override void RotateToTarget(Vector3 target)
        {
            _targetDirection = target - _baseRotationObj.position;

            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotateTowards = Vector3.RotateTowards(_baseRotationObj.forward, _targetDirection, _movement, 0f);
            _baseLookRotation = Quaternion.LookRotation(_baseRotateTowards);

            _baseRotationObj.rotation = _baseLookRotation;

            _xRotAngleCheck = ReturnRotationAngleCheck(_baseRotationObj.localEulerAngles.x);
            _yRotAngleCheck = ReturnRotationAngleCheck(_baseRotationObj.localEulerAngles.y);

            _xClamped = Mathf.Clamp(_xRotAngleCheck, _minRotationAngle.x, _maxRotationAngle.x);
            _yClamped = Mathf.Clamp(_yRotAngleCheck, _minRotationAngle.y, _maxRotationAngle.y);

            _baseRotationObj.localRotation = Quaternion.Euler(_xClamped, _yClamped, _baseRotationObj.localEulerAngles.z);
        }

        protected override void RotateToStart()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotationObj.rotation = Quaternion.Slerp(_baseRotationObj.rotation, _baseInitialRotation, _movement);
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
