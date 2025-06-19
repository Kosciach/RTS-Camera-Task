using UnityEditor;
using UnityEngine;

namespace Kosciach.RTSCameraTask.RTSCamera.Editor
{
    [CustomEditor(typeof(RTSCameraBounds))]
    public class RTSCameraBoundsEditor : UnityEditor.Editor
    {
        private RTSCameraBounds _bounds;

        
        private void OnEnable()
        {
            _bounds = (RTSCameraBounds)target;
        }
        
        private void OnSceneGUI()
        {
            Transform transform = _bounds.transform;
            Handles.color = Color.gray;
            
            for (int i = 0; i < _bounds.Points.Length; i++)
            {
                //Get point
                Vector2 point2D = _bounds.Points[i];
                Vector3 point = new Vector3(point2D.x, 0, point2D.y);
                
                //Create moveable handle
                EditorGUI.BeginChangeCheck();
                Vector3 newPoint = Handles.FreeMoveHandle(point, 0.75f, Vector3.zero, Handles.SphereHandleCap);

                //Register moved handle
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_bounds, "Polygon Point Moved");
                    _bounds.Points[i] = new Vector2(newPoint.x, newPoint.z);
                    EditorUtility.SetDirty(_bounds);
                }
                
                //Draw line
                Vector2 nextPoint2D = _bounds.Points[(i + 1) % _bounds.Points.Length];
                Vector3 nextPoint = new Vector3(nextPoint2D.x, 0, nextPoint2D.y);
                Handles.DrawLine(point, nextPoint);
            }
        }
    }
}