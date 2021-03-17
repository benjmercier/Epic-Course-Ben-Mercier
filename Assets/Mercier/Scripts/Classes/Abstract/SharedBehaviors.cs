using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mercier.Scripts.Interfaces;
using Mercier.Scripts.Classes.Custom;
using Mercier.Scripts.PropertyAttributes;

namespace Mercier.Scripts.Classes.Abstract
{
    [SelectionBase]
    public abstract class SharedBehaviors : MonoBehaviour, IEventable, IDamageable<float>
    {
        #region Stats
        [Header("Stats")]
        [SerializeField]
        protected Stats _health;
        public float Health { get { return _health.current; } set { _health.current = value; } }
        [SerializeField]
        protected Stats _armor;
        public float Armor { get { return _armor.current; } set { _armor.current = value; } }

        [SerializeField]
        protected float _attackStrength = 10f;

        [SerializeField]
        protected int _currencyIfDestroyed;

        protected float _delta;
        protected float _zero = 0f;
        #endregion

        #region Rotation
        [Header("Rotation Settings")]
        [SerializeField]
        protected Rotation _primaryRotation;
        protected Rotation _auxiliaryRotation;

        protected List<Rotation> _rotationList = new List<Rotation>();
        #endregion

        #region Targeting
        [SerializeField]
        protected Targeting _targeting;
        public GameObject ActiveTarget { get { return _targeting.activeTarget; } }
        public GameObject RotationTarget { get { return _targeting.rotationTarget; } }
        #endregion

        #region DissolveShader
        [ReadOnly]
        protected Renderer[] _renderersInChildren;

        protected float _defaultDissolveValue = 0f;
        protected float _maxDissolveValue = 1f;
        protected float _currentDissolveValue;

        protected Color[] _fireColorArray =
        {
            new Color32(168, 6, 6, 255), // dark candy apple red
            new Color32(187, 0, 9, 255), // ue red
            new Color32(203, 12, 9, 255), // venetian red
            new Color32(216, 41, 28, 255), // maximum red
            new Color32(235, 58, 30, 255) // plochere's vermilion 
        };
        #endregion

        protected WaitForSeconds _onDeathWait = new WaitForSeconds(3.5f);

        #region Events
        public static event Action<GameObject> onCheckForRotationTarget;
        public static event Action<GameObject, int> onObjDestroyed;
        #endregion

        protected virtual void Awake()
        {
            if (_primaryRotation.objTransform != null)
            {
                _rotationList.Add(_primaryRotation);
            }
            else
            {
                Debug.LogError("SharedBehavior::Awake()::_primaryRotation is NULL.");
            }
            

            if (_auxiliaryRotation.objTransform != null)
            {
                _rotationList.Add(_auxiliaryRotation);
            }
            else
            {
                Debug.LogError("SharedBehavior::Awake()::_auxiliaryRotation is NULL and will not be calculated");
            }
        }

        public virtual void OnEnable()
        {
            Health = _health.max;
            Armor = _armor.max;

            for (int i = 0; i < _rotationList.Count; i++)
            {
                _rotationList[i].idleRotation = _rotationList[i].objTransform.localRotation;
            }

            AttackRadius.onAttackRadiusTriggered += UpdateTargetList;
            Classes.RotationTarget.onConfirmRotationTarget += AssignRotationTarget;
        }

        public virtual void OnDisable()
        {
            AttackRadius.onAttackRadiusTriggered -= UpdateTargetList;
            Classes.RotationTarget.onConfirmRotationTarget -= AssignRotationTarget;
        }

