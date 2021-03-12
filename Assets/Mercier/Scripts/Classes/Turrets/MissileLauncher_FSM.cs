using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Classes.Turrets
{
    public class MissileLauncher_FSM : TurretFullRotation
    {
        public enum MissileType
        {
            Normal,
            Homing
        }

        [Header("Missile Launcher Settings")]
        [SerializeField]
        private MissileType _missileType;
        [SerializeField]
        private GameObject _missilePrefab;
        [SerializeField]
        private GameObject[] _missilePositions;
        [SerializeField]
        private float _fireDelay = 2f;
        [SerializeField]
        private float _launchSpeed = 10f;
        [SerializeField]
        private float _power = 20f;
        [SerializeField]
        private float _fuseDelay = 0f;
        [SerializeField]
        private float _reloadTime = 0.25f;
        [SerializeField]
        private float _destroyTime = 10.0f;

        private bool _launched = false;

        private int _missileLaunchIndex = 0;
        private GameObject _missileToLaunch;
        private Vector3 _missileLaunchRotation = new Vector3(-90f, 0f, 0f);

        private Queue<GameObject> _openPositionQueue = new Queue<GameObject>();
        private List<GameObject> _currentMissileSalvo = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();

            if (_missilePrefab == null)
            {
                Debug.LogError("MissileLauncher::Awake()::_missilePrefab is NULL");
            }

            if (!_missilePositions.Any())
            {
                Debug.LogError("MissileLauncher::Awake()::_missilePositions is NULL");
            }
        }

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

        protected override void EngageTarget()
        {

        }

        protected override void DisengageTarget()
        {

        }

        public override void TurretAttack()
        {
            if (_launched == false)
            {
                _launched = true;

                StartCoroutine(MissileLaunchRoutine());
            }
        }

        private IEnumerator MissileLaunchRoutine()
        {
            if (_missileLaunchIndex < _missilePositions.Length)
            {
                _currentMissileSalvo.Clear();

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

                    // add active missiles to list
                    _currentMissileSalvo.Add(_missileToLaunch);

                    // change to event
                    _missileToLaunch.GetComponent<Missile>().AssignMissileRules(_missileType, _activeTarget.transform, _launchSpeed, _power, _fuseDelay, _destroyTime);
                    _missilePositions[i].SetActive(false);

                    // add inactive missilePositions to queue
                    _openPositionQueue.Enqueue(_missilePositions[i]);

                    _missileLaunchIndex++;

                    yield return new WaitForSeconds(_fireDelay); // cache and call to helper 
                }

                StartCoroutine(MissileReloadRoutine());
            }
        }

        private IEnumerator MissileReloadRoutine()
        {
            if (_openPositionQueue.Any(i => !i.activeInHierarchy))
            {
                for (int i = _missilePositions.Length - _openPositionQueue.Count(a => !a.activeInHierarchy); i < _missilePositions.Length; i++)
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

        private void OnMissileHitTarget(GameObject missile, GameObject target)
        {
            if (_currentMissileSalvo.Contains(missile))
            {
                Debug.Log("MISSILE HIT!");
                OnTurretAttack();
            }
        }
    }
}

