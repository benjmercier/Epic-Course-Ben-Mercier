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
    public class CameraController : MonoSingleton<CameraController>, IEventable, CameraInputActions.ICameraActions
    {
        private CameraInputActions _cameraIA;

        private List<ActiveAction<InputAction>> _activeInputActions = new List<ActiveAction<InputAction>>();
        private InputAction _moveAction;
        private InputAction _zoomAction;
        private InputAction _rotateAction;
        private InputAction _toggleRotateAction;

        private Vector2 _moveInput, _zoomInput, _rotationInput;
        private Vector3 _moveDirection, _moveTarget;

        [SerializeField]
        private Camera _camera;

        //private Vector3 _defaultPosition = new Vector3(-25f, 30f, -25f);
        //private Vector3 _defaultRotation = new Vector3(50f, 0f, 0f);

        [SerializeField]
        private float _minXAxis = -8f, _maxXAxis = 8f, _minZAxis = -5f, _maxZAxis = 10f;
        [SerializeField]
        private float _defaultZoom, _minZoom, _maxZoom;

        [SerializeField]
        private float _moveSpeed = 5f, _targetSpeed = 10f, _rotationSpeed = 5f;

        [SerializeField]
        private float _screenBorderPercent = 10f;

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
        }

        private void FixedUpdate()
        {
            CalculateMovement(_moveInput);
            //CalculateZoom(_zoomInput);
            //CalculateRotation(_rotationInput);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void CalculateMovement(Vector2 input)
        {
            _moveDirection = new Vector3(input.x, 0, input.y);

            _moveTarget += (transform.forward * _moveDirection.z + transform.right * _moveDirection.x) * _targetSpeed * Time.fixedDeltaTime;
            _moveTarget.x = Mathf.Clamp(_moveTarget.x, _minXAxis, _maxXAxis);
            _moveTarget.z = Mathf.Clamp(_moveTarget.z, _minZAxis, _maxZAxis);

            transform.position = Vector3.Lerp(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            _zoomInput = context.ReadValue<Vector2>();
        }

        private void CalculateZoom(Vector2 input)
        {
            
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            
        }

        private void CalculateRotation(Vector2 input)
        {

        }

        public void OnToggleRotate(InputAction.CallbackContext context)
        {
            
        }

        public void OnNewaction(InputAction.CallbackContext context)
        {

        }
    }
}

