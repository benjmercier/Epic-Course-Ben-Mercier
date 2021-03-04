using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mercier.Scripts.Classes
{
    public class TurretAttackRadius : MonoBehaviour
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

        [SerializeField]
        private float _maxAngle = 45f;
        private float _cosAngle;
        private float _angle;

        [SerializeField]
        private List<GameObject> _attackList = new List<GameObject>();
        [SerializeField]
        private GameObject _activeTarget;

        private Quaternion _initialRotation;
        private Vector3 _castDirection;
        private Vector3 _targetDirection;
        private Vector3 _rotateTowards;
        private Quaternion _lookRotation;
        private float _movement;

        private bool _hasFired = false;

        // Start is called before the first frame update
        void Start()
        {
            _initialRotation = _rotationObj.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            RotateToTargetPos();
        }

        private bool CastLineOfSight()
        {
            _castDirection = _activeTarget.transform.position - _rotationObj.transform.position;

            _cosAngle = Vector3.Dot(_castDirection.normalized, _rotationObj.transform.forward);

            _angle = Mathf.Acos(_cosAngle) * Mathf.Rad2Deg;

            return _angle <= _maxAngle;
        }

        private void RotateToTargetPos()
        {
            if (_activeTarget != null)
            {
                if (CastLineOfSight())
                {
                    _hasFired = true;

                    _targetDirection = _activeTarget.transform.position - _rotationObj.transform.position;

                    _movement = _rotationSpeed * Time.deltaTime;

                    _rotateTowards = Vector3.RotateTowards(_rotationObj.transform.forward, _targetDirection, _movement, 0f);
                    _lookRotation = Quaternion.LookRotation(_rotateTowards);

                    _xClamped = Mathf.Clamp(_lookRotation.eulerAngles.x, _minMaxX.x, _minMaxX.y);
                    _yClamped = Mathf.Clamp(_lookRotation.eulerAngles.y, _minMaxY.x + 180, _minMaxY.y + 180);

                    _rotationObj.transform.rotation = Quaternion.Euler(_xClamped, _yClamped, _lookRotation.eulerAngles.z);
                }
                else
                {
                    if (_hasFired)
                    {
                        _hasFired = false;

                        if (_attackList.Contains(_activeTarget))
                        {
                            _attackList.Remove(_activeTarget);
                            _activeTarget = null;
                            _activeTarget = _attackList.FirstOrDefault();
                        }
                    }
                    RotateToDefaultPos();
                }
            }
            else
            {
                RotateToDefaultPos();
            }
        }

        private void RotateToDefaultPos()
        {
            _movement = _rotationSpeed * Time.deltaTime;

            _rotationObj.transform.rotation = Quaternion.Slerp(_rotationObj.transform.rotation, _initialRotation, _movement);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _attackList.Add(other.gameObject);

                if (_attackList.Count <= 1)
                {
                    _activeTarget = _attackList.FirstOrDefault();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                _attackList.Remove(other.gameObject);
            }
        }
    }
}