        protected virtual void Update()
        {

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

        // register to onDestroyed event
        protected virtual void AssignNewTarget(GameObject activeTarget, int currency)
        {
            if (_targeting.activeTarget == activeTarget)
            {
                _targeting.targetList.Remove(activeTarget);

                //_targeting.activeTarget = ReturnActiveTarget();
                SetActiveTarget();
            }
            else
            {
                if (_targeting.targetList.Contains(activeTarget))
                {
                    _targeting.targetList.Remove(activeTarget);
                }
            }
        }

        protected virtual void SetActiveTarget()
        {
            if (_targeting.targetList.Any(t => ReturnTargetInLineOfSight(t.transform.position)))
            {
                _targeting.activeTarget = _targeting.targetList.Where(t => ReturnTargetInLineOfSight(t.transform.position)).
                    OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).FirstOrDefault();

                OnCheckForRotationTarget(_targeting.activeTarget);
            }
            else
            {
                _targeting.activeTarget = null;
                _targeting.rotationTarget = null;
            }
        }

        protected virtual bool ReturnTargetInLineOfSight(Vector3 targetPos)
        {
            var targetVector = targetPos - this.transform.position;

            var dotAngle = Vector3.Dot(targetVector.normalized, transform.forward);

            return dotAngle > 0 ? true : false; 
            // change for missile / full rotation = return true
            // also check with edge towers
        }

        // cast event to RotationTarget
        protected virtual void OnCheckForRotationTarget(GameObject activeTarget)
        {
            onCheckForRotationTarget?.Invoke(activeTarget);
        }

        // registered to RotationTarget
        protected virtual void AssignRotationTarget(GameObject activeTarget, GameObject rotationTarget)
        {
            if (_targeting.activeTarget == activeTarget)
            {
                _targeting.rotationTarget = rotationTarget;
            }
        }

        public virtual void RotateToTarget(Vector3 targetPos)
        {
            // from Enemy
            if (_targeting.activeTarget != null)
            {
                if (ReturnTargetInLineOfSight(targetPos))
                {
                    _primaryRotation.movement = _primaryRotation.speed * Time.deltaTime;
                    _primaryRotation.targetDirection = targetPos - _primaryRotation.objTransform.position;
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

        public virtual void RotateToStart()
        {
            // from Enemy
            if (_targeting.activeTarget == null)
            {
                _primaryRotation.CalculateMovement();

                _primaryRotation.objTransform.localRotation = Quaternion.Slerp(_primaryRotation.objTransform.localRotation,
                    _primaryRotation.idleRotation, _primaryRotation.movement);
            }
        }

        public virtual bool IsAtStart()
        {
            _primaryRotation.angleToIdle = Quaternion.Angle(_primaryRotation.objTransform.rotation, _primaryRotation.idleRotation);
            _auxiliaryRotation.angleToIdle = Quaternion.Angle(_auxiliaryRotation.objTransform.rotation, _auxiliaryRotation.idleRotation);

            return _primaryRotation.angleToIdle < _primaryRotation.maxIdleOffset && _auxiliaryRotation.angleToIdle < _auxiliaryRotation.maxIdleOffset;
        }

        protected virtual Quaternion RestrictLocalEulerAngleY(Vector3 localEulerAngles)
        {
            return Quaternion.Euler(localEulerAngles.x, 0, localEulerAngles.z);
        }

        protected virtual bool IsFacingTarget(Rotation rotationObj)
        {
            if (_targeting.rotationTarget != null)
            {
                _targeting.targetVector = _targeting.rotationTarget.transform.position - rotationObj.objTransform.position;

                _targeting.targetAngle = Vector3.Angle(rotationObj.objTransform.forward, _targeting.targetVector);

                return _targeting.targetAngle < rotationObj.maxLookOffset;
            }

            return false;
        }

        protected virtual void ReceiveDamage(GameObject damagedObj, float damageAmount)
        {
            if (this.gameObject == damagedObj)
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

                OnObjDestroyed(this.gameObject, _currencyIfDestroyed);
            }
        }

        protected virtual void OnObjDestroyed(GameObject objDestroyed, int currency)
        {
            onObjDestroyed?.Invoke(objDestroyed, currency);
        }

        protected abstract IEnumerator DestroyedRoutine();

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

