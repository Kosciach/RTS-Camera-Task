using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    public class RTSCameraMove : RTSCameraComponent
    {
        [BoxGroup("References"), SerializeField] private Transform _cameraTarget;
        [BoxGroup("References"), SerializeField] private RTSCameraBounds _bounds;
        
        //Input
        private Vector2 _mouseDeltaInput;
        private Vector2 _wsadInput;
        private bool _isLMB;
        private bool _isRMB;
        
        //Move
        private Vector3 _move;
        private Vector3 _moveVelocity;
        private Vector3 _moveTarget;
        private Vector2 _edgeScrolling;
        
        
        protected override void OnSetup()
        {
            _inputMgr.Inputs.Camera.MouseDelta.performed += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.MouseDelta.canceled += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.WSAD.performed += ReadWSADInput;
            _inputMgr.Inputs.Camera.WSAD.canceled += ReadWSADInput;
            _inputMgr.Inputs.Camera.LMB.performed += ReadLMBInput;
            _inputMgr.Inputs.Camera.LMB.canceled += ReadLMBInput;
            _inputMgr.Inputs.Camera.RMB.performed += ReadRMBInput;
            _inputMgr.Inputs.Camera.RMB.canceled += ReadRMBInput;
        }

        protected override void OnDispose()
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

        protected override void OnTick(float p_deltaTime)
        {
            CheckEdgeScrolling();
            Move(p_deltaTime);
        }
        
        private void CheckEdgeScrolling()
        {
            _edgeScrolling = Vector2.zero;
            
            if(!_config.UseEdgeScrolling || _isRMB)
            {
                return;
            }

            Vector3 mousePos = UnityEngine.Input.mousePosition;
            _edgeScrolling.x = GetEdgeScroll(mousePos.x, Screen.width);
            _edgeScrolling.y = GetEdgeScroll(mousePos.y, Screen.height);
        }
        
        private int GetEdgeScroll(float p_position, float p_max)
        {
            float zone = p_max * _config.EdgeScrollingZoneFactor;
            
            //Left, Down
            if (p_position >= 0 && p_position <= zone)
            {
                return -1;
            }

            //Right, Up
            if (p_position <= p_max && p_position >= p_max - zone)
            {
                return 1;
            }

            return 0;
        }

        private void Move(float p_deltaTime)
        {
            //Set flips
            int flipX = (_config.FlipMoveX ? -1 : 1);
            int flipY = (_config.FlipMoveY ? -1 : 1);
            
            //Set input (mouse > wsad)
            Vector2 input = Vector2.zero;
            float speed = 0;
            
            if (_isLMB)
            {
                input = _mouseDeltaInput;
                speed = _config.MoveMouseSpeed;
            }
            else if (_wsadInput.magnitude > 0)
            {
                input = -_wsadInput;
                speed = _config.MoveWSADSpeed;
            }
            else if (_edgeScrolling.magnitude > 0)
            {
                input = -_edgeScrolling;
                speed = _config.MoveWSADSpeed;
            }
            
            //Acceleration
            if (input.magnitude != 0)
            {
                //Calculate current direction
                Vector3 moveDir = (_cameraTarget.forward * input.y * flipY)
                                + (_cameraTarget.right * input.x * flipX);
                moveDir *= speed * p_deltaTime;
                
                //Lerp velocity to direction
                _moveVelocity = Vector3.Lerp(_moveVelocity, moveDir, _config.MoveAcceleration * p_deltaTime);
            }
            else //Inertia
            {
                _moveVelocity = Vector3.Lerp(_moveVelocity, Vector3.zero, _config.MoveInnertia * p_deltaTime);
            }
            
            //Apply current move
            _move += _moveVelocity;

            //Clamp current move
            Vector3 newPos = _cameraTarget.position + _moveVelocity;
            newPos = _bounds.ClampPos(newPos);
            
            //Apply move to cinemachine target
            _cameraTarget.position = newPos;
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
