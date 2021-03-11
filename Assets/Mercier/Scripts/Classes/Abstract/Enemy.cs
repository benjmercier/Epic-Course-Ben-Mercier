using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Managers;
using Mercier.Scripts.AnimBehaviors;

namespace Mercier.Scripts.Classes
{
    // can add [RequireTypeOf()] to require specific component
    public abstract class Enemy : MonoBehaviour, IDamageable<float>, IEventable
    {
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private Animator _enemyAnim;
        private Vector3 _target;

        [SerializeField]
        protected float _speed;        
        [SerializeField]
        protected float _attackStrength;        
        [SerializeField]
        protected int _currencyToReward;

        private float _delta;
        private float _zero = 0f;

        [Header("Health")]
        [SerializeField]
        protected float _maxHealth;
        [SerializeField]
        private float _currentHealth;
        [Header("Armor")]
        [SerializeField]
        protected float _maxArmor;
        [SerializeField]
        private float _currentArmor;

        public float Health
        {
            get
            {
                return _currentHealth;
            }
            set
            {
                _currentHealth = value;
            }
        }
                
        public float Armor
        {
            get
            {
                return _currentArmor;
            }
            set
            {
                _currentArmor = value;
            }
        }

        public static event Action<GameObject, int> onEnemyDeath;
        private WaitForSeconds _onDeathWait = new WaitForSeconds(3.5f);

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

        public void OnEnable()
        {
            _enemyAnim.SetBool("isDead", false);

            Health = _maxHealth;
            Armor = _maxArmor;

            _target = SpawnManager.Instance.AssignTargetPos();

            _navMeshAgent.SetDestination(_target);

            _navMeshAgent.speed = UpdateSpeed(_speed);

            TurretOld.onTurretAttack += ReceiveDamage;
            OnDeathBehavior.onDeathAnimStateExit += Die;
        }

        public void OnDisable()
        {
            TurretOld.onTurretAttack -= ReceiveDamage;
            OnDeathBehavior.onDeathAnimStateExit -= Die;
        }

        protected void Start()
        {
            
        }

        protected virtual float UpdateSpeed(float speed)
        {
            return speed;
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
            _enemyAnim.SetBool("isDead", true);

            StartCoroutine(DieRoutine());
        }

        protected virtual IEnumerator DieRoutine()
        {
            yield return _onDeathWait;

            _enemyAnim.WriteDefaultValues();
            gameObject.SetActive(false);
        }

        // Mech2 would transition to standing up even if using OnStateExit() so used coroutine
        protected virtual void Die(GameObject enemy) 
        {
            if (this.gameObject == enemy)
            {
                //_enemyAnim.WriteDefaultValues();
                //gameObject.SetActive(false);
            }
        }        
    }
}