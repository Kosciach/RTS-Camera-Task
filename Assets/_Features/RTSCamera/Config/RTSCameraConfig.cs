using NaughtyAttributes;
using UnityEngine;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    [CreateAssetMenu(fileName = "RTSCameraConfig", menuName = "Scriptable Objects/RTSCameraConfig")]
    public class RTSCameraConfig : ScriptableObject
    {
        //Rotation Settings
        [Header("Rot"), HorizontalLine]
        [SerializeField] private float _rotSpeed = 100;
        [SerializeField] private float _rotAcceleration = 5;
        [SerializeField] private float _rotInnertia = 3;
        [SerializeField] private bool _flipRotX;
        [SerializeField] private bool _flipRotY = true;
        [SerializeField] private float _minXRot = 15;
        [SerializeField] private float _minYRot = 80;
        
        public float RotSpeed => _rotSpeed;
        public float RotAcceleration => _rotAcceleration;
        public float RotInnertia => _rotInnertia;
        public bool FlipRotX => _flipRotX;
        public bool FlipRotY => _flipRotY;
        public float MinXRot => _minXRot;
        public float MinYRot => _minYRot;
        
        //Movement Settings
        [Header("Move"), HorizontalLine]
        [SerializeField] private float _moveMouseSpeed = 5;
        [SerializeField] private float _moveWSADSpeed = 10;
        [SerializeField] private float _moveAcceleration = 5;
        [SerializeField] private float _moveInnertia = 3;
        [SerializeField] private bool _flipMoveX = true;
        [SerializeField] private bool _flipMoveY = true;
        [SerializeField] private bool _useEdgeScrolling = true;
        [SerializeField, Range(0, 0.5f)] private float _edgeScrollingZoneFactor = 0.1f;
        
        public float MoveMouseSpeed => _moveMouseSpeed;
        public float MoveWSADSpeed => _moveWSADSpeed;
        public float MoveAcceleration => _moveAcceleration;
        public float MoveInnertia => _moveInnertia;
        public bool FlipMoveX => _flipMoveX;
        public bool FlipMoveY => _flipMoveY;
        public bool UseEdgeScrolling => _useEdgeScrolling;
        public float EdgeScrollingZoneFactor => _edgeScrollingZoneFactor;
        
        //Zoom Settings
        [Header("Zoom"), HorizontalLine]
        [SerializeField] private float _zoomSpeed = 300;
        [SerializeField] private float _zoomSmoothing = 200;
        [SerializeField] private float _minZoom = 10;
        [SerializeField] private float _maxZoom = 20;
        [SerializeField] private float _maxZoomTilt = 10;
        
        public float ZoomSpeed => _zoomSpeed;
        public float ZoomSmoothing => _zoomSmoothing;
        public float MinZoom => _minZoom;
        public float MaxZoom => _maxZoom;
        public float MaxZoomTilt => _maxZoomTilt;
    }
}
