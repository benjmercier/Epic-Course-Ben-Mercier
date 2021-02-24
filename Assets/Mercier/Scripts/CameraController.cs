using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mercier.Scripts.Managers;
using Mercier.InputActions;

namespace Mercier.Scripts
{
    public class CameraController : MonoSingleton<CameraController>, IEnableable
    {
        private CameraInputActions _cameraIA;

        private Vector3 _defaultPosition = new Vector3(-25f, 30f, -25f);
        private Vector3 _defaultRotation = new Vector3(50f, -90f, 0f);

        [SerializeField]
        private Camera _mainCamera;

        private Vector2 _moveInput;
        private Vector2 _zoomInput;
        [SerializeField]
        private float _speed = 5f;

        public void OnEnable()
        {
            //_cameraIA.Enable();
            
        }

        public void OnDisable()
        {
            //_cameraIA.Disable();
        }

        private void FixedUpdate()
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
            transform.position += direction * _speed * Time.deltaTime;


            //transform.Translate(new Vector3(input.y, 0, input.x) * _speed * Time.deltaTime);
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            _zoomInput = context.ReadValue<Vector2>();
        }

        private void CalculateZoom(Vector2 input)
        {
            
        }
    }
}

