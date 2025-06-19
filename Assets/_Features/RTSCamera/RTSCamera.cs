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
        [HorizontalLine]
        [BoxGroup("References"), SerializeField] private RTSCameraConfig _config;
        [BoxGroup("References"), SerializeField] private RTSCameraBounds _bounds;
        
        //Input
        private Vector2 _mouseDeltaInput;
        private Vector2 _wsadInput;
        private bool _isLMB;
        private bool _isRMB;
        private float _scrollInput;
        
        //Rot
        private Vector2 _rot;
        private Vector2 _rotVelocity;
        private Vector2 _rotTarget;
        
        //Move
        private Vector3 _move;
        private Vector3 _moveVelocity;
        private Vector3 _moveTarget;
        private Vector2 _edgeScrolling;
        
        //Zoom
        private float _targetZoom;
        private float _zoomSmoothDampRef;
        
        
        private void Awake()
        {
            _inputMgr = FindFirstObjectByType<InputManager>();
            
            SetStartZoom();
            
            _inputMgr.Inputs.Camera.MouseDelta.performed += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.MouseDelta.canceled += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.WSAD.performed += ReadWSADInput;
            _inputMgr.Inputs.Camera.WSAD.canceled += ReadWSADInput;
            _inputMgr.Inputs.Camera.LMB.performed += ReadLMBInput;
            _inputMgr.Inputs.Camera.LMB.canceled += ReadLMBInput;
            _inputMgr.Inputs.Camera.RMB.performed += ReadRMBInput;
            _inputMgr.Inputs.Camera.RMB.canceled += ReadRMBInput;
            _inputMgr.Inputs.Camera.Scroll.performed += ReadScrollInput;
            _inputMgr.Inputs.Camera.Scroll.canceled += ReadScrollInput;
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
            _inputMgr.Inputs.Camera.Scroll.performed -= ReadScrollInput;
            _inputMgr.Inputs.Camera.Scroll.canceled -= ReadScrollInput;
            
        }

        private void Update()
        {
            float deltaTime = Mathf.Min(Time.deltaTime, 0.033f);

            CheckEdgeScrolling();
            Rotate(deltaTime);
            Move(deltaTime);
            Zoom(deltaTime);
        }

        private void SetStartZoom()
        {
            _targetZoom = _cineOrbitFollow.Radius;
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
        
        private void Rotate(float p_deltaTime)
        {
            Vector2 rotDir = Vector2.zero;
            
            //Acceleration
            if (_isRMB)
            {
                //Calculate current direction
                rotDir.x = _mouseDeltaInput.x * (_config.FlipRotX ? -1 : 1);
                rotDir.y = _mouseDeltaInput.y * (_config.FlipRotY ? -1 : 1);
                rotDir *= _config.RotSpeed * p_deltaTime;
                
                //Lerp velocity to direction
                _rotVelocity = Vector2.Lerp(_rotVelocity, rotDir, _config.RotAcceleration * p_deltaTime);
            }
            else //Inertia
            {
                _rotVelocity = Vector2.Lerp(_rotVelocity, Vector2.zero, _config.RotInnertia * p_deltaTime);
            }
            
            //Apply current rot
            _rot += _rotVelocity;

            //Clamp current rot
            _cineOrbitFollow.VerticalAxis.Range = new Vector2(_config.MinXRot, _config.MinYRot);
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

        private void Zoom(float p_deltaTime)
        {
            if (_scrollInput != 0)
            {
                _targetZoom += -_scrollInput * _config.ZoomSpeed * p_deltaTime;
                _targetZoom = Mathf.Clamp(_targetZoom, _config.MinZoom, _config.MaxZoom);
            }
            
            _cineOrbitFollow.Radius = Mathf.SmoothDamp(_cineOrbitFollow.Radius, _targetZoom, ref _zoomSmoothDampRef, _config.ZoomSmoothing * p_deltaTime);
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
        
        private void ReadScrollInput(InputAction.CallbackContext p_ctx)
        {
            _scrollInput = p_ctx.ReadValue<float>();
        }
#endregion
    }
}
