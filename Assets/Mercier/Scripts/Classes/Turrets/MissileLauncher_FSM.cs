using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Classes.Turrets
{
    public class MissileLauncher_FSM : FullTurretRotation
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

            Missile.onTargetEnemyHit += OnMissileHitTarget;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            Missile.onTargetEnemyHit -= OnMissileHitTarget;
        }

        private IEnumerator MissileLaunchRoutine()
        {
            if (_missileLaunchIndex < _missilePositions.Length)
            {
                _currentMissileSalvo.Clear();

                for (int i = _missileLaunchIndex; i < _missilePositions.Length; i++)
                {
                    if (_targeting.activeTarget == null)
                    {
                        StartCoroutine(MissileReloadRoutine());

                        break;
                    }

                    _missileToLaunch = Instantiate(_missilePrefab, _missilePositions[i].transform);
                    _missileToLaunch.transform.localPosition = Vector3.zero;
                    _missileToLaunch.transform.localEulerAngles = Vector3.zero;
                    _missileToLaunch.transform.parent = null;

                    // add active missiles to list
                    _currentMissileSalvo.Add(_missileToLaunch);

                    // change to event
                    _missileToLaunch.GetComponent<Missile>().AssignMissileRules
                        (_missileType, _targeting.activeTarget.transform, _launchSpeed, _power, _fuseDelay, _destroyTime);

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

                    if (_openPositionQueue.Count < 1)
                    {
                        _launched = false;

                        break;
                    }

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
                //Debug.Log("MISSILE HIT!");
                OnTurretAttack();
            }
        }

        protected override void EngageTarget()
        {
            
        }

        protected override void DisengageTarget()
        {
            
        }

        public override void TurretAttack()
        {
            Debug.Log("TurretAttack()");
            if (IsLookingAtTarget())
            {
                Debug.Log("IsLookingAtTarget()");
                if (_launched == false)
                {
                    _launched = true;

                    StartCoroutine(MissileLaunchRoutine());
                }
            }
        }

        protected override IEnumerator DestroyedRoutine()
        {
            throw new System.NotImplementedException();
        }
    }
}

