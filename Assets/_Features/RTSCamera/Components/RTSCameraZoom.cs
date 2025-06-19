using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    using Input;
    
    public class RTSCameraZoom : RTSCameraComponent
    {
        [BoxGroup("References"), SerializeField] private CinemachineOrbitalFollow _cineOrbitFollow;
        
        //Input
        private float _scrollInput;
        
        //Zoom
        private float _targetZoom;
        private float _zoomSmoothDampRef;
        
        
        protected override void OnSetup()
        {
            _targetZoom = _cineOrbitFollow.Radius;
            
            _inputMgr.Inputs.Camera.Scroll.performed += ReadScrollInput;
            _inputMgr.Inputs.Camera.Scroll.canceled += ReadScrollInput;
        }

        protected override void OnDispose()
        {
            _inputMgr.Inputs.Camera.Scroll.performed -= ReadScrollInput;
            _inputMgr.Inputs.Camera.Scroll.canceled -= ReadScrollInput;
            
        }

        protected override void OnTick(float p_deltaTime)
        {
            if (_scrollInput != 0)
            {
                _targetZoom += -_scrollInput * _config.ZoomSpeed * p_deltaTime;
                _targetZoom = Mathf.Clamp(_targetZoom, _config.MinZoom, _config.MaxZoom);
            }
            
            _cineOrbitFollow.Radius = Mathf.SmoothDamp(_cineOrbitFollow.Radius, _targetZoom, ref _zoomSmoothDampRef, _config.ZoomSmoothing * p_deltaTime);
        }
        
#region ReadInputs
        private void ReadScrollInput(InputAction.CallbackContext p_ctx)
        {
            _scrollInput = p_ctx.ReadValue<float>();
        }
#endregion
    }
}
