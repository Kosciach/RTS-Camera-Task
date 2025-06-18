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

        [BoxGroup("References"), SerializeField] private Camera _camera;
        [BoxGroup("References"), SerializeField] private CinemachineCamera _cineCamera;
        [BoxGroup("References"), SerializeField] private CinemachineOrbitalFollow _cineOrbitFollow;
        [BoxGroup("References"), SerializeField] private Transform _cameraTarget;
        
        [Header("Rot"), HorizontalLine]
        [BoxGroup("Settings"), SerializeField] private float _rotSpeed = 100;
        [BoxGroup("Settings"), SerializeField] private float _rotAcceleration = 5;
        [BoxGroup("Settings"), SerializeField] private float _rotInnertia = 3;
        [BoxGroup("Settings"), SerializeField] private bool _flipRotX;
        [BoxGroup("Settings"), SerializeField] private bool _flipRotY = true;
        [BoxGroup("Settings"), SerializeField] private float _minXRot = 15;
        [BoxGroup("Settings"), SerializeField] private float _minYRot = 80;
        
        [Header("Move"), HorizontalLine]
        [BoxGroup("Settings"), SerializeField] private float _moveMouseSpeed = 5;
        [BoxGroup("Settings"), SerializeField] private float _moveWSADSpeed = 10;
        [BoxGroup("Settings"), SerializeField] private float _moveAcceleration = 5;
        [BoxGroup("Settings"), SerializeField] private float _moveInnertia = 3;
        [BoxGroup("Settings"), SerializeField] private bool _flipMoveX = true;
        [BoxGroup("Settings"), SerializeField] private bool _flipMoveY = true;
        
        //Input
        private Vector2 _mouseDeltaInput;
        private Vector2 _wsadInput;
        private bool _isLMB;
        private bool _isRMB;
        
        //Rot
        private Vector2 _rot;
        private Vector2 _rotVelocity;
        private Vector2 _rotTarget;
        
        //Move
        private Vector3 _move;
        private Vector3 _moveVelocity;
        private Vector3 _moveTarget;
        
        private void Awake()
        {
            _inputMgr = FindFirstObjectByType<InputManager>();
            
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
            
            Rotate(deltaTime);
            Move(deltaTime);
        }

        private void Rotate(float p_deltaTime)
        {
            Vector2 rotDir = Vector2.zero;
            
            //Acceleration
            if (_isRMB)
            {
                //Calculate current direction
                rotDir.x = _mouseDeltaInput.x * (_flipRotX ? -1 : 1);
                rotDir.y = _mouseDeltaInput.y * (_flipRotY ? -1 : 1);
                rotDir *= _rotSpeed * p_deltaTime;
                
                //Lerp velocity to direction
                _rotVelocity = Vector2.Lerp(_rotVelocity, rotDir, _rotAcceleration * p_deltaTime);
            }
            else //Inertia
            {
                _rotVelocity = Vector2.Lerp(_rotVelocity, Vector2.zero, _rotInnertia * p_deltaTime);
            }
            
            //Apply current rot
            _rot += _rotVelocity;

            //Clamp current rot
            _cineOrbitFollow.VerticalAxis.Range = new Vector2(_minXRot, _minYRot);
            _rot.x = _cineOrbitFollow.HorizontalAxis.ClampValue(_rot.x);
            _rot.y = _cineOrbitFollow.VerticalAxis.ClampValue(_rot.y);
            
            //Apply rot to cinemachine
            _cineOrbitFollow.HorizontalAxis.Value = _rot.x;
            _cineOrbitFollow.VerticalAxis.Value = _rot.y;
            
            //Rotate camera target for later move
            _cameraTarget.rotation = Quaternion.Euler(0, _rot.x, 0);
        }
        
        private void Move(float p_deltaTime)
        {
            Vector3 moveDir = Vector3.zero;
            
            //Set flips
            int flipX = (_flipMoveX ? -1 : 1);
            int flipY = (_flipMoveY ? -1 : 1);
            
            
            //Set input (mouse > wsad)
            Vector2 input = Vector2.zero;
            float speed = 0;
            
            if (_isLMB)
            {
                input = _mouseDeltaInput;
                speed = _moveMouseSpeed;
            }
            else if (_wsadInput.magnitude > 0)
            {
                input = -_wsadInput;
                speed = _moveWSADSpeed;
            }
            
            //Acceleration
            if (input.magnitude != 0)
            {
                //Calculate current direction
                moveDir = (_cameraTarget.forward * input.y * flipY)
                          + (_cameraTarget.right * input.x * flipX);
                moveDir *= speed * p_deltaTime;
                
                //Lerp velocity to direction
                _moveVelocity = Vector3.Lerp(_moveVelocity, moveDir, _moveAcceleration * p_deltaTime);
            }
            else //Inertia
            {
                _moveVelocity = Vector3.Lerp(_moveVelocity, Vector3.zero, _moveInnertia * p_deltaTime);
            }
            
            //Apply current move
            _move += _moveVelocity;

            //Clamp current move

            
            //Apply move to cinemachine target
            _cameraTarget.position += _moveVelocity;
        }

#region ReadInputs
        private void ReadMouseDeltaInput(InputAction.CallbackContext p_ctx)
        {
            _mouseDeltaInput = p_ctx.ReadValue<Vector2>();
        }
        
        private void ReadWSADInput(InputAction.CallbackContext p_ctx)
        {
            _wsadInput = p_ctx.ReadValue<Vector2>();
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
