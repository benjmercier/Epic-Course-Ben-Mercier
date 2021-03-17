using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mercier.Scripts.Managers;

namespace Mercier.Scripts.Classes.Abstract
{
    public abstract class BaseEnemy : SharedBehaviors
    {
        [Header("Enemy Settings")]
        [SerializeField]
        protected NavMeshAgent _navMeshAgent;
        [SerializeField]
        protected Animator _enemyAnim;
        [SerializeField]
        protected float _speed;
        [SerializeField]
        protected int _currencyToReward;

        protected Vector3 _navTarget;

        public static event Action<GameObject, int> onEnemyDeath;

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

            _renderersInChildren.ToList().ForEach(m => m.material.SetFloat("_fillAmount", _defaultDissolveValue));

            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, false);

            _navMeshAgent.enabled = true;
            _navTarget = SpawnManager.Instance.AssignTargetPos();

            _navMeshAgent.SetDestination(_navTarget);
            _navMeshAgent.speed = AssignSpeed(_speed);
        }

        protected override void Update()
        {
            if (_targeting.activeTarget != null)
            {
                _enemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, true);
                RotateToTarget(_targeting.activeTarget.transform.position);
            }
            else
            {
                _enemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, false);
                RotateToStart();
            }
        }

        protected virtual float AssignSpeed(float speed)
        {
            return speed;
        }

        protected override void OnObjDestroyed(GameObject objDestroyed, int currency)
        {
            // add reward to player currency

            base.OnObjDestroyed(objDestroyed, currency);
            //onEnemyDeath?.Invoke(objDestroyed, currency);

            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
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

