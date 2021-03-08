using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mercier.Scripts.Classes
{
    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class GatlingGun : Turret
    {
        [Header("Gatling Gun Settings")]
        [SerializeField]
        private Transform _gunBarrel; //Reference to hold the gun barrel
        [SerializeField]
        private GameObject _muzzleFlash; //reference to the muzzle flash effect to play when firing
        [SerializeField]
        private ParticleSystem _bulletCasings; //reference to the bullet casing effect to play when firing
        [SerializeField]
        private AudioClip _fireSound; //Reference to the audio clip
        [SerializeField]
        private AudioSource _audioSource; //reference to the audio source component
        private bool _startWeaponNoise = true;

        protected override void Awake()
        {
            base.Awake();

            if (_gunBarrel == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::" + _gunBarrel.ToString() + " is null.");
            }

            if (_muzzleFlash == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::" + _muzzleFlash.ToString() + " is null.");
            }

            if (_bulletCasings == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::" + _bulletCasings.ToString() + " is null.");
            }

            if (_fireSound == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::" + _fireSound.ToString() + " is null.");
            }

            if (_audioSource == null)
            {
                Debug.LogError("Gatling_Gun::Awake()::" + _audioSource.ToString() + " is null.");
            }
        }

        void Start()
        {
            _muzzleFlash.SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = _fireSound; //assign the clip to play
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
            _muzzleFlash.SetActive(true);
            _bulletCasings.Emit(1);

            if (_startWeaponNoise)
            {
                _audioSource.Play();
                _startWeaponNoise = false;
            }
        }

        protected override void DisengageTarget()
        {
            _muzzleFlash.SetActive(false);
            _audioSource.Stop();
            _startWeaponNoise = true;
        }

        void RotateBarrel()
        {
            _gunBarrel.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f);

        }

        protected override void RotateToTarget(Vector3 target)
        {
            _targetDirection = target - _baseRotationObj.position;

            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotateTowards = Vector3.RotateTowards(_baseRotationObj.forward, _targetDirection, _movement, 0f);
            _baseLookRotation = Quaternion.LookRotation(_baseRotateTowards);

            _xClamped = Mathf.Clamp(_baseLookRotation.eulerAngles.x, _minRotationAngle.x, _maxRotationAngle.x);
            _yClamped = Mathf.Clamp(_baseLookRotation.eulerAngles.y, _minRotationAngle.y + _yAngleOffset, _maxRotationAngle.y + _yAngleOffset);

            _baseRotationObj.rotation = Quaternion.Euler(_xClamped, _yClamped, _baseLookRotation.eulerAngles.z);
        }

        protected override void RotateToStart()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _baseRotationObj.rotation = Quaternion.Slerp(_baseRotationObj.rotation, _baseInitialRotation, _movement);
        }
    }
}
