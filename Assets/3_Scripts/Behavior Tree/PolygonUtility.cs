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

    public static Vector3 GetRandomPointInPolygon(Vector3[] polygonVertices)
    {
        Bounds bounds = GetPolygonBounds(polygonVertices);
        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
        while (!IsPointInPolygon(randomPoint, polygonVertices))
        {
            randomPoint = new Vector3(Random.Range(bounds.min.x, bounds.max.x),0,Random.Range(bounds.min.z, bounds.max.z));
        }
        return randomPoint;
    }

    private static Bounds GetPolygonBounds(Vector3[] polygonVertices)
    {
        Bounds bounds = new Bounds(polygonVertices[0], Vector3.zero);

        for (int i = 1; i < polygonVertices.Length; i++)
        {
            bounds.Encapsulate(polygonVertices[i]);
        }
        return bounds;
    }

    public static bool IsPointInPolygon(Vector3 point, Vector3[] polygonVertices)
    {
        int intersections = 0;
        for (int i = 0; i < polygonVertices.Length; i++)
        {
            Vector3 a = polygonVertices[i];
            Vector3 b = polygonVertices[(i + 1) % polygonVertices.Length];
            if (RayIntersectsSegment(point, Vector3.right, a, b))
            {
                intersections++;
            }
        }
        return (intersections % 2) == 1;
    }

    private static bool RayIntersectsSegment(Vector3 p, Vector3 r, Vector3 a, Vector3 b)
    {
        Vector3 ap = a - p;
        Vector3 ab = b - a;
        Vector3 cross = Vector3.Cross(r, ab);

        if (cross.y == 0)
        {
            return false;
        }

        float t = Vector3.Cross(ap, ab).y / cross.y;
        if (t < 0 || t > 1)
        {
            return false;
        }

        float u = Vector3.Cross(ap, r).y / cross.y;
        if (u < 0)
        {
            return false;
        }

        return true;
    }
}
