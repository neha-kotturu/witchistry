using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MathUtils
{
    public class MathUtils
    {

        public static float PointToPlaneDistance(Vector3 point, Vector3 planeNormal, Vector3 pointOnPlane)
        {
            return Mathf.Abs(Vector3.Dot(point - pointOnPlane, planeNormal.normalized));
        }

        public static (Vector3, Vector3) FindPlane(List<Vector3> points)
        {
            if (points.Count < 3)
            {
                Debug.LogError("At least three points are required to define a plane.");
                return (Vector3.up, Vector3.zero); // Default fallback
            }

            Vector3 centroid = Vector3.zero;
            foreach (Vector3 point in points)
            {
                centroid += point;
            }
            centroid /= points.Count;

            Vector3 normalSum = Vector3.zero;
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 v1 = points[i] - centroid;
                Vector3 v2 = points[(i + 1) % points.Count] - centroid;

                Vector3 crossProduct = Vector3.Cross(v1, v2);
                float areaWeight = crossProduct.magnitude; 

                normalSum += crossProduct.normalized * areaWeight;
            }

            Vector3 preciseNormal = normalSum.normalized;
            return (preciseNormal, centroid);
        }


    }

}
