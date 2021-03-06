using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes;

namespace GameDevHQ.FileBase.Gatling_Gun
{
    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Gatling_Gun : Turret
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

            /*
            if (Input.GetMouseButton(0)) //Check for left click (held) user input
            { 
                RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel
                Muzzle_Flash.SetActive(true); //enable muzzle effect particle effect
                bulletCasings.Emit(1); //Emit the bullet casing particle effect  

                if (_startWeaponNoise == true) //checking if we need to start the gun sound
                {
                    _audioSource.Play(); //play audio clip attached to audio source
                    _startWeaponNoise = false; //set the start weapon noise value to false to prevent calling it again
                }

            }
            else if (Input.GetMouseButtonUp(0)) //Check for left click (release) user input
            {      
                Muzzle_Flash.SetActive(false); //turn off muzzle flash particle effect
                _audioSource.Stop(); //stop the sound effect from playing
                _startWeaponNoise = true; //set the start weapon noise value to true
            }*/
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
    }

}
