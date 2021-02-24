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

        private Vector3 _defaultPosition = new Vector3(-25f, 30f, -25f);
        private Vector3 _defaultRotation = new Vector3(50f, -90f, 0f);

        [SerializeField]
        private Camera _mainCamera;

        private Vector2 _moveInput;
        private Vector2 _zoomInput;
        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private float _borderThicknessPercent = 10f;
        [SerializeField]
        private Vector2 _levelMoveLimit;
        [SerializeField]
        private float _minZoom, _maxZoom;

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
                new ActiveAction<InputAction>(_zoomAction, _cameraIA.Camera.Zoom)
            });

            _activeInputActions.ToList().ForEach(a => a.ReturnActiveAction().Enable()); // .ToList() is only there for LINQ, is it better to just not use it?
        }

        public void OnDisable()
        {
            _activeInputActions.ToList().ForEach(a => a.ReturnActiveAction().Disable());

            _cameraIA.Camera.Disable();
        }

        private void Update()
        {
            CalculateMovement(_moveInput);
            CalculateZoom(_zoomInput);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void CalculateMovement(Vector2 input)
        {
            Vector3 direction = transform.forward * input.y + transform.right * input.x;
            //direction.x = Mathf.Clamp(direction.x, -_levelMoveLimit.x, _levelMoveLimit.x);
            //direction.y = Mathf.Clamp(direction.y, -_levelMoveLimit.y, _levelMoveLimit.y);

            transform.position += direction * _speed * Time.deltaTime;
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            _zoomInput = context.ReadValue<Vector2>();
        }

        private void CalculateZoom(Vector2 input)
        {
            //_zoomInput = _cameraIA.Camera.Zoom.ReadValue<Vector2>();
        }

        public void OnNewaction(InputAction.CallbackContext context)
        {
            
        }
    }
}

