using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Managers;
using Mercier.Scripts.Classes.Abstract.Turret;
using Mercier.Scripts.Classes.Custom;

namespace Mercier.Scripts.Classes
{
    [SelectionBase]
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
        [SerializeField]
        protected Renderer[] _renderersInChildren;

        protected Vector3 _navTarget;

        [Header("Enemy Stats")]
        [SerializeField]
        protected Stats _health;
        public float Health { get { return _health.current; } set { _health.current = value; } }
        [SerializeField]
        protected Stats _armor;
        public float Armor { get { return _armor.current; } set { _armor.current = value; } }

        private float _delta;
        private float _zero = 0f;

        [Header("Rotation Settings")]
        [SerializeField]
        protected Rotation _primaryRotation;
        
        [SerializeField]
        protected Targeting _targeting;

        private float _defaultDissolveValue = 0f;
        private float _maxDissolveValue = 1f;
        private float _currentDissolveValue;

        private Color[] _fireColorArray =
        {
            new Color32(168, 6, 6, 255), // dark candy apple red
            new Color32(187, 0, 9, 255), // ue red
            new Color32(203, 12, 9, 255), // venetian red
            new Color32(216, 41, 28, 255), // maximum red
            new Color32(235, 58, 30, 255) // plochere's vermilion 
        };

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
            
            _renderersInChildren = GetComponentsInChildren<Renderer>();
        }

        public virtual void OnEnable()
        {
            _renderersInChildren.ToList().ForEach(m => m.material.SetFloat("_fillAmount", _defaultDissolveValue));
            
            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, false);

            Health = _health.max;
            Armor = _armor.max;

            _navMeshAgent.enabled = true;
            _navTarget = SpawnManager.Instance.AssignTargetPos();

            _navMeshAgent.SetDestination(_navTarget);
            _navMeshAgent.speed = UpdateSpeed(_speed);

            _primaryRotation.idleRotation = _primaryRotation.objTransform.localRotation;

            Turret.onTurretAttack += ReceiveDamage;
            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
        }

        public virtual void OnDisable()
        {
            Turret.onTurretAttack -= ReceiveDamage;
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
        }

        protected void Update()
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

        protected virtual float UpdateSpeed(float speed)
        {
            return speed;
        }

        // registered to AttackRadius OnTriggerEnter/Exit
        protected virtual void UpdateTargetList(GameObject obj, GameObject target, bool addToList)
        {
            if (this.gameObject == obj)
            {
                if (addToList)
                {
                    _targeting.targetList.Add(target);
                }
                else
                {
                    _targeting.targetList.Remove(target);
                }

                ManageActiveTarget(target);
            }
        }

        protected virtual void ManageActiveTarget(GameObject target)
        {
            if (_targeting.activeTarget != null && _targeting.activeTarget != target)
            {
                return;
            }

            SetActiveTarget();
        }

        protected virtual void SetActiveTarget()
        {
            if (_targeting.targetList.Any(t => ReturnTargetInLineOfSight(t.transform.position)))
            {
                _targeting.activeTarget = _targeting.targetList.Where(t => ReturnTargetInLineOfSight(t.transform.position)).
                    OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();
            }
            else
            {
                _targeting.activeTarget = null;
            }
        }

        protected virtual bool ReturnTargetInLineOfSight(Vector3 targetPos)
        {
            // check if any turret objects in front of enemy regardless of angle 
            //1 = directly in front, 0 = directly by side, -1 = directly behind (normalized)
            _targeting.targetVector = targetPos - this.transform.position;

            _targeting.dotAngle = Vector3.Dot(_targeting.targetVector.normalized, transform.forward);

            return _targeting.dotAngle > 0 ? true : false;
        }

        protected virtual void RotateToTarget(Vector3 targetPos)
        {
            // check if in line of sight
            // if yes, rotate
            // if no, set new target
            if (_targeting.activeTarget != null)
            {
                if (ReturnTargetInLineOfSight(targetPos))
                {
                    _primaryRotation.CalculateMovement();

                    _primaryRotation.CalculateDirection(targetPos);

                    _primaryRotation.lookRotation = Quaternion.LookRotation(_primaryRotation.targetDirection);

                    _primaryRotation.objTransform.rotation = Quaternion.Slerp(_primaryRotation.objTransform.rotation,
                        _primaryRotation.lookRotation, _primaryRotation.movement);
                }
                else
                {
                    SetActiveTarget();
                }
            }
        }

        protected virtual void RotateToStart()
        {
            if (_targeting.activeTarget == null)
            {
                _primaryRotation.CalculateMovement();

                _primaryRotation.objTransform.localRotation = Quaternion.Slerp(_primaryRotation.objTransform.localRotation,
                    _primaryRotation.idleRotation, _primaryRotation.movement);
            }
        }

        protected virtual void Attack(float attackStrength)
        {

        }

        private void ReceiveDamage(GameObject target, float damageAmount)
        {
            if (this.gameObject == target)
            {
                OnDamage(Health, Armor, damageAmount, out _health.current, out _armor.current);
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

                _delta = _health.max;
            }

            health -= (_delta / _health.max) * damageAmount;
            
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
            _navMeshAgent.enabled = false;
            _enemyAnim.SetBool(AnimationManager.Instance.IsFiringParam, false);
            _enemyAnim.SetBool(AnimationManager.Instance.IsDestroyedParam, true);

            StartCoroutine(DestroyedRoutine());
        }

        protected virtual IEnumerator DestroyedRoutine()
        {
            yield return _onDeathWait;

            StartCoroutine(MaterialDissolveRoutine(() =>
            {
                

                _enemyAnim.WriteDefaultValues();
                gameObject.SetActive(false);
            }));
        }    

        protected IEnumerator MaterialDissolveRoutine(Action onComplete = null)
        {
            while (_currentDissolveValue < _maxDissolveValue)
            {
                _currentDissolveValue += 0.0075f;

                _renderersInChildren.ToList().ForEach(m => m.material.SetColor("_edgeColor",
                    _fireColorArray[UnityEngine.Random.Range(0, _fireColorArray.Length)] * UnityEngine.Random.Range(-3f, 3f)));

                _renderersInChildren.ToList().ForEach(m => m.material.SetFloat("_fillAmount", _currentDissolveValue));

                yield return new WaitForEndOfFrame();
            }

            onComplete?.Invoke();
        }
    }
}