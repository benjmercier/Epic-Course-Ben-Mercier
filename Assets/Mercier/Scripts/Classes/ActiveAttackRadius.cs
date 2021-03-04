using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public class ActiveAttackRadius : MonoBehaviour
    {
        [SerializeField]
        private GameObject _rotationObj;
        [SerializeField]
        private float _rotationSpeed = 5f;
        [SerializeField]
        private Vector2 _minMaxX = new Vector2(-25f, 35f);
        private float _xClamped;
        [SerializeField]
        private Vector2 _minMaxY = new Vector2(-45f, 45f);
        private float _yClamped;
        
        //private Queue<GameObject> _attackQueue = new Queue<GameObject>();
        [SerializeField]
        private GameObject _activeTarget;
        private Vector3 _targetDirection;
        private Vector3 _rotateTowards;
        private Quaternion _lookRotation;
        private float _movement;

        private Quaternion _initialRotation;
        private Vector3 _initialPosition;
        [SerializeField]
        private List<GameObject> _attackList = new List<GameObject>();

        private bool _canFire = false;
        private bool _hasFired = false;

        public enum TurretState
        {
            Idle,
            Searching,
            Attacking,
            Destroyed
        }

        public TurretState currentState;

        void Start()
        {
            if (_rotationObj == null)
            {
                Debug.LogError("ActiveAttackRadius::Start()::" + gameObject.transform.parent.name + "'s _rotationObj is NULL.");
            }

            currentState = TurretState.Idle;

            _initialRotation = _rotationObj.transform.rotation;
            _initialPosition = _rotationObj.transform.position;
        }

        void Update()
        {
            RotateToTargetPos();
            //CheckTurretState();
        }

        private void CheckTurretState()
        {
            switch (currentState)
            {
                case TurretState.Idle:
                    break;

                case TurretState.Searching:
                    break;

                case TurretState.Attacking:
                    break;

                case TurretState.Destroyed:
                    break;

                default:
                    currentState = TurretState.Idle;
                    break;
            }
        }

        private void RotateToTargetPos()
        {
            if (_activeTarget != null)
            {
                _targetDirection = _activeTarget.transform.position - _rotationObj.transform.position;

                _movement = _rotationSpeed * Time.deltaTime;

                var temp = _targetDirection;

                float angle = Vector3.Angle(temp, _rotationObj.transform.forward);

                if (angle <= _minMaxY.y && angle >= _minMaxY.x)
                {
                    //currentState = TurretState.Attacking;
                    _canFire = true;

                    _rotateTowards = Vector3.RotateTowards(_rotationObj.transform.forward, _targetDirection, _movement, 0f);
                    _lookRotation = Quaternion.LookRotation(_rotateTowards);

                    _xClamped = Mathf.Clamp(_lookRotation.eulerAngles.x, _minMaxX.x, _minMaxX.y);
                    _yClamped = Mathf.Clamp(_lookRotation.eulerAngles.y, _minMaxY.x + 180, _minMaxY.y + 180);

                    _rotationObj.transform.rotation = Quaternion.Euler(_xClamped, _yClamped, _lookRotation.eulerAngles.z);

                    _hasFired = true;
                }
                else
                {
                    // remove from list
                    // if no other objects in list
                    // go to default pos
                    // else target that object
                    _canFire = false;

                    if (!_canFire && _hasFired)
                    {
                        _attackList.Remove(_activeTarget);

                        if (_attackList.Count <= 0)
                        {
                            _activeTarget = null;
                        }
                        else
                        {
                            AssignNewActiveTarget();
                        }

                        _hasFired = false;
                    }
                }
            }
            else
            {
                RotateToDefaultPos();
            }
        }

        private void RotateToDefaultPos()
        {
            //_targetDirection = _initialPosition - _rotationObj.transform.localPosition;

            //_rotateTowards = Vector3.RotateTowards(_rotationObj.transform.forward, _initialRotation.eulerAngles, _rotationSpeed * Time.deltaTime, 0f);
            //_lookRotation = Quaternion.LookRotation(_rotateTowards);
            //Debug.Log("Default Look Rotation: " + _lookRotation);
            //_rotationObj.transform.rotation = _lookRotation;

            _rotationObj.transform.rotation = Quaternion.Slerp(_rotationObj.transform.rotation, _initialRotation, _rotationSpeed * Time.deltaTime);
        }

        private void AssignNewActiveTarget()
        {

            _activeTarget = _attackList[_attackList.Count - 1];

            /*
            try
            {
                _activeTarget = _attackQueue.Peek();
            }
            catch
            {
                Debug.Log("None in queue.");
                _activeTarget = null;
            }*/
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // add to list
                _attackList.Add(other.gameObject);

                if (_attackList.Count <= 1)
                {
                    Debug.Log("Target acquired>");
                    AssignNewActiveTarget();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _attackList.Remove(_activeTarget);
            }
        }

        /*

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // add to queue
                _attackQueue.Enqueue(other.gameObject);

                if (_attackQueue.Count <= 1)
                {
                    //_activeTarget = _attackQueue.Peek();
                    AssignNewActiveTarget();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // remove from queue
                Debug.Log("Dequeued!");
                _attackQueue = new Queue<GameObject>(_attackQueue.Where(i => i != other.gameObject));
            }
        }*/
    }
}

