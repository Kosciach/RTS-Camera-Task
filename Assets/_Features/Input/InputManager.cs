using System;
using UnityEngine;

namespace Kosciach.RTSCameraTask.Input
{
    public class InputManager : MonoBehaviour
    {
        private InputActions _inputs;
        public InputActions Inputs => _inputs;
        
        private void Awake()
        {
            _inputs = new InputActions();
            _inputs.Enable();
        }
    }
}
