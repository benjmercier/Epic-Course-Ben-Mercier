using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;

namespace Mercier.InputActions.Processors
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class MouseScreenPosProcessor : InputProcessor<Vector2>
    {
#if UNITY_EDITOR
        static MouseScreenPosProcessor()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<MouseScreenPosProcessor>();
        }

        [Tooltip("Mouse Pos.")]
        private int _screenHeight = Screen.height; // 399
        private int _screenWidth = Screen.width; // 709

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            if (Mouse.current.position.ReadValue().x >= _screenWidth - (_screenWidth * 0.1f))
            {
                return value;
            }
            else if (Mouse.current.position.ReadValue().x <= _screenWidth * 0.10f)
            {
                return value;
            }
            else if (Mouse.current.position.ReadValue().y >= _screenHeight - (_screenHeight * 0.1f))
            {
                return value;
            }
            else if (Mouse.current.position.ReadValue().y <= _screenHeight * 0.10f)
            {
                return value;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }
}

