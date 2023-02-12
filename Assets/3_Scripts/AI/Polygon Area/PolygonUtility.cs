using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonUtility
{
    public static Vector3[] AdjustPolygonSequence(Vector3[] polygonVertices)
    {
        Vector3[] adjustedVertices = new Vector3[polygonVertices.Length];
        Vector3 center = GetPolygonCenter(polygonVertices);

        // sort vertices based on their angle with the center
        System.Array.Sort(polygonVertices, (a, b) =>
        {
            float angleA = Mathf.Atan2(a.z - center.z, a.x - center.x);
            float angleB = Mathf.Atan2(b.z - center.z, b.x - center.x);
            if (angleA < angleB) return -1;
            if (angleA > angleB) return 1;
            return 0;
        });

        System.Array.Copy(polygonVertices, adjustedVertices, polygonVertices.Length);
        return adjustedVertices;
    }

    public static List<Vector3> AdjustPolygonSequence(List<Vector3> polygonVertices)
    {
        Vector3[] adjustedVertices = new Vector3[polygonVertices.Count];
        Vector3[] polygonArr = polygonVertices.ToArray();
        Vector3 center = GetPolygonCenter(polygonArr);

        // sort vertices based on their angle with the center
        System.Array.Sort(polygonArr, (a, b) =>
        {
            float angleA = Mathf.Atan2(a.z - center.z, a.x - center.x);
            float angleB = Mathf.Atan2(b.z - center.z, b.x - center.x);
            if (angleA < angleB) return -1;
            if (angleA > angleB) return 1;
            return 0;
        });

        System.Array.Copy(polygonArr, adjustedVertices, polygonVertices.Count);
        return adjustedVertices.ToList();
    }

    public static Vector3 GetPolygonCenter(Vector3[] polygonVertices)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 vertex in polygonVertices)
        {
            center += vertex;
        }
        center /= polygonVertices.Length;
        return center;
    }

    public static void DrawPolygon(Vector3[] polygonVertices)
    {
        for (int i = 0; i < polygonVertices.Length; i++)
        {
            Vector3 currentVertex = polygonVertices[i];
            Vector3 nextVertex = polygonVertices[(i + 1) % polygonVertices.Length];
            Debug.DrawLine(currentVertex, nextVertex, Color.yellow);
        }
    }

    public static void DebugIsInPolygon(Vector3[] polygonVertices, Color debugColor)
    {
        for (int i = 0; i < polygonVertices.Length; i++)
        {
            Vector3 currentVertex = polygonVertices[i];
            Vector3 nextVertex = polygonVertices[(i + 1) % polygonVertices.Length];
            Debug.DrawLine(currentVertex, nextVertex, debugColor);
        }
    }

    public static void DebugIsInPolygon(List<Transform> polygonVertices, Color debugColor)
    {
        for (int i = 0; i < polygonVertices.Count; i++)
        {
            Vector3 currentVertex = polygonVertices[i].position;
            Vector3 nextVertex = polygonVertices[(i + 1) % polygonVertices.Count].position;
            Debug.DrawLine(currentVertex, nextVertex, debugColor);
        }
    }
}
