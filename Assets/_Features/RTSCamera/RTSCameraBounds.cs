using NaughtyAttributes;
using UnityEngine;

namespace Kosciach.RTSCameraTask.RTSCamera
{
    public class RTSCameraBounds : MonoBehaviour
    {
        [InfoBox("There must be at least 3 points!")]
        [SerializeField] private Vector2[] _points;
        internal Vector2[] Points => _points;

        
        public Vector3 ClampPos(Vector3 p_pos)
        {
            Vector2 pos2D = new Vector2(p_pos.x, p_pos.z);

            //Return the same pos if in polygon
            if (IsPointInPolygon(pos2D))
            {
                return p_pos;
            }

            //If outside polygon go to closest point
            Vector2 closestPoint2D = FindClosestPointOnPolygon(pos2D);
            return new Vector3(closestPoint2D.x, p_pos.y, closestPoint2D.y);
        }
        
        private bool IsPointInPolygon(Vector2 p_pos2D)
        {
            bool result = false;
            int j = _points.Length - 1;
            
            for (int i = 0; i < _points.Length; i++)
            {
                //Check cross
                if (_points[i].y < p_pos2D.y && _points[j].y >= p_pos2D.y || 
                    _points[j].y < p_pos2D.y && _points[i].y >= p_pos2D.y)
                {
                    
                    //Check intersection
                    if (_points[i].x + (p_pos2D.y - _points[i].y) /
                        (_points[j].y - _points[i].y) *
                        (_points[j].x - _points[i].x) < p_pos2D.x)
                    {
                        result = !result;
                    }
                }
                
                j = i;
            }
            
            return result;
        }

        private Vector2 FindClosestPointOnPolygon(Vector2 p_pos2D)
        {
            float closestDistance = float.MaxValue;
            Vector2 closestPoint2D = p_pos2D;

            for (int i = 0; i < _points.Length; i++)
            {
                //Get closest point on segment
                Vector2 segmentStart = _points[i];
                Vector2 segmentEnd = _points[(i + 1) % _points.Length];
                Vector2 closestSegmentPoint = FindClosestPointOnSegment(segmentStart, segmentEnd, p_pos2D);
                
                //Check if point is closest
                float distance = Vector2.SqrMagnitude(p_pos2D - closestSegmentPoint);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint2D = closestSegmentPoint;
                }
            }

            return closestPoint2D;
        }

        private Vector2 FindClosestPointOnSegment(Vector2 p_segmentStart, Vector2 p_segmentEnd, Vector2 p_pos2D)
        {
            //Get segment direction
            Vector2 segmentDirection = p_segmentEnd - p_segmentStart;
            float rawSegmentDistance = segmentDirection.magnitude;
            segmentDirection.Normalize();

            //Project point onto segment
            Vector2 toPoint = p_pos2D - p_segmentStart;
            float projection = Vector2.Dot(toPoint, segmentDirection);
            projection = Mathf.Clamp(projection, 0f, rawSegmentDistance);
            
            return p_segmentStart + segmentDirection * projection;
        }
    }
}
