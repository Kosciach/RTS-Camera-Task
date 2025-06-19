using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    using Input;
    
    public class RTSCameraRotate : RTSCameraComponent
    {
        [BoxGroup("References"), SerializeField] private CinemachineOrbitalFollow _cineOrbitFollow;
        [BoxGroup("References"), SerializeField] private Transform _cameraTarget;
        [BoxGroup("References"), SerializeField] private RTSCameraBounds _bounds;
        
        //Input
        private Vector2 _mouseDeltaInput;
        private bool _isRMB;
        
        //Rot
        private Vector2 _rot;
        private Vector2 _rotVelocity;
        private Vector2 _rotTarget;
        
        
        protected override void OnSetup()
        {
            _inputMgr.Inputs.Camera.MouseDelta.performed += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.MouseDelta.canceled += ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.RMB.performed += ReadRMBInput;
            _inputMgr.Inputs.Camera.RMB.canceled += ReadRMBInput;
        }

        protected override void OnDispose()
        {
            _inputMgr.Inputs.Camera.MouseDelta.performed -= ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.MouseDelta.canceled -= ReadMouseDeltaInput;
            _inputMgr.Inputs.Camera.RMB.performed -= ReadRMBInput;
            _inputMgr.Inputs.Camera.RMB.canceled -= ReadRMBInput;
            
        }

        protected override void OnTick(float p_deltaTime)
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
            
            //Rotate camera target to show direction
            _cameraTarget.rotation = Quaternion.Euler(0, _rot.x, 0);
        }
        
#region ReadInputs
        private void ReadMouseDeltaInput(InputAction.CallbackContext p_ctx)
        {
            _mouseDeltaInput = p_ctx.ReadValue<Vector2>();
        }
        
        private void ReadRMBInput(InputAction.CallbackContext p_ctx)
        {
            _isRMB = p_ctx.ReadValue<float>() > 0;
        }
#endregion
    }
}
