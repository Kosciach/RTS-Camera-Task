using System;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    using Input;
    
    public class RTSCamera : MonoBehaviour
    {
        private InputManager _inputMgr;

        [BoxGroup("References"), SerializeField] private CinemachineCamera _cineCamera;
        [BoxGroup("References"), SerializeField] private CinemachineOrbitalFollow _cineOrbitFollow;
        [BoxGroup("References"), SerializeField] private Transform _cameraTarget;
        
        [Header("Rot"), HorizontalLine]
        [BoxGroup("Settings"), SerializeField] private float _rotSpeed = 3;
        [BoxGroup("Settings"), SerializeField] private bool _flipRotX;
        [BoxGroup("Settings"), SerializeField] private bool _flipRotY;
        
        //Input
        private Vector2 _mouseDeltaInput;
        private Vector2 _wsadInput;
        private bool _isLMB;
        private bool _isRMB;
        
        //Rot
        private float _horizontalRot;
        private float _verticalRot;
        
        
        private void Awake()
        {
            _inputMgr = FindFirstObjectByType<InputManager>();

            _horizontalRot = _cineOrbitFollow.HorizontalAxis.Value;
            _verticalRot = _cineOrbitFollow.VerticalAxis.Value;
            
            _inputMgr.Inputs.Camera.MouseDelta.performed += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.MouseDelta.canceled += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.WSAD.performed += ReadWSADInput;
            _inputMgr.Inputs.Camera.WSAD.canceled += ReadWSADInput;
            _inputMgr.Inputs.Camera.LMB.performed += ReadLMBInput;
            _inputMgr.Inputs.Camera.LMB.canceled += ReadLMBInput;
            _inputMgr.Inputs.Camera.RMB.performed += ReadRMBInput;
            _inputMgr.Inputs.Camera.RMB.canceled += ReadRMBInput;
        }

        private void OnDestroy()
        {
            _inputMgr.Inputs.Camera.MouseDelta.performed -= ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.MouseDelta.canceled -= ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.WSAD.performed -= ReadWSADInput;
            _inputMgr.Inputs.Camera.WSAD.canceled -= ReadWSADInput;
            _inputMgr.Inputs.Camera.LMB.performed -= ReadLMBInput;
            _inputMgr.Inputs.Camera.LMB.canceled -= ReadLMBInput;
            _inputMgr.Inputs.Camera.RMB.performed -= ReadRMBInput;
            _inputMgr.Inputs.Camera.RMB.canceled -= ReadRMBInput;
        }

        private void Update()
        {
            float deltaTime = Mathf.Min(Time.unscaledDeltaTime, 0.033f);
            
            Move(deltaTime);
            Rotate(deltaTime);
        }

        private void Move(float p_deltaTime)
        {
            
        }

        private void Rotate(float p_deltaTime)
        {
            if (!_isRMB) return;
            
            Vector2 rotDir = Vector2.zero;
            rotDir.x = _mouseDeltaInput.x * (_flipRotX ? -1 : 1);
            rotDir.y = _mouseDeltaInput.y * (_flipRotY ? -1 : 1);
            
            Vector2 rot = rotDir * _rotSpeed * Time.deltaTime;

            _horizontalRot += rot.x;
            _verticalRot += rot.y;
            
            _cineOrbitFollow.HorizontalAxis.Value = _horizontalRot;
            _cineOrbitFollow.VerticalAxis.Value = _verticalRot;
        }

#region ReadInputs
        private void ReadMouseDeltaInput(InputAction.CallbackContext p_ctx)
        {
            _mouseDeltaInput = p_ctx.ReadValue<Vector2>();
        }
        
        private void ReadWSADInput(InputAction.CallbackContext p_ctx)
        {
            _mouseDeltaInput = p_ctx.ReadValue<Vector2>();
        }
        
        private void ReadLMBInput(InputAction.CallbackContext p_ctx)
        {
            _isLMB = p_ctx.ReadValue<float>() > 0;
        }
        
        private void ReadRMBInput(InputAction.CallbackContext p_ctx)
        {
            _isRMB = p_ctx.ReadValue<float>() > 0;
        }
#endregion
    }
}
