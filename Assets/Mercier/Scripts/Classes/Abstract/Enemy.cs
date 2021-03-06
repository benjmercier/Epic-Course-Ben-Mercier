using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes
{
    // can add [RequireTypeOf()] to require specific component
    public abstract class Enemy : MonoBehaviour, IDamageable<float>, IEventable
    {
        public int iD;

        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        private Vector3 _target;

        [SerializeField]
        protected float _speed;
        [SerializeField]
        protected float _maxHealth;
        [SerializeField]
        protected float _attackStrength;
        [SerializeField]
        protected float _maxArmor;
        [SerializeField]
        protected int _currencyToReward;

        private float _delta;
        private float _zero = 0f;

        [SerializeField]
        private float _currentHealth;
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

        protected void Awake()
        {
            Health = _maxHealth;
            Armor = _maxArmor;

            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }
        }

        public void OnEnable()
        {
            _target = SpawnManager.Instance.AssignTargetPos();

            _navMeshAgent.SetDestination(_target);

            _navMeshAgent.speed = UpdateSpeed(_speed);

            Turret.onTurretAttack += ReceiveDamage;
        }

        public void OnDisable()
        {
            Turret.onTurretAttack -= ReceiveDamage;
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
                Debug.Log("Current Armor: " + _currentArmor);
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
            Debug.Log("Current Health: " + _currentHealth);
            
        }

        protected virtual void OnDeath(int reward)
        {
            // add reward to player currency

            Destroy(this.gameObject);
        }
    }
}


