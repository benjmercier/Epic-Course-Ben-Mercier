using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mercier.Scripts.Managers;
using Mercier.Scripts.Classes.Abstract.Enemy.EnemyStates;
using Mercier.Scripts.Classes.Custom;
using Mercier.Scripts.Classes.Abstract.Turret;

namespace Mercier.Scripts.Classes.Abstract.Enemy
{
    public abstract class BaseEnemy : BaseBehavior<BaseEnemyState>
    {
        private BaseEnemyState _currentEnemyState;
        public BaseEnemyState CurrentEnemyState { get { return _currentEnemyState; } }
        public readonly EnemyIdleState enemyIdleState = new EnemyIdleState();
        public readonly EnemyCoolDownState enemyCoolDownState = new EnemyCoolDownState();
        public readonly EnemyAttackingState enemyAttackingState = new EnemyAttackingState();
        public readonly EnemyDestroyedState enemyDestroyedState = new EnemyDestroyedState();

        [Space, SerializeField]
        protected EnemyStats _enemyStats;
        public EnemyStats EnemyStats { get { return _enemyStats; } }

        [Header("Enemy Components")]
        [SerializeField]
        protected NavMeshAgent _navMeshAgent;
        [SerializeField]
        protected Animator _enemyAnim;
        public Animator EnemyAnim { get { return _enemyAnim; } }
        [SerializeField]
        protected GameObject _rotationTarget;

        protected Vector3 _navTarget;

        public static event Action onDamage;
        public static event Action<GameObject, int> onEnemyDestroyed;

        protected override void Awake()
        {
            base.Awake();

            if (_navMeshAgent == null)
            {
                Debug.LogError("Enemy::Awake()::" + gameObject.name + "'s _navMeshAgent is NULL.");
            }

            if (_enemyAnim == null)
            {
                Debug.LogError("Enemy::Awake()::" + gameObject.name + "'s _enemyAnimis NULL.");
            }
            
            _renderersInChildren = GetComponentsInChildren<Renderer>();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _enemyStats.currentHealth = _enemyStats.maxHealth;
            _enemyStats.currentArmor = _enemyStats.maxArmor;

            TransitionToState(enemyIdleState);

            _renderersInChildren.ToList().ForEach(m => m.material.SetFloat("_fillAmount", _defaultDissolveValue));

            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, false);

            _navMeshAgent.enabled = true;
            _navTarget = SpawnManager.Instance.AssignTargetPos();

            _navMeshAgent.SetDestination(_navTarget);
            _navMeshAgent.speed = AssignSpeed(_enemyStats.speed);

            //BaseTurret.onTurretAttack += ReceiveDamage;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            //BaseTurret.onTurretAttack -= ReceiveDamage;
        }

        protected override void Update()
        {
            _currentEnemyState.Update(this);
            Debug.Log("CurrentState: " + _currentEnemyState);
        }

        protected override void LateUpdate()
        {
            _currentEnemyState.LateUpdate(this);
        }

        public override void TransitionToState(BaseEnemyState state)
        {
            _currentEnemyState = state;
            _currentEnemyState.EnterState(this);
        }

        protected virtual float AssignSpeed(float speed)
        {
            return speed;
        }

        public override void ReceiveDamage(GameObject damagedObj, float damageAmount)
        {
            if (this.gameObject == damagedObj)
            {
                OnDamageReceived(_enemyStats.currentHealth, _enemyStats.currentArmor, damageAmount, out _enemyStats.currentHealth, out _enemyStats.currentArmor);
            }
        }

        public override void OnDamageReceived(float health, float armor, float damageAmount, out float curHealth, out float curArmor)
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

                _delta = _enemyStats.maxHealth;
            }

            health -= (_delta / _enemyStats.maxHealth) * damageAmount;

            curHealth = health;

            if (curHealth <= 0)
            {
                curHealth = 0;

                OnObjDestroyed(_rotationTarget, _enemyStats.reward);
            }
        }

        protected override void OnObjDestroyed(GameObject objDestroyed, int currency)
        {
            onEnemyDestroyed?.Invoke(objDestroyed, currency);

            _navMeshAgent.isStopped = true;
            //_navMeshAgent.enabled = false;
            _enemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, false);
            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, true);

            StartCoroutine(DestroyedRoutine());
        }

        protected override IEnumerator DestroyedRoutine()
        {
            yield return _onDeathWait;

            StartCoroutine(MaterialDissolveRoutine(() =>
            {
                _enemyAnim.WriteDefaultValues();
                gameObject.SetActive(false);
            }));
        }
    }
}

