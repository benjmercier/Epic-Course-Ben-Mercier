using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Mercier.Scripts.Managers;
using Mercier.InputActions;
using Mercier.Scripts.Classes;

namespace Mercier.Scripts
{
    public class CameraController : MonoSingleton<CameraController>,/* IEventable,*/ CameraInputActions.ICameraActions
    {
        private CameraInputActions _cameraIA;

        private List<ActiveAction<InputAction>> _activeInputActions = new List<ActiveAction<InputAction>>();
        private InputAction _moveAction;
        private InputAction _zoomAction;
        private InputAction _rotateAction;
        private InputAction _toggleRotateAction;

        private Vector2 _moveInput, 
            _zoomInput, 
            _rotationInput;

        [Header("Camera Settings")]
        [SerializeField]
        private Camera _camera;
        private Vector3 _cameraTargetPos;
        [SerializeField]
        private float _lookOffset;
        [SerializeField]
        private float _angle;
        [SerializeField]
        private Vector2 _minMaxXPos = new Vector2(-38f, -5f);
        [SerializeField]
        private Vector2 _minMaxYPos = new Vector2(-7f, 20f);
        private Vector3 _levelMidpoint;
        private float _xMidpoint,
            _yMidpoint;
        private Vector3 _moveDirection,
            _moveTarget;
        [SerializeField]
        private float _moveSpeed = 5f;
        private float _targetSpeed = 10f;

        private Vector3 _defaultPosition = new Vector3(-25f, 30f, -25f);

        [Header("Zoom Settings")]
        [SerializeField]
        private float _defaultZoom;
        [SerializeField]
        private float _maxZoomIn = 2f;
        [SerializeField]
        private float _maxZoomOut = 40f;
        private float _currentZoom;
        public float CurrentZoom
        {
            get => _currentZoom;
            private set
            {
                _currentZoom = value;
                CalculateCameraTarget();
            }
        }
        [SerializeField]
        private float _zoomSpeed = 10f;

        [Header("Rotation Settings")]
        [SerializeField]
        private float _rotationSpeed = 5f;
        private float _rotationSlerp = 4f;
        private bool _canRotate = false;
        private Quaternion _rotationTarget;

        [Space(15)]
        [SerializeField]
        private float _screenBorderPercent = 10f;

        /*
        public void OnEnable()
        {
            if (_cameraIA == null)
            {
                _cameraIA = new CameraInputActions();
                _cameraIA.Camera.SetCallbacks(this);
            }

            _cameraIA.Camera.Enable();

            _activeInputActions.AddRange(new List<ActiveAction<InputAction>>
            {
                new ActiveAction<InputAction>(_moveAction, _cameraIA.Camera.Move),
                new ActiveAction<InputAction>(_zoomAction, _cameraIA.Camera.Zoom),
                new ActiveAction<InputAction>(_rotateAction, _cameraIA.Camera.Rotate),
                new ActiveAction<InputAction>(_toggleRotateAction, _cameraIA.Camera.ToggleRotate)
            });

            _activeInputActions.ToList().ForEach(a => a.ReturnActiveAction().Enable()); // .ToList() is only there for LINQ, is it better to just not use it?
        }

        public void OnDisable()
        {
            _activeInputActions.ToList().ForEach(a => a.ReturnActiveAction().Disable());

            _cameraIA.Camera.Disable();
        }*/

        private void Start()
        {
            if (_camera == null)
            {
                Debug.LogError("CameraController::Start()::Camera is NULL.");
            }

            _xMidpoint = (_minMaxXPos.x + _minMaxXPos.y) / 2;
            _yMidpoint = (_minMaxYPos.x + _minMaxYPos.y) / 2;
            _levelMidpoint = new Vector3(_xMidpoint, 0f, _yMidpoint);
            _moveTarget = _levelMidpoint;

            _camera.transform.rotation = Quaternion.AngleAxis(_angle, Vector3.right);

            CurrentZoom = _defaultZoom;

            _rotationTarget = transform.rotation;
        }

        private void FixedUpdate()
        {
            CalculateMovement(_moveDirection);
            CalculateZoom(_zoomInput);
        }

        private void LateUpdate()
        {
            CalculateRotation(_rotationInput); // moved to LateUpdate() to prevent unnecessary calls
        }

        private void CalculateCameraTarget()
        {
            //sets camera position based on look offset, angle, and zoom
            _cameraTargetPos = (Vector3.up * _lookOffset) + (Quaternion.AngleAxis(_angle, Vector3.right) * Vector3.back) * _currentZoom;
        }

        public void OnMove(InputAction.CallbackContext context) // if using Invoke Unity Events on Player Input component, do we need to worry about unsubscribing?
        {
            _moveInput = context.ReadValue<Vector2>();
            //Debug.Log("MoveInput: " + _moveInput);
            /*
            Debug.Log("Current context.control: " + context.control.name);
            Vector2 a = context.ReadValue<Vector2>();
            Debug.Log("Context value: " + a);

            if (context.control.name == "delta") // may change to "position"
            {
                if (a.x >= Screen.width - (Screen.width * _screenBorderPercent))
                {
                    a = context.ReadValue<Vector2>();
                    _moveInput = a;
                }
            }
            else
            {
                _moveInput = context.ReadValue<Vector2>();
            }*/

            _moveDirection = new Vector3(_moveInput.x, 0, _moveInput.y);
        }

        private void CalculateMovement(Vector3 direction)
        {
            _moveTarget += (transform.forward * _moveDirection.z + transform.right * _moveDirection.x) * _targetSpeed * Time.fixedDeltaTime;
            _moveTarget.x = Mathf.Clamp(_moveTarget.x, _minMaxXPos.x, _minMaxXPos.y);
            _moveTarget.z = Mathf.Clamp(_moveTarget.z, _minMaxYPos.x, _minMaxYPos.y);

            transform.position = Vector3.Lerp(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
            {
                return;
            }

            _zoomInput = context.ReadValue<Vector2>();

            CurrentZoom = Mathf.Clamp(_currentZoom - _zoomInput.y, _maxZoomIn, _maxZoomOut);
        }

        private void CalculateZoom(Vector2 input)
        {
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, _cameraTargetPos, _zoomSpeed * Time.deltaTime);
        }

        public void OnToggleRotate(InputAction.CallbackContext context)
        {
            _canRotate = context.ReadValue<float>() == 1;
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            if (_canRotate)
            {
                _rotationInput = context.ReadValue<Vector2>();
            }
            else
            {
                _rotationInput = Vector2.zero;
            }
        }

        private void CalculateRotation(Vector2 input)
        {
            _rotationTarget *= Quaternion.AngleAxis(input.x * _rotationSpeed * Time.deltaTime, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, _rotationTarget, _rotationSlerp * Time.deltaTime);
        }

        public void OnNewaction(InputAction.CallbackContext context)
        {

        }
    }
}

