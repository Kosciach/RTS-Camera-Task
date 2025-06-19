using UnityEngine;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    using Input;
    
    public abstract class RTSCameraComponent : MonoBehaviour
    {
        protected RTSCameraConfig _config;
        protected InputManager _inputMgr;
        
        internal void Setup(RTSCameraConfig p_config, InputManager p_inputMgr)
        {
            _config = p_config;
            _inputMgr = p_inputMgr;
            
            OnSetup();
        }
        
        internal void Dispose()
        {
            OnDispose();
        }
        
        internal void Tick(float p_deltaTime)
        {
            OnTick(p_deltaTime);
        }
        
        protected virtual void OnSetup() { }
        protected virtual void OnDispose() { }
        protected virtual void OnTick(float p_deltaTime) { }
    }
}
