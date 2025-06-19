using NaughtyAttributes;
using UnityEngine;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    using Input;
    
    public class RTSCameraController : MonoBehaviour
    {
        private RTSCameraComponent[] _components;
        
        [BoxGroup("References"), SerializeField] private RTSCameraConfig _config;
        [BoxGroup("References"), SerializeField] private InputManager _inputMgr;

        
        private void Awake()
        {
            _inputMgr = FindFirstObjectByType<InputManager>();

            _components = GetComponents<RTSCameraComponent>();
            foreach (RTSCameraComponent component in _components)
            {
                component.Setup(_config, _inputMgr);
            }
        }

        private void OnDestroy()
        {
            foreach (RTSCameraComponent component in _components)
            {
                component.Dispose();
            }
        }

        private void Update()
        {
            float deltaTime = Mathf.Min(Time.deltaTime, 0.033f);

            foreach (RTSCameraComponent component in _components)
            {
                component.Tick(deltaTime);
            }
        }
    }
}
