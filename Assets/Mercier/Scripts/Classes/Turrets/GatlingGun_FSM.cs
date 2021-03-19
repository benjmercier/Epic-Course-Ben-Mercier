using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Classes.Turrets
{
    public class GatlingGun_FSM : LimitedTurretRotation
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

        private void Start()
        {
            _muzzleFlash.ToList().ForEach(i => i.SetActive(false));
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
            _audioSource.clip = _fireSound;
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

        private void RotateBarrel()
        {
            _gunBarrel.ToList().ForEach(i => i.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f));
        }

        protected override IEnumerator DestroyedRoutine()
        {
            throw new NotImplementedException();
        }
    }
}

