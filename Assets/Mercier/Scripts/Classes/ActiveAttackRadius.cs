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
        
        private Queue<GameObject> _attackQueue = new Queue<GameObject>();
        private GameObject _activeTarget;
        private Vector3 _targetDirection;
        private Vector3 _rotateTowards;
        private Quaternion _lookRotation;
        private float _movement;

        private Quaternion _initialRotation;

        void Start()
        {
            if (_rotationObj == null)
            {
                Debug.LogError("ActiveAttackRadius::Start()::" + gameObject.transform.parent.name + "'s _rotationObj is NULL.");
            }

            _initialRotation = _rotationObj.transform.rotation;
        }

        void Update()
        {
            RotateToTarget();
        }

        private void RotateToTarget()
        {
            if (_activeTarget != null)
            {
                _targetDirection = _activeTarget.transform.position - _rotationObj.transform.position;

                _movement = _rotationSpeed * Time.deltaTime;

                _rotateTowards = Vector3.RotateTowards(_rotationObj.transform.forward, _targetDirection, _movement, 0f);
                _lookRotation = Quaternion.LookRotation(_rotateTowards);
                Debug.Log("lookRotation: " + _lookRotation.eulerAngles.y);
                _xClamped = Mathf.Clamp(_lookRotation.eulerAngles.x, _minMaxX.x, _minMaxX.y);
                _yClamped = Mathf.Clamp(_lookRotation.eulerAngles.y, _minMaxY.x + 180, _minMaxY.y + 180);

                float angle = Vector3.Angle(_targetDirection, _rotationObj.transform.forward);
                if (angle <= _minMaxY.y && angle >= _minMaxY.x)
                {
                    _rotationObj.transform.rotation = Quaternion.Euler(_xClamped, _yClamped, _lookRotation.eulerAngles.z);
                }
                else
                {
                    _rotationObj.transform.rotation = Quaternion.Slerp(_rotationObj.transform.rotation, _initialRotation, _movement);
                }

                
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // add to queue
                _attackQueue.Enqueue(other.gameObject);

                if (_attackQueue.Count <= 1)
                {
                    _activeTarget = _attackQueue.Peek();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // remove from queue
                try
                {
                    _attackQueue = new Queue<GameObject>(_attackQueue.Where(i => i != other.gameObject));
                }
                catch
                {
                    Debug.Log("ActiveAttackRadius::OnTriggerExit()::" + gameObject.transform.parent.name + "'s _attackQueue unable to remove " + other.gameObject.name);
                }
            }
        }
    }
}

