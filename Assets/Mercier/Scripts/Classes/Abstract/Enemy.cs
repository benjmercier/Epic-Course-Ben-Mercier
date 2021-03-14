using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Managers;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Classes
{
    // can add [RequireTypeOf()] to require specific component
    public abstract class Enemy : MonoBehaviour, IDamageable<float>, IEventable
    {
        [Header("Enemy Settings")]
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private Animator _enemyAnim;  
        [SerializeField]
        protected float _speed;      
        [SerializeField]
        protected int _currencyToReward;

        [Header("Health")]
        [SerializeField]
        protected float _maxHealth;
        [SerializeField]
        private float _currentHealth;
        public float Health { get { return _currentHealth; } set { _currentHealth = value; } }

        [Header("Armor")]
        [SerializeField]
        protected float _maxArmor;
        [SerializeField]
        private float _currentArmor;
        public float Armor { get { return _currentArmor; } set { _currentArmor = value; } }

        [Header("Attack Settings")]
        [SerializeField]
        protected List<GameObject> _activeTargetList = new List<GameObject>();
        [SerializeField]
        protected GameObject _activeTarget;
        public GameObject ActiveTarget { get { return _activeTarget; } }
        //[SerializeField]
        //protected GameObject _rotationTarget;
        //public GameObject RotationTarget { get { return _rotationTarget; } }
        [SerializeField]
        protected float _attackStrength = 10f;
        public float AttackStrength { get { return _attackStrength; } }

        protected Vector3 _targetVector;
        protected float _dotAngle;
        protected float _targetAngle;

        [Header("Rotation Settings")]
        [SerializeField]
        protected Transform _primaryRotationObj;
        [SerializeField]
        protected float _primaryRotationSpeed = 2.5f;
        [SerializeField]
        protected Vector2 _maxRotationAngle = new Vector2(90f, 90f);
        [SerializeField]
        protected Vector2 _minRotationAngle = new Vector2(-90f, -90f);

        protected Vector3 _primaryTargetDirection;
        protected float _primaryMovement;

        protected Quaternion _primaryInitialRotation;
        public Quaternion PrimaryInitialRotation { get { return _primaryInitialRotation; } }
        protected Vector3 _primaryRotateTowards;
        protected Quaternion _primaryLookRotation;

        protected float _xAngleCheck;
        protected float _yAngleCheck;

        protected float _xClamped;
        protected float _yClamped;

        protected Vector3 _navTarget;

        private float _delta;
        private float _zero = 0f;


        protected WaitForSeconds _onDeathWait = new WaitForSeconds(3.5f);

        public static event Action<GameObject, int> onEnemyDeath;        

        protected void Awake()
        {
            if (_navMeshAgent == null)
            {
                Debug.LogError("Enemy::Awake()::" + gameObject.name + "'s _navMeshAgent is NULL.");
            }

            if (_enemyAnim == null)
            {
                Debug.LogError("Enemy::Awake()::" + gameObject.name + "'s _enemyAnimis NULL.");
            }
        }

        public virtual void OnEnable()
        {
            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, false);

            _primaryInitialRotation = transform.localRotation;

            Health = _maxHealth;
            Armor = _maxArmor;

            _navTarget = SpawnManager.Instance.AssignTargetPos();

            _navMeshAgent.SetDestination(_navTarget);
            _navMeshAgent.speed = UpdateSpeed(_speed);

            Turret.onTurretAttack += ReceiveDamage;
            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
        }

        public virtual void OnDisable()
        {
            Turret.onTurretAttack -= ReceiveDamage;
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
        }

        protected void Start()
        {
            
        }

        protected void Update()
        {
            if (_activeTarget != null)
            {
                _enemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, true);
                RotateToTarget(_activeTarget.transform.position);
            }
            else
            {
                _enemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, false);
                RotateToStart();
            }
            
        }

        protected virtual float UpdateSpeed(float speed)
        {
            return speed;
        }

        // registered to AttackRadius OnTriggerEnter/Exit
        protected virtual void UpdateTargetList(GameObject enemy, GameObject target, bool addToList)
        {
            if (this.gameObject == enemy)
            {
                if (addToList)
                {
                    _activeTargetList.Add(target);
                }
                else
                {
                    _activeTargetList.Remove(target);
                }

                ManageActiveTarget(target);
            }
        }

        protected virtual void ManageActiveTarget(GameObject target)
        {
            if (_activeTarget != null && _activeTarget != target)
            {
                return;
            }

            SetActiveTarget();
        }

        protected virtual void SetActiveTarget()
        {
            if (_activeTargetList.Any(t => ReturnTargetInLineOfSight(t.transform.position)))
            {
                _activeTarget = _activeTargetList.Where(t => ReturnTargetInLineOfSight(t.transform.position)).
                    OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();
            }
            else
            {
                _activeTarget = null;
            }
        }

        protected virtual bool ReturnTargetInLineOfSight(Vector3 targetPos)
        {
            // check if any turret objects in front of enemy regardless of angle 1 = directly in front, 0 = directly by side, -1 = directly behind (normalized)
            _targetVector = targetPos - this.transform.position;

            _dotAngle = Vector3.Dot(_targetVector.normalized, transform.forward);

            return _dotAngle > 0 ? true : false;
        }

        protected virtual void RotateToTarget(Vector3 targetPos)
        {
            // check if in line of sight
            // if yes, rotate
            // if no, set new target
            if (_activeTarget != null)
            {
                if (ReturnTargetInLineOfSight(targetPos))
                {
                    _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

                    _primaryTargetDirection = targetPos - _primaryRotationObj.transform.position;
                    _primaryLookRotation = Quaternion.LookRotation(_primaryTargetDirection);

                    _primaryRotationObj.rotation = Quaternion.Slerp(_primaryRotationObj.rotation, _primaryLookRotation, _primaryMovement);
                }
                else
                {
                    SetActiveTarget();
                }
            }
        }

        protected virtual void RotateToStart()
        {
            if (_activeTarget == null)
            {
                _primaryMovement = _primaryRotationSpeed * Time.deltaTime;

                _primaryRotationObj.localRotation = Quaternion.Slerp(_primaryRotationObj.localRotation, _primaryInitialRotation, _primaryMovement);
            }
        }

        protected virtual void Attack(float attackStrength)
        {

        }

        private void ReceiveDamage(GameObject target, float damageAmount)
        {
            if (this.gameObject == target)
            {
                OnDamage(Health, Armor, damageAmount, out _currentHealth, out _currentArmor);
            }
        }
        
        public virtual void OnDamage(float health, float armor, float damageAmount, out float curHealth, out float curArmor)
        {
            if (armor > _zero)
            {
                armor -= damageAmount;

                _delta = health - armor;
                
                if (armor < _zero)
                {
                    armor = _zero;
                }

                curArmor = armor;
            }
            else
            {
                if (armor != _zero)
                {
                    armor = _zero;
                }

                curArmor = armor;

                _delta = _maxHealth;
            }

            health -= (_delta / _maxHealth) * damageAmount;
            
            curHealth = health;

            if (curHealth <= 0)
            {
                curHealth = 0;

                OnDeath(this.gameObject, _currencyToReward);
            }
        }

        protected virtual void OnDeath(GameObject enemy, int reward)
        {
            // add reward to player currency

            onEnemyDeath?.Invoke(enemy, reward);
            _navMeshAgent.isStopped = true;
            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, true);

            StartCoroutine(DestroyedRoutine());
        }

        protected virtual IEnumerator DestroyedRoutine()
        {
            yield return _onDeathWait;

            _enemyAnim.WriteDefaultValues();
            gameObject.SetActive(false);
        }    
    }
}